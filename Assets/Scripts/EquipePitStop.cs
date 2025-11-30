using System.Collections;
using UnityEngine;

public class EquipePitStop
{
    // Enums
    public enum PitStopStatus
    {
        Parado,
        Saindo,
        Reabastecendo,
        TrocandoPneus,
        Erro
    }

    // Constantes
    private const float PESO_ERRO_TROCA_PNEU = 10f;
    private const float PESO_ERRO_REABASTECER = 5f;
    private const float TEMPO_REABASTECER_BASAL = 5f;
    private const float TEMPO_TROCA_PNEUS_BASAL = 2f;

    private const float TEMPO_PENALIDADE_ERRO_PITSTOP = 5f;

    // ===============================================================

    // Habilidade do pessoal do PitStop
    public float HabilidadeReabastecer { get; private set; }
    public float HabilidadeTrocaPneus { get; private set; }

    // Tempo total gasto no pit.
    public float TempoTotalPit { get; private set; }

    // Tipo de pneu que será instalado.
    public Pneu.TipoPneu TipoPneu { get; set; } = Pneu.TipoPneu.Macio;

    // Status atual do pitstop.
    public PitStopStatus StatusPitStop { get; set; } = PitStopStatus.Parado;

    // Booleanas de controle
    public bool PitStopNestaVolta { get; set; } = false;
    public bool EntrouNoPitStopNessaVolta { get; set; } = false;
    public bool ParadoNoPitStop { get; set; } = false;
    public bool ConsertandoNoPit { get; set; } = false;
    public bool Reparar { get; set; } = false;
    public bool VaiTrocarPneu { get; set; }
    public bool VaiReabastecer { get; set; }
    public bool AconteceuErro { get; set; } = false;


    // Contadores.
    public float contadorStatusAtual = 1f;

    public EquipePitStop(float habilidadeReab, float habilidadeTrocaPn)
    {
        HabilidadeReabastecer = habilidadeReab;
        HabilidadeTrocaPneus = habilidadeTrocaPn;
    }

    // ==================================================================================================================================================
    // ================================================================ REALIZA O PITSTOP ===============================================================
    // ==================================================================================================================================================
    public IEnumerator RealizarReparo(Carro carro, MonoBehaviour executor)
    {
        // Coloca como false no inicio para nao bugar o pilotoController.
        Reparar = false;

        if (VaiTrocarPneu)
            yield return executor.StartCoroutine(TrocarPneus(carro));

        if (VaiReabastecer)
            yield return executor.StartCoroutine(Reabastecer(carro, GetTempoTotalReabastecer(carro)));

        if (AconteceuErro)
            yield return executor.StartCoroutine(ProcessaErroPit());

        // Após realizar o PitStop, seta as variáveis.]
        AconteceuErro = false;
        StatusPitStop = PitStopStatus.Parado;
        ConsertandoNoPit = false;
        yield return null;
    }

    private IEnumerator TrocarPneus(Carro carro)
    {
        // Seta o status do pitstop para Trocando Pneus.
        StatusPitStop = PitStopStatus.TrocandoPneus;

        // Sorteia o erro conforme a chance.
        float sorteio = Random.Range(0, 100);
        if (sorteio <= GetChanceErroTrocaPneus(carro))
            AconteceuErro = true;

        // Obtém o tempo total de troca de pneus.
        float tempoTotal = GetTempoTotalTrocaPneus();
        float tempoDecorrido = 0f;

        // Executa o laço enquanto o tempo decorrido é menor que o tempo total.
        while (tempoDecorrido < tempoTotal)
        {
            // Incrementa o tempo decorrido com o delta do tempo.
            tempoDecorrido += Time.deltaTime;

            // Atualiza o slider de progresso (valor entre 0 e 1).
            contadorStatusAtual = tempoDecorrido / tempoTotal;

            // Espera até o próximo quadro.
            yield return null;
        }

        // Diminui um pneu total do piloto para o campeonato.
        carro.pilotoController.piloto.DiminuirQntPneu(carro.pilotoController.piloto.EquipePitStop.TipoPneu);

        // Atualiza os pneus do carro.
        carro.carroceria.pneuFL = new Pneu(TipoPneu, carro, Pneu.SlotPneu.FL);
        carro.carroceria.pneuFR = new Pneu(TipoPneu, carro, Pneu.SlotPneu.FR);
        carro.carroceria.pneuRL = new Pneu(TipoPneu, carro, Pneu.SlotPneu.RL);
        carro.carroceria.pneuRR = new Pneu(TipoPneu, carro, Pneu.SlotPneu.RR);
    }


    private IEnumerator Reabastecer(Carro carro, float tempoTotal)
    {
        // Seta o status do pitstop para Reabastecendo.
        StatusPitStop = PitStopStatus.Reabastecendo;

        // Sorteia o erro conforme a chance.
        float sorteio = Random.Range(0, 100);
        if (sorteio <= GetChanceErroReabastecer(carro))
            AconteceuErro = true;

        float valorFinal = 100f;
        float incrementoPorSegundo = (valorFinal - carro.carroceria.combustivelAtual) / tempoTotal;

        // Variável para controlar o tempo decorrido.
        float tempoDecorrido = 0f;

        while (tempoDecorrido < tempoTotal)
        {
            // Incrementa o tempo decorrido.
            tempoDecorrido += Time.deltaTime;

            // Atualiza o combustível do carro.
            carro.carroceria.combustivelAtual += incrementoPorSegundo * Time.deltaTime;
            carro.carroceria.combustivelAtual = Mathf.Min(carro.carroceria.combustivelAtual, valorFinal);

            // Atualiza o contador de status com o progresso (0 a 1).
            contadorStatusAtual = tempoDecorrido / tempoTotal;

            // Aguarda o próximo quadro.
            yield return null;
        }

        // Certifica-se de que o progresso está em 1 ao terminar.
        contadorStatusAtual = 1f;
    }
    private IEnumerator ProcessaErroPit()
    {
        // Seta o status para Erro.
        StatusPitStop = PitStopStatus.Erro;

        // Tempo adicional por erro.
        float tempoErro = TEMPO_PENALIDADE_ERRO_PITSTOP;
        float tempoDecorrido = 0f;

        // Executa o laço para o tempo de erro.
        while (tempoDecorrido < tempoErro)
        {
            // Incrementa o tempo decorrido.
            tempoDecorrido += Time.deltaTime;

            // Atualiza o slider de progresso com base na proporção (0 a 1).
            contadorStatusAtual = tempoDecorrido / tempoErro;

            // Espera até o próximo quadro.
            yield return null;
        }

        // Certifica-se de que o progresso está em 1 ao terminar.
        contadorStatusAtual = 1f;

        // Volta ao status anterior ou mantém o controle de erro.
        AconteceuErro = false; // Reseta o erro para evitar impactos futuros.
    }


    // ==================================================================================================================================================
    // ================================================================ CALCULO DE TEMPOS ===============================================================
    // ==================================================================================================================================================
    public float GetTempoTotalPit(Carro carro)
    {
        TempoTotalPit = 0f;
        if (VaiReabastecer)
            TempoTotalPit += GetTempoTotalReabastecer(carro);

        if (VaiTrocarPneu)
            TempoTotalPit += GetTempoTotalTrocaPneus();

        return TempoTotalPit;
    }
    public float GetTempoTotalReabastecer(Carro carro)
    {
        float fatorCombustivelRestante = (1 - (carro.carroceria.combustivelAtual / 100));
        float fatorHabilidade = 1.5f - (HabilidadeReabastecer / 100);
        return (TEMPO_REABASTECER_BASAL * fatorHabilidade) * fatorCombustivelRestante;
    }

    public float GetTempoTotalTrocaPneus()
    {
        float fatorHabilidade = 1.5f - (HabilidadeTrocaPneus / 100);
        return TEMPO_TROCA_PNEUS_BASAL * fatorHabilidade;
    }
    // ==================================================================================================================================================
    // =========================================================== CALCULO DE CHANCES DE ERRO ===========================================================
    // ==================================================================================================================================================
    public float GetChanceErroTotal(Carro carro)
    {
        float chanceErro = 0f;

        chanceErro += (1.5f - (HabilidadeTrocaPneus / 100)) * PESO_ERRO_TROCA_PNEU;
        chanceErro += ((1.5f - (HabilidadeReabastecer / 100)) * PESO_ERRO_REABASTECER) * (1 - (carro.carroceria.combustivelAtual / 100));

        return chanceErro;
    }

    public float GetChanceErroTrocaPneus(Carro carro)
    {
        float chanceErro = 0f;
        chanceErro += (1.5f - (HabilidadeTrocaPneus / 100)) * PESO_ERRO_TROCA_PNEU;

        return chanceErro;
    }
    public float GetChanceErroReabastecer(Carro carro)
    {
        float chanceErro = 0f;
        chanceErro += ((1.5f - (HabilidadeReabastecer / 100)) * PESO_ERRO_REABASTECER) * (1 - (carro.carroceria.combustivelAtual / 100));

        return chanceErro;
    }

    public string GetPitStopStatus()
    {
        switch (StatusPitStop)
        {
            case PitStopStatus.Parado:
                return "Aguardando";

            case PitStopStatus.Saindo:
                return "Saindo dos boxes";

            case PitStopStatus.Reabastecendo:
                return "Abastecendo";

            case PitStopStatus.TrocandoPneus:
                return "Trocando Pneus";

            case PitStopStatus.Erro:
                return "Erro de pit! Resolvendo...";

            default:
                return "ERRO";
        }
    }
}
