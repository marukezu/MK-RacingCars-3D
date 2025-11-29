using System;
using TMPro;

public class Painel_RACE_DataHora : Panel
{
    public override PanelType Type => PanelType.RACE_DATAHORA;

    public TextMeshProUGUI TXT_HoraAtual;

    public override void AtualizarPainel()
    {
        base.AtualizarPainel();

        TXT_HoraAtual.text = TimeSpan.FromSeconds(TimeAndDate_Manager.Instance.GetSegundosAtuais()).ToString(@"hh\:mm"); 
    }
}
