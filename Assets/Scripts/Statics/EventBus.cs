using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PilotoSim_Mentalidade;

public static class EventBus
{
    // ===================================================================
    // ============================ Car Info =============================
    // ===================================================================

    // Motor e Marcha
    public static Action<PilotoSim, float> On_Car_SpeedChanged { get; set; }
    public static Action<PilotoSim, int> On_Car_ShiftChanged { get; set; }
    public static Action<PilotoSim, float> On_Car_RPMChanged { get; set; }

    // ===================================================================
    // =========================== Piloto Info ===========================
    // ===================================================================

    // Posição
    public static Action<PilotoSim, int> On_Piloto_PosicaoAtualChanged { get; set; }

    // Módulo - Atributos
    public static Action<PilotoSim, float> On_Atributo_Moral_Changed { get; set; }
    public static Action<PilotoSim, float> On_Atributo_Aceleracao_Changed { get; set; }
    public static Action<PilotoSim, float> On_Atributo_Ultrapassagem_Changed { get; set; }
    public static Action<PilotoSim, float> On_Atributo_Defesa_Changed { get; set; }
    public static Action<PilotoSim, float> On_Atributo_DesgastePneus_Changed { get; set; }
    public static Action<PilotoSim, float> On_Atributo_ConsumoCombustivel_Changed { get; set; }

    // Módulo - Nitro
    public static Action<PilotoSim, float> On_Nitro_NitroChanged { get; set; }

    // Modulo - Ultrapassagem (PilotoSim_Ultrapassagem && SE_Modulo_Ultrapassagem)

    // Módulo - Modos de Condução
    public static Action<PilotoSim, float> On_ModosConducao_ConducaoChanged { get; set; }
    public static Action<PilotoSim, float> On_ModosConducao_PotenciaChanged { get; set; }
    public static Action<PilotoSim, float> On_ModosConducao_Car_ConditionCombustivelChanged { get; set; }
    public static Action<PilotoSim, float> On_ModosConducao_Car_ConditionPneusChanged { get; set; }
    public static Action<PilotoSim, SE_Modulo_ModosConducao.TrendLevel> On_ModosConducao_PneusTrendChanged { get; set; }
    public static Action<PilotoSim, SE_Modulo_ModosConducao.TrendLevel> On_ModosConducao_CombustivelTrendChanged { get; set; }

    // Módulo - Mentalidade
    public static Action<PilotoSim, TipoMentalidade> On_Mentalidade_Changed { get; set; }


}
