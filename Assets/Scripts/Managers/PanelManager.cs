using System.Collections.Generic;
using System.Linq;
using Unity.Splines.Examples;
using UnityEngine;
using static Panel;

public class PanelManager : MonoBehaviour
{
    public static PanelManager Instance;

    // Aponta para o canvas principal da Cena, OBS: O painel que será instanciado os paineis modulares PRECISAM ter o nome Exato do "GameObject.Find("NOME")"
    private RectTransform _canvas;
    public RectTransform Canvas
    {
        get
        {
            if (_canvas == null)
                _canvas = GameObject.Find("Painel JOGO").GetComponent<RectTransform>();
            return _canvas;
        }
    }

    [Header("====== Paineis RACE ======")]
    [SerializeField] private GameObject Painel_RACE_Jogo;
    [SerializeField] private GameObject Painel_RACE_Placar;
    [SerializeField] private GameObject Painel_RACE_CarInfo;
    [SerializeField] private GameObject Painel_RACE_PilotoController;
    [SerializeField] private GameObject Painel_RACE_PitStopManager;
    [SerializeField] private GameObject Painel_RACE_Radio;
    [SerializeField] private GameObject Painel_RACE_GameSpeed;
    [SerializeField] private GameObject Painel_RACE_SessaoClassificacao;
    [SerializeField] private GameObject Painel_RACE_DataHora;
    [SerializeField] private GameObject Painel_RACE_Info_SessaoClassificacao;
    [SerializeField] private GameObject Painel_RACE_Info_SessaoCorrida;

    [Header("====== Paineis Flutuantes ======")]
    [SerializeField] private GameObject Panel_Floating_CarInfo;
    [SerializeField] private GameObject Panel_Floating_DuelInfo;

    // Dicionarios
    private Dictionary<PanelType, GameObject> painelDictionary;

    // Lista de paineis ativos no momento
    private List<GameObject> painelList = new List<GameObject>();

    // Para o Floating Panel - Car Info
    private List<PilotoSim> UltimosPilotos = null;

    private void OnDestroy()
    {
        SubscribeEvents(false);
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        DontDestroyOnLoad(gameObject);

        InitializeDictionary();

        SubscribeEvents(true);
    }

    private void SubscribeEvents(bool subscribe)
    {
        if (subscribe)
        {
            EventBus.On_Piloto_PosicaoAtualChanged += Instanciar_PaineisFlutuantes_CarInfo;
        }
        else
        {
            EventBus.On_Piloto_PosicaoAtualChanged -= Instanciar_PaineisFlutuantes_CarInfo;
        }
    }

    private void InitializeDictionary()
    {
        painelDictionary = new Dictionary<PanelType, GameObject>
        {
            // Paineis de RACE
            { PanelType.RACE_JOGO, Painel_RACE_Jogo },
            { PanelType.RACE_PLACAR, Painel_RACE_Placar },
            { PanelType.RACE_CARINFO, Painel_RACE_CarInfo },
            { PanelType.RACE_PILOTOCONTROLLER, Painel_RACE_PilotoController },
            { PanelType.RACE_PITSTOP_MANAGER, Painel_RACE_PitStopManager },
            { PanelType.RACE_RADIO, Painel_RACE_Radio },
            { PanelType.RACE_GAMESPEED, Painel_RACE_GameSpeed },
            { PanelType.RACE_SESSAOCLASSIFICACAO, Painel_RACE_SessaoClassificacao },
            { PanelType.RACE_DATAHORA, Painel_RACE_DataHora },

            { PanelType.RACE_INFO_SESSAOCLASSIFICACAO, Painel_RACE_Info_SessaoClassificacao },
            { PanelType.RACE_INFO_SESSAOCORRIDA, Painel_RACE_Info_SessaoCorrida },

            // Paineis flutuantes
            { PanelType.FLOATING_CARINFO, Panel_Floating_CarInfo },
            { PanelType.FLOATING_DUELINFO, Panel_Floating_DuelInfo },
        };
    }

    // ==========================================================================================
    // =========================== FUNÇÕES PARA MANUSEIO DOS PAINEIS ============================
    // ==========================================================================================
    public GameObject GetPanel(PanelType panelType)
    {
        foreach (GameObject panel in painelList)
        {
            Panel panelScript = panel.GetComponent<Panel>();
            if (panelScript.Type == panelType)
                return panel;
        }

        return null;
    }
    public GameObject InstanciarERetornarPainel(PanelType panelType, PanelParams param = null)
    {
        if (painelDictionary.TryGetValue(panelType, out GameObject painel))
        {
            GameObject newPanel = Instantiate(painel, Canvas);
            newPanel.GetComponent<Panel>().Initialize(param);
            painelList.Add(newPanel);

            return newPanel;
        }

        return null;
    }
    public GameObject InstanciarERetornarPainelFlutuante(PanelType panelType, PanelParams param = null)
    {
        if (painelDictionary.TryGetValue(panelType, out GameObject painel))
        {
            GameObject newPanel = Instantiate(painel);
            newPanel.GetComponent<Panel>().Initialize(param);
            painelList.Add(newPanel);

            return newPanel;
        }

        return null;
    }
    public void FecharPainel(PanelType panelType)
    {
        GameObject painelEncontrado = painelList
            .FirstOrDefault(p => p.GetComponent<Panel>()?.Type == panelType);

        if (painelEncontrado != null)
        {
            painelEncontrado.GetComponent<Panel>().FecharPainel();
        }
    }
    public void FecharTodosPaineis()
    {
        foreach (GameObject painel in new List<GameObject>(painelList))
        {
            if (painel != null)
                painel.GetComponent<Panel>().FecharPainel();
        }

        painelList.Clear(); // limpa a lista de vez
    }

    public void UnregisterPanel(Panel panel)
    {
        painelList.Remove(panel.gameObject);
    }

    // ==========================================================================================
    // =========================== FUNÇÕES DE INSTACIAÇÃO DE PAINEIS ============================
    // ==========================================================================================
    private void Instanciar_PaineisFlutuantes_CarInfo(PilotoSim pilotoQueMudou, int pos)
    {
        PilotoSim meuPiloto = GameManager.Instance.carEmFoco.piloto;

        // 1️⃣ Ordena por posição real
        var ordenados = PilotosDataBase.Pilotos_CampeonatoAtual
            .OrderBy(p => p.PosicaoCorrida)
            .ToList();

        int minhaPos = meuPiloto.PosicaoCorrida;
        int total = ordenados.Count;

        // 2️⃣ Recalcula quem DEVE aparecer
        List<PilotoSim> novos = new List<PilotoSim>();
        novos.Add(meuPiloto);

        if (minhaPos > 1)
            novos.Add(ordenados[minhaPos - 2]);  // piloto à frente

        if (minhaPos < total)
            novos.Add(ordenados[minhaPos]);      // piloto atrás

        // 3️⃣ Se NADA mudou desde o último trio, não faz nada
        if (UltimosPilotos != null &&
            novos.SequenceEqual(UltimosPilotos))
            return;

        UltimosPilotos = novos.ToList();

        // 4️⃣ Destroi paineis antigos
        foreach (var panel in painelList)
        {
            Panel ps = panel.GetComponent<Panel>();
            if (ps.Type == PanelType.FLOATING_CARINFO)
                ps.FecharPainel();
        }

        // 5️⃣ Instancia os novos
        foreach (var p in novos)
        {
            PanelParams prm = new PanelParams();
            prm.pilotoSim1 = p;
            InstanciarERetornarPainelFlutuante(PanelType.FLOATING_CARINFO, prm);
        }
    }

    public void InstanciarPaineisClassificacao()
    {
        InstanciarERetornarPainel(PanelType.RACE_CARINFO);
        InstanciarERetornarPainel(PanelType.RACE_DATAHORA);
        InstanciarERetornarPainel(PanelType.RACE_PILOTOCONTROLLER);
        InstanciarERetornarPainel(PanelType.RACE_SESSAOCLASSIFICACAO);
        InstanciarERetornarPainel(PanelType.RACE_GAMESPEED);
    }
    public void InstanciarPaineisCorrida()
    {
        InstanciarERetornarPainel(PanelType.RACE_PILOTOCONTROLLER);
    }
}