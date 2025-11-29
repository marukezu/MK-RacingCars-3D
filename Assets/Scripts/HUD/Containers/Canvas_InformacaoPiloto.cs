using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Canvas_InformacaoPiloto : MonoBehaviour
{
    public Carro carro;

    [Header("====== Texts ======")]
    public TextMeshProUGUI TXT_PilotoNome;
    public TextMeshProUGUI TXT_PilotoPosicao;
    public TextMeshProUGUI TXT_GasolinaAtual;
    public TextMeshProUGUI TXT_CondicaoPneus;

    [Header("====== Images ======")]
    public Image IMG_PilotoSprite;
    public Image IMG_TipoPneu;
    public Image IMG_ModoMotor;
    public Image IMG_ModoDirecao;

    [Header("====== Sliders ======")]
    public Slider Slider_Combustivel;

    [Header("====== Camera Settings ======")]
    private Transform cameraTransform;

    private void Start()
    {
        // Pega a referência para a câmera principal
        cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        if (carro == null || carro.pilotoController == null) return;

        UpdateCanvasInfo();
    }

    private void LateUpdate()
    {
        // Faz o canvas apontar para a câmera
        transform.LookAt(transform.position + cameraTransform.forward);
    }

    private void UpdateCanvasInfo()
    {
        // Pega o Piloto Controller.
        PilotoController controller = carro.pilotoController;

        // Ativa ou desativa o gameobject se for o pilotoSelecionado.
        GetComponent<Canvas>().enabled = carro.pilotoController.piloto == GameManager.Instance.pilotoEmFoco;

        TXT_PilotoNome.text = controller.piloto.PilotoNome;
        TXT_PilotoPosicao.text = controller.posicaoAtual.ToString("F0") + "°";
        TXT_GasolinaAtual.text = controller.carro.carroceria.combustivelAtual.ToString("F0") + "%";
        TXT_CondicaoPneus.text = controller.carro.carroceria.GetCondicaoMediaPneus().ToString("F0") + "%";
        IMG_TipoPneu.sprite = controller.carro.carroceria.pneuFL.GetPneuSpriteIcon();

        // Cor do texto Pneu, baseado na durabilidade restante.
        if (controller.carro.carroceria.GetCondicaoMediaPneus() <= 35)
            TXT_CondicaoPneus.color = Color.red;
        else
            TXT_CondicaoPneus.color = Color.black;

        // Cor do texto Combustivel, baseado no combustivel restante.
        if (controller.carro.carroceria.combustivelAtual <= 15)
            TXT_GasolinaAtual.color = Color.red;
        else
            TXT_GasolinaAtual.color = Color.black;

        // Slider Combustivel
        Slider_Combustivel.maxValue = 100;
        Slider_Combustivel.value = controller.carro.carroceria.combustivelAtual;

        // Img_modoMotor do tipo de motor usado.
        switch (controller.carro.carroceria.modoMotor)
        {
            case Carro.TipoDeMotor.Economia:
                IMG_ModoMotor.sprite = SpritesManager.Instance.icon_modoMotorBaixa;
                break;

            case Carro.TipoDeMotor.Equilibrado:
                IMG_ModoMotor.sprite = SpritesManager.Instance.icon_modoMotorMedia;
                break;

            case Carro.TipoDeMotor.Forte:
                IMG_ModoMotor.sprite = SpritesManager.Instance.icon_modoMotorForte;
                break;

            case Carro.TipoDeMotor.Agressivo:
                IMG_ModoMotor.sprite = SpritesManager.Instance.icon_modoMotorAgressivo;
                break;
        }

        // Img_modoDirecao do tipo de direção usado.
        switch (controller.carro.carroceria.modoDirecao)
        {
            case Carro.TipoDeConducao.Economia:
                IMG_ModoDirecao.sprite = SpritesManager.Instance.icon_modoDirecaoBaixa;
                break;

            case Carro.TipoDeConducao.Equilibrado:
                IMG_ModoDirecao.sprite = SpritesManager.Instance.icon_modoDirecaoMedia;
                break;

            case Carro.TipoDeConducao.Forte:
                IMG_ModoDirecao.sprite = SpritesManager.Instance.icon_modoDirecaoForte;
                break;

            case Carro.TipoDeConducao.Agressivo:
                IMG_ModoDirecao.sprite = SpritesManager.Instance.icon_modoDirecaoAgressivo;
                break;
        }
    }
}
