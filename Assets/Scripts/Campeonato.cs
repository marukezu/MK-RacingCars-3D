using System.Collections.Generic;

public class Pista
{
    public string Nome;
    public string SceneName;
    public RaceManager.TipoDeCorrida etapaPista;

    public Pista(string nome, string sceneName)
    {
        Nome = nome;
        SceneName = sceneName;
        etapaPista = RaceManager.TipoDeCorrida.Corrida;
    }
}

public class Campeonato
{

    // Enums
    public enum Torneio
    {
        Debug_CUP,
        Copper_CUP,
        Silver_CUP,
        Golden_CUP
    }
    public enum Equipe
    {
        Lancia,
        LanciaStradale,
        RinaultAlpine,
        MiniCooper
    }

    // Dados do campeonato.
    public int quantidadeDePistas;
    public int pistaAtualIndex = 0;
    public List<Pista> pistas = new List<Pista>();

    // Premiação em dinheiro para a equipe participante pela sua posição final da corrida.
    public float premiacaoPrimeiroLugar;
    public float premiacaoSegundoLugar;
    public float premiacaoTerceiroLugar;
    public float premiacaoQuartoLugar;

    // Pontuações recebidas por pilotos pela sua posição final da corrida.
    public int pontuacaoPrimeiroLugar = 15;
    public int pontuacaoSegundoLugar = 13;
    public int pontuacaoTerceiroLugar = 11;
    public int pontuacaoQuartoLugar = 9;
    public int pontuacaoQuintoLugar = 7;
    public int pontuacaoSextoLugar = 5;
    public int pontuacaoSetimoLugar = 3;
    public int pontuacaoOitavoLugar = 1;

    // Limites de Pneus usados por pista.
    public int pneusDuros = 3;
    public int pneusMedios = 3;
    public int pneusMacios = 3;
    public int pneusMolhado = 3;
    public int pneusChuva = 3;

    // Os adversarios do jogador.
    public Piloto piloto3;
    public Piloto piloto4;
    public Piloto piloto5;
    public Piloto piloto6;
    public Piloto piloto7;
    public Piloto piloto8;

    // Uma lista dos adversarios para melhor manipulação
    public List<Piloto> pilotosParticipantes;

    public void InitializeCampeonato(Torneio torneio)
    {
        pilotosParticipantes = new List<Piloto>();

        if (torneio == Torneio.Debug_CUP)
        {
            // Inicializa os pilotos.
            piloto3 = new Piloto(3, "Victor Thunderbolt", 75, 75, 75, 75, Carro.Modelo.MiniCupa_GREEN, new Carro_Upgrades(), Piloto.NumeroDoPiloto.Um, pneusDuros, pneusMedios, pneusMacios, pneusMolhado, pneusChuva);
            piloto4 = new Piloto(4, "Amanda Blaze", 75, 75, 75, 75, Carro.Modelo.MiniCupa_GREEN, new Carro_Upgrades(), Piloto.NumeroDoPiloto.Dois, pneusDuros, pneusMedios, pneusMacios, pneusMolhado, pneusChuva);

            piloto5 = new Piloto(5, "Leonardo Apex", 75, 75, 75, 75, Carro.Modelo.MiniCupa_YELLOW, new Carro_Upgrades(), Piloto.NumeroDoPiloto.Um, pneusDuros, pneusMedios, pneusMacios, pneusMolhado, pneusChuva);
            piloto6 = new Piloto(6, "Camila Drift", 75, 75, 75, 75, Carro.Modelo.MiniCupa_YELLOW, new Carro_Upgrades(), Piloto.NumeroDoPiloto.Dois, pneusDuros, pneusMedios, pneusMacios, pneusMolhado, pneusChuva);

            piloto7 = new Piloto(7, "Enzo Speedster", 75, 75, 75, 75, Carro.Modelo.MiniCupa_BLUE, new Carro_Upgrades(), Piloto.NumeroDoPiloto.Um, pneusDuros, pneusMedios, pneusMacios, pneusMolhado, pneusChuva);
            piloto8 = new Piloto(8, "Diego Rocket", 75, 75, 75, 75, Carro.Modelo.MiniCupa_BLUE, new Carro_Upgrades(), Piloto.NumeroDoPiloto.Dois, pneusDuros, pneusMedios, pneusMacios, pneusMolhado, pneusChuva);

            // Adiciona as pistas.
            Pista pista1 = new Pista("Pista de Terra", "Race01-Pista-Terra");
            pistas.Add(pista1); 

            // Dados do campeonato.
            quantidadeDePistas = pistas.Count;
            premiacaoPrimeiroLugar = 2500f;
            premiacaoSegundoLugar = 1500f;
            premiacaoTerceiroLugar = 800f;
            premiacaoQuartoLugar = 500f;

        }
        else if (torneio == Torneio.Copper_CUP)
        {
            // Inicializa os pilotos.
            piloto3 = new Piloto(3, "Victor Thunderbolt", 79, 96, 92, 94, Carro.Modelo.MiniCupa_GREEN, new Carro_Upgrades(), Piloto.NumeroDoPiloto.Um, pneusDuros, pneusMedios, pneusMacios, pneusMolhado, pneusChuva);
            piloto4 = new Piloto(4, "Amanda Blaze", 74, 96, 92, 94, Carro.Modelo.MiniCupa_GREEN, new Carro_Upgrades(), Piloto.NumeroDoPiloto.Dois, pneusDuros, pneusMedios, pneusMacios, pneusMolhado, pneusChuva);

            piloto5 = new Piloto(5, "Leonardo Apex", 78, 96, 92, 94, Carro.Modelo.MiniCupa_YELLOW, new Carro_Upgrades(), Piloto.NumeroDoPiloto.Um, pneusDuros, pneusMedios, pneusMacios, pneusMolhado, pneusChuva);
            piloto6 = new Piloto(6, "Camila Drift", 73, 96, 92, 94, Carro.Modelo.MiniCupa_YELLOW, new Carro_Upgrades(), Piloto.NumeroDoPiloto.Dois, pneusDuros, pneusMedios, pneusMacios, pneusMolhado, pneusChuva);

            piloto7 = new Piloto(7, "Enzo Speedster", 77, 96, 92, 94, Carro.Modelo.MiniCupa_BLUE, new Carro_Upgrades(), Piloto.NumeroDoPiloto.Um, pneusDuros, pneusMedios, pneusMacios, pneusMolhado, pneusChuva);
            piloto8 = new Piloto(8, "Diego Rocket", 73, 96, 92, 94, Carro.Modelo.MiniCupa_BLUE, new Carro_Upgrades(), Piloto.NumeroDoPiloto.Dois, pneusDuros, pneusMedios, pneusMacios, pneusMolhado, pneusChuva);

            // Adiciona as pistas.
            Pista pista1 = new Pista("Pista de Terra", "Race02-Pista-Circuito-Deserto");
            Pista pista2 = new Pista("Pista de Terra", "Race01-Pista-Terra");
            Pista pista3 = new Pista("Pista de Terra", "Race01-Pista-Terra");
            Pista pista4 = new Pista("Pista de Terra", "Race01-Pista-Terra");
            Pista pista5 = new Pista("Pista de Terra", "Race01-Pista-Terra");
            Pista pista6 = new Pista("Pista de Terra", "Race01-Pista-Terra");
            pistas.Add(pista1); pistas.Add(pista2); pistas.Add(pista3);
            pistas.Add(pista4); pistas.Add(pista5); pistas.Add(pista6);

            // Dados do campeonato.
            quantidadeDePistas = pistas.Count;
            premiacaoPrimeiroLugar = 2500f;
            premiacaoSegundoLugar = 1500f;
            premiacaoTerceiroLugar = 800f;
            premiacaoQuartoLugar = 500f;

        }
        else if (torneio == Torneio.Silver_CUP)
        {
            // Inicializa os pilotos.
            piloto3 = new Piloto(3, "Felipe Ignition", 88, 96, 92, 94, Carro.Modelo.MiniCupa_GREEN, new Carro_Upgrades(), Piloto.NumeroDoPiloto.Um, pneusDuros, pneusMedios, pneusMacios, pneusMolhado, pneusChuva);
            piloto4 = new Piloto(4, "Pedro Throttle", 83, 96, 92, 94, Carro.Modelo.MiniCupa_GREEN, new Carro_Upgrades(), Piloto.NumeroDoPiloto.Dois, pneusDuros, pneusMedios, pneusMacios, pneusMolhado, pneusChuva);

            piloto5 = new Piloto(5, "Gustavo Circuit", 88, 96, 92, 94, Carro.Modelo.MiniCupa_YELLOW, new Carro_Upgrades(), Piloto.NumeroDoPiloto.Um, pneusDuros, pneusMedios, pneusMacios, pneusMolhado, pneusChuva);
            piloto6 = new Piloto(6, "Isabella Sprint", 82, 96, 92, 94, Carro.Modelo.MiniCupa_YELLOW, new Carro_Upgrades(), Piloto.NumeroDoPiloto.Dois, pneusDuros, pneusMedios, pneusMacios, pneusMolhado, pneusChuva);

            piloto7 = new Piloto(7, "Bianca Flame", 86, 96, 92, 94, Carro.Modelo.MiniCupa_BLUE, new Carro_Upgrades(), Piloto.NumeroDoPiloto.Um, pneusDuros, pneusMedios, pneusMacios, pneusMolhado, pneusChuva);
            piloto8 = new Piloto(8, "Bruno Overdrive", 81, 96, 92, 94, Carro.Modelo.MiniCupa_BLUE, new Carro_Upgrades(), Piloto.NumeroDoPiloto.Dois, pneusDuros, pneusMedios, pneusMacios, pneusMolhado, pneusChuva);

            // Adiciona as pistas.
            Pista pista1 = new Pista("Pista de Terra", "Race01-Pista-Terra");
            Pista pista2 = new Pista("Pista de Terra", "Race01-Pista-Terra");
            Pista pista3 = new Pista("Pista de Terra", "Race01-Pista-Terra");
            Pista pista4 = new Pista("Pista de Terra", "Race01-Pista-Terra");
            Pista pista5 = new Pista("Pista de Terra", "Race01-Pista-Terra");
            Pista pista6 = new Pista("Pista de Terra", "Race01-Pista-Terra");
            Pista pista7 = new Pista("Pista de Terra", "Race01-Pista-Terra");
            Pista pista8 = new Pista("Pista de Terra", "Race01-Pista-Terra");
            Pista pista9 = new Pista("Pista de Terra", "Race01-Pista-Terra");
            pistas.Add(pista1); pistas.Add(pista2); pistas.Add(pista3);
            pistas.Add(pista4); pistas.Add(pista5); pistas.Add(pista6);
            pistas.Add(pista7); pistas.Add(pista8); pistas.Add(pista9);

            // Dados do campeonato.
            quantidadeDePistas = pistas.Count;
            premiacaoPrimeiroLugar = 7500f;
            premiacaoSegundoLugar = 5000f;
            premiacaoTerceiroLugar = 2000f;
            premiacaoQuartoLugar = 1000f;

        }
        else if (torneio == Torneio.Golden_CUP)
        {
            // Inicializa os pilotos.
            piloto3 = new Piloto(3, "L.Hamilton", 98, 96, 92, 94, Carro.Modelo.MiniCupa_GREEN, new Carro_Upgrades(), Piloto.NumeroDoPiloto.Um, pneusDuros, pneusMedios, pneusMacios, pneusMolhado, pneusChuva);
            piloto4 = new Piloto(4, "G.Russell", 94, 96, 92, 94, Carro.Modelo.MiniCupa_GREEN, new Carro_Upgrades(), Piloto.NumeroDoPiloto.Dois, pneusDuros, pneusMedios, pneusMacios, pneusMolhado, pneusChuva);

            piloto5 = new Piloto(5, "M.Verstappen", 98, 96, 92, 94, Carro.Modelo.MiniCupa_YELLOW, new Carro_Upgrades(), Piloto.NumeroDoPiloto.Um, pneusDuros, pneusMedios, pneusMacios, pneusMolhado, pneusChuva);
            piloto6 = new Piloto(6, "S.Pérez", 93, 96, 92, 94, Carro.Modelo.MiniCupa_YELLOW, new Carro_Upgrades(), Piloto.NumeroDoPiloto.Dois, pneusDuros, pneusMedios, pneusMacios, pneusMolhado, pneusChuva);

            piloto7 = new Piloto(7, "F.Alonso", 96, 96, 92, 94, Carro.Modelo.MiniCupa_BLUE, new Carro_Upgrades(), Piloto.NumeroDoPiloto.Um, pneusDuros, pneusMedios, pneusMacios, pneusMolhado, pneusChuva);
            piloto8 = new Piloto(8, "Lance Stroll", 92, 96, 92, 94, Carro.Modelo.MiniCupa_BLUE, new Carro_Upgrades(), Piloto.NumeroDoPiloto.Dois, pneusDuros, pneusMedios, pneusMacios, pneusMolhado, pneusChuva);

            // Adiciona as pistas.
            Pista pista1 = new Pista("Pista de Terra", "Race01-Pista-Terra");
            Pista pista2 = new Pista("Pista de Terra", "Race01-Pista-Terra");
            Pista pista3 = new Pista("Pista de Terra", "Race01-Pista-Terra");
            Pista pista4 = new Pista("Pista de Terra", "Race01-Pista-Terra");
            Pista pista5 = new Pista("Pista de Terra", "Race01-Pista-Terra");
            Pista pista6 = new Pista("Pista de Terra", "Race01-Pista-Terra");
            Pista pista7 = new Pista("Pista de Terra", "Race01-Pista-Terra");
            Pista pista8 = new Pista("Pista de Terra", "Race01-Pista-Terra");
            Pista pista9 = new Pista("Pista de Terra", "Race01-Pista-Terra");
            Pista pista10 = new Pista("Pista de Terra", "Race01-Pista-Terra");
            Pista pista11 = new Pista("Pista de Terra", "Race01-Pista-Terra");
            Pista pista12 = new Pista("Pista de Terra", "Race01-Pista-Terra");
            pistas.Add(pista1); pistas.Add(pista2); pistas.Add(pista3);
            pistas.Add(pista4); pistas.Add(pista5); pistas.Add(pista6);
            pistas.Add(pista7); pistas.Add(pista8); pistas.Add(pista9);
            pistas.Add(pista10); pistas.Add(pista11); pistas.Add(pista12);

            // Dados do campeonato.
            quantidadeDePistas = pistas.Count;
            premiacaoPrimeiroLugar = 16000f;
            premiacaoSegundoLugar = 9000f;
            premiacaoTerceiroLugar = 5000f;
            premiacaoQuartoLugar = 2500f;
        }

        // Adiciona todos os pilotos a lista.
        pilotosParticipantes.Add(PlayerSettings.pilotoJogador);
        pilotosParticipantes.Add(PlayerSettings.pilotoAliado);
        pilotosParticipantes.Add(piloto3);
        pilotosParticipantes.Add(piloto4);
        pilotosParticipantes.Add(piloto5);
        pilotosParticipantes.Add(piloto6);
        pilotosParticipantes.Add(piloto7);
        pilotosParticipantes.Add(piloto8);
    }

    public Pista GetPistaAtual()
    {
        return pistas[pistaAtualIndex];
    }

    public void AvancarProximaEtapaCampeonato()
    {
        // Se a pista esta em classificação, avança pra corrida.
        if (pistas[pistaAtualIndex].etapaPista == RaceManager.TipoDeCorrida.Classificacao)
        {
            pistas[pistaAtualIndex].etapaPista = RaceManager.TipoDeCorrida.Corrida;
            return;
        }

        // Se a pista anterior foi concluida, avança o index.
        pistaAtualIndex++;

        // Se acabou as pistas do campeonato, todas voltam ao estado Classificação e retorna a pista '0'.
        if (pistaAtualIndex >= pistas.Count)
        {
            foreach (Pista pista in pistas)
            {
                pista.etapaPista = RaceManager.TipoDeCorrida.Classificacao;
            }

            pistaAtualIndex = 0;
        }
    }

    public void PontuarPilotos()
    {
        foreach (Piloto piloto in pilotosParticipantes)
        {
            switch(piloto.Carro.pilotoController.posicaoAtual)
            {
                case 1:
                    piloto.PontuacaoCampeonato += pontuacaoPrimeiroLugar;
                    break;

                case 2:
                    piloto.PontuacaoCampeonato += pontuacaoSegundoLugar;
                    break;

                case 3:
                    piloto.PontuacaoCampeonato += pontuacaoTerceiroLugar;
                    break;

                case 4:
                    piloto.PontuacaoCampeonato += pontuacaoQuartoLugar;
                    break;

                case 5:
                    piloto.PontuacaoCampeonato += pontuacaoQuintoLugar;
                    break;

                case 6:
                    piloto.PontuacaoCampeonato += pontuacaoSextoLugar;
                    break;

                case 7:
                    piloto.PontuacaoCampeonato += pontuacaoSetimoLugar;
                    break;

                case 8:
                    piloto.PontuacaoCampeonato += pontuacaoOitavoLugar;
                    break;
            }
        }
    }
}
