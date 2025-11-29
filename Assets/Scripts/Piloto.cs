using System.Collections.Generic;
using UnityEngine;
using static Pneu;

public class Piloto
{
    // enums
    
    public enum NumeroDoPiloto
    {
        Um,
        Dois
    }

    public enum EstadoDoPiloto
    {
        None,
        ParadoNoPit,
        VoltaSaida,
        VoltaRapida,
        VoltaEntrada,
        Ultrapassagem,
    }

    // Dados básicos
    public int ID {  get; set; }
    public string PilotoNome { get; set; }
    public Sprite PilotoSprite { get; set; }

    // Campeonato
    public int PontuacaoCampeonato { get; set; }
    public int PosicaoGrid { get; set; }

    // Habilidades de condução
    public float HabilidadeAceleracao { get; set; }
    public float HabilidadeUltrapassagem { get; set; }
    public float HabilidadeDesgastePneus { get; set; }
    public float HabilidadeGastoCombustivel { get; set; }

    // Equipe e carro.
    public NumeroDoPiloto PilotoNumero { get; set; }
    public EstadoDoPiloto PilotoEstado { get; set; }
    public Carro.Modelo CarroModelo { get; set; }
    public Carro Carro { get; set; }
    public Carro_Upgrades CarroUpgrades { get; set; }
    public EquipePitStop EquipePitStop { get; set; }

    // ==================
    // Dados do Carro
    // ==================

    // Pneus
    public int PneuMacioQuantidade { get; set; }
    public int PneuMedioQuantidade { get; set; }
    public int PneuDuroQuantidade { get; set; }
    public int PneuMolhadoQuantidade { get; set; }
    public int PneuChuvaQuantidade { get; set; }

    // Modos de condução iniciais.
    public Carro.TipoDeConducao ModoDirecaoInicial { get; set; }
    public Carro.TipoDeMotor ModoMotorInicial { get; set; }

    public bool AutoMode { get; set; }


    public Piloto(int iD, string pilotoNome, float pilotoAceleracao, float pilotoUltrapassagem, float pilotoDesgastePneus, float pilotoGastoCombustivel,
                  Carro.Modelo carroModelo, Carro_Upgrades upgrades, NumeroDoPiloto pilotoNumero,
                  int pneusDuros, int pneusMedios, int pneusMacios, int pneusMolhado, int pneusChuva)
    {
        ID = iD;
        PilotoNome = pilotoNome;

        // Habilidades de condução
        HabilidadeAceleracao = pilotoAceleracao;
        HabilidadeUltrapassagem = pilotoUltrapassagem;
        HabilidadeDesgastePneus = pilotoDesgastePneus;
        HabilidadeGastoCombustivel = pilotoGastoCombustivel;

        // Equipe e carro.
        CarroUpgrades = upgrades;
        PilotoNumero = pilotoNumero;
        EquipePitStop = new EquipePitStop(35, 35);
        PilotoEstado = EstadoDoPiloto.None;
        CarroModelo = carroModelo;
        PosicaoGrid = iD; // Posição do grid recebe a ID no início para EVITAR bugs, já garante uma posição logo na criação.

        // Modos de condução do carro.
        ModoDirecaoInicial = Carro.TipoDeConducao.Equilibrado;
        ModoMotorInicial = Carro.TipoDeMotor.Equilibrado;
        AutoMode = true;

        // Pneus
        PneuMacioQuantidade = pneusMacios;
        PneuMedioQuantidade = pneusMedios;
        PneuDuroQuantidade = pneusDuros;
        PneuMolhadoQuantidade = pneusMolhado;
        PneuChuvaQuantidade = pneusChuva;
    }

    public int GetPosicaoCampeonato()
    {
        if (PlayerSettings.campeonato == null) return 0;

        Campeonato campeonato = PlayerSettings.campeonato;
        int posicao = 1;

        foreach (Piloto piloto in PlayerSettings.campeonato.pilotosParticipantes)
        {
            if (piloto.PontuacaoCampeonato > PontuacaoCampeonato)
                posicao++;
        }

        return posicao;
    }

    public string GetNomeModeloCarro()
    {
        switch (CarroModelo)
        {
            // Mini Cupa's
            case Carro.Modelo.MiniCupa_RED:
                return "Mini Cupa";

            case Carro.Modelo.MiniCupa_BLUE:
                return "Mini Cupa";

            case Carro.Modelo.MiniCupa_GREEN:
                return "Mini Cupa";

            case Carro.Modelo.MiniCupa_YELLOW:
                return "Mini Cupa";

            case Carro.Modelo.Lanzia:
                return "Lanzia";

            case Carro.Modelo.LanziaStradale:
                return "Lanzia Stradale";

            case Carro.Modelo.RinaultAlpine:
                return "Rinault Alpine";

            default:
                return "null";
        }
    }

    public void AumentarQntPneu(Pneu.TipoPneu tipoPneu)
    {
        switch (tipoPneu)
        {
            case TipoPneu.Duro:
                PneuDuroQuantidade++;
                break;

            case TipoPneu.Medio:
                PneuMedioQuantidade++;
                break;

            case TipoPneu.Macio:
                PneuMacioQuantidade++;
                break;

            case TipoPneu.Molhado:
                PneuMolhadoQuantidade++;
                break;

            case TipoPneu.Chuva:
                PneuChuvaQuantidade++;
                break;
        }
    }
    public void DiminuirQntPneu(Pneu.TipoPneu tipoPneu)
    {
        switch (tipoPneu)
        {
            case TipoPneu.Duro:
                PneuDuroQuantidade--;
                break;

            case TipoPneu.Medio:
                PneuMedioQuantidade--;
                break;

            case TipoPneu.Macio:
                PneuMacioQuantidade--;
                break;

            case TipoPneu.Molhado:
                PneuMolhadoQuantidade--;
                break;

            case TipoPneu.Chuva:
                PneuChuvaQuantidade--;
                break;
        }
    }

    public string GetFalaPeloEstado()
    {
        // Define as falas por estado.
        var falasPorEstado = new Dictionary<EstadoDoPiloto, List<string>>
    {
        {
            EstadoDoPiloto.VoltaSaida, new List<string>
            {
                "Saindo do pit agora, tudo limpo por aí?",
                "Ok, aquecendo os pneus na saída.",
                "Cheque os dados, pressão está boa?",
                "Vou pegar ritmo na volta de saída.",
                "Pegando o ritmo, me avisem se algo mudar."
            }
        },
        {
            EstadoDoPiloto.VoltaRapida, new List<string>
            {
                "Abrindo volta rápida agora, tudo no limite.",
                "Pneus no ponto, foco total nessa volta.",
                "Volta limpa até agora, iniciando agressividade.",
                "Velocidade máxima, sem erros.",
                "Vamos com tudo, essa tem que ser boa."
            }
        },
        {
            EstadoDoPiloto.VoltaEntrada, new List<string>
            {
                "Volta concluída, voltando para os boxes.",
                "Fiz o meu melhor, agora é com vocês.",
                "Pneus desgastados, retornando aos boxes.",
                "Retornando aos boxes, me confirmem a posição.",
                "Fechando a volta, tudo sob controle aqui."
            }
        }
    };

        // Verifica se o estado atual existe no dicionário.
        if (falasPorEstado.TryGetValue(PilotoEstado, out var falas))
        {
            // Sorteia uma fala aleatória.
            int sorteio = Random.Range(0, falas.Count);
            return falas[sorteio];
        }

        // Retorno padrão caso o estado não esteja mapeado.
        Debug.LogError($"Estado do piloto não mapeado: {PilotoEstado}");
        return "Algo errado no script.";
    }

}
