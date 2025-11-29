using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Container_Painel_Placar : MonoBehaviour
{
    // Piloto responsavel por esse container.
    public PilotoController controller; // Passado pelo script que instancia esse UI_Container.

    // Dados da hud do container.
    public Text Txt_pilotoPosition;
    public Text Txt_pilotoNome;
    public Text Txt_tempoDiferenca;
    public Text Txt_tempoVoltaAtual;
    public Text Txt_tempoUltimaVolta;
    public Text Txt_tempoVoltaMaisRapida;
    public Text Txt_voltas;
    public Text Txt_gas;
    public Text Txt_pneus;
    public Slider Slider_Combustivel;
    public Image Img_pneus;
    public Image Img_modoMotor;
    public Image Img_modoDirecao;

    private void Update()
    {
        if (controller != null && controller.carro != null)
        {
            float tempoVoltaAtual = controller.tempoVoltaAtual;
            float tempoUltimaVolta = controller.tempoUltimaVolta;
            float tempoVoltaMaisRapida = controller.tempoVoltaMaisRapida;

            // Textos.
            Txt_pilotoPosition.text = controller.posicaoAtual.ToString();
            Txt_pilotoNome.text = controller.piloto.PilotoNome;

            if (RaceManager.Instance.tipoDeCorrida == RaceManager.TipoDeCorrida.Classificacao)
                Txt_tempoDiferenca.text = "+0.0s";

            else if (RaceManager.Instance.tipoDeCorrida == RaceManager.TipoDeCorrida.Corrida)
                Txt_tempoDiferenca.text = "+" + controller.tempoDiferencaCarroFrente.ToString("F1") + "s";

            Txt_tempoVoltaAtual.text = GameManager.Instance.ConverterTempoParaFormato(tempoVoltaAtual);
            Txt_tempoUltimaVolta.text = GameManager.Instance.ConverterTempoParaFormato(tempoUltimaVolta);
            Txt_tempoVoltaMaisRapida.text = GameManager.Instance.ConverterTempoParaFormato(tempoVoltaMaisRapida);
            Txt_voltas.text = controller.voltaAtual.ToString();
            Txt_gas.text = controller.carro.carroceria.combustivelAtual.ToString("F0") + "%";
            Txt_pneus.text = controller.carro.carroceria.GetCondicaoMediaPneus().ToString("F0") + "%";
            Img_pneus.sprite = controller.carro.carroceria.pneuFL.GetPneuSpriteIcon();

            // Cor do text Pneu, baseado na durabilidade restante.
            if (controller.carro.carroceria.GetCondicaoMediaPneus() <= 35)
                Txt_pneus.color = Color.red;
            else
                Txt_pneus.color = Color.black;

            // Cor do text Combustivel, baseado no combustivel restante.
            if (controller.carro.carroceria.combustivelAtual <= 15)
                Txt_gas.color = Color.red;
            else
                Txt_gas.color = Color.black;

            // Slider Combustivel
            Slider_Combustivel.maxValue = 100;
            Slider_Combustivel.value = controller.carro.carroceria.combustivelAtual;

            // Img_modoMotor do tipo de motor usado.
            switch (controller.carro.carroceria.modoMotor)
            {
                case Carro.TipoDeMotor.Economia:
                    Img_modoMotor.sprite = SpritesManager.Instance.icon_modoMotorBaixa;
                    break;

                case Carro.TipoDeMotor.Equilibrado:
                    Img_modoMotor.sprite = SpritesManager.Instance.icon_modoMotorMedia;
                    break;

                case Carro.TipoDeMotor.Forte:
                    Img_modoMotor.sprite = SpritesManager.Instance.icon_modoMotorForte;
                    break;

                case Carro.TipoDeMotor.Agressivo:
                    Img_modoMotor.sprite = SpritesManager.Instance.icon_modoMotorAgressivo;
                    break;
            }

            // Img_modoDirecao do tipo de direção usado.
            switch (controller.carro.carroceria.modoDirecao)
            {
                case Carro.TipoDeConducao.Economia:
                    Img_modoDirecao.sprite = SpritesManager.Instance.icon_modoDirecaoBaixa;
                    break;

                case Carro.TipoDeConducao.Equilibrado:
                    Img_modoDirecao.sprite = SpritesManager.Instance.icon_modoDirecaoMedia;
                    break;

                case Carro.TipoDeConducao.Forte:
                    Img_modoDirecao.sprite = SpritesManager.Instance.icon_modoDirecaoForte;
                    break;

                case Carro.TipoDeConducao.Agressivo:
                    Img_modoDirecao.sprite = SpritesManager.Instance.icon_modoDirecaoAgressivo;
                    break;
            }
        }
    }
}
