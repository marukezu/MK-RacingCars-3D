public static class PlayerSettings
{
    // Data
    public static float diaAtual = 0;

    // Jogador e aliado. (pilotos 1 e 2)
    public static Piloto pilotoJogador = null;
    public static Piloto pilotoAliado = null;

    public static Campeonato campeonato = null;


    // Função para criar um novo save.
    public static void InitializeNewSave()
    {
        int pneusDuros = 3;
        int pneusMedios = 3;
        int pneusMacios = 3;
        int pneusMolhado = 3;
        int pneusChuva = 3;

        // Configuração do Piloto 01 do jogador.
        string playerName = "Marukezu";
        float p1_hAcel = 75;
        float p1_hUltra = 75;
        float p1_hDesgP = 75;
        float p1_hGastoC = 75;

        // Configuração do Piloto 02 do jogador.
        string aliadoName = "Fernando Silva";
        float p2_hAcel = 75;
        float p2_hUltra = 75;
        float p2_hDesgP = 75;
        float p2_hGastoC = 75;

        // Configuração da Equipe do jogador.
        Carro.Modelo carroModelo = Carro.Modelo.MiniCupa_RED;

        // Inicializa o piloto do jogador.
        pilotoJogador = new Piloto(1, playerName, p1_hAcel, p1_hUltra,p1_hDesgP,p1_hGastoC, carroModelo, new Carro_Upgrades(), Piloto.NumeroDoPiloto.Um, pneusDuros, pneusMedios, pneusMacios, pneusMolhado, pneusChuva);

        // Inicializa o piloto aliado.
        pilotoAliado = new Piloto(2, aliadoName, p2_hAcel, p2_hUltra, p2_hDesgP, p2_hGastoC, carroModelo, new Carro_Upgrades(), Piloto.NumeroDoPiloto.Dois, pneusDuros, pneusMedios, pneusMacios, pneusMolhado, pneusChuva);

        campeonato = new Campeonato();
        campeonato.InitializeCampeonato(Campeonato.Torneio.Debug_CUP);
    }
}
