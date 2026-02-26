using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SE_Modulo_ModosConducao;
using static PilotoSim_Mentalidade;

public class Panel_Race_PilotoController : Panel
{
    public override PanelType Type => PanelType.RACE_PILOTOCONTROLLER;

    private PilotoSim piloto => GameManager.Instance.carEmFoco.piloto;  

    [Header("====== Informações Principais ======")]
    public TextMeshProUGUI TXT_PilotoNome;
    public TextMeshProUGUI TXT_PilotoPosicao;

    [Header("====== Condução e Potência ======")]
    public Slider Slider_Conducao;
    public Slider Slider_Potencia;
    public Slider Slider_CondPneus;
    public Slider Slider_CondCombustivel;

    [Header("====== Condução e Potência (Setas de Indicações de Consumo/Recuperação) ======")]
    public GameObject[] GO_SetasPositivas_Pneus;
    public GameObject[] GO_SetasNegativas_Pneus;
    public GameObject[] GO_SetasPositivas_Combustivel;
    public GameObject[] GO_SetasNegativas_Combustivel;

    [Header("====== Mentalidade ======")]
    public Button BTN_Mentalidade_Defensiva;
    public Button BTN_Mentalidade_Forte;
    public Button BTN_Mentalidade_Ofensiva;
    public Button BTN_Mentalidade_Agressiva;

    [Header("====== Delegar Comandos ======")]
    public Toggle Toggle_Delegar_Conducao;
    public Toggle Toggle_Delegar_Mentalidade;

    public override void Initialize(PanelParams param = null)
    {
        Slider_CondPneus.maxValue = 100;
        Slider_CondCombustivel.maxValue = 100;

        // Delegate aos buttons
        BTN_Mentalidade_Defensiva.onClick.AddListener(BTN_Mentalidade_Defensiva_Action);
        BTN_Mentalidade_Forte.onClick.AddListener(BTN_Mentalidade_Forte_Action);
        BTN_Mentalidade_Ofensiva.onClick.AddListener(BTN_Mentalidade_Ofensiva_Action);
        BTN_Mentalidade_Agressiva.onClick.AddListener(BTN_Mentalidade_Agressiva_Action);

        // Delegate ao Toggle -> Delegar Condução.
        Toggle_Delegar_Conducao.isOn = piloto.IA_Controla_Conducao;
        Toggle_Delegar_Conducao.onValueChanged.AddListener(Set_Toggle_DelegarConducao);

        // Delegate ao Toggles -> Delegar Mentalidade.
        Toggle_Delegar_Mentalidade.isOn = piloto.IA_Controla_Mentalidade;
        Toggle_Delegar_Mentalidade.onValueChanged.AddListener(Set_Toggle_DelegarMentalidade);

        // Detectar clique no Slider Condução
        Slider_Conducao.GetComponent<ClickDetector>().onClick = () =>
        {
            if (piloto.IA_Controla_Conducao)
            {
                piloto.Set_DelegarConducao(false);
                Toggle_Delegar_Conducao.isOn = false;
            }
        };

        // Detectar clique no Slider Potência
        Slider_Potencia.GetComponent<ClickDetector>().onClick = () =>
        {
            if (piloto.IA_Controla_Conducao)
            {
                piloto.Set_DelegarConducao(false);
                Toggle_Delegar_Conducao.isOn = false;
            }
        };

        // Seta o painel a primeira vez.
        SetText_PilotoNome(piloto, piloto.Nome);
        SetText_PilotoPosition(piloto, piloto.PosicaoCorrida);
        SetSlider_CondPneus(piloto, piloto.mod_Conducao.Cond_Pneus);
        SetSlider_CondCombustivel(piloto, piloto.mod_Conducao.Cond_Combustivel);
        Slider_Conducao.value = piloto.mod_Conducao.Fator_Conducao;
        Slider_Potencia.value = piloto.mod_Conducao.Fator_Potencia;

        SubscribeEvents(true);
    }

    public override void AtualizarPainel()
    {
        SetSlider_Conducao();
        SetSlider_Potencia();
        SetButtons_Colors();
    }

    public override void SubscribeEvents(bool subscribe)
    {
        if (subscribe)
        {
            EventBus.On_Piloto_PosicaoAtualChanged += SetText_PilotoPosition;
            EventBus.On_ModosConducao_Car_ConditionPneusChanged += SetSlider_CondPneus;
            EventBus.On_ModosConducao_Car_ConditionCombustivelChanged += SetSlider_CondCombustivel;
            EventBus.On_ModosConducao_PneusTrendChanged += SetPneusTrend;
            EventBus.On_ModosConducao_CombustivelTrendChanged += SetCombustivelTrend;
        }
        else
        {
            EventBus.On_Piloto_PosicaoAtualChanged -= SetText_PilotoPosition;
            EventBus.On_ModosConducao_Car_ConditionPneusChanged -= SetSlider_CondPneus;
            EventBus.On_ModosConducao_Car_ConditionCombustivelChanged -= SetSlider_CondCombustivel;
            EventBus.On_ModosConducao_PneusTrendChanged -= SetPneusTrend;
            EventBus.On_ModosConducao_CombustivelTrendChanged -= SetCombustivelTrend;
        }
    }

    private void SetText_PilotoNome(PilotoSim p, string value)
    {
        if (!ReferenceEquals(p, piloto)) return;
        TXT_PilotoNome.text = piloto.Nome;
    }

    private void SetText_PilotoPosition(PilotoSim p, int position)
    {
        if (!ReferenceEquals(p, piloto)) return;

        // Define a cor baseada na posição
        Color cor;

        if (position == 1)
        {
            // Ouro (ouro suave, não neon)
            cor = new Color32(230, 200, 40, 255);
        }
        else if (position == 2 || position == 3)
        {
            // Prata
            cor = new Color32(200, 200, 220, 255);
        }
        else
        {
            // Azul intermediário (posição OK)
            cor = new Color32(150, 150, 150, 255);
        }

        // Aplica a cor e o texto
        TXT_PilotoPosicao.text = position.ToString() + "°";
        TXT_PilotoPosicao.color = cor;
    }

    // ===============================================================
    // ===================== Condução e Potência =====================
    // ===============================================================
    private void SetSlider_CondPneus(PilotoSim p, float value)
    {
        if (!ReferenceEquals(p, piloto)) return;
        Slider_CondPneus.value = value;
    }

    private void SetSlider_CondCombustivel(PilotoSim p, float value)
    {
        if (!ReferenceEquals(p, piloto)) return;
        Slider_CondCombustivel.value = value;
    }

    private void SetSlider_Conducao()
    {
        if (piloto.IA_Controla_Conducao)
            Slider_Conducao.value = piloto.mod_Conducao.Fator_Conducao;

        else
            piloto.mod_Conducao.Set_FatorConducao(Slider_Conducao.value);
    }

    private void SetSlider_Potencia()
    {
        if (piloto.IA_Controla_Conducao)
            Slider_Potencia.value = piloto.mod_Conducao.Fator_Potencia;

        else
            piloto.mod_Conducao.Set_FatorPotencia(Slider_Potencia.value);
    }

    private void SetPneusTrend(PilotoSim p, TrendLevel trend)
    {
        if (!ReferenceEquals(p, piloto)) return;

        // desliga tudo
        foreach (var go in GO_SetasPositivas_Pneus) go.SetActive(false);
        foreach (var go in GO_SetasNegativas_Pneus) go.SetActive(false);

        switch (trend)
        {
            case TrendLevel.Up1:
                GO_SetasPositivas_Pneus[0].SetActive(true);
                break;
            case TrendLevel.Up2:
                GO_SetasPositivas_Pneus[0].SetActive(true);
                GO_SetasPositivas_Pneus[1].SetActive(true);
                break;
            case TrendLevel.Up3:
                GO_SetasPositivas_Pneus[0].SetActive(true);
                GO_SetasPositivas_Pneus[1].SetActive(true);
                GO_SetasPositivas_Pneus[2].SetActive(true);
                break;

            case TrendLevel.Down1:
                GO_SetasNegativas_Pneus[0].SetActive(true);
                break;
            case TrendLevel.Down2:
                GO_SetasNegativas_Pneus[0].SetActive(true);
                GO_SetasNegativas_Pneus[1].SetActive(true);
                break;
            case TrendLevel.Down3:
                GO_SetasNegativas_Pneus[0].SetActive(true);
                GO_SetasNegativas_Pneus[1].SetActive(true);
                GO_SetasNegativas_Pneus[2].SetActive(true);
                break;

            case TrendLevel.Neutral:
            default:
                break;
        }
    }

    private void SetCombustivelTrend(PilotoSim p, TrendLevel trend)
    {
        if (!ReferenceEquals(p, piloto)) return;

        // desliga tudo
        foreach (var go in GO_SetasPositivas_Combustivel) go.SetActive(false);
        foreach (var go in GO_SetasNegativas_Combustivel) go.SetActive(false);

        switch (trend)
        {
            case TrendLevel.Up1:
                GO_SetasPositivas_Combustivel[0].SetActive(true);
                break;
            case TrendLevel.Up2:
                GO_SetasPositivas_Combustivel[0].SetActive(true);
                GO_SetasPositivas_Combustivel[1].SetActive(true);
                break;
            case TrendLevel.Up3:
                GO_SetasPositivas_Combustivel[0].SetActive(true);
                GO_SetasPositivas_Combustivel[1].SetActive(true);
                GO_SetasPositivas_Combustivel[2].SetActive(true);
                break;

            case TrendLevel.Down1:
                GO_SetasNegativas_Combustivel[0].SetActive(true);
                break;
            case TrendLevel.Down2:
                GO_SetasNegativas_Combustivel[0].SetActive(true);
                GO_SetasNegativas_Combustivel[1].SetActive(true);
                break;
            case TrendLevel.Down3:
                GO_SetasNegativas_Combustivel[0].SetActive(true);
                GO_SetasNegativas_Combustivel[1].SetActive(true);
                GO_SetasNegativas_Combustivel[2].SetActive(true);
                break;

            case TrendLevel.Neutral:
            default:
                break;
        }
    }

    // ===============================================================
    // ========================= Mentalidade =========================
    // ===============================================================
    private void BTN_Mentalidade_Defensiva_Action()
    {
        // Desativa a mentalidade e seta o toggle de acordo
        piloto.Set_DelegarMentalidade(false);
        Toggle_Delegar_Mentalidade.isOn = piloto.IA_Controla_Mentalidade;

        piloto.mod_Mentalidade.SelecionarMentalidade(TipoMentalidade.Defensivo, false);
    }

    private void BTN_Mentalidade_Forte_Action()
    {
        // Desativa a mentalidade e seta o toggle de acordo
        piloto.Set_DelegarMentalidade(false);
        Toggle_Delegar_Mentalidade.isOn = piloto.IA_Controla_Mentalidade;

        piloto.mod_Mentalidade.SelecionarMentalidade(TipoMentalidade.Forte, false);
    }

    private void BTN_Mentalidade_Ofensiva_Action()
    {
        // Desativa a mentalidade e seta o toggle de acordo
        piloto.Set_DelegarMentalidade(false);
        Toggle_Delegar_Mentalidade.isOn = piloto.IA_Controla_Mentalidade;

        piloto.mod_Mentalidade.SelecionarMentalidade(TipoMentalidade.Ofensivo, false);
    }

    private void BTN_Mentalidade_Agressiva_Action()
    {
        // Desativa a mentalidade e seta o toggle de acordo
        piloto.Set_DelegarMentalidade(false);
        Toggle_Delegar_Mentalidade.isOn = piloto.IA_Controla_Mentalidade;

        piloto.mod_Mentalidade.SelecionarMentalidade(TipoMentalidade.Agressivo, false);
    }

    private void SetButtons_Colors()
    {
        Color32 c_desativado = new Color32(94, 27, 27, 255);
        Color32 c_selecionado = new Color32(153, 45, 45, 255);

        var botoes = new[]
        {
            BTN_Mentalidade_Defensiva,
            BTN_Mentalidade_Forte,
            BTN_Mentalidade_Ofensiva,
            BTN_Mentalidade_Agressiva
        };

        // Pega o index pela mentalidade atual.
        int indexSelecionado = piloto.mod_Mentalidade.MentalidadeAtual switch
        {
            TipoMentalidade.Defensivo => 0,
            TipoMentalidade.Forte => 1,
            TipoMentalidade.Ofensivo => 2,
            TipoMentalidade.Agressivo => 3,
            _ => 0
        };

        // Seta as cores pelo index
        for (int i = 0; i < botoes.Length; i++)
        {
            botoes[i].GetComponent<Image>().color = (i == indexSelecionado)
                                                    ? c_selecionado
                                                    : c_desativado;
        }
    }

    // ===============================================================
    // ====================== Delegar Comandos =======================
    // ===============================================================
    private void Set_Toggle_DelegarConducao(bool isOn)
    {
        piloto.Set_DelegarConducao(isOn);
    }

    private void Set_Toggle_DelegarMentalidade(bool isOn)
    {
        piloto.Set_DelegarMentalidade(isOn);
    }
}
