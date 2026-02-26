using UnityEngine;

public class TimeAndDate_Manager : MonoBehaviour
{
    public static TimeAndDate_Manager Instance;

    [SerializeField] private Transform luzDirecional;
    [SerializeField] private int duracaoDiaEmSegundos = 1200; // exemplo

    private float segundos;      // 0 a 86400
    private float multiplicador; // aceleração do tempo

    private const float SEGUNDOS_DIA = 86400f;

    private void Awake()
    {
        Instance ??= this;
    }

    private void Start()
    {
        // Iniciar às 10h da manhã
        segundos = SEGUNDOS_DIA / 2.3f;

        SetDuracaoDia(duracaoDiaEmSegundos);
    }

    private void Update()
    {
        segundos += Time.deltaTime * multiplicador;

        if (segundos >= SEGUNDOS_DIA)
            segundos -= SEGUNDOS_DIA;

        AtualizarSol();
    }

    private void AtualizarSol()
    {
        // Converte o valor para 0 → 1 e aplica rotação suave
        float t = segundos / SEGUNDOS_DIA;
        float rotacaoX = Mathf.Lerp(-90f, 270f, t);

        luzDirecional.rotation = Quaternion.Euler(rotacaoX, 0f, 0f);
    }

    // Define quantos segundos o dia dura dentro do jogo
    public void SetDuracaoDia(int duracaoReal)
    {
        multiplicador = SEGUNDOS_DIA / duracaoReal;
    }

    public float GetSegundosAtuais() => segundos;

    public bool EstaDeNoite()
    {
        float hora = segundos / 3600f; // 0 → 24
        return hora < 6f || hora >= 17f;
    }
}
