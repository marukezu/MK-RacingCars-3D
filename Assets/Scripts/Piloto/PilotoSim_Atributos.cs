using UnityEngine;

public class PilotoSim_Atributos
{
    public PilotoSim piloto;

    // Variáveis Públicas - para consulta dos valores.
    public float Moral => Get_Moral();
    public float MoralMultiplier => Get_MoralMultiplier();  
    public float H_Acel => Get_H_Aceleracao();
    public float H_Ultra => Get_H_Ultrapassagem();
    public float H_Defesa => Get_H_Defesa();
    public float H_DesgastePneus => Get_H_DesgastePneus();
    public float H_ConsumoCombustivel => Get_H_ConsumoCombustivel();

    // Structs
    private struct BaseStats
    {
        public float moral;
        public float h_acel;
        public float h_ultra;
        public float h_defesa;
        public float h_desgaste;
        public float h_combustivel;

        public void Set_Aceleracao(float value)
            => h_acel = Mathf.Clamp01(value);

        public void Set_Ultrapassagem(float value)
            => h_ultra = Mathf.Clamp01(value);

        public void Set_Defesa(float value)
            => h_defesa = Mathf.Clamp01(value);

        public void Set_DesgastePneus(float value)
            => h_desgaste = Mathf.Clamp01(value);

        public void Set_ConsumoCombustivel(float value)
            => h_combustivel = Mathf.Clamp01(value);
    }

    // Instancias de Structs
    private BaseStats baseStats;

    // Construtor
    public PilotoSim_Atributos(
        PilotoSim piloto,
        float acel,
        float ultra,
        float defesa,
        float desgaste,
        float combustivel)
    {
        this.piloto = piloto;

        baseStats = new BaseStats()
        {
            h_acel = acel,
            h_ultra = ultra,
            h_defesa = defesa,
            h_desgaste = desgaste,
            h_combustivel = combustivel
        };
    }

    // ===================================================================
    // ============================== GETS ===============================
    // ===================================================================
    private float Get_Moral()
    {
        return baseStats.moral;
    }

    private float Get_MoralMultiplier()
    {
        float moralBase = Get_Moral();
        return Mathf.Lerp(0.7f, 1, moralBase);
    }

    private float Get_H_Aceleracao()
    {
        float acelBase = baseStats.h_acel;

        // Multiplicadores.
        float moralMult = MoralMultiplier;

        float finalValue = acelBase * moralMult;

        return finalValue;
    }

    private float Get_H_Ultrapassagem()
    {
        float ultraBase = baseStats.h_ultra;

        // Multiplicadores.
        float moralMult = MoralMultiplier;

        // Soma final de todos os multiplicadores.
        float finalValue = ultraBase * moralMult;

        return finalValue;
    }

    private float Get_H_Defesa()
    {
        float defBase = baseStats.h_defesa;

        // Multiplicadores.
        float moralMult = MoralMultiplier;

        // Soma final de todos os multiplicadores.
        float finalValue = defBase * moralMult;

        return finalValue;
    }

    private float Get_H_DesgastePneus()
    {
        float desgBase = baseStats.h_desgaste;

        // Multiplicadores.
        float moralMult = MoralMultiplier;

        // Soma final de todos os multiplicadores.
        float finalValue = desgBase * moralMult;

        return finalValue;
    }

    private float Get_H_ConsumoCombustivel()
    {
        float consumoBase = baseStats.h_combustivel;

        // Multiplicadores.
        float moralMult = MoralMultiplier;

        // Soma final de todos os multiplicadores.
        float finalValue = consumoBase * moralMult;

        return finalValue;
    }

    // ===================================================================
    // ============================== SETS ===============================
    // ===================================================================
    public void Set_Aceleracao(float value)
    {
        baseStats.Set_Aceleracao(value);
    }

    public void Set_Ultrapassagem(float value)
    {
        baseStats.Set_Ultrapassagem(value);
    }

    public void Set_Defesa(float value)
    {
        baseStats.Set_Defesa(value);
    }

    public void Set_DesgastePneus(float value)
    {
        baseStats.Set_DesgastePneus(value);
    }

    public void Set_ConsumoCombustivel(float value)
    {
        baseStats.Set_ConsumoCombustivel(value);
    }
}
