using UnityEngine;

public class PilotoSim_Mentalidade
{
    public enum TipoMentalidade
    {
        Defensivo,
        Forte,
        Ofensivo,
        Agressivo
    }

    public enum Personalidade
    {
        Cauteloso, // Personalidade de -> Tem um alvo na frente? Vamos atacar, mas tem alguém colado em mim, defender.
        Agressivo, // Personalidade de -> Piloto gosta de atacar, mas se tem perigo atrás, procura evitar
        TudoOuNada // Personalidade de -> Passar o da frente acima de tudo.
    }

    public PilotoSim piloto;

    // =====================================================================
    // =========================== PROPRIEDADES ============================
    // =====================================================================

    // Mentalidade selecionada
    public TipoMentalidade MentalidadeAtual => dados.mentalidade;
    public Personalidade P_Personalidade => dados.personalidade;

    // Mentalidade do Piloto

    // Multiplicadores expostos para o SE
    public float MultVelocidade => dados.multVelocidade;
    public float MultConsumo => dados.multConsumo;
    public float MultRegen => dados.multRegen;
    public float MultDuelo_Defensor => dados.multDueloDefensor;
    public float MultDuelo_Atacante => dados.multDueloAtacante;
    public float MultNitro => dados.multConsumoNitro;

    // Bools
    public bool escolhaTravada => timers.delayTrocaMentalidade > 0 || bloqueadaPorDuelo;
    public bool bloqueadaPorDuelo = false;

    // =====================================================================
    // ============================= STRUCTS ===============================
    // =====================================================================

    private struct DadosMentalidade
    {
        public TipoMentalidade mentalidade;
        public Personalidade personalidade;

        // multiplicadores finais (calculados no SetMentalidade)
        public float multVelocidade;
        public float multRegen;
        public float multConsumo;
        public float multDueloDefensor;
        public float multDueloAtacante;
        public float multConsumoNitro;
    }

    private struct Timers
    {
        public float delayTrocaMentalidade;

        public void Tick(float dt)
        {
            delayTrocaMentalidade -= dt;
        }
    }

    private DadosMentalidade dados;
    private Timers timers;

    // =====================================================================
    // ============================= CONSTRUTOR ============================
    // =====================================================================

    public PilotoSim_Mentalidade(PilotoSim piloto, Personalidade personalidade)
    {
        this.piloto = piloto;

        dados = new DadosMentalidade
        {
            mentalidade = TipoMentalidade.Forte, // padrão equilibrado
            personalidade = personalidade
        };

        timers = new Timers();

        AplicarMentalidade(TipoMentalidade.Forte);
    }

    // =====================================================================
    // =========================== MÉTODOS PÚBLICOS =========================
    // =====================================================================

    public void Atualizar(float dt)
    {
        timers.Tick(dt);
    }

    public void SelecionarMentalidade(TipoMentalidade tipo, bool IA)
    {
        // Garante que não seja usada nem por player nem por piloto se a mentalidade estiver travada.
        if (escolhaTravada && piloto.IA_Controla_Mentalidade)
            return;

        // se for a IA, trava escolha por 2.5s
        if (IA) 
            Set_Delay_TrocarMentalidade(2.5f);

        // Aplica a nova mentalidade
        AplicarMentalidade(tipo);
    }

    // =====================================================================
    // ============================ LÓGICA INTERNA ==========================
    // =====================================================================

    private void AplicarMentalidade(TipoMentalidade tipo)
    {
        dados.mentalidade = tipo;

        switch (tipo)
        {
            case TipoMentalidade.Defensivo:
                dados.multVelocidade = 0.99f;     // -1%
                dados.multRegen = 2.00f;     // regen 2x
                dados.multConsumo = 0.9f;     // consumo 0.9x
                dados.multDueloDefensor = 1.40f;     // +40% força defendendo
                dados.multDueloAtacante = 1.00f;     // sem bonus/debuff atacando
                dados.multConsumoNitro = 1.00f;     // normal
                break;

            case TipoMentalidade.Forte:
                dados.multVelocidade = 1.005f;    // +0.5%
                dados.multRegen = 1.00f;     // regen 1x
                dados.multConsumo = 1.00f;     // consumo 1x
                dados.multDueloDefensor = 1.15f;     // +15%
                dados.multDueloAtacante = 1.10f;     // +10%
                dados.multConsumoNitro = 1.00f;
                break;

            case TipoMentalidade.Ofensivo:
                dados.multVelocidade = 1.0075f;   // +0.75%
                dados.multRegen = 0.95f;     // regen 0.95x
                dados.multConsumo = 1.05f;     // consumo 1.05x
                dados.multDueloDefensor = 0.55f;     // +45% chance perder defesa
                dados.multDueloAtacante = 1.25f;     // +25% atacando
                dados.multConsumoNitro = 0.90f;     // nitro dura mais
                break;

            case TipoMentalidade.Agressivo:
                dados.multVelocidade = 1.01f;     // +1%
                dados.multRegen = 0.90f;     // regen 0.9x
                dados.multConsumo = 1.10f;     // consumo 1.10x
                dados.multDueloDefensor = 0.10f;     // 90% risco perder defesa
                dados.multDueloAtacante = 1.40f;     // +40% atacando
                dados.multConsumoNitro = 0.85f;     // nitro estendido
                break;
        }

        EventBus.On_Mentalidade_Changed?.Invoke(piloto, MentalidadeAtual);
    }

    public void Set_Delay_TrocarMentalidade(float value)
    {
        timers.delayTrocaMentalidade = value;
    }
}
