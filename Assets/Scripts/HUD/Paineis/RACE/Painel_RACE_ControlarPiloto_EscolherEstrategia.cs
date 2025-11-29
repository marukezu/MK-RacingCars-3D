using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Painel_RACE_ControlarPiloto_EscolherEstrategia : Panel
{
    Piloto pilotoEmFoco => GameManager.Instance.pilotoEmFoco;

    [Header("====== Buttons - Modo Motor ======")]
    public Button Btn_ModoMotor_Economizar;
    public Button Btn_ModoMotor_Equilibrado;
    public Button Btn_ModoMotor_Forte;
    public Button Btn_ModoMotor_Agressivo;

    [Header("====== Buttons - Modo Direção ======")]
    public Button Btn_Modo_Direcao_Economizar;
    public Button Btn_Modo_Direcao_Equilibrado;
    public Button Btn_Modo_Direcao_Forte;
    public Button Btn_Modo_Direcao_Agressivo;

    public Toggle Toggle_Auto;

    public override void Initialize(object param1 = null, object param2 = null, object param3 = null)
    {
        base.Initialize(param1, param2, param3);

        Btn_ModoMotor_Economizar.onClick.AddListener(delegate { BTN_ModoMotor_Economizar(); });
        Btn_ModoMotor_Equilibrado.onClick.AddListener(delegate { BTN_ModoMotor_Equilibrado(); });
        Btn_ModoMotor_Forte.onClick.AddListener(delegate { BTN_ModoMotor_Forte(); });
        Btn_ModoMotor_Agressivo.onClick.AddListener(delegate { BTN_ModoMotor_Agressivo(); });

        Btn_Modo_Direcao_Economizar.onClick.AddListener(delegate { BTN_ModoDirecao_Economizar(); });
        Btn_Modo_Direcao_Equilibrado.onClick.AddListener(delegate { BTN_ModoDirecao_Equilibrado(); });
        Btn_Modo_Direcao_Forte.onClick.AddListener(delegate { BTN_ModoDirecao_Forte(); });
        Btn_Modo_Direcao_Agressivo.onClick.AddListener(delegate { BTN_ModoDirecao_Agressivo(); });

        Toggle_Auto.onValueChanged.AddListener(delegate { Toggle_AutoOnValueChanged(); });
    }

    public override void AbrirPainel(object param1 = null, object param2 = null, object param3 = null)
    {
        base.AbrirPainel(param1, param2, param3);
    }

    public override void AtualizarPainel()
    {
        base.AtualizarPainel();

        Toggle_Auto.isOn = pilotoEmFoco.AutoMode;
    }

    // Botões modo de motor.
    private void BTN_ModoMotor_Economizar()
    {
        pilotoEmFoco.Carro.comandos.SetModoMotor(Carro.TipoDeMotor.Economia);
    }
    private void BTN_ModoMotor_Equilibrado()
    {
        pilotoEmFoco.Carro.comandos.SetModoMotor(Carro.TipoDeMotor.Equilibrado);
    }
    private void BTN_ModoMotor_Forte()
    {
        pilotoEmFoco.Carro.comandos.SetModoMotor(Carro.TipoDeMotor.Forte);
    }
    private void BTN_ModoMotor_Agressivo()
    {
        pilotoEmFoco.Carro.comandos.SetModoMotor(Carro.TipoDeMotor.Agressivo);
    }

    // Toggle Auto
    private void Toggle_AutoOnValueChanged()
    {
        pilotoEmFoco.AutoMode = Toggle_Auto.isOn;
    }

    // Botões modo de direção.
    private void BTN_ModoDirecao_Economizar()
    {
        pilotoEmFoco.Carro.comandos.SetModoDirecao(Carro.TipoDeConducao.Economia);
    }
    private void BTN_ModoDirecao_Equilibrado()
    {
        pilotoEmFoco.Carro.comandos.SetModoDirecao(Carro.TipoDeConducao.Equilibrado);
    }
    private void BTN_ModoDirecao_Forte()
    {
        pilotoEmFoco.Carro.comandos.SetModoDirecao(Carro.TipoDeConducao.Forte);
    }
    private void BTN_ModoDirecao_Agressivo()
    {
        pilotoEmFoco.Carro.comandos.SetModoDirecao(Carro.TipoDeConducao.Agressivo);
    }

}
