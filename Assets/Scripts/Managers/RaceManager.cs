using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RaceManager : MonoBehaviour
{
    // Enums
    public enum TipoDeCorrida
    {
        Treino,
        Classificacao,
        Corrida
    }

    public static RaceManager Instance;

    [Header("=========== CONFIGURAÇÃO DE CORRIDA ============")]
    public bool corridaEmAndamento;
    public bool corridaFinalizada;
    public int voltasTotais;

    [Header("=========== CONFIGURAÇÃO DE CLASSIFICAÇÃO ============")]
    public float tempoTotalClassificacao = 3600; // Inicia com tempo de 1 HORA real para classificação.
    public bool classificacaoEmAndamento;
    public bool classificacaoFinalizada;

    [HideInInspector] public TipoDeCorrida tipoDeCorrida;
    [HideInInspector] public float contadorTempoTotalSessao;
    [HideInInspector] public float carrosNaPista = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Update()
    {
        // Se estiver em classificação.
        if (tipoDeCorrida == TipoDeCorrida.Classificacao)
        {
            if (classificacaoEmAndamento && !classificacaoFinalizada)
            {
                // Contador do tempo de duração da classificação
                tempoTotalClassificacao -= Time.deltaTime;

                // Se o tempo de classificação terminar.
                if (tempoTotalClassificacao <= 0)
                {
                    // Finaliza a classificação.
                    tempoTotalClassificacao = 0f;
                    classificacaoFinalizada = true;
                    Instance.StartCoroutine(FinalizarClassificacao());
                }
            }
        }

        // Se estiver em corrida.
        else if (tipoDeCorrida == TipoDeCorrida.Corrida)
        {
            if (corridaEmAndamento)
            {
                // Contador tempo total corrida.
                contadorTempoTotalSessao += Time.deltaTime;

                // Corrida não finalizada e checa quando o primeiro jogador finalizar a corrida.
                if (!corridaFinalizada)
                {
                    foreach (Piloto piloto in PlayerSettings.campeonato.pilotosParticipantes)
                    {
                        if (piloto.Carro.pilotoController.voltaAtual > voltasTotais)
                        {
                            corridaFinalizada = true;
                            StartCoroutine(FinalizarCorrida());
                            break;
                        }
                    }
                }
            }
        }
    }

    // =============================================================================================================
    // ========================================== Inicializa o Manager =============================================
    // =============================================================================================================
    public void Inicializar()
    {
        // Inicia um novo save na classe estática PlayerSettings
        PlayerSettings.InitializeNewSave();

        // Inicia o tipo de corrida (Classificação - Corrida)
        tipoDeCorrida = PlayerSettings.campeonato.GetPistaAtual().etapaPista;

        // Ajusta a velocidade do jogo.
        GameManager.Instance.SetGameSpeed(1f);

        // Fade Escuro -> Claro
        StartCoroutine(GameManager.Instance.FazerPanelFade(PanelManager.Instance.Canvas.GetComponent<Image>(), 3f, false));

        // Inicializa o jogo conforme o tipo de corrida.
        switch (tipoDeCorrida)
        {
            case TipoDeCorrida.Classificacao:
                InitializeClassificacao();
                break;

            case TipoDeCorrida.Corrida:
                InitializeCorrida();
                break;
        }

        // Inicializa a Camera
        CameraManager.Instance.Inicializar();
    }

    #region Classificação
    // =============================================================================================================
    // ======================================= Inicializa a Classificação ==========================================
    // =============================================================================================================
    private void InitializeClassificacao()
    {
        // Ativa os paineis de informação da classificação.
        PanelManager.Instance.InstanciarERetornarPainel(Panel.PanelType.RACE_INFO_SESSAOCLASSIFICACAO);

        // Lista de pilotos.
        List<Piloto> pilotos = PlayerSettings.campeonato.pilotosParticipantes;

        // Ordena pela ordem correta
        pilotos.OrderBy(p => p.ID).ToList();

        // Instancia os carros e associa ao piloto correspondente.
        for (int i = 0; i < pilotos.Count; i++)
        {
            Piloto piloto = pilotos[i];
            Transform posicao = PistaManager.Instance.posicoesPitStop[i];

            // Instancia o carro.
            GameObject carroGO = Instantiate(
                GameManager.Instance.GetCarroByPiloto(piloto),
                posicao.position,
                posicao.rotation
            );

            // Obtém o componente Carro.
            Carro carro = carroGO.GetComponent<Carro>();

            // Inicializa o carro para classificação.
            carro.InicializarParaClassificacao(piloto);
        }
    }

    private IEnumerator FinalizarClassificacao()
    {
        // Ordena os pilotos pela volta mais rápida
        var pilotosOrdenados = PlayerSettings.campeonato.pilotosParticipantes
                                .OrderBy(p => p.Carro.pilotoController.tempoVoltaMaisRapida)
                                .ToList();

        // Atualiza a posição dos pilotos no grid
        int posicao = 1;
        foreach (Piloto piloto in pilotosOrdenados)
        {
            piloto.PosicaoGrid = posicao;
            posicao++;
        }

        // Faz a câmera apontar para o primeiro piloto (vencedor)
        Piloto vencedor = pilotosOrdenados[0];
        GameManager.Instance.SetPilotoEmFoco(vencedor);

        // Bloqueia a troca de câmera
        CameraManager.Instance.podeTrocarCamera = false;
        GameManager.Instance.SetGameSpeed(1f);

        // Fecha paineis se abertos.
        PanelManager.Instance.FecharPainel(Panel.PanelType.RACE_PLACAR);
        PanelManager.Instance.FecharPainel(Panel.PanelType.RACE_PITSTOP_MANAGER);

        // Espera 5 segundos
        yield return new WaitForSeconds(5f);

        // Faz o fade e espera sua conclusão
        yield return GameManager.Instance.FazerPanelFade(PanelManager.Instance.Canvas.GetComponent<Image>(), 2f, true);

        // Espera mais um breve tempo
        yield return new WaitForSeconds(0.5f);

        // Avança a etapa do campeonato.
        PlayerSettings.campeonato.AvancarProximaEtapaCampeonato();

        // Carrega a próxima cena
        SceneManager.LoadScene(PlayerSettings.campeonato.GetPistaAtual().SceneName);
    }
    #endregion

    #region Sessão de Corrida
    // =============================================================================================================
    // ========================================== Inicializa a Corrida =============================================
    // =============================================================================================================
    private void InitializeCorrida()
    {
        // Ativa o painel de informação da corrida.
        PanelManager.Instance.InstanciarERetornarPainel(Panel.PanelType.RACE_INFO_SESSAOCORRIDA);


        var ordemPilotos = PlayerSettings.campeonato.pilotosParticipantes
                    .OrderBy(piloto => piloto.PosicaoGrid)
                    .ToList();

        // Lista de posições no grid (assume que você já tem uma lista de posições configurada)
        Transform[] posicoesGrid = { PistaManager.Instance.posicao_Grid_1, PistaManager.Instance.posicao_Grid_2, PistaManager.Instance.posicao_Grid_3, PistaManager.Instance.posicao_Grid_4,
                                     PistaManager.Instance.posicao_Grid_5, PistaManager.Instance.posicao_Grid_6, PistaManager.Instance.posicao_Grid_7, PistaManager.Instance.posicao_Grid_8 };

        Carro carroInstanciado = null;

        for (int i = 0; i < ordemPilotos.Count && i < posicoesGrid.Length; i++)
        {
            Piloto pilotoAtual = ordemPilotos[i]; // Pega o piloto correspondente
            Transform posicaoAtual = posicoesGrid[i]; // Pega a posição no grid correspondente

            // Instancia o carro para o piloto
            GameObject carro = Instantiate(GameManager.Instance.GetCarroByPiloto(pilotoAtual),
                                           posicaoAtual.position,
                                           posicaoAtual.rotation);

            // Associações - configurações
            carroInstanciado = carro.GetComponent<Carro>();
            carroInstanciado.InicializarParaCorrida(pilotoAtual);
        }
    }
    public IEnumerator ContagemLargada()
    {
        // Contador de largada.
        yield return new WaitForSeconds(1f);
        Debug.Log("5");
        yield return new WaitForSeconds(1f);
        Debug.Log("4");
        yield return new WaitForSeconds(1f);
        Debug.Log("3");
        yield return new WaitForSeconds(1f);
        Debug.Log("2");
        yield return new WaitForSeconds(1f);
        Debug.Log("1");
        yield return new WaitForSeconds(1f);

        AutorizarLargada();
    }
    public void AutorizarLargada()
    {
        foreach (Piloto piloto in PlayerSettings.campeonato.pilotosParticipantes)
        {
            piloto.Carro.pilotoController.foiDadoLargada = true;
        }
        corridaEmAndamento = true;
    }
    private IEnumerator FinalizarCorrida()
    {
        // Ordena os pilotos pela posição de corrida
        var pilotosOrdenados = PlayerSettings.campeonato.pilotosParticipantes
                                .OrderBy(p => p.Carro.pilotoController.posicaoAtual)
                                .ToList();

        // Adiciona a pontuação da corrida aos pilotos.
        PlayerSettings.campeonato.PontuarPilotos();

        // Avisa na hud que a corrida terminou.
        Debug.Log("Corrida Finalizada");

        // Aguarda até que todos os carros finalizem a corrida ou no máximo 15 segundos.
        yield return new WaitForSeconds(15f);

        // Faz a câmera apontar para o primeiro piloto (vencedor)
        Piloto vencedor = pilotosOrdenados[0];
        CameraManager.Instance.AtualizarCamera(vencedor.Carro);

        // Bloqueia a troca de câmera
        CameraManager.Instance.podeTrocarCamera = false;
        GameManager.Instance.SetGameSpeed(1f);

        // Fecha paineis se abertos.
        PanelManager.Instance.FecharPainel(Panel.PanelType.RACE_PLACAR);
        PanelManager.Instance.FecharPainel(Panel.PanelType.RACE_PITSTOP_MANAGER);

        // Exibe a hud da nova pontuação do campeonato.

        // Espera 5 segundos
        yield return new WaitForSeconds(5f);

        // Faz o fade e espera sua conclusão
        yield return GameManager.Instance.FazerPanelFade(PanelManager.Instance.Canvas.GetComponent<Image>(), 2f, true);

        // Espera mais um breve tempo
        yield return new WaitForSeconds(0.5f);

        // Avança a etapa do campeonato.
        PlayerSettings.campeonato.AvancarProximaEtapaCampeonato();

        // Carrega a próxima cena
        SceneManager.LoadScene(PlayerSettings.campeonato.GetPistaAtual().SceneName);
    }
    #endregion
}
