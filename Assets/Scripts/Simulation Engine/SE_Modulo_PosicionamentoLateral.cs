using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PilotoSim_Ultrapassagem;
using static SE_Modulo_AceleracaoFrenagem;

public class SE_Modulo_PosicionamentoLateral
{
    public SimulationEngine simEngine;

    public SE_Modulo_PosicionamentoLateral(SimulationEngine simEngine)
    {
        this.simEngine = simEngine;
    }

    public void AtualizarFaixaCarro(PilotoSim p, int idx, List<PilotoSim> ordenados, float dt)
    {
        if (!simEngine.raceManager.largadaAutorizada)
            return;

        float targetOffset = p.mod_PosLateral.baseOffset;

        // Se estiver ultrapassando
        if (p.mod_Ultrapassagem.condicao == Condicao.Ultrapassando)
        {
            float ultrapassadoOffset =
                p.mod_Ultrapassagem.pilotoQueEstouUltrapassando.mod_PosLateral.baseOffset;

            // Coloca no lado correto, dependendo da seleção no modulo de ultrapassagem
            if (p.mod_Ultrapassagem.ladoDeUltrapassagem == LadoDeUltrapassagem.Direita)
                targetOffset = 1.25f + ultrapassadoOffset;

            else
                targetOffset = -1.25f + ultrapassadoOffset;
        }

        // Suavização dependente da velocidade
        float velocidade = Mathf.Clamp(p.mod_acelFrenagem.Car_SpeedKM, 0.1f, BASE_SPEED_KMH);
        float fator = velocidade / BASE_SPEED_KMH;
        float suavizacao = Mathf.Lerp(0f, 1.5f, fator);

        float novoOffset =
            Mathf.Lerp(p.mod_PosLateral.offsetReal, targetOffset, dt * suavizacao);

        p.mod_PosLateral.Set_OffsetReal(novoOffset);
    }

}
