using UnityEngine;

public class Panel : MonoBehaviour
{
    public enum PanelType
    {
        // GamePlay - Race
        RACE_JOGO, // Painel base, onde será instanciado os demais paineis
        RACE_PLACAR,
        RACE_CARINFO,
        RACE_PILOTOCONTROLLER,
        RACE_PITSTOP_MANAGER, // Painel de PitStop, onde se planeja a próxima parada
        RACE_RADIO, // Painel onde são instanciados as mensagens de rádio
        RACE_GAMESPEED, // Painel que controla a velocidade do jogo
        RACE_SESSAOCLASSIFICACAO, // Painel de informação da sessão de classificação.
        RACE_DATAHORA,

        //
        RACE_INFO_SESSAOCLASSIFICACAO,
        RACE_INFO_SESSAOCORRIDA,
    }

    public virtual PanelType Type { get; set; }
    protected bool Initialized = false;

    protected virtual void OnDestroy()
    {
        PanelManager.Instance.UnregisterPanel(this);
    }

    private void Update()
    {
        // Inicializa uma vez o painel.
        if (!Initialized)
        {
            Initialize();
            Initialized = true;
        }

        // Atualiza sempre que o painel estiver ativo.
        if (gameObject.activeSelf)
        {
            AtualizarPainel();
        }
    }

    // Initialize para containers.
    public virtual void Initialize(object param1 = null, object param2 = null, object param3 = null) { }
    public virtual void AtualizarPainel() { }

    // Funções para GameObjects do tipo Panel.
    public virtual void AbrirPainel(object param1 = null, object param2 = null, object param3 = null)
    {
        gameObject.SetActive(true);
    }
    public virtual void OcultarPainel()
    {
        gameObject.SetActive(false);
    }
    public virtual void FecharPainel()
    {
        Destroy(gameObject);
    }
}
