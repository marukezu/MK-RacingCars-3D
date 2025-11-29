using UnityEngine;
using UnityEngine.UI;

public class Painel_RACE_SessaoClassificacao : Panel
{
    public override PanelType Type => PanelType.RACE_SESSAOCLASSIFICACAO;

    public Text Txt_TempoRestante_Classificacao;

    public override void AtualizarPainel()
    {
        base.AtualizarPainel();

        Txt_TempoRestante_Classificacao.text = GameManager.Instance.ConverterTempoParaFormato(RaceManager.Instance.tempoTotalClassificacao);
    }
}
