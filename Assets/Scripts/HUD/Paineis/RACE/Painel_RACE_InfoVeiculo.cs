using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Painel_RACE_InfoVeiculo : Panel
{
    public override PanelType Type => PanelType.RACE_CARINFO;

    public Text TXT_NomeJogador, TXT_CarSpeed, TXT_MarchaAtual, TXT_CarRPMNow;
    public Slider Sli_CarAceleration, Sli_CarBrake, Sli_CarRPM;
    public Image Image_Turbo;

    public override void AtualizarPainel()
    {
        base.AtualizarPainel();

        if (PlayerSettings.pilotoJogador == null || PlayerSettings.pilotoAliado == null) return;

        Piloto piloto = GameManager.Instance.pilotoEmFoco;

        // Textos do painel
        TXT_NomeJogador.text = piloto.PilotoNome;
        TXT_CarSpeed.text = piloto.Carro.carroceria.velocidadeKMAtual.ToString("F0");
        TXT_MarchaAtual.text = piloto.Carro.motorMarcha.marchaAtual.ToString("F0");
        TXT_CarRPMNow.text = piloto.Carro.motorMarcha.motorAtualRPM.ToString("F0");

        // Slider de Aceleração
        Sli_CarAceleration.minValue = 0;
        Sli_CarAceleration.maxValue = 1;
        float novaPotenciaAceleracao = piloto.Carro.comandos.potenciaAceleracao;
        Sli_CarAceleration.value = Mathf.Lerp(Sli_CarAceleration.value, novaPotenciaAceleracao, Time.deltaTime * 5f); // Ajuste a velocidade de interpolação conforme necessário

        // Slider de Frenagem
        Sli_CarBrake.minValue = 0;
        Sli_CarBrake.maxValue = 1;
        float novaPotenciaFrenagem = piloto.Carro.comandos.potenciaFrenagem;
        Sli_CarBrake.value = Mathf.Lerp(Sli_CarBrake.value, novaPotenciaFrenagem, Time.deltaTime * 5f); // Ajuste a velocidade de interpolação conforme necessário

        // Slider do RPM
        Sli_CarRPM.minValue = 0;
        Sli_CarRPM.maxValue = piloto.Carro.motorMarcha.motorMaxRPM;
        Sli_CarRPM.value = piloto.Carro.motorMarcha.motorAtualRPM;

        // Imagens (Icones)
        Image_Turbo.gameObject.SetActive(piloto.Carro.motorMarcha.turboAtivado);
    }   
}
