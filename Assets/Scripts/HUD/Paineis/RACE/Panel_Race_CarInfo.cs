using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Panel_Race_CarInfo : Panel
{
    public override PanelType Type => PanelType.RACE_CARINFO;

    public PilotoSim piloto => GameManager.Instance.carEmFoco.piloto;

    public TextMeshProUGUI TXT_CarSpeed;
    public TextMeshProUGUI TXT_CarShift;
    public TextMeshProUGUI TXT_CarRPM;

    public Slider Slider_RPM;

    public override void Initialize(PanelParams param = null)
    {
        Slider_RPM.maxValue = 8500;

        // Inicializa os componentes do painel
        SetText_CarSpeed(piloto, piloto.mod_acelFrenagem.Car_SpeedKM);
    }

    public override void SubscribeEvents(bool subscribe)
    {
        if (subscribe)
        {
            EventBus.On_Car_SpeedChanged += SetText_CarSpeed;
            EventBus.On_Car_ShiftChanged += SetText_CarShift;
            EventBus.On_Car_RPMChanged += SetText_CarRPM;
            EventBus.On_Car_RPMChanged += SetSlider_CarRPM;
        }
        else
        {
            EventBus.On_Car_SpeedChanged -= SetText_CarSpeed;
            EventBus.On_Car_ShiftChanged -= SetText_CarShift;
            EventBus.On_Car_RPMChanged -= SetText_CarRPM;
            EventBus.On_Car_RPMChanged -= SetSlider_CarRPM;
        }
    }

    private void SetText_CarSpeed(PilotoSim p, float value)
    {
        if (!ReferenceEquals(p, GameManager.Instance.carEmFoco.piloto)) return;
        TXT_CarSpeed.text = value.ToString();
    }
    private void SetText_CarShift(PilotoSim p, int value)
    {
        if (!ReferenceEquals(p, GameManager.Instance.carEmFoco.piloto)) return;
        TXT_CarShift.text = value.ToString();
    }
    private void SetText_CarRPM(PilotoSim p, float value)
    {
        if (!ReferenceEquals(p, GameManager.Instance.carEmFoco.piloto)) return;
        TXT_CarRPM.text = value.ToString();
    }
    private void SetSlider_CarRPM(PilotoSim p, float value)
    {
        if (!ReferenceEquals(p, GameManager.Instance.carEmFoco.piloto)) return;
        Slider_RPM.value = value;
    }
}
