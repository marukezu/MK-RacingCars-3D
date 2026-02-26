using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PilotoSim_Ultrapassagem;

public class SE_Modulo_Ultrapassagem
{
    public SimulationEngine simEngine;

    public SE_Modulo_Ultrapassagem(SimulationEngine simulationEngine)
    {
        this.simEngine = simulationEngine;
    }

    public void TentarUltrapassagem(PilotoSim p, int idx, List<PilotoSim> ordenados, float dt)
    {
        if (p.DistanceTotal < 15f) return;

        // Lider ou em delay de ultrapassagem, não tenta ultrapassagem
        if (idx == 0 || !p.mod_Ultrapassagem.PodeTentarUltrapassar) return;

        // Se já estiver em alguma manobra de ultrapassagem, retorna
        if (p.mod_Ultrapassagem.condicao != Condicao.Nada) return;

        // Distâncias
        PilotoSim pFrente = idx > 0 ? ordenados[idx - 1] : null;
        PilotoSim pAtras = idx < ordenados.Count - 1 ? ordenados[idx + 1] : null;

        float distFrente = pFrente != null ? pFrente.DistanceTotal - p.DistanceTotal : Mathf.Infinity;
        float distTras = pAtras != null ? p.DistanceTotal - pAtras.DistanceTotal : Mathf.Infinity;

        // Se o carro a frente não está em uma ultrapassagem, eu estou próximo dele e mais rápido.
        if (pFrente.mod_Ultrapassagem.condicao == Condicao.Nada && distFrente < 10)
        {
            Debug.Log(p.Nome + "Calculando chance");
            float chanceUltrapassagem = CalcularChanceUltrapassagem(p, pFrente, dt);

            bool ultrapassou = UnityEngine.Random.value < chanceUltrapassagem;

            if (ultrapassou)
                Preparar_Ultrapassagem(p, pFrente);

            else
                ChancePerdida(p);
        }
    }

    // ======================================================================
    // ========= MÉTODOS DE CONCLUSÃO DE TENTATIVA DE ULTRAPASSAGEM =========
    // ======================================================================
    private void Preparar_Ultrapassagem(PilotoSim p, PilotoSim defensor)
    {
        // Marca o defensor
        defensor.mod_Ultrapassagem.Set_Condition(Condicao.SendoUltrapassado);
        defensor.mod_Ultrapassagem.Set_PilotoMeUltrapassando(p);    

        // Marca o atacante
        p.mod_Ultrapassagem.Set_Condition(Condicao.Ultrapassando);
        p.mod_Ultrapassagem.Set_PilotoQueEstouUltrapassando(defensor);

        // Escolhe um lado para ultrapassar
        bool direita = UnityEngine.Random.value < 0.5f;

        p.mod_Ultrapassagem.ladoDeUltrapassagem = direita ? LadoDeUltrapassagem.Direita : LadoDeUltrapassagem.Esquerda;

        // Realiza a coroutina.
        CoroutineRunner.Instance.StartCoroutine(Tentar_Ultrapassagem(p, defensor));
    }

    private IEnumerator Tentar_Ultrapassagem(PilotoSim p, PilotoSim defensor)
    {
        float timeout = 10f;        // tempo máximo para ultrapassar.
        float tempo = 0f;

        // Aguarda até que:
        // 1) A ultrapassagem esteja pronta para finalizar, OU
        // 2) O tempo acabe
        while (p.mod_Ultrapassagem.condicao != Condicao.ProntoParaFinalizar ||
               tempo < timeout)
        {
            tempo += Time.deltaTime;
            yield return null;
        }

        // Se não conseguiu ultrapassar em 5s → falhou
        bool falhou = p.mod_Ultrapassagem.condicao != Condicao.ProntoParaFinalizar;

        Debug.Log(p.Nome + "Falhou? " + falhou);

        if (falhou)
            p.mod_Ultrapassagem.Set_Condition(Condicao.FalhouUltrapassagem);

        yield return new WaitUntil(() => p.mod_Ultrapassagem.condicao == Condicao.ProntoParaFinalizar);

        // Marca resultado da ultrapassagem
        defensor.mod_Ultrapassagem.Set_Condition(Condicao.Nada);
        defensor.mod_Ultrapassagem.Set_PilotoMeUltrapassando(null);

        p.mod_Ultrapassagem.Set_Condition(Condicao.Nada);
        p.mod_Ultrapassagem.Set_PilotoQueEstouUltrapassando(null);

        // Delay para tentar novamente
        defensor.mod_Ultrapassagem.Set_Delay_Ultrapassagem(2f);
        p.mod_Ultrapassagem.Set_Delay_Ultrapassagem(2f);
    }

    public void Checar_Se_Conseguiu_Ultrapassar(PilotoSim p) 
    {
        // Se está ultrapassando.
        if (p.mod_Ultrapassagem.condicao == Condicao.Ultrapassando)
        {
            float distanciaAdversario = p.DistanceTotal - p.mod_Ultrapassagem.pilotoQueEstouUltrapassando.DistanceTotal;

            if (distanciaAdversario > 3.8f)
                p.mod_Ultrapassagem.Set_Condition(Condicao.ProntoParaFinalizar);
        }

        // Se Falhou na ultrapassagem.
        else if (p.mod_Ultrapassagem.condicao == Condicao.FalhouUltrapassagem)
        {
            float distanciaAdversario = p.mod_Ultrapassagem.pilotoQueEstouUltrapassando.DistanceTotal - p.DistanceTotal;

            if (distanciaAdversario > 3.3f)
                p.mod_Ultrapassagem.Set_Condition(Condicao.ProntoParaFinalizar);
        }       
    }

    private void ChancePerdida(PilotoSim p)
    {
        p.mod_Ultrapassagem.Set_Delay_Ultrapassagem(0.5f); // meio Segundos para tentar novamente.
    }

    // ======================================================================
    // ========== CALCULOS PARA DECISÃO SE IRÁ ULTRAPASSAR OU NÃO ===========
    // ======================================================================
    private float CalcularChanceUltrapassagem(PilotoSim atacante, PilotoSim defensor, float dt)
    {
        float scoreA = CalcularScoreDuelo(atacante, true);
        float scoreD = CalcularScoreDuelo(defensor, false);

        float diff = scoreA - scoreD;

        // sigmoide (suave)
        float chance = 1f / (1f + Mathf.Exp(-diff));

        // DISTÂNCIA
        float dist = defensor.DistanceTotal - atacante.DistanceTotal;
        float distFactor = Mathf.InverseLerp(12f, 2f, dist);
        chance *= Mathf.Lerp(0.7f, 1.2f, distFactor);

        // CURVA
        float curva = simEngine.mod_AceleracaoFrenagem.Calcular_Multiplicador_Curva(atacante);
        chance *= Mathf.Lerp(0.3f, 1f, curva);

        // DIFERENÇA DE VELOCIDADE
        float speedDiff = atacante.mod_acelFrenagem.Car_SpeedMS
                        - defensor.mod_acelFrenagem.Car_SpeedMS;

        float speedFactor = Mathf.Clamp01((speedDiff + 3f) / 6f);
        chance *= Mathf.Lerp(0.6f, 1.3f, speedFactor);

        // PNEUS
        float pneuDiff = atacante.mod_Conducao.Cond_Pneus - defensor.mod_Conducao.Cond_Pneus;
        chance *= Mathf.Lerp(0.9f, 1.1f, (pneuDiff + 20f) / 40f);

        // ALEATORIEDADE CONTROLADA
        chance *= UnityEngine.Random.Range(0.97f, 1.03f);

        // EVITA ultrapassagem "instantânea"
        chance = Mathf.Clamp01(chance);

        //
        chance *= Mathf.Lerp(0.5f, 1f, dt * 60f);

        return chance;
    }

    private float CalcularScoreDuelo(PilotoSim p, bool atacante)
    {
        PilotoSim_ModosConducao modCond = p.mod_Conducao;

        // Score somado por Pesos de cada atributo.
        float score = 0f;

        // 1) Fator de condução/potência
        score += Mathf.Lerp(0.20f, 1.25f, modCond.Fator_Conducao);
        score += Mathf.Lerp(0.20f, 1.25f, modCond.Fator_Potencia);

        // Aqui o score fica entre 0.40 ~ 2.5

        // 2) Condição atual dos Pneus
        score += Mathf.Lerp(0.75f, 1f, modCond.Cond_Pneus / 100);

        // Aqui o score fica entre 1.15 ~ 3.5

        // 4) Skill do piloto
        if (atacante)
            score += Mathf.Lerp(0.5f, 1f, p.mod_Atributos.H_Ultra);
        else
            score += Mathf.Lerp(0.5f, 1f, p.mod_Atributos.H_Defesa);

        // Aqui o score fica entre 1.65 ~ 4.5

        // 5) Se o defensor ficar economizando -> chances altas de perder posição.
        if (!atacante)
        {
            float defensiva = 2f - (modCond.Fator_Conducao + modCond.Fator_Potencia);
            score *= Mathf.Lerp(1f, 0.35f, defensiva);
        }

        // 6) Combustível restante -> Sem combustivel = alta penalidade.
        if (modCond.Cond_Combustivel <= 0)
        {
            score *= 0.25f;
        }

        // 7) Aleatoriedade controlada (+- 2%)
        score *= UnityEngine.Random.Range(0.98f, 1.02f);

        return score;
    }
}
