using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

[DisallowMultipleComponent]
public class SplineManager : MonoBehaviour
{
    public static SplineManager Instance;

    [Header("Spline da pista")]
    public SplineContainer splinePista;
    public SplineContainer splinePitStop;

    public float TotalLength;               // comprimento total da spline (metros)

    [Header("Config")]
    [Tooltip("Se true, recalcula o comprimento em Start(). Útil enquanto edita a spline.")]
    public bool recalcOnStart = true;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(this);
    }

    private void Start()
    {
        if (recalcOnStart) RecalculateTotalLength();
    }

    /// <summary>
    /// Força o recálculo do comprimento total da spline (em metros, worldspace).
    /// </summary>
    public void RecalculateTotalLength()
    {
        if (splinePista == null)
        {
            Debug.LogWarning("SplineManager: splinePista não atribuída.");
            TotalLength = 0f;
            return;
        }

        // Calcula o comprimento em world space da primeira spline (índice 0).
        // Se você tiver várias splines em SplineContainer, adapte para usar o índice correto.
        try
        {
            TotalLength = splinePista.CalculateLength(0);
        }
        catch (Exception ex)
        {
            Debug.LogError($"SplineManager: erro ao calcular comprimento da spline: {ex.Message}");
            TotalLength = 0f;
        }
    }

    /// <summary>
    /// Converte uma distância em metros (ao longo da pista) para um valor t normalizado (0..1).
    /// Distâncias maiores que TotalLength são enroladas (loop).
    /// </summary>
    public float SplineTFromDistance(float dist)
    {
        if (splinePista == null || TotalLength <= 0f)
        {
            return 0f;
        }

        // Garante valor positivo (permite dist negativas também)
        float d = dist % TotalLength;
        if (d < 0f) d += TotalLength;

        // Normaliza
        float t = d / TotalLength;

        // Clamp por segurança
        return Mathf.Clamp01(t);
    }

    /// <summary>
    /// Retorna a posição worldspace para um t normalizado (0..1) na spline (índice 0).
    /// </summary>
    public Vector3 GetPosition(float splineT)
    {
        if (splinePista == null)
            return Vector3.zero;

        // Safety clamp
        splineT = Mathf.Clamp01(splineT);

        // Evaluate leva: (splineIndex, t, out position, out tangent, out up)
        float3 posF; float3 tan; float3 up;
        // Avalia a spline no ponto "splineT"
        bool ok = splinePista.Evaluate(0, splineT, out posF, out tan, out up);
        if (!ok)
        {
            // fallback simples
            var fallback = splinePista.EvaluatePosition(0, splineT);
            return new Vector3(fallback.x, fallback.y, fallback.z);
        }

        return new Vector3(posF.x, posF.y, posF.z);
    }

    /// <summary>
    /// Retorna a rotação (worldspace) alinhada com o tangente/up da spline no t normalizado.
    /// </summary>
    public Quaternion GetRotation(float splineT)
    {
        if (splinePista == null)
            return Quaternion.identity;

        splineT = Mathf.Clamp01(splineT);

        float3 posF; float3 tanF; float3 upF;
        bool ok = splinePista.Evaluate(0, splineT, out posF, out tanF, out upF);
        if (!ok)
        {
            // fallback para calcular a tangente com EvaluateTangent ou EvaluatePosition+small offset
            float3 forward = splinePista.EvaluateTangent(0, splineT);
            float3 up = splinePista.EvaluateUpVector(0, splineT);
            var fwd = new Vector3(forward.x, forward.y, forward.z);
            var upv = new Vector3(up.x, up.y, up.z);
            if (fwd.sqrMagnitude <= Mathf.Epsilon) return Quaternion.identity;
            return Quaternion.LookRotation(fwd.normalized, upv.normalized);
        }

        var tangent = new Vector3(tanF.x, tanF.y, tanF.z);
        var upVec = new Vector3(upF.x, upF.y, upF.z);

        if (tangent.sqrMagnitude <= Mathf.Epsilon) return Quaternion.identity;
        return Quaternion.LookRotation(tangent.normalized, upVec.normalized);
    }

    public Vector3 GetTangent(float splineT)
    {
        splineT = Mathf.Clamp01(splineT);
        float3 posF, tan, up;
        bool ok = splinePista.Evaluate(0, splineT, out posF, out tan, out up);
        if (!ok)
        {
            // fallback simples
            float3 nextPos = splinePista.EvaluatePosition(0, Mathf.Clamp01(splineT + 0.01f));
            tan = math.normalize(nextPos - posF);
        }
        return new Vector3(tan.x, tan.y, tan.z);
    }

    public Vector3 GetRight(float t)
    {
        Vector3 tangent = GetTangent(t).normalized;
        return Vector3.Cross(Vector3.up, tangent).normalized;
    }

    public float FindNearestT(Vector3 worldPos, int resolution = 8000)
    {
        float bestT = 0f;
        float bestDist = float.MaxValue;

        for (int i = 0; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            Vector3 p = GetPosition(t);

            float dist = (p - worldPos).sqrMagnitude;

            if (dist < bestDist)
            {
                bestDist = dist;
                bestT = t;
            }
        }

        return bestT;
    }

    public float DistanceFromT(float t, int resolution = 8000)
    {
        t = Mathf.Clamp01(t);

        float total = 0f;
        Vector3 prev = GetPosition(0f);

        for (int i = 1; i <= resolution; i++)
        {
            float ti = i / (float)resolution;

            Vector3 curr = GetPosition(ti);

            float seg = Vector3.Distance(curr, prev);

            if (ti >= t)
            {
                // adiciona apenas a parte necessária
                float extra = Vector3.Distance(curr, GetPosition(t));
                total += extra;
                return total;
            }

            total += seg;
            prev = curr;
        }

        return total;
    }

    /// <summary>
    /// Retorna um splineT suavizado para um carro, baseado na sua distância atual.
    /// </summary>
    public float GetSplineTSuave(float currentT, float distance, float dt, float smoothSpeed = 8f)
    {
        // Calcula o targetT normalizado pela distância
        float targetT = SplineTFromDistance(distance);

        // Calcula a diferença mínima considerando loop 0–1
        float delta = targetT - currentT;
        if (delta > 0.5f) delta -= 1f;
        if (delta < -0.5f) delta += 1f;

        // Suaviza
        float newT = currentT + delta * dt * smoothSpeed;

        // Mantém dentro do range 0..1
        if (newT > 1f) newT -= 1f;
        if (newT < 0f) newT += 1f;

        return newT;
    }



}
