using UnityEngine;
using UnityEngine.UI;

public class Painel_RACE_PitstopManager : Panel
{
    public override PanelType Type => PanelType.RACE_PITSTOP_MANAGER;
    private Piloto piloto => GameManager.Instance.pilotoEmFoco;
    private int pneuSelecionado;

    [Header("====== Painel Informação Carro - Texts ======")]
    public Text Txt_PilotoName;
    public Text Txt_CombustivelRestante;
    public Text Txt_DuracaoPneuFL;
    public Text Txt_DuracaoPneuFR;
    public Text Txt_DuracaoPneuRL;
    public Text Txt_DuracaoPneuRR;
    public Text Txt_ChanceErroPit;
    public Text Txt_TempoGastoPit;

    [Header("====== Painel Informação Carro - Images ======")]
    public Image IMG_PneuFL;
    public Image IMG_PneuFR;
    public Image IMG_PneuRL;
    public Image IMG_PneuRR;

    [Header("====== Painel Informação Carro - Sliders ======")]
    public Slider Slider_Combustivel;

    [Header("====== Painel Selecionar Alterações - Texts ======")]
    public Text Txt_PneuDuroRestantes;
    public Text Txt_PneuMedioRestantes;
    public Text Txt_PneuMacioRestantes;

    [Header("====== Painel Selecionar Alterações - Images ======")]
    public Image IMG_PneuSelecionado;

    [Header("====== Painel Selecionar Alterações - Toogles ======")]
    public Toggle Toggle_reabastecer;

    [Header("====== Painel Selecionar Alterações - Buttons ======")]
    public Button Btn_PneuDuro;
    public Button Btn_PneuMedio;
    public Button Btn_PneuMacio;

    [Header("====== Buttons ======")]
    public Button Btn_ConfirmarPit;
    public Button Btn_CancelarPit;

    public override void AbrirPainel(object param1 = null, object param2 = null, object param3 = null)
    {
        base.AbrirPainel(param1, param2, param3);

        Btn_PneuDuro.onClick.AddListener(delegate { BTN_PneuDuro(); });
        Btn_PneuMedio.onClick.AddListener(delegate { BTN_PneuMedio(); });
        Btn_PneuMacio.onClick.AddListener(delegate { BTN_PneuMacio(); });
        Btn_ConfirmarPit.onClick.AddListener(delegate { BTN_ConfirmarPit(); });
        Btn_CancelarPit.onClick.AddListener(delegate { BTN_CancelarPit(); });

        Toggle_reabastecer.isOn = false;
        pneuSelecionado = 0;
    }

    public override void AtualizarPainel()
    {
        Txt_PilotoName.text = piloto.PilotoNome;
        Txt_CombustivelRestante.text = piloto.Carro.carroceria.combustivelAtual.ToString("F0") + "%";
        Txt_DuracaoPneuFL.text = piloto.Carro.carroceria.pneuFL.durabilidadeAtual.ToString("F0");
        Txt_DuracaoPneuFR.text = piloto.Carro.carroceria.pneuFR.durabilidadeAtual.ToString("F0");
        Txt_DuracaoPneuRL.text = piloto.Carro.carroceria.pneuRL.durabilidadeAtual.ToString("F0");
        Txt_DuracaoPneuRR.text = piloto.Carro.carroceria.pneuRR.durabilidadeAtual.ToString("F0");
        Txt_TempoGastoPit.text = GameManager.Instance.ConverterTempoParaFormato(piloto.EquipePitStop.TempoTotalPit);

        IMG_PneuFL.sprite = piloto.Carro.carroceria.pneuFL.GetPneuSpriteIcon();
        IMG_PneuFR.sprite = piloto.Carro.carroceria.pneuFR.GetPneuSpriteIcon();
        IMG_PneuRL.sprite = piloto.Carro.carroceria.pneuRL.GetPneuSpriteIcon();
        IMG_PneuRR.sprite = piloto.Carro.carroceria.pneuRR.GetPneuSpriteIcon();

        Slider_Combustivel.maxValue = 100;
        Slider_Combustivel.value = piloto.Carro.carroceria.combustivelAtual;

        Txt_PneuDuroRestantes.text = piloto.PneuDuroQuantidade.ToString();
        Txt_PneuMedioRestantes.text = piloto.PneuMedioQuantidade.ToString();
        Txt_PneuMacioRestantes.text = piloto.PneuMacioQuantidade.ToString();

        // ========

        float tempoGastoPit = 0f;
        float chanceErro = 0f;

        if (pneuSelecionado == 0)
        {
            IMG_PneuSelecionado.sprite = null;
        }

        // Se vai reabastecer
        if (Toggle_reabastecer.isOn)
        {
            tempoGastoPit += piloto.EquipePitStop.GetTempoTotalReabastecer(piloto.Carro);
            chanceErro += piloto.EquipePitStop.GetChanceErroReabastecer(piloto.Carro);
        }

        // Se vai trocar pneu
        if (pneuSelecionado != 0)
        {
            tempoGastoPit += piloto.EquipePitStop.GetTempoTotalTrocaPneus();
            chanceErro += piloto.EquipePitStop.GetChanceErroTrocaPneus(piloto.Carro);
        }

        Txt_TempoGastoPit.text = GameManager.Instance.ConverterTempoParaFormato(tempoGastoPit);
        Txt_ChanceErroPit.text = chanceErro.ToString("F1") + "%";
    }

    private void BTN_PneuDuro()
    {
        if (piloto.PneuDuroQuantidade <= 0) return;

        IMG_PneuSelecionado.sprite = SpritesManager.Instance.icon_pneuDuro;
        pneuSelecionado = 1;
    }
    private void BTN_PneuMedio()
    {
        if (piloto.PneuMedioQuantidade <= 0) return;

        IMG_PneuSelecionado.sprite = SpritesManager.Instance.icon_pneuMedio;
        pneuSelecionado = 2;
    }
    private void BTN_PneuMacio()
    {
        if (piloto.PneuMacioQuantidade <= 0) return;

        IMG_PneuSelecionado.sprite = SpritesManager.Instance.icon_pneuMacio;
        pneuSelecionado = 3;
    }

    private void BTN_ConfirmarPit()
    {
        // Checa se vai trocar pneus.
        switch (pneuSelecionado)
        {
            case 0:
                piloto.EquipePitStop.VaiTrocarPneu = false;
                break;

            case 1:
                piloto.EquipePitStop.TipoPneu = Pneu.TipoPneu.Duro;
                piloto.EquipePitStop.VaiTrocarPneu = true;
                break;

            case 2:
                piloto.EquipePitStop.TipoPneu = Pneu.TipoPneu.Medio;
                piloto.EquipePitStop.VaiTrocarPneu = true;
                break;

            case 3:
                piloto.EquipePitStop.TipoPneu = Pneu.TipoPneu.Macio;
                piloto.EquipePitStop.VaiTrocarPneu = true;
                break;

            default:
                break;
        }

        // Checa se vai reabastecer.
        if (Toggle_reabastecer.isOn)
        {
            piloto.EquipePitStop.VaiReabastecer = true;
        }
        else
        {
            piloto.EquipePitStop.VaiReabastecer = false;
        }

        // Se ele vai trocar pneu ou reabastecer configura o pit.
        if (piloto.EquipePitStop.VaiReabastecer || piloto.EquipePitStop.VaiTrocarPneu)
        {
            piloto.EquipePitStop.PitStopNestaVolta = true;
            piloto.EquipePitStop.Reparar = true;
        }

        GameManager.Instance.SetGameSpeed(1f);
        FecharPainel();
    }

    private void BTN_CancelarPit()
    {
        GameManager.Instance.SetGameSpeed(1f);
        FecharPainel();
    }
}
