using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Painel_RACE_InformacaoPREClassificacao : Panel
{
    [Header("====== Buttons ======")]
    public Button Btn_IniciarClassificacao;

    public override void AbrirPainel(object param1 = null, object param2 = null, object param3 = null)
    {
        base.AbrirPainel(param1, param2, param3);

        Btn_IniciarClassificacao.onClick.AddListener(delegate { BTN_IniciarClassificacao(); });
    }

    // ================================================================================================================
    // =================================================== Buttons ====================================================
    // ================================================================================================================
    private void BTN_IniciarClassificacao()
    {
        // Aponta para o RaceManager a inicialização da Classificação.
        RaceManager.Instance.classificacaoEmAndamento = true;

        // Inicializa os Paineis de Classificação.
        PanelManager.Instance.InstanciarPaineisClassificacao();

        // Fecha o painel informativo de Classificação.
        FecharPainel();
    }
}
