using System.Collections.Generic;
using UnityEngine;
using static PilotoSim_Mentalidade;
using static PilotoSim_Ultrapassagem;
using static RacingHeuristics;

public class SE_Modulo_ModosConducao
{
    public enum TrendLevel
    {
        Neutral,
        Up1,
        Up2,
        Up3,
        Down1,
        Down2,
        Down3
    }

    public SimulationEngine simEngine;

    // Para controlar os modos de condução através da Mentalidade.
    private readonly Dictionary<TipoMentalidade, PerfilMentalidade> perfisMentalidade =
    new Dictionary<TipoMentalidade, PerfilMentalidade>
    {
        { TipoMentalidade.Defensivo, new PerfilMentalidade {
            fatorBase = 0.2f,
            boostAtaque = 0f,
            boostDefesa = 0.25f, 
            boostLivre = 0.05f,
            boostEconomiaForte = 0f,
            boostEconomiaFraca = 0f,
            penalidadeRecursosBaixos = -0.20f, // O quanto ele reduz o uso por estar com pouco combustivel.
            alcanceAtaque = 10f,
        }},
        { TipoMentalidade.Forte, new PerfilMentalidade {
            fatorBase = 0.6f,
            boostAtaque = 0.15f,
            boostDefesa = 0.15f,
            boostLivre = 0.10f,
            boostEconomiaForte = -0.30f,
            boostEconomiaFraca = -0.10f,
            penalidadeRecursosBaixos = -0.10f,
            alcanceAtaque = 25f,
        }},
        { TipoMentalidade.Ofensivo, new PerfilMentalidade {
            fatorBase = 0.7f,
            boostAtaque = 0.20f,
            boostDefesa = 0.10f,
            boostLivre = 0.15f,
            boostEconomiaForte = -0.35f,
            boostEconomiaFraca = -0.10f,
            penalidadeRecursosBaixos = -0.05f,
            alcanceAtaque = 25f,
        }},
        { TipoMentalidade.Agressivo, new PerfilMentalidade {
            fatorBase = 0.8f,
            boostAtaque = 0.20f,
            boostDefesa = 0.10f,
            boostLivre = 0.20f,
            boostEconomiaForte = -0.35f,
            boostEconomiaFraca = -0.10f,
            penalidadeRecursosBaixos = -0.05f,
            alcanceAtaque = 25f,
        }},
    };

    private readonly Dictionary<Personalidade, float> modPersonalidade =
    new Dictionary<Personalidade, float>
    {
        { Personalidade.Cauteloso, 0.0f },
        { Personalidade.Agressivo, +0.05f },
        { Personalidade.TudoOuNada, +0.10f },
    };

    // Struct Mentalidade
    public struct PerfilMentalidade
    {
        public float fatorBase;

        public float boostDefesa;
        public float boostAtaque;
        public float boostLivre;
        public float boostEconomiaForte;
        public float boostEconomiaFraca;

        public float penalidadeRecursosBaixos;
        public float alcanceAtaque;
    }

    public SE_Modulo_ModosConducao(SimulationEngine simEngine)
    {
        this.simEngine = simEngine;
    }

    public void Atualizar_Conducao(PilotoSim p, float dt)
    {
        p.mod_Conducao.Atualizar(dt);
    }

    public void Atualizar_Desgaste_Pneus(PilotoSim p, float dt)
    {
        var mod = p.mod_Conducao;

        float t = mod.Fator_Conducao; // 0–1

        // =====================================================
        // 1) Calcula GASTO puro (ajuste o lerp aos poucos)
        // =====================================================
        // Exemplo: t=0 → gasta 0.1 | t=1 → gasta 1
        float gasto = Mathf.Lerp(0.1f, 0.3f, t);

        // Sempre negativo (consumo)
        float delta = -gasto * dt;

        // =====================================================
        // 2) Habilidade reduz desgaste
        // =====================================================
        float hab = p.mod_Atributos.H_DesgastePneus;     // 0–1
        float reducao = Mathf.Lerp(0f, 0.25f, hab);      // até 25% melhor
        delta *= (1f - reducao);

        // =====================================================
        // 3) Aplica valor final
        // =====================================================
        float novo = Mathf.Clamp(mod.Cond_Pneus + delta, 0f, 100f);
        mod.Set_CondicaoPneus(novo);

        // =====================================================
        // 4) Trend (opcional)
        // =====================================================
        TrendLevel trend = GetTrendLevel(p, true);
        EventBus.On_ModosConducao_PneusTrendChanged?.Invoke(p, trend);
    }

    public void Atualizar_Consumo_Combustivel(PilotoSim p, float dt)
    {
        var mod = p.mod_Conducao;

        float t = mod.Fator_Potencia; // 0–1

        // =====================================================
        // 1) Consumo puro (ajuste o lerp)
        // =====================================================
        float gasto = Mathf.Lerp(0.05f, 0.25f, t);

        float delta = -gasto * dt;

        // =====================================================
        // 2) Habilidade reduz consumo
        // =====================================================
        float hab = p.mod_Atributos.H_ConsumoCombustivel;
        float reducao = Mathf.Lerp(0f, 0.25f, hab);
        delta *= (1f - reducao);

        // =====================================================
        // 3) Aplica
        // =====================================================
        float novo = Mathf.Clamp(mod.Cond_Combustivel + delta, 0f, 100f);
        mod.Set_CondicaoCombustivel(novo);

        // =====================================================
        // 4) Trend
        // =====================================================
        TrendLevel trend = GetTrendLevel(p, false);
        EventBus.On_ModosConducao_CombustivelTrendChanged?.Invoke(p, trend);
    }


    private TrendLevel GetTrendLevel(PilotoSim p, bool pneus)
    {
        float fatorConsumo = 0f;

        if (pneus)
            fatorConsumo = p.mod_Conducao.Fator_Conducao;
        else
            fatorConsumo = p.mod_Conducao.Fator_Potencia;

        if (fatorConsumo < 0.25)
            return TrendLevel.Neutral;

        else if (fatorConsumo >= 0.25f)
            return TrendLevel.Up1;

        else if (fatorConsumo > 0.5f)
            return TrendLevel.Up2;

        else
            return TrendLevel.Up3;
    }

    public void IA_Selecionar_ModosConducao(PilotoSim p, List<PilotoSim> ordenados, int idx)
    {
        if (!RaceManager.Instance.largadaAutorizada)
            return;

        if (!p.IA_Controla_Conducao)
            return;

        var mod = p.mod_Conducao;

        // Delay de decisão
        if (mod.escolhaFeita)
            return;

        mod.Set_EscolhaFeita();

        PilotoSim pFrente = idx > 0 ? ordenados[idx - 1] : null;
        PilotoSim pAtras = idx < ordenados.Count - 1 ? ordenados[idx + 1] : null;

        float distFrente = pFrente != null ? pFrente.DistanceTotal - p.DistanceTotal : Mathf.Infinity;
        float distTras = pAtras != null ? p.DistanceTotal - pAtras.DistanceTotal : Mathf.Infinity;

        // ===== NOVO: Decisões separadas =====
        float pneuDecision = DecidirConducaoPneus(p, distFrente, distTras);
        float combDecision = DecidirConducaoCombustivel(p, distFrente, distTras);

        mod.Set_FatorConducao(pneuDecision);
        mod.Set_FatorPotencia(combDecision);
    }

    private float DecidirConducaoPneus(PilotoSim p, float distFrente, float distTras)
    {
        var mod = p.mod_Conducao;
        var ment = p.mod_Mentalidade;
        var perfil = perfisMentalidade[ment.MentalidadeAtual];
        float baseVal = perfil.fatorBase;

        float pneus = mod.Cond_Pneus; // 0–100
        float desgaste = Mathf.InverseLerp(100f, 20f, pneus); // mais baixo = mais economia

        float pressãoAtras = Mathf.InverseLerp(30f, 0f, distTras);
        float pressãoFrente = Mathf.InverseLerp(30f, 0f, distFrente);

        float fator = 0f;

        // - Gastar pneus para atacar
        fator += pressãoFrente * perfil.boostAtaque;

        // - Gastar pneus para defender (segurar linha)
        fator += pressãoAtras * perfil.boostDefesa;

        // - Economia automática quando o desgaste está alto
        fator -= desgaste * Mathf.Abs(perfil.boostEconomiaForte);

        // - Personalidade influencia
        fator += modPersonalidade[ment.P_Personalidade];

        // - Recursos baixos diminuem gasto
        if (pneus < 25f)
            fator -= Mathf.Abs(perfil.penalidadeRecursosBaixos);

        // Combinação final
        float final = baseVal + fator;

        final += Random.Range(-0.02f, 0.02f);

        return Mathf.Clamp01(final);
    }


    private float DecidirConducaoCombustivel(PilotoSim p, float distFrente, float distTras)
    {
        var mod = p.mod_Conducao;
        var ment = p.mod_Mentalidade;
        var perfil = perfisMentalidade[ment.MentalidadeAtual];
        float baseVal = perfil.fatorBase;

        float combust = mod.Cond_Combustivel;
        float baixoComb = Mathf.InverseLerp(100f, 20f, combust);

        float pressãoFrente = Mathf.InverseLerp(30f, 0f, distFrente);
        float pressãoAtras = Mathf.InverseLerp(30f, 0f, distTras);

        float fator = 0f;

        // - Pressão na frente exige mais motor
        fator += pressãoFrente * perfil.boostAtaque;

        // - Pressão atrás exige potência para não ser ultrapassado
        fator += pressãoAtras * perfil.boostDefesa;

        // - Economia quando combustível está baixo
        fator -= baixoComb * Mathf.Abs(perfil.penalidadeRecursosBaixos);

        // - Modo econômico com pista livre atrás
        if (distTras > 40f && distFrente > 40f)
            fator += perfil.boostEconomiaFraca;

        // Personalidade
        fator += modPersonalidade[ment.P_Personalidade];

        // Combinação final
        float final = baseVal + fator;

        final += Random.Range(-0.02f, 0.02f);

        return Mathf.Clamp01(final);
    }

}
