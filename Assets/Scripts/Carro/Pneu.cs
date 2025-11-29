using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Carro;

public class Pneu
{
    // Enums
    public enum TipoPneu
    {
        Duro,
        Medio,
        Macio,
        Molhado,
        Chuva
    }

    public enum SlotPneu
    {
        FL,
        FR,
        RL,
        RR
    }

    public Carro carro;

    // Informação Principal.
    public TipoPneu tipoPneu;
    public SlotPneu slotPneu;
    public string nomePneu;
    public float fatorPotencia;
    public float fatorDesgasto;
    public float durabilidadeAtual;
    public float pneusConsumoBasal = 0.075f;

    // Para uso na HUD
    public float HUD_Info_velocidadePneu;
    public float HUD_Info_durabilidadePneu;


    public Pneu(TipoPneu tipoPneu, Carro carro, SlotPneu slotPneu)
    {
        this.tipoPneu = tipoPneu;
        this.carro = carro;
        this.slotPneu = slotPneu;

        InitializePneu();
    }

    private void InitializePneu()
    {
        switch (tipoPneu)
        {
            case TipoPneu.Duro:
                nomePneu = "Duro";
                fatorPotencia = 0.97f;
                fatorDesgasto = 0.70f;
                durabilidadeAtual = 100;

                HUD_Info_velocidadePneu = 55f;
                HUD_Info_durabilidadePneu = 95f;
                break;

            case TipoPneu.Medio:
                nomePneu = "Médio";
                fatorPotencia = 1f;
                fatorDesgasto = 1f;
                durabilidadeAtual = 100;

                HUD_Info_velocidadePneu = 70f;
                HUD_Info_durabilidadePneu = 70f;
                break;

            case TipoPneu.Macio:
                nomePneu = "Macio";
                fatorPotencia = 1.03f;
                fatorDesgasto = 1.30f;
                durabilidadeAtual = 100;

                HUD_Info_velocidadePneu = 95f;
                HUD_Info_durabilidadePneu = 55f;
                break;

            case TipoPneu.Molhado:
                nomePneu = "Molhado";
                fatorPotencia = 0.9f;
                fatorDesgasto = 1f;
                durabilidadeAtual = 100;

                HUD_Info_velocidadePneu = 70f;
                HUD_Info_durabilidadePneu = 70f;
                break;

            case TipoPneu.Chuva:
                nomePneu = "Chuva";
                fatorPotencia = 0.9f;
                fatorDesgasto = 1f;
                durabilidadeAtual = 100;

                HUD_Info_velocidadePneu = 70f;
                HUD_Info_durabilidadePneu = 70f;
                break;
        }
    }

    // ======================================================================================
    // ================================= Desgaste do Pneu  ==================================
    // ======================================================================================
    public void AtualizaCondicaoPneu()
    {
        // Se o carro estiver em movimento, inicia o gasto dos pneus.
        if (carro.carroceria.velocidadeKMAtual > 5)
        {
            float random = Random.Range(0f, 0.1f);
            float consumo = (((Mathf.Clamp(carro.carroceria.velocidadeKMAtual / 100, 0f, 1f) * (fatorDesgasto + random)) * carro.carroceria.GetFatorModoDirecao("gasto")) * pneusConsumoBasal) * Time.deltaTime;
            durabilidadeAtual = Mathf.Clamp(durabilidadeAtual - consumo, 0f, 100f);
        }
    }

    public Sprite GetPneuSpriteIcon()
    {
        switch (tipoPneu)
        {
            case TipoPneu.Duro:
                return SpritesManager.Instance.icon_pneuDuro;

            case TipoPneu.Medio:
                return SpritesManager.Instance.icon_pneuMedio;

            case TipoPneu.Macio:
                return SpritesManager.Instance.icon_pneuMacio;

            case TipoPneu.Molhado:
                return SpritesManager.Instance.icon_pneuMolhado;

            case TipoPneu.Chuva:
                return SpritesManager.Instance.icon_pneuChuva;

            default:
                return null;
        }
    }
}
