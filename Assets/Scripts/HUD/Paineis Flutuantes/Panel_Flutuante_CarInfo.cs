using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Panel_Flutuante_CarInfo : Panel
{
    public override PanelType Type => PanelType.FLOATING_CARINFO;

    // PilotoSim que esse painel pertence.
    private PilotoSim piloto;

    [Header("====== Textos do Painel ======")]
    public TextMeshProUGUI TXT_PilotoNome;
    public TextMeshProUGUI TXT_PilotoPosition;

    [Header("====== Sliders do Painel ======")]
    public Slider Slider_Combustivel;
    public Slider Slider_Pneus;

    [Header("====== Images do Painel ======")]
    public Image IMG_Mentalidade;

    public override void Initialize(PanelParams param = null)
    {
        // Pega referência do carsim pelo parametro de inicialização.
        piloto = param.pilotoSim1;

        // Linka o painel flutuante.
        flutuante = true;
        flutuanteTarget = param.pilotoSim1.carsim.transform;
        flutuanteOffset = Vector3.up * 25f;

        // Inicia valores de Sliders.
        Slider_Combustivel.maxValue = 100;
        Slider_Pneus.maxValue = 100;

        // Inicializa os textos do painel a primeira vez.
        SetText_PilotoNome(piloto, piloto.Nome);
        SetText_PilotoPosition(piloto, piloto.PosicaoCorrida);
        SetSlider_CarCombustivel(piloto, piloto.mod_Conducao.Cond_Combustivel);
        SetSlider_CarPneus(piloto, piloto.mod_Conducao.Cond_Pneus);

        // Se inscreve nos Eventos
        SubscribeEvents(true);
    }

    public override void SubscribeEvents(bool subscribe)
    {
        if (subscribe)
        {
            EventBus.On_Piloto_PosicaoAtualChanged += SetText_PilotoPosition;
            EventBus.On_ModosConducao_Car_ConditionCombustivelChanged += SetSlider_CarCombustivel;
            EventBus.On_ModosConducao_Car_ConditionPneusChanged += SetSlider_CarPneus;
            EventBus.On_Mentalidade_Changed += SetImage_Mentalidade;
        }
        else
        {
            EventBus.On_Piloto_PosicaoAtualChanged -= SetText_PilotoPosition;
            EventBus.On_ModosConducao_Car_ConditionCombustivelChanged -= SetSlider_CarCombustivel;
            EventBus.On_ModosConducao_Car_ConditionPneusChanged -= SetSlider_CarPneus;
            EventBus.On_Mentalidade_Changed -= SetImage_Mentalidade;
        }
    }

    private void SetText_PilotoNome(PilotoSim p, string value)
    {
        if (!ReferenceEquals(p, piloto)) return;
        TXT_PilotoNome.text = value;

        // Seta a cor diferente para o painel do player.
        TXT_PilotoNome.color = ReferenceEquals(piloto, GameManager.Instance.carEmFoco.piloto) ? Color.yellow : Color.black;
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

        // Aplica no texto.
        TXT_PilotoPosition.text = position.ToString() + "°";
        TXT_PilotoPosition.color = cor;
    }

    private void SetSlider_CarCombustivel(PilotoSim p, float value)
    {
        if (!ReferenceEquals(p, piloto)) return;
        Slider_Combustivel.value = value;
    }

    private void SetSlider_CarPneus(PilotoSim p, float value)
    {
        if (!ReferenceEquals(p, piloto)) return;
        Slider_Pneus.value = value;
    }

    private void SetImage_Mentalidade(PilotoSim p, PilotoSim_Mentalidade.TipoMentalidade value)
    {
        if (!ReferenceEquals(p, piloto)) return;

        var spriteKey = SpritesManager.mapaMentalidade[value];
        IMG_Mentalidade.sprite = SpritesManager.Instance.GetSprite(spriteKey);
    }
}
