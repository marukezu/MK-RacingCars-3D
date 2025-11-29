using UnityEngine.UI;

public class Painel_RACE_Jogo : Panel
{
    public override PanelType Type => PanelType.RACE_JOGO;

    public Button Btn_Expansao_Painel_ControlarPiloto;
    public Button Btn_Expansao_Painel_Classificacao;
    public Button Btn_Expansao_Painel_InfoVeiculo;
    public Button Btn_Expansao_Painel_DataHora;
    public Button Btn_Expansao_Painel_GameSpeed;

    public override void Initialize(object param1 = null, object param2 = null, object param3 = null)
    {
        base.Initialize(param1, param2, param3);

        Btn_Expansao_Painel_ControlarPiloto.onClick.AddListener(delegate { BTN_Expansao_Painel_ControlarPiloto(); });
        Btn_Expansao_Painel_Classificacao.onClick.AddListener(delegate { BTN_Expansao_Painel_Placar(); });
        Btn_Expansao_Painel_InfoVeiculo.onClick.AddListener(delegate { BTN_Expansao_Painel_InfoVeiculo(); });
        Btn_Expansao_Painel_DataHora.onClick.AddListener(delegate { BTN_Expansao_Painel_DataHora(); });
        Btn_Expansao_Painel_GameSpeed.onClick.AddListener(delegate { BTN_Expansao_Painel_GameSpeed(); });
    }

    public void BTN_Expansao_Painel_ControlarPiloto()
    {
        // Se o painel não estiver em cena.
        if (PanelManager.Instance.GetPanel(PanelType.RACE_PILOTOCONTROLLER) == null)
            PanelManager.Instance.InstanciarERetornarPainel(PanelType.RACE_PILOTOCONTROLLER);
        // Se o painel estiver em cena.
        else
            PanelManager.Instance.FecharPainel(PanelType.RACE_PILOTOCONTROLLER);
    }

    public void BTN_Expansao_Painel_Placar()
    {
        // Se o painel não estiver em cena.
        if (PanelManager.Instance.GetPanel(PanelType.RACE_PLACAR) == null)
            PanelManager.Instance.InstanciarERetornarPainel(PanelType.RACE_PLACAR);
        // Se o painel estiver em cena.
        else
            PanelManager.Instance.FecharPainel(PanelType.RACE_PLACAR);
    }

    public void BTN_Expansao_Painel_InfoVeiculo()
    {
        // Se o painel não estiver em cena.
        if (PanelManager.Instance.GetPanel(PanelType.RACE_CARINFO) == null)
            PanelManager.Instance.InstanciarERetornarPainel(PanelType.RACE_CARINFO);
        // Se o painel estiver em cena.
        else
            PanelManager.Instance.FecharPainel(PanelType.RACE_CARINFO);
    }

    public void BTN_Expansao_Painel_DataHora()
    {
        // Se o painel não estiver em cena.
        if (PanelManager.Instance.GetPanel(PanelType.RACE_DATAHORA) == null)
            PanelManager.Instance.InstanciarERetornarPainel(PanelType.RACE_DATAHORA);
        // Se o painel estiver em cena.
        else
            PanelManager.Instance.FecharPainel(PanelType.RACE_DATAHORA);
    }

    public void BTN_Expansao_Painel_GameSpeed()
    {
        // Se o painel não estiver em cena.
        if (PanelManager.Instance.GetPanel(PanelType.RACE_GAMESPEED) == null)
            PanelManager.Instance.InstanciarERetornarPainel(PanelType.RACE_GAMESPEED);
        // Se o painel estiver em cena.
        else
            PanelManager.Instance.FecharPainel(PanelType.RACE_GAMESPEED);
    }
}
