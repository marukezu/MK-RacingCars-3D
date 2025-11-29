using UnityEngine;
using UnityEngine.UI;

public class Painel_RACE_InformacaoPRECorrida : Panel
{
    [Header("====== Buttons ======")]
    public Button Btn_IniciarCorrida;

    public override void AbrirPainel(object param1 = null, object param2 = null, object param3 = null)
    {
        base.AbrirPainel(param1, param2, param3);

        Btn_IniciarCorrida.onClick.AddListener(delegate { BTN_IniciarCorrida(); });
    }

    // ================================================================================================================
    // =================================================== Buttons ====================================================
    // ================================================================================================================
    private void BTN_IniciarCorrida()
    {
        // Inicia a corrida.
        RaceManager.Instance.StartCoroutine(RaceManager.Instance.ContagemLargada());

        // Inicializa os Paineis de Corrida.
        PanelManager.Instance.InstanciarPaineisCorrida();

        // Fecha o Painel Informativo
        FecharPainel();
    }
}
