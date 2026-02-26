using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PilotoSim_Ultrapassagem
{
    public enum Condicao
    {
        Nada,
        Ultrapassando,
        SendoUltrapassado,
        UltrapassagemSucesso,
        FalhouUltrapassagem,
        ProntoParaFinalizar,
    }

    public enum LadoDeUltrapassagem
    {
        Direita,
        Esquerda
    }

    private PilotoSim piloto;

    public bool PodeTentarUltrapassar => timers.delay_Tentativa_Ultrapassagem <= 0;
    public PilotoSim pilotoQueEstouUltrapassando { get; private set; }
    public PilotoSim pilotoMeUltrapassando { get; private set; }

    public Condicao condicao { get; private set; } = Condicao.Nada;
    public LadoDeUltrapassagem ladoDeUltrapassagem;

    // ====================
    //       STRUCTS
    // ====================

    private struct Timers
    {
        public float delay_Tentativa_Ultrapassagem; // Para o atacante.
    }

    private Timers timers;

    // ====================
    //      CONSTRUTOR
    // ====================
    public PilotoSim_Ultrapassagem(PilotoSim piloto)
    {
        this.piloto = piloto;

        condicao = Condicao.Nada;
    }

    // ===============================================================
    // ================== TICK - Simulation Engine ===================
    // ===============================================================

    public void Tick(float dt)
    {
        timers.delay_Tentativa_Ultrapassagem -= dt;
    }

    // ===============================================================
    // ============================ SETS =============================
    // ===============================================================

    public void Set_Delay_Ultrapassagem(float value)
    {
        timers.delay_Tentativa_Ultrapassagem = value;
    }

    public void Set_PilotoQueEstouUltrapassando(PilotoSim piloto)
    {
        pilotoQueEstouUltrapassando = piloto;
    }

    public void Set_PilotoMeUltrapassando(PilotoSim piloto)
    {
        pilotoMeUltrapassando = piloto;
    }

    public void Set_Condition(Condicao condicao)
    {
        this.condicao = condicao;
    }
}
