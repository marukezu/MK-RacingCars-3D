using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SpritesManager : MonoBehaviour
{
    public static SpritesManager Instance;

    #region Enums
    public enum Sprites_ModoMotor { Baixa, Media, Forte, Agressivo }
    public enum Sprites_ModoDirecao { Baixa, Media, Forte, Agressivo }
    public enum Sprites_Pneu { Duro, Medio, Macio, Molhado, Chuva }
    public enum Sprites_PowerLevel { Level0, Level1, Level2, Level3, Level4, Level5 }
    public enum Sprites_DuelResult { Failed, Success }
    public enum Sprites_Mentalidade { Defensiva, Forte, Ofensiva, Agressiva }
    #endregion

    #region Sprites - Modo Motor
    [Header("====== ICONS - Modo Motor ======")]
    [SerializeField] private Sprite icon_modoMotorBaixa;
    [SerializeField] private Sprite icon_modoMotorMedia;
    [SerializeField] private Sprite icon_modoMotorForte;
    [SerializeField] private Sprite icon_modoMotorAgressivo;
    #endregion

    #region Sprites - Modo Direção
    [Header("====== ICONS - Modo Direção ======")]
    [SerializeField] private Sprite icon_modoDirecaoBaixa;
    [SerializeField] private Sprite icon_modoDirecaoMedia;
    [SerializeField] private Sprite icon_modoDirecaoForte;
    [SerializeField] private Sprite icon_modoDirecaoAgressivo;
    #endregion

    #region Sprites - Pneus
    [Header("====== ICONS - Pneus ======")]
    [SerializeField] private Sprite icon_pneuDuro;
    [SerializeField] private Sprite icon_pneuMedio;
    [SerializeField] private Sprite icon_pneuMacio;
    [SerializeField] private Sprite icon_pneuMolhado;
    [SerializeField] private Sprite icon_pneuChuva;
    #endregion

    #region Sprites - Power Levels (Cargas)
    [Header("====== ICONS - Power Changes ======")]
    [SerializeField] private Sprite icon_powerLevel_0;
    [SerializeField] private Sprite icon_powerLevel_1;
    [SerializeField] private Sprite icon_powerLevel_2;
    [SerializeField] private Sprite icon_powerLevel_3;
    [SerializeField] private Sprite icon_powerLevel_4;
    [SerializeField] private Sprite icon_powerLevel_5;
    #endregion

    #region Sprites - Duel - Success - Failed
    [Header("====== ICONS - Duel Result ======")]
    [SerializeField] private Sprite icon_duel_success;
    [SerializeField] private Sprite icon_duel_failed;
    #endregion

    #region Sprites - Mentalidade
    [Header("====== ICONS - Mentalidade ======")]
    [SerializeField] private Sprite icon_mentalidade_defensiva;
    [SerializeField] private Sprite icon_mentalidade_forte;
    [SerializeField] private Sprite icon_mentalidade_ofensiva;
    [SerializeField] private Sprite icon_mentalidade_agressiva;
    #endregion

    #region Dictionaries
    private Dictionary<Sprites_ModoMotor, Sprite> dictModoMotor;
    private Dictionary<Sprites_ModoDirecao, Sprite> dictModoDirecao;
    private Dictionary<Sprites_Pneu, Sprite> dictPneus;
    private Dictionary<Sprites_PowerLevel, Sprite> dictPowerLevels;
    private Dictionary<Sprites_DuelResult, Sprite> dictDuelResult;
    private Dictionary<Sprites_Mentalidade, Sprite> dictMentalidades;
    #endregion

    // Dictionary - Mentalidades - Mapa de Enums
    public static readonly Dictionary<PilotoSim_Mentalidade.TipoMentalidade, SpritesManager.Sprites_Mentalidade> mapaMentalidade =
    new Dictionary<PilotoSim_Mentalidade.TipoMentalidade, SpritesManager.Sprites_Mentalidade>
    {
        { PilotoSim_Mentalidade.TipoMentalidade.Defensivo, SpritesManager.Sprites_Mentalidade.Defensiva },
        { PilotoSim_Mentalidade.TipoMentalidade.Forte,     SpritesManager.Sprites_Mentalidade.Forte },
        { PilotoSim_Mentalidade.TipoMentalidade.Ofensivo,  SpritesManager.Sprites_Mentalidade.Ofensiva },
        { PilotoSim_Mentalidade.TipoMentalidade.Agressivo, SpritesManager.Sprites_Mentalidade.Agressiva },
    };


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        ValidarSpritesIcon();
        InitializeDictionaries();
    }

    #region Inicialização e Validação
    private void ValidarSpritesIcon()
    {
        foreach (var field in GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
        {
            if (field.FieldType == typeof(Sprite))
            {
                Sprite sprite = field.GetValue(this) as Sprite;
                if (sprite == null)
                    Debug.LogWarning($"Sprite não atribuído no Inspector: {field.Name}");
            }
        }
    }

    private void InitializeDictionaries()
    {
        // Modo Motor
        dictModoMotor = new Dictionary<Sprites_ModoMotor, Sprite>
        {
            { Sprites_ModoMotor.Baixa, icon_modoMotorBaixa },
            { Sprites_ModoMotor.Media, icon_modoMotorMedia },
            { Sprites_ModoMotor.Forte, icon_modoMotorForte },
            { Sprites_ModoMotor.Agressivo, icon_modoMotorAgressivo }
        };

        // Modo Direção
        dictModoDirecao = new Dictionary<Sprites_ModoDirecao, Sprite>
        {
            { Sprites_ModoDirecao.Baixa, icon_modoDirecaoBaixa },
            { Sprites_ModoDirecao.Media, icon_modoDirecaoMedia },
            { Sprites_ModoDirecao.Forte, icon_modoDirecaoForte },
            { Sprites_ModoDirecao.Agressivo, icon_modoDirecaoAgressivo }
        };

        // Pneus
        dictPneus = new Dictionary<Sprites_Pneu, Sprite>
        {
            { Sprites_Pneu.Duro, icon_pneuDuro },
            { Sprites_Pneu.Medio, icon_pneuMedio },
            { Sprites_Pneu.Macio, icon_pneuMacio },
            { Sprites_Pneu.Molhado, icon_pneuMolhado },
            { Sprites_Pneu.Chuva, icon_pneuChuva }
        };

        // Power Levels / Cargas
        dictPowerLevels = new Dictionary<Sprites_PowerLevel, Sprite>
        {
            { Sprites_PowerLevel.Level0, icon_powerLevel_0 },
            { Sprites_PowerLevel.Level1, icon_powerLevel_1 },
            { Sprites_PowerLevel.Level2, icon_powerLevel_2 },
            { Sprites_PowerLevel.Level3, icon_powerLevel_3 },
            { Sprites_PowerLevel.Level4, icon_powerLevel_4 },
            { Sprites_PowerLevel.Level5, icon_powerLevel_5 }
        };

        // Duelos - Resultados (Success - Failed)
        dictDuelResult = new Dictionary<Sprites_DuelResult, Sprite>
        {
            { Sprites_DuelResult.Success, icon_duel_success },
            { Sprites_DuelResult.Failed, icon_duel_failed }
        };

        // Mentalidades
        dictMentalidades = new Dictionary<Sprites_Mentalidade, Sprite>
        {
            {Sprites_Mentalidade.Defensiva, icon_mentalidade_defensiva },
            {Sprites_Mentalidade.Forte, icon_mentalidade_forte },
            {Sprites_Mentalidade.Ofensiva, icon_mentalidade_ofensiva },
            {Sprites_Mentalidade.Agressiva, icon_mentalidade_agressiva },
        };
    }
    #endregion

    #region Métodos Públicos para Acesso
    public Sprite GetSprite(Sprites_ModoMotor modo)
    {
        return dictModoMotor.TryGetValue(modo, out var sprite) ? sprite : null;
    }

    public Sprite GetSprite(Sprites_ModoDirecao modo)
    {
        return dictModoDirecao.TryGetValue(modo, out var sprite) ? sprite : null;
    }

    public Sprite GetSprite(Sprites_Pneu pneu)
    {
        return dictPneus.TryGetValue(pneu, out var sprite) ? sprite : null;
    }

    public Sprite GetSprite(Sprites_PowerLevel level)
    {
        return dictPowerLevels.TryGetValue(level, out var sprite) ? sprite : null;
    }

    public Sprite GetSprite(Sprites_DuelResult result)
    {
        return dictDuelResult.TryGetValue(result, out var sprite) ? sprite : null;
    }

    public Sprite GetSprite(Sprites_Mentalidade result)
    {
        return dictMentalidades.TryGetValue(result, out var sprite) ? sprite : null;
    }
    #endregion
}