using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Painel_RACE_ControlarPiloto : Panel
{
    public override PanelType Type => PanelType.RACE_PILOTOCONTROLLER;
    private Piloto pilotoEmFoco => GameManager.Instance.pilotoEmFoco;

    [Header("====== SubPanels ======")]
    public Painel_RACE_ControlarPiloto_EscolherEstrategia panelEstrategia;

    [Header("====== Texts ======")]
    public Text Txt_nomePiloto;
    public Text Txt_posicaoPiloto;
    public Text Txt_tempo_VoltaAtual;
    public Text Txt_tempo_UltimaVolta;
    public Text Txt_tempo_MaisRapidaVolta;
    public Text Txt_gasolina;
    public Text Txt_gasolina_gastoPorVolta;
    public Text Txt_condicaoPneus;

    [Header("====== Images ======")]
    public Image Img_ModoMotor;
    public Image Img_ModoDirecao;

    [Header("====== Sliders ======")]
    public Slider Slider_Gasolina;

    [Header("====== Buttons ======")]
    public Button Btn_AlterarPiloto;
    public Button Btn_EnviarPiloto;
    public Button Btn_Estrategia;
    public Button Btn_PitStop;

    public override void AbrirPainel(object param1 = null, object param2 = null, object param3 = null)
    {
        base.AbrirPainel(param1, param2, param3);

        Btn_Estrategia.onClick.AddListener(delegate { BTN_Estrategia(); });
        Btn_PitStop.onClick.AddListener(delegate { BTN_PitStop(); });
        Btn_AlterarPiloto.onClick.AddListener(delegate { BTN_AlterarPiloto(); });
        Btn_EnviarPiloto.onClick.AddListener(delegate { BTN_EnviarPiloto(); });

        if (RaceManager.Instance.tipoDeCorrida == RaceManager.TipoDeCorrida.Corrida)
        {
            Btn_EnviarPiloto.gameObject.SetActive(false);
        }
        else if (RaceManager.Instance.tipoDeCorrida == RaceManager.TipoDeCorrida.Classificacao)
        {
            Btn_EnviarPiloto.gameObject.SetActive(true);
        }
    }

    public override void AtualizarPainel()
    {
        base.AtualizarPainel();

        if (pilotoEmFoco == null) return;

        // Dados básicos do piloto
        Txt_nomePiloto.text = pilotoEmFoco.PilotoNome;
        Txt_posicaoPiloto.text = pilotoEmFoco.Carro.pilotoController.posicaoAtual.ToString();

        // Tempos de voltas
        Txt_tempo_VoltaAtual.text = GameManager.Instance.ConverterTempoParaFormato(pilotoEmFoco.Carro.pilotoController.tempoVoltaAtual);
        Txt_tempo_UltimaVolta.text = GameManager.Instance.ConverterTempoParaFormato(pilotoEmFoco.Carro.pilotoController.tempoUltimaVolta);
        Txt_tempo_MaisRapidaVolta.text = GameManager.Instance.ConverterTempoParaFormato(pilotoEmFoco.Carro.pilotoController.tempoVoltaMaisRapida);

        // Combustivel atual
        Txt_gasolina.text = pilotoEmFoco.Carro.carroceria.combustivelAtual.ToString("F0") + "%";
        if (pilotoEmFoco.Carro.carroceria.combustivelAtual < 25)
            Txt_gasolina.color = Color.red;
        else
            Txt_gasolina.color = Color.white;

        // Gasto Combustivel por volta
        if (pilotoEmFoco.Carro.carroceria.consumoCombustivelPorVolta != 0)
            Txt_gasolina_gastoPorVolta.text = pilotoEmFoco.Carro.carroceria.consumoCombustivelPorVolta.ToString("F1") + "/v";
        else
            Txt_gasolina_gastoPorVolta.text = "0.0/v";

        if (pilotoEmFoco.Carro.carroceria.deltaPositivo)
            Txt_gasolina_gastoPorVolta.color = Color.green;
        else
            Txt_gasolina_gastoPorVolta.color = Color.red;

        // Condição dos Pneus
        Txt_condicaoPneus.text = pilotoEmFoco.Carro.carroceria.GetCondicaoMediaPneus().ToString("F0") + "%";
        if (pilotoEmFoco.Carro.carroceria.GetCondicaoMediaPneus() <= 35)
            Txt_condicaoPneus.color = Color.red;
        else
            Txt_condicaoPneus.color = Color.white;

        // Slider de gasolina
        Slider_Gasolina.maxValue = 100;
        Slider_Gasolina.value = pilotoEmFoco.Carro.carroceria.combustivelAtual;

        // Imagem do modo de motor.
        switch (pilotoEmFoco.Carro.carroceria.modoMotor)
        {
            case Carro.TipoDeMotor.Economia:
                Img_ModoMotor.sprite = SpritesManager.Instance.icon_modoMotorBaixa;
                break;

            case Carro.TipoDeMotor.Equilibrado:
                Img_ModoMotor.sprite = SpritesManager.Instance.icon_modoMotorMedia;
                break;

            case Carro.TipoDeMotor.Forte:
                Img_ModoMotor.sprite = SpritesManager.Instance.icon_modoMotorForte;
                break;

            case Carro.TipoDeMotor.Agressivo:
                Img_ModoMotor.sprite = SpritesManager.Instance.icon_modoMotorAgressivo;
                break;
        }

        // Imagem do modo de direção.
        switch (pilotoEmFoco.Carro.carroceria.modoDirecao)
        {
            case Carro.TipoDeConducao.Economia:
                Img_ModoDirecao.sprite = SpritesManager.Instance.icon_modoDirecaoBaixa;
                break;

            case Carro.TipoDeConducao.Equilibrado:
                Img_ModoDirecao.sprite = SpritesManager.Instance.icon_modoDirecaoMedia;
                break;

            case Carro.TipoDeConducao.Forte:
                Img_ModoDirecao.sprite = SpritesManager.Instance.icon_modoDirecaoForte;
                break;

            case Carro.TipoDeConducao.Agressivo:
                Img_ModoDirecao.sprite = SpritesManager.Instance.icon_modoDirecaoAgressivo;
                break;
        }
    }

    public void BTN_AlterarPiloto()
    {
        // Se a sessão terminou, retorna.
        if (RaceManager.Instance.classificacaoFinalizada || RaceManager.Instance.corridaFinalizada) return;

        // Se estiver na janela de pitstop, retorna
        if (PanelManager.Instance.GetPanel(PanelType.RACE_PITSTOP_MANAGER)) return;

        // Se estiver no piloto do jogador.
        if (pilotoEmFoco == PlayerSettings.pilotoJogador)
        {
            GameManager.Instance.SetPilotoEmFoco(PlayerSettings.pilotoAliado);
        }

        // Se estiver no piloto aliado.
        else if (pilotoEmFoco == PlayerSettings.pilotoAliado)
        {
            GameManager.Instance.SetPilotoEmFoco(PlayerSettings.pilotoJogador);
        }
    }
    public void BTN_EnviarPiloto() // Botão usado na classificação.
    {
        PilotoController pilotoController = GameManager.Instance.pilotoEmFoco.Carro.pilotoController;

        // Se o carro estiver concertando no pit, ou o carro ja estiver na pista, retorna.
        if (pilotoController.piloto.EquipePitStop.ConsertandoNoPit || pilotoController.estaNaPista) return;

        // Se a sessão terminou, retorna.
        if (RaceManager.Instance.classificacaoFinalizada || RaceManager.Instance.corridaFinalizada) return;

        pilotoController.piloto.EquipePitStop.ParadoNoPitStop = false;
        pilotoController.iniciouClassificacao = true;
        pilotoController.estaNaPista = true;
        RaceManager.Instance.carrosNaPista++;
    }
    public void BTN_Estrategia()
    {
        // Se a sessão terminou, retorna.
        if (RaceManager.Instance.classificacaoFinalizada || RaceManager.Instance.corridaFinalizada) return;

        // Desabilita o SubPanel Escolher Estratégia
        panelEstrategia.gameObject.SetActive(!panelEstrategia.gameObject.activeSelf);
    }
    public void BTN_PitStop()
    {
        // Se estiver concertando ja no pit, não abre a janela.
        if (GameManager.Instance.pilotoEmFoco.EquipePitStop.ConsertandoNoPit) return;

        // Se a sessão terminou, retorna.
        if (RaceManager.Instance.classificacaoFinalizada || RaceManager.Instance.corridaFinalizada) return;

        // Se o painel de PitStop estiver aberto
        if (PanelManager.Instance.GetPanel(PanelType.RACE_PITSTOP_MANAGER))
        {
            // Fecha o Painel
            PanelManager.Instance.FecharPainel(PanelType.RACE_PITSTOP_MANAGER);
            GameManager.Instance.SetGameSpeed(1f);
        }
        // Se o painel de PitStop não estiver aberto
        else
        {
            // Instancia o novo painel de pitstop
            PanelManager.Instance.InstanciarERetornarPainel(PanelType.RACE_PITSTOP_MANAGER);
        }     
    }
}
