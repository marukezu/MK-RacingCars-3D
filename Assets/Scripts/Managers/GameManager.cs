using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // ==
    public static GameManager Instance;

    [HideInInspector] public Piloto pilotoEmFoco { get; private set; }

    [Header("=========== PREFAB CARROS ============")]
    public GameObject carro_Lanzia;
    public GameObject carro_LanziaStradale;
    public GameObject carro_RinaultAlpine;

    [Header("=========== PREFAB MINU CUPA'S ============")]
    public GameObject carro_MiniCupa_RED;
    public GameObject carro_MiniCupa_GREEN;
    public GameObject carro_MiniCupa_BLUE;
    public GameObject carro_MiniCupa_YELLOW;

    // Controle do Tempo de Jogo (Velocidade - Time.timeScale)
    private float gameSpeed = 1f;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        DontDestroyOnLoad(Instance);
    }

    void Start()
    {
        SetGameSpeed(1f);
    }

    // =============================================================================================================
    // ================================ Controle da velocidade do tempo de jogo ====================================
    // =============================================================================================================
    public float GetGameSpeed()
    {
        return gameSpeed;
    }

    public void SetGameSpeed(float speed)
    {
        gameSpeed = speed;
        Time.timeScale = gameSpeed;
    }

    // =============================================================================================================
    // ================================================== Gets =====================================================
    // =============================================================================================================
    public GameObject GetCarroByPiloto(Piloto piloto)
    {
        switch (piloto.CarroModelo)
        {
            case Carro.Modelo.Lanzia:
                return carro_Lanzia;

            case Carro.Modelo.LanziaStradale:
                return carro_LanziaStradale;

            case Carro.Modelo.RinaultAlpine:
                return carro_RinaultAlpine;

                // MINI CUPA'S
            case Carro.Modelo.MiniCupa_RED:
                return carro_MiniCupa_RED;

            case Carro.Modelo.MiniCupa_BLUE:
                return carro_MiniCupa_BLUE;

            case Carro.Modelo.MiniCupa_GREEN:
                return carro_MiniCupa_GREEN;

            case Carro.Modelo.MiniCupa_YELLOW:
                return carro_MiniCupa_YELLOW;

            default:
                return null;
        }
    }

    // =============================================================================================================
    // ================================================== Sets =====================================================
    // =============================================================================================================
    public void SetPilotoEmFoco(Piloto piloto)
    {
        pilotoEmFoco = piloto;
        CameraManager.Instance.AtualizarCamera(pilotoEmFoco.Carro);
    }

    // =============================================================================================================
    // =============================================== Conversores =================================================
    // =============================================================================================================
    public string ConverterTempoParaFormato(float tempoEmSegundos)
    {
        // Calcula minutos, segundos e milissegundos
        int minutos = Mathf.FloorToInt(tempoEmSegundos / 60f);
        int segundos = Mathf.FloorToInt(tempoEmSegundos % 60f);
        int milissegundos = Mathf.FloorToInt((tempoEmSegundos * 1000f) % 1000f);

        // Retorna no formato "00:00:000"
        return $"{minutos:00}:{segundos:00}:{milissegundos:000}";
    }

    public void SetTextGradientColor(Text textComponent, float value)
    {
        // Limita o valor entre 0 e 100
        value = Mathf.Clamp(value, 0f, 100f);

        // Define as cores do degradê
        Color red = Color.red;   // 0%
        Color green = Color.green; // 100%

        // Ajusta o valor para distribuir a transição
        float adjustedValue;
        if (value > 40f)
        {
            // Para valores entre 100 e 40, ocupa 30% da mudança de cor
            adjustedValue = Mathf.Lerp(0.7f, 1f, (value - 40f) / 60f); // Mapeia 40-100 para 0.7-1
        }
        else
        {
            // Para valores entre 39 e 0, ocupa 70% da mudança de cor
            adjustedValue = Mathf.Lerp(0f, 0.7f, value / 40f); // Mapeia 0-39 para 0-0.7
        }

        // Interpolação final entre vermelho e verde
        Color finalColor = Color.Lerp(red, green, adjustedValue);

        // Aplica a cor no Text
        textComponent.color = finalColor;
    }

    // =============================================================================================================
    // ======================================= Funções para Paineis/Canvas =========================================
    // =============================================================================================================
    public IEnumerator FazerPanelFade(Image panelImage, float duration, bool escurecer)
    {
        Color initialColor = panelImage.color;
        Color targetColor;
        if (escurecer)
            targetColor = new Color(initialColor.r, initialColor.g, initialColor.b, 1f); // Totalmente opaco

        else
            targetColor = new Color(initialColor.r, initialColor.g, initialColor.b, 0f); // Totalmente opaco

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            panelImage.color = Color.Lerp(initialColor, targetColor, elapsedTime / duration);
            yield return null;
        }

        // Certifica-se de que a cor final seja aplicada
        panelImage.color = targetColor;
    }

}
