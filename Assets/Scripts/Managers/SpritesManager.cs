using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpritesManager : MonoBehaviour
{
    public static SpritesManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    [Header("====== ICONS - Modo Motor ======")]
    public Sprite icon_modoMotorBaixa;
    public Sprite icon_modoMotorMedia;
    public Sprite icon_modoMotorForte;
    public Sprite icon_modoMotorAgressivo;

    [Header("====== ICONS - Modo Direção ======")]
    public Sprite icon_modoDirecaoBaixa;
    public Sprite icon_modoDirecaoMedia;
    public Sprite icon_modoDirecaoForte;
    public Sprite icon_modoDirecaoAgressivo;

    [Header("====== ICONS - Pneus ======")]
    public Sprite icon_pneuDuro;
    public Sprite icon_pneuMedio;
    public Sprite icon_pneuMacio;
    public Sprite icon_pneuMolhado;
    public Sprite icon_pneuChuva;

}
