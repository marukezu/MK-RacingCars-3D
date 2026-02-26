using UnityEngine;

public class CarSimVisual_Carroceria : MonoBehaviour
{
    public CarSimVisual carSimVisual;

    [Header("Configurações da carroceria")]
    public float alturaExtra = 0.55f;      // distância extra da carroceria acima das rodas
    public float suavizacaoPos = 5f;       // suavização do movimento vertical
    public float suavizacaoRot = 5f;       // suavização da rotação da carroceria

    private void Awake()
    {
        carSimVisual ??= GetComponent<CarSimVisual>();
    }

    public void AtualizarPosicaoCarroceria()
    {
        if (carSimVisual.suspensao == null) return;

        Transform[] rodas = { carSimVisual.suspensao.rodaFL, carSimVisual.suspensao.rodaFR, carSimVisual.suspensao.rodaRL, carSimVisual.suspensao.rodaRR };

        Vector3 mediaPos = Vector3.zero;
        float alturaChaoMin = float.MaxValue;
        int cont = 0;

        foreach (Transform r in rodas)
        {
            if (r == null) continue;

            mediaPos += r.position;

            if (Physics.Raycast(r.position + Vector3.up * 1f, Vector3.down, out RaycastHit hit, 5f))
            {
                alturaChaoMin = Mathf.Min(alturaChaoMin, hit.point.y);
            }

            cont++;
        }

        if (cont == 0) return;

        mediaPos /= cont;

        float alturaMaxima = mediaPos.y + alturaExtra * 1.15f;
        float alturaMinima = alturaChaoMin + alturaExtra;
        float alturaAlvo = Mathf.Max(alturaMinima, alturaMaxima);

        Vector3 alvoPos = new Vector3(transform.position.x, alturaAlvo, transform.position.z);

        // sobe/desce imediatamente
        transform.position = alvoPos;
    }

    public void AtualizarRotacaoCarroceria()
    {
        if (carSimVisual.suspensao == null) return;

        float yFL = carSimVisual.suspensao.rodaFL.position.y;
        float yFR = carSimVisual.suspensao.rodaFR.position.y;
        float yRL = carSimVisual.suspensao.rodaRL.position.y;
        float yRR = carSimVisual.suspensao.rodaRR.position.y;

        float frenteMedia = (yFL + yFR) * 0.5f;
        float trasMedia = (yRL + yRR) * 0.5f;
        float deltaX = frenteMedia - trasMedia;

        float esquerdaMedia = (yFL + yRL) * 0.5f;
        float direitaMedia = (yFR + yRR) * 0.5f;
        float deltaZ = direitaMedia - esquerdaMedia;

        float comprimento = carSimVisual.suspensao.distanciaEixos * 2f;
        float largura = carSimVisual.suspensao.distanciaLateral * 2f;

        float pitch = -Mathf.Atan2(deltaX, comprimento) * Mathf.Rad2Deg;
        float roll = Mathf.Atan2(deltaZ, largura) * Mathf.Rad2Deg;

        float yaw = transform.rotation.eulerAngles.y;

        Quaternion rotAlvo = Quaternion.Euler(pitch, yaw, roll);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotAlvo, Time.deltaTime * 10f);
    }
}
