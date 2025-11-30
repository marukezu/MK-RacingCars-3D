using UnityEngine;

public class Carro_ParticleTrail : MonoBehaviour
{
    public Carro carro;

    public ParticleSystem dustTrail_FL;
    public ParticleSystem dustTrail_FR;

    [Header("Velocidade em Km/h")]
    private float velocidadeMin = 5f;
    private float velocidadeMax = 150f;

    [Header("Emission")]
    private float emissionMin = 50f;
    private float emissionMax = 300f;

    [Header("Simulation Speed")]
    private float simSpeedMin = 0.1f;
    private float simSpeedMax = 1f;

    private ParticleSystem.EmissionModule emFL;
    private ParticleSystem.EmissionModule emFR;

    private ParticleSystem.MainModule mainFL;
    private ParticleSystem.MainModule mainFR;

    private void Start()
    {
        emFL = dustTrail_FL.emission;
        emFR = dustTrail_FR.emission;

        mainFL = dustTrail_FL.main;
        mainFR = dustTrail_FR.main;
    }

    private void Update()
    {
        float v = carro.carroceria.velocidadeKMAtual;

        // Sem velocidade → poeira desligada
        if (v < velocidadeMin)
        {
            emFL.rateOverTime = 0;
            emFR.rateOverTime = 0;

            mainFL.simulationSpeed = simSpeedMin;
            mainFR.simulationSpeed = simSpeedMin;
            return;
        }

        // Normaliza velocidade entre 0e1
        float t = Mathf.InverseLerp(velocidadeMin, velocidadeMax, v);

        // Ajusta emissão
        float emissionValue = Mathf.Lerp(emissionMin, emissionMax, t);
        emFL.rateOverTime = emissionValue;
        emFR.rateOverTime = emissionValue;

        // Ajusta simulation speed (o mais importante)
        float simSpeedValue = Mathf.Lerp(simSpeedMin, simSpeedMax, t);
        mainFL.simulationSpeed = simSpeedValue;
        mainFR.simulationSpeed = simSpeedValue;
    }
}
