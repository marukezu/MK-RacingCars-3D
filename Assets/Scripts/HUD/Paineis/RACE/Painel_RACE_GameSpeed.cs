using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Painel_RACE_GameSpeed : Panel
{
    public override PanelType Type => PanelType.RACE_GAMESPEED;

    // usado para pausar o tempo, armazena (em realTimeSpeed) o valor de timeScale, seta o timeScale para 0, se clicar novamente seta o timeScale como realTimeSpeed que estava armazenado.
    private float realTimeSpeed;

    // Botões
    public Button Btn_PlayPause;
    public Button Btn_MoreSpeed;
    public Button Btn_LessSpeed;

    // Texts
    public Text Txt_TimeSpeed;

    public override void AbrirPainel(object param1 = null, object param2 = null, object param3 = null)
    {
        base.AbrirPainel(param1, param2, param3);

        Btn_PlayPause.onClick.AddListener(delegate { BTN_PlayPause(); });
        Btn_MoreSpeed.onClick.AddListener(delegate { BTN_MoreSpeed(); });
        Btn_LessSpeed.onClick.AddListener(delegate { BTN_LessSpeed(); });
    }

    public override void AtualizarPainel()
    {
        base.AtualizarPainel();

        // Atualiza o texto de timeScale
        float timeScale = GameManager.Instance.GetGameSpeed();
        if (timeScale > 0)
            Txt_TimeSpeed.text = GameManager.Instance.GetGameSpeed().ToString() + 'x';
        else
            Txt_TimeSpeed.text = "Pause";
    }

    // ===================================================================================
    // ================================= AÇÃO DOS BOTÕES =================================
    // ===================================================================================
    private void BTN_PlayPause()
    {
        float timeScale = GameManager.Instance.GetGameSpeed();

        switch (timeScale)
        {
            case > 0:
                realTimeSpeed = timeScale;
                GameManager.Instance.SetGameSpeed(0);
                break;
            case <= 0:
                GameManager.Instance.SetGameSpeed(realTimeSpeed);
                break;
            default:
                return;
        }
    }
    private void BTN_MoreSpeed()
    {
        float timeScale = GameManager.Instance.GetGameSpeed();

        switch (timeScale)
        {
            case 0.5f:
                GameManager.Instance.SetGameSpeed(1f);
                break;
            case 1:
                GameManager.Instance.SetGameSpeed(1.5f);
                break;
            case 1.5f:
                GameManager.Instance.SetGameSpeed(2);
                break;
            default:
                return;
        }
    }
    private void BTN_LessSpeed()
    {
        float timeScale = GameManager.Instance.GetGameSpeed();

        switch (timeScale)
        {
            case 2:
                GameManager.Instance.SetGameSpeed(1.5f);
                break;
            case 1.5f:
                GameManager.Instance.SetGameSpeed(1);
                break;
            case 1f:
                GameManager.Instance.SetGameSpeed(0.5f);
                break;
            default:
                return;
        }
    }
}
