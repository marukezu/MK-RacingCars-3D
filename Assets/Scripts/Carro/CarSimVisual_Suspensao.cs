using UnityEngine;

[DisallowMultipleComponent]
public class CarSimVisual_Suspensao : MonoBehaviour
{
    public CarSimVisual carSimVisual;

    [Header("Rodas")]
    public Transform rodaFL;
    public Transform rodaFR;
    public Transform rodaRL;
    public Transform rodaRR;

    [Header("Configuração")]
    public float distanciaEixos = 1f;           // Distancia calculada a partir do CENTRO do carro.
    public float distanciaLateral = 0.75f;       // Distancia lateral a partir do CENTRO do carro.
    public float alturaSuspensao = 0f;        // altura da roda acima do chão
    public float comprimentoRaycast = 5f;       // distância acima da roda para iniciar o raycast
    public float suavizacao = 10f;              // suavização do movimento

    [HideInInspector]
    public Vector3 pontoChaoAtual;              // posição atual da roda no chão
    [HideInInspector]
    public Vector3 normalChao;                  // normal do chão embaixo da roda

    private void Awake()
    {
        carSimVisual ??= GetComponent<CarSimVisual>();
    }

    public void AtualizarPosicao()
    {
        if (carSimVisual == null || carSimVisual.spline == null || carSimVisual.piloto == null) return;

        // 1️⃣ Pega posição e rotação da spline
        Vector3 posSpline = carSimVisual.spline.GetPosition(carSimVisual.piloto.SplineT);
        Quaternion rotSpline = carSimVisual.spline.GetRotation(carSimVisual.piloto.SplineT);

        // Aplica lateralOffset do piloto
        Vector3 rightSpline = carSimVisual.spline.GetRight(carSimVisual.piloto.SplineT);
        posSpline += rightSpline * carSimVisual.piloto.mod_PosLateral.offsetReal * 2f;

        // 2️⃣ Calcula posição base de cada roda a partir do centro e rotação da spline
        Vector3 rodaFLBase = posSpline + rotSpline * new Vector3(-distanciaLateral, 0f, distanciaEixos);
        Vector3 rodaFRBase = posSpline + rotSpline * new Vector3(distanciaLateral, 0f, distanciaEixos);
        Vector3 rodaRLBase = posSpline + rotSpline * new Vector3(-distanciaLateral, 0f, -distanciaEixos);
        Vector3 rodaRRBase = posSpline + rotSpline * new Vector3(distanciaLateral, 0f, -distanciaEixos);

        // Array de rodas e posições base
        Transform[] rodas = { rodaFL, rodaFR, rodaRL, rodaRR };
        Vector3[] baseRodas = { rodaFLBase, rodaFRBase, rodaRLBase, rodaRRBase };

        // 3️⃣ Atualiza posição Y de cada roda via raycast
        for (int i = 0; i < rodas.Length; i++)
        {
            Transform r = rodas[i];
            if (r == null) continue;

            Vector3 origem = baseRodas[i] + Vector3.up * comprimentoRaycast;
            if (Physics.Raycast(origem, Vector3.down, out RaycastHit hit, comprimentoRaycast * 2f))
            {
                Vector3 alvo = hit.point + Vector3.up * alturaSuspensao;
                r.position = alvo;
            }
            else
            {
                // fallback: mantém posição base + alturaSuspensao
                Vector3 fallback = baseRodas[i] + Vector3.up * alturaSuspensao;
                r.position = fallback;
            }
        }
    }

    public void AtualizarRotacao()
    {
        if (carSimVisual == null || carSimVisual.spline == null || carSimVisual.piloto == null) return;

        // Tangente da spline (direção do movimento)
        Vector3 forwardSpline = carSimVisual.spline.GetTangent(carSimVisual.piloto.SplineT).normalized;

        // Array de rodas
        Transform[] rodas = { rodaFL, rodaFR, rodaRL, rodaRR };

        // Para cada roda, alinhamos com o solo usando Raycast
        foreach (Transform r in rodas)
        {
            if (r == null) continue;

            // Faz um raycast para pegar a normal do chão
            if (Physics.Raycast(r.position + Vector3.up * 1f, Vector3.down, out RaycastHit hit, comprimentoRaycast))
            {
                // Normal do chão
                Vector3 normalChao = hit.normal;

                // Tangente projetada no plano do chão para evitar rotação "flipping"
                Vector3 forwardProj = Vector3.ProjectOnPlane(forwardSpline, normalChao).normalized;

                // Gira a roda: LookRotation com tangente e normal
                Quaternion alvoRot = Quaternion.LookRotation(forwardProj, normalChao);

                // Suavização opcional
                r.rotation = Quaternion.Slerp(r.rotation, alvoRot, Time.deltaTime * suavizacao);
            }
        }
    }

}
