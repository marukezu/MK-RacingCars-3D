using System.Collections.Generic;
using UnityEngine;
using static PilotoSim_Mentalidade;
using static UnityEditor.PlayerSettings;

public static class PilotosDataBase
{
    public static List<PilotoSim> Pilotos_CampeonatoAtual;

    static PilotosDataBase()
    {
        Pilotos_CampeonatoAtual = new List<PilotoSim>()
        {
            //            ID, Nome,             Personalidade               Acel,  Ultra, Defesa, Desgaste, Combustivel
            new PilotoSim(6, "Marukezu",        Personalidade.Agressivo,     0.80f, 0.72f, 0.80f, 0.67f, 0.83f),

            // 🥇 1. Max Verstappen – extremamente agressivo, dominante
            new PilotoSim(1, "Max Verstappen",  Personalidade.Agressivo,     0.90f, 0.92f, 0.86f, 0.78f, 0.75f),

            // 🥈 2. Lewis Hamilton – defensivo forte, ultrapassagem excepcional
            new PilotoSim(2, "Lewis Hamilton",  Personalidade.Agressivo,         0.85f, 0.89f, 0.90f, 0.80f, 0.82f),

            // 🥉 3. Lando Norris – rápido, consistente, extremamente limpo
            new PilotoSim(3, "Lando Norris",    Personalidade.Cauteloso,     0.84f, 0.82f, 0.83f, 0.85f, 0.88f),

            // Charles Leclerc – extremamente rápido, mas desgaste maior
            new PilotoSim(4, "Charles Leclerc", Personalidade.TudoOuNada,     0.88f, 0.86f, 0.78f, 0.72f, 0.70f),

            // George Russell – bom defensor, muito consistente
            new PilotoSim(5, "George Russell",  Personalidade.Cauteloso,     0.82f, 0.80f, 0.86f, 0.83f, 0.80f),

            // Carlos Sainz – estável, estrategista, desgaste e defesa fortes
            new PilotoSim(7, "Carlos Sainz",    Personalidade.Agressivo,         0.80f, 0.78f, 0.87f, 0.88f, 0.84f),

            // Sergio Pérez – bom gerenciador de pneus, defensor sólido
            new PilotoSim(8, "Sergio Pérez",    Personalidade.Agressivo,     0.78f, 0.83f, 0.84f, 0.90f, 0.85f)

        };
    }
}
public class PilotoSim
{
    public enum EstadoDoPiloto
    {
        Pista,
        EntradaPit,
        SaidaPit,
        Parado,
    }

    public CarSimVisual carsim;
    public EstadoDoPiloto estado;

    // Básicos
    public int ID => basicos.ID;
    public string Nome => basicos.Nome;

    // Simulação
    public float Distance => simulation.distance;
    public float PrevDistance => simulation.prevDistance;
    public float DistanceTotal => simulation.distanceTotal;
    public float SplineT => simulation.splineT;
    public bool PassouLargadaPrimeiraVez => simulation.passouLargadaPrimeiraVez;

    // Corrida
    public int PosicaoGrid => corrida.posicaoGrid;
    public int PosicaoCorrida => corrida.posicaoCorrida;

    // Delegação de Comandos
    public bool IA_Controla_Conducao => delegarComandos.IA_Controla_Conducao;
    public bool IA_Controla_Nitro => delegarComandos.IA_Controla_Nitro;
    public bool IA_Controla_Mentalidade => delegarComandos.IA_Controla_Mentalidade;

    // ==========================
    //      STRUCTS DE DADOS
    // ==========================

    private struct DadosBasicos
    {
        public int ID;
        public string Nome;
    }

    private struct DadosDeSimulacao
    {
        public float distance;
        public float prevDistance;
        public float distanceTotal;
        public float speed;
        public float splineT;
        public bool passouLargadaPrimeiraVez;
    }

    private struct DadosDaCorrida
    {
        public int posicaoGrid;
        public int posicaoCorrida;
    }

    private struct DadosDelegarComandos
    {
        public bool IA_Controla_Conducao;
        public bool IA_Controla_Nitro;
        public bool IA_Controla_Mentalidade;
    }

    // ==========================
    //   INSTANCIAS DE STRUCTS
    // ==========================

    private DadosBasicos basicos;
    private DadosDeSimulacao simulation;
    private DadosDaCorrida corrida;
    private DadosDelegarComandos delegarComandos;

    // ==========================
    //     MÓDULOS PILOTOSIM
    // ==========================

    public PilotoSim_Atributos mod_Atributos;
    public PilotoSim_AceleracaoFrenagem mod_acelFrenagem;
    public PilotoSim_PosicionamentoLateral mod_PosLateral;
    public PilotoSim_Mentalidade mod_Mentalidade;
    public PilotoSim_Ultrapassagem mod_Ultrapassagem;
    public PilotoSim_ModosConducao mod_Conducao;

    // ==========================
    //        CONSTRUTOR
    // ==========================

    public PilotoSim(
        int id,
        string nome,
        Personalidade personalidade,
        float acel,
        float ultra,
        float defesa,
        float desgaste,
        float combustivel)
    {
        // Basicos
        basicos = new DadosBasicos
        {
            ID = id,
            Nome = nome
        };

        // Dados da Corrida
        corrida = new DadosDaCorrida
        {
            posicaoGrid = id,
            posicaoCorrida = id
        };

        // Dados de simulação iniciam zerados automaticamente
        simulation = new DadosDeSimulacao();

        // Dados de delegação de comandos (Controle de Potências em corrida)
        delegarComandos = new DadosDelegarComandos
        {
            IA_Controla_Conducao = true,
            IA_Controla_Nitro = true,
            IA_Controla_Mentalidade = true
        };

        estado = EstadoDoPiloto.Pista;

        // Módulo de Atributos
        mod_Atributos = new PilotoSim_Atributos(
            this,
            acel,
            ultra,
            defesa,
            desgaste,
            combustivel);

        // Demais módulos
        mod_PosLateral = new PilotoSim_PosicionamentoLateral(this);
        mod_acelFrenagem = new PilotoSim_AceleracaoFrenagem(this);
        mod_Mentalidade = new PilotoSim_Mentalidade(this, personalidade);
        mod_Ultrapassagem = new PilotoSim_Ultrapassagem(this);
        mod_Conducao = new PilotoSim_ModosConducao(this);
    }

    // ========================================================================
    // ================================= SETS =================================
    // ========================================================================

    // Dados de Simulação.
    public void Set_PassouLargadaPrimeiraVez(bool value)
    {
        simulation.passouLargadaPrimeiraVez = value;
    }

    public void Set_SplineT(float t)
    {
        simulation.splineT = t;
    }

    public void Set_PrevDistance(float distance)
    {
        simulation.prevDistance = distance;
    }

    public void Set_Distance(float distance)
    {
        simulation.distance = distance;
    }

    public void Set_DistanceTotal(float distanceReal)
    {
        simulation.distanceTotal = distanceReal;
    }

    // Dados de Corrida
    public void Set_GridPosition(int value)
    {
        corrida.posicaoGrid = value;
    }

    public void Set_PosicaoAtual(int value)
    {
        corrida.posicaoCorrida = value;
        EventBus.On_Piloto_PosicaoAtualChanged?.Invoke(this, corrida.posicaoCorrida);
    }

    // Dados de Delegação de Comandos
    public void Set_DelegarConducao(bool delegar)
    {
        delegarComandos.IA_Controla_Conducao = delegar;
    }
    public void Set_DelegarNitro(bool delegar)
    {
        delegarComandos.IA_Controla_Nitro = delegar;
    }
    public void Set_DelegarMentalidade(bool delegar)
    {
        delegarComandos.IA_Controla_Mentalidade = delegar;
    }
}
