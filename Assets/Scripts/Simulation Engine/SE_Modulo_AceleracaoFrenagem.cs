using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PilotoSim_Ultrapassagem;

public class SE_Modulo_AceleracaoFrenagem
{
    // Constantes
    public const float BASE_SPEED_KMH = 260;

    public SimulationEngine simEngine;

    public SE_Modulo_AceleracaoFrenagem(SimulationEngine simEngine)
    {
        this.simEngine = simEngine;
    }

    // =================================================================================
    // == MÉTODO PRINCIPAL, RESPONSÁVEL POR CALCULAR ACELERAÇÃO E FRENAGEM DO VEICULO ==
    // =================================================================================
    public void CalcularVelocidadeFinal(PilotoSim p, int idx, List<PilotoSim> ordenados, float dt)
    {
        if (!simEngine.raceManager.largadaAutorizada)
            return;

        // =====================================
        // 1) VELOCIDADE BRUTA (RETA)
        // =====================================
        float rawKMH =
            Calcular_Velocidade_Base() *
            Calcular_Multiplicador_Habilidade(p) *
            Calcular_Multiplicador_Mentalidade(p) *
            Calcular_Multiplicador_ModosConducao(p) *
            Calcular_Multiplicador_PneusECombustivel(p) *
            Calcular_Multiplicador_Ultrapassagens(p);

        float rawMS = rawKMH / 3.6f;

        // =====================================
        // 2) LIMITA POR CURVA
        // =====================================
        float curvaMult = Calcular_Multiplicador_Curva(p);
        float curvaLimitMS = rawMS * curvaMult;

        // =====================================
        // 3) LIMITA POR COLISÃO COM CARRO A FRENTE
        // =====================================
        float colisaoLimitMS = curvaLimitMS; // começa igual
        if (idx > 0)
        {
            PilotoSim frente = ordenados[idx - 1];
            float dist = frente.DistanceTotal - p.DistanceTotal;

            float minDist = Mathf.Lerp(3f, 6f, p.mod_acelFrenagem.Car_SpeedKM / BASE_SPEED_KMH);

            // se estiver entrando em distância perigosa
            if (dist < minDist)
            {
                // Se o alvo a frente NÃO estiver envolvido em ultrapassagem comigo. 
                if (!ReferenceEquals(frente, p.mod_Ultrapassagem.pilotoQueEstouUltrapassando) &&
                    !ReferenceEquals(frente, p.mod_Ultrapassagem.pilotoMeUltrapassando))
                        // aproxima da frente da frente
                        colisaoLimitMS = Mathf.Min(colisaoLimitMS, frente.mod_acelFrenagem.Car_SpeedMS * 0.95f);
            }
        }

        // =====================================
        // 4) Escolhe targetSpeed final
        // =====================================
        float targetMS = Mathf.Min(rawMS, curvaLimitMS, colisaoLimitMS);

        // =====================================
        // 5) Define forças
        // =====================================
        float accForce = (targetMS > p.mod_acelFrenagem.Car_SpeedMS) ? 1f : 0f;
        float brakeForce = (targetMS < p.mod_acelFrenagem.Car_SpeedMS) ? 1f : 0f;

        // Pode sofisticar amanhã — por enquanto, simples e funcional.

        const float ACCEL_RATE = 7.5f; // “quão rápido acelera”
        const float BRAKE_RATE = 20f; // “quão rápido freia”

        // =====================================
        // 6) Move velocidade real
        // =====================================
        float atual = p.mod_acelFrenagem.Car_SpeedMS;

        // acelera para cima
        atual = Mathf.MoveTowards(atual, targetMS, ACCEL_RATE * accForce * dt);

        // freia para baixo
        atual = Mathf.MoveTowards(atual, targetMS, BRAKE_RATE * brakeForce * dt);

        // =====================================
        // 7) Aplica
        // =====================================
        p.mod_acelFrenagem.SetSpeedMS(atual);
    }

    // =================================================================================
    // =========== MÉTODOS - CALCULOS DE VELOCIDADE FINAL E MULTIPLICADORES ============
    // =================================================================================

    private float Calcular_Velocidade_Base()
    {
        // 1) Defina uma base fixa por carro/categoria
        float baseSpeed = BASE_SPEED_KMH;

        return baseSpeed;
    }

    private float Calcular_Multiplicador_Ultrapassagens(PilotoSim p)
    {
        float multiplicador = 1f;

        // Se estou sendo ultrapassado
        if (p.mod_Ultrapassagem.condicao == Condicao.SendoUltrapassado)
        {
            // Se quem está me ultrapassando está setado como Ultrapassando(não falhou ainda)
            if (p.mod_Ultrapassagem.pilotoMeUltrapassando.mod_Ultrapassagem.condicao == Condicao.Ultrapassando)
                multiplicador = 0.98f;
        }            

        else if (p.mod_Ultrapassagem.condicao == Condicao.Ultrapassando)
            multiplicador = 1.02f;

        else if (p.mod_Ultrapassagem.condicao == Condicao.FalhouUltrapassagem)
            multiplicador = 0.98f;

        return multiplicador;
    }

    private float Calcular_Multiplicador_Habilidade(PilotoSim p)
    {
        float fatorHabilidade = Mathf.Lerp(0.8f, 1, p.mod_Atributos.H_Acel);
        return fatorHabilidade; // 0.8 ~ 1
    }

    private float Calcular_Multiplicador_Mentalidade(PilotoSim p)
    {
        return p.mod_Mentalidade.MultVelocidade;
    }

    private float Calcular_Multiplicador_PneusECombustivel(PilotoSim p)
    {
        float condPneus = p.mod_Conducao.Cond_Pneus;
        float condCombustivel = p.mod_Conducao.Cond_Combustivel;

        float fatorPneus = Mathf.Lerp(0.95f, 1, condPneus / 100);
        float fatorCombustivel = Mathf.Lerp(0.95f, 1, condCombustivel / 100);

        return (fatorPneus + fatorCombustivel) / 2;
    }

    private float Calcular_Multiplicador_ModosConducao(PilotoSim p)
    {
        float condAtual = p.mod_Conducao.Fator_Conducao;
        float potAtual = p.mod_Conducao.Fator_Potencia;

        float fatorConducao = Mathf.Lerp(0.98f, 1, condAtual);
        float fatorPotencia = Mathf.Lerp(0.98f, 1, potAtual);

        return (fatorConducao + fatorPotencia) / 2;
    }

    public float Calcular_Multiplicador_Curva(PilotoSim p)
    {
        const float distanciaParaFrente = 12f; // metros reais
        const float distanciaParaTras = 12f;

        float tFrente = simEngine.splineManager.SplineTFromDistance(p.DistanceTotal + distanciaParaFrente);
        float tTras = simEngine.splineManager.SplineTFromDistance(p.DistanceTotal - distanciaParaTras);

        Vector3 d1 = simEngine.splineManager.GetTangent(tTras);
        Vector3 d2 = simEngine.splineManager.GetTangent(tFrente);

        float ang = Vector3.Angle(d1, d2); // ângulo REAL ao longo de ~24m

        // curva suave baseada no seu modelo atual:
        if (ang < 5f)
            return 1f;

        if (ang < 15f)
            return Mathf.Lerp(1f, 0.85f, (ang - 15f) / 10f);

        if (ang < 25f)
            return Mathf.Lerp(0.85f, 0.65f, (ang - 25f) / 10f);

        return Mathf.Lerp(0.65f, 0.50f, Mathf.Clamp01((ang - 35f) / 20f));
    }
}