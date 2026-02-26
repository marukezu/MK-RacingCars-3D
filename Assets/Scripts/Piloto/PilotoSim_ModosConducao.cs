using UnityEngine;

public class PilotoSim_ModosConducao 
{
    // Constantes
    public const float BASE_DELAY_ESCOLHAFEITA = 1.75f;

    public PilotoSim piloto;

    // Fatores de Condução
    public float Fator_Conducao => fatores.fat_conducao;
    public float Fator_Potencia => fatores.fat_potencia;

    // Condições
    public float Cond_Combustivel => condicoes.cond_combustivel;
    public float Cond_Pneus => condicoes.cond_pneus;

    // Bools
    public bool escolhaFeita => timers.delayEscolhaFeita > 0;

    // ==========================
    //      STRUCTS DE DADOS
    // ==========================

    private struct FatoresConducao
    {
        public float fat_conducao; // 0-1
        public float fat_potencia; // 0-1

        public FatoresConducao(float conducao = 0.5f, float potencia = 0.5f)
        {
            fat_conducao = conducao;
            fat_potencia = potencia;
        }
    }

    private struct Condicoes 
    {
        public float cond_combustivel;
        public float cond_pneus;

        public Condicoes(float combustivel = 100f, float pneus = 100f)
        {
            cond_combustivel = combustivel;
            cond_pneus = pneus;
        }
    }

    private struct Timers
    {
        // Impõe um tempo para que a IA rode o método de trocar o comportamento de uso do Nitro novamente.
        public float delayEscolhaFeita;
    }

    // INSTÂNCIAS DAS STRUCTS
    private FatoresConducao fatores;
    private Condicoes condicoes;
    private Timers timers;

    public PilotoSim_ModosConducao(PilotoSim piloto)
    {
        this.piloto = piloto;

        fatores = new FatoresConducao();
        condicoes = new Condicoes(100f, 100f);
        timers = new Timers();
    }

    public void Atualizar(float dt)
    {
        timers.delayEscolhaFeita -= dt;
    }

    public void Set_FatorConducao(float value)
    {
        fatores.fat_conducao = Mathf.Clamp(value, 0, 1);
        EventBus.On_ModosConducao_ConducaoChanged?.Invoke(piloto, Fator_Conducao);
    }

    public void Set_FatorPotencia(float value)
    {
        fatores.fat_potencia = Mathf.Clamp(value, 0, 1);
        EventBus.On_ModosConducao_PotenciaChanged?.Invoke(piloto, Fator_Potencia);
    }

    public void Set_CondicaoPneus(float value)
    {
        condicoes.cond_pneus = Mathf.Clamp(value, 0, 100);
        EventBus.On_ModosConducao_Car_ConditionPneusChanged?.Invoke(piloto, Cond_Pneus);
    }

    public void Set_CondicaoCombustivel(float value)
    {
        condicoes.cond_combustivel = Mathf.Clamp(value, 0, 100); 
        EventBus.On_ModosConducao_Car_ConditionCombustivelChanged?.Invoke(piloto, Cond_Combustivel);
    }

    public void Set_EscolhaFeita()
    {
        timers.delayEscolhaFeita = BASE_DELAY_ESCOLHAFEITA;
    }
}
