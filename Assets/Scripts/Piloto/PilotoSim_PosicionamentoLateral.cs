using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PilotoSim_PosicionamentoLateral
{
    public PilotoSim piloto;

    // Alvo “natural” do piloto (suave)
    public float baseOffset { get; private set; }

    // Offset real suavizado (posição atual)
    public float offsetReal { get; private set; }

    private float offsetTimer = 0f;

    public PilotoSim_PosicionamentoLateral(PilotoSim piloto)
    {
        this.piloto = piloto;
        baseOffset = 0;
        offsetReal = 0;
    }

    public void Tick(float dt)
    {
        offsetTimer -= dt;

        // Quando timer estoura → escolhe NOVO alvo suave
        if (offsetTimer <= 0f)
        {
            baseOffset = Random.Range(-0.6f, 0.6f);  // NOVO ALVO
            offsetTimer = Random.Range(2f, 4f);
        }
    }

    public void Set_OffsetReal(float value)
    {
        offsetReal = value;
    }
}

