using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    public static RaceManager Instance;

    // Managers
    public SplineManager splineManager;
    public SimulationEngine engineSin;

    public Transform[] gridSlots;

    [HideInInspector] public List<CarSimVisual> carsVisuais;

    public bool largadaAutorizada = false;
    public bool ultrapassagemAutorizada = false;

    private void Awake()
    {
        Instance ??= this;
    }

    void Start()
    {
        // Instancia os CarSimVisual
        Instanciar_CarSimVisuals();

        // Inicializa o piloto em foco
        var pilotoFoco = PilotosDataBase.Pilotos_CampeonatoAtual
            .FirstOrDefault(p => p.Nome == "Marukezu");

        if (pilotoFoco != null)
            GameManager.Instance.SetPilotoEmFoco(pilotoFoco.carsim);

        // Camera
        CameraManager.Instance.AtualizarCamera(GameManager.Instance.carEmFoco);

        // Instancia Paineis
        PanelManager.Instance.InstanciarPaineisCorrida();

        // Inicia o Simulation Engine
        engineSin = new SimulationEngine(splineManager, this);

        // Dar Largada
        StartCoroutine(DarLargada());
    }


    void Update()
    {
        engineSin.Tick(Time.deltaTime);  
    }

    private void Instanciar_CarSimVisuals()
    {
        carsVisuais = new List<CarSimVisual>();

        foreach (PilotoSim p in PilotosDataBase.Pilotos_CampeonatoAtual)
        {
            // 1️⃣ Encontra o slot correto pelo nome
            Transform slot = System.Array.Find(gridSlots, s => s.name == p.PosicaoGrid.ToString());

            if (slot == null)
            {
                Debug.LogWarning($"Slot para posição {p.PosicaoGrid} não encontrado!");
                continue; // pula este piloto se não achar o slot
            }

            // 2️⃣ Instancia o carro no slot
            GameObject newCar = GameManager.Instance.InstantiateCarSimVisual(slot.position, slot.rotation);
            CarSimVisual carScript = newCar.GetComponent<CarSimVisual>();

            // 3️⃣ Linka os scripts
            carScript.piloto = p;
            carScript.spline = splineManager;
            p.carsim = carScript;

            // 4️⃣ Calcula splineT e distance a partir do slot
            float nearestT = splineManager.FindNearestT(slot.position);
            p.Set_SplineT(nearestT);
            p.Set_Distance(splineManager.DistanceFromT(nearestT));

            // 5️⃣ Calcula lateralOffset (grid visual)
            Vector3 splinePos = splineManager.GetPosition(nearestT);
            Vector3 right = splineManager.GetRight(nearestT);

            float offSetLatInicial = (p.PosicaoGrid % 2 == 0) ? -1.4f : 1.4f;
            p.mod_PosLateral.Set_OffsetReal(offSetLatInicial);

            // 6️⃣ Adiciona o carro à lista de carros visuais
            carsVisuais.Add(carScript);
        }
    }


    private IEnumerator DarLargada()
    {
        yield return new WaitForSeconds(5);

        largadaAutorizada = true;

        yield return new WaitForSeconds(5);

        // Libera 'coisas' para todos os pilotos
        foreach(var p in PilotosDataBase.Pilotos_CampeonatoAtual)
        {
            
        }

        ultrapassagemAutorizada = true;
    }
}
