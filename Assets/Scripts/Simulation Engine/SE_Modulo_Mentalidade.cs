using System.Collections.Generic;
using UnityEngine;
using static PilotoSim_Mentalidade;
using static PilotoSim_Ultrapassagem;

public class SE_Modulo_Mentalidade
{
    public SimulationEngine simEngine;

    private readonly Dictionary<Personalidade, PerfilPersonalidade> perfis =
    new Dictionary<Personalidade, PerfilPersonalidade>
    {
        {
            Personalidade.Cauteloso,
            new PerfilPersonalidade {
                limiteRecursoCritico = 10f,
                limiteRecursoBaixo = 50f,

                distanciaAmeaca = 10f,
                distanciaOportunidadeLeve = 20f,
                distanciaOportunidadeForte = 10f,

                mentalidadeQuandoSemRecurso = TipoMentalidade.Defensivo,
                mentalidadeAmeaca = TipoMentalidade.Forte,
                mentalidadeOportunidadeLeve = TipoMentalidade.Ofensivo,
                mentalidadeOportunidadeForte = TipoMentalidade.Agressivo,
            }
        },
        {
            Personalidade.Agressivo,
            new PerfilPersonalidade {
                limiteRecursoCritico = 5f,
                limiteRecursoBaixo = 35f,

                distanciaAmeaca = 10f,
                distanciaOportunidadeLeve = 40f,
                distanciaOportunidadeForte = 10f,

                mentalidadeQuandoSemRecurso = TipoMentalidade.Defensivo,
                mentalidadeAmeaca = TipoMentalidade.Forte,
                mentalidadeOportunidadeLeve = TipoMentalidade.Ofensivo,
                mentalidadeOportunidadeForte = TipoMentalidade.Agressivo,
            }
        },
        {
            Personalidade.TudoOuNada,
            new PerfilPersonalidade {
                limiteRecursoCritico = 1f,
                limiteRecursoBaixo = 20f,

                distanciaAmeaca = 10f,
                distanciaOportunidadeLeve = 60f,
                distanciaOportunidadeForte = 10f,

                mentalidadeQuandoSemRecurso = TipoMentalidade.Defensivo,
                mentalidadeAmeaca = TipoMentalidade.Forte,
                mentalidadeOportunidadeLeve = TipoMentalidade.Ofensivo,
                mentalidadeOportunidadeForte = TipoMentalidade.Agressivo,
            }
        }
    };

    public struct PerfilPersonalidade
    {
        public float limiteRecursoCritico;
        public float limiteRecursoBaixo;

        public float distanciaAmeaca;
        public float distanciaOportunidadeLeve;
        public float distanciaOportunidadeForte;

        public TipoMentalidade mentalidadeQuandoSemRecurso;
        public TipoMentalidade mentalidadeAmeaca;
        public TipoMentalidade mentalidadeOportunidadeLeve;
        public TipoMentalidade mentalidadeOportunidadeForte;
    }

    public SE_Modulo_Mentalidade(SimulationEngine simEngine)
    {
        this.simEngine = simEngine;
    }

    public void IA_SelecionarMentalidade(PilotoSim p, int idx, List<PilotoSim> ordenados, float dt)
    {
        PilotoSim_Mentalidade modMent = p.mod_Mentalidade;

        // Se estiver travada a escolha, retorna.
        if (modMent.escolhaTravada)
            return;

        // Se a IA não está setada para controlar mentalidade, não controla.
        if (!p.IA_Controla_Mentalidade)
            return;

        // Não está travada, seleciona nova mentalidade.
        DecisaoPorPersonalidade(p, idx, ordenados);
    }

    private void DecisaoPorPersonalidade(PilotoSim p, int idx, List<PilotoSim> ordenados)
    {
        // 1) Distâncias
        PilotoSim pFrente = idx > 0 ? ordenados[idx - 1] : null;
        PilotoSim pAtras = idx < ordenados.Count - 1 ? ordenados[idx + 1] : null;

        float distFrente = pFrente != null ? pFrente.DistanceTotal - p.DistanceTotal : Mathf.Infinity;
        float distTras = pAtras != null ? p.DistanceTotal - pAtras.DistanceTotal : Mathf.Infinity;

        // Pega referencia do modulo
        PilotoSim_Mentalidade modMent = p.mod_Mentalidade;

        // Seleciona a mentalidade
        modMent.SelecionarMentalidade(DecidirMentalidade(p, distTras, distFrente), true);
    }

    private TipoMentalidade DecidirMentalidade(PilotoSim p, float distTras, float distFrente)
    {
        var modCond = p.mod_Conducao;
        var modMent = p.mod_Mentalidade;

        var perfil = perfis[modMent.P_Personalidade];

        float pneu = modCond.Cond_Pneus;
        float comb = modCond.Cond_Combustivel;

        float menorRecurso = Mathf.Min(pneu, comb);

        // 2. Sem recurso → economia
        if (menorRecurso <= perfil.limiteRecursoCritico)
            return perfil.mentalidadeQuandoSemRecurso;

        // 3. Ameaça atrás
        if (distTras <= perfil.distanciaAmeaca)
            return perfil.mentalidadeAmeaca;

        // 4. Oportunidade forte (muito perto)
        if (distFrente <= perfil.distanciaOportunidadeForte)
            return perfil.mentalidadeOportunidadeForte;

        // 5. Oportunidade leve
        if (distFrente <= perfil.distanciaOportunidadeLeve)
            return perfil.mentalidadeOportunidadeLeve;

        // 6. Nenhuma decisão especial → mantém forte.
        return TipoMentalidade.Forte;
    }
}
