using UnityEngine;

/// <summary>
/// Classe que armazena Dados para serem passados por parâmetro a um painel
/// </summary>
public class PanelParams
{
    // Dados Avulsos
    public string stringParam;
    public int intParam;

    // Cars
    public CarSimVisual carSim1;
    public CarSimVisual carSim2;

    // Pilotos
    public PilotoSim pilotoSim1;
    public PilotoSim pilotoSim2;
}

/// <summary>
/// Classe base para todos os painéis do jogo.
/// </summary>
public abstract class Panel : MonoBehaviour
{
    // Tipos de painéis
    public enum PanelType
    {
        // GamePlay - Race
        RACE_JOGO,
        RACE_PLACAR,
        RACE_CARINFO,
        RACE_PILOTOCONTROLLER,
        RACE_PITSTOP_MANAGER,
        RACE_RADIO,
        RACE_GAMESPEED,
        RACE_SESSAOCLASSIFICACAO,
        RACE_DATAHORA,
        RACE_INFO_SESSAOCLASSIFICACAO,
        RACE_INFO_SESSAOCORRIDA,

        // Paineis Flutuantes
        FLOATING_CARINFO,
        FLOATING_DUELINFO,
    }

    public abstract PanelType Type { get; }

    protected bool Initialized = false;

    // Dados para painel flutuante
    protected bool flutuante = false;
    protected Transform flutuanteTarget;
    protected Vector3 flutuanteOffset = Vector3.up * 2f;

    #region Unity Callbacks
    protected virtual void OnEnable()
    {
        
    }

    protected virtual void OnDisable()
    {
        SubscribeEvents(false);
    }

    protected virtual void Update()
    {
        if (gameObject.activeSelf)
        {
            AtualizarPainel();
        }
    }

    protected virtual void LateUpdate()
    {
        // Atualiza posição para paineis flutuantes
        if (flutuante)
        {
            transform.position = flutuanteTarget.position + flutuanteOffset;
            transform.LookAt(GameManager.Instance.mainCamera.transform);
            transform.Rotate(0f, 180f, 0f); // gira em Y
        }
    }

    protected virtual void OnDestroy()
    {
        PanelManager.Instance?.UnregisterPanel(this);
    }
    #endregion

    #region Métodos principais
    public virtual void Initialize(PanelParams param = null)
    {
        // Implementação padrão: pode ser sobrescrita
    }

    public virtual void AtualizarPainel()
    {
        // Sobrescrever nos paineis filhos
    }

    public virtual void SubscribeEvents(bool subscribe)
    {
        // Sobrescrever nos paineis filhos
    }

    public virtual void AbrirPainel(PanelParams param = null)
    {
        gameObject.SetActive(true);
        Initialize(param);
    }

    public virtual void OcultarPainel()
    {
        gameObject.SetActive(false);
    }

    public virtual void FecharPainel()
    {
        Destroy(gameObject);
    }

    public virtual void SetTarget(Transform target, Vector3 customOffset)
    {
        flutuanteTarget = target;
        flutuanteOffset = customOffset;
    }
    #endregion
}
