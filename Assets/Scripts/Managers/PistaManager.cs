using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PistaManager : MonoBehaviour
{
    public static PistaManager Instance;

    [Header("=========== CONFIGURAÇÕES DE ENTRADA (DEVE PREENCHER) ============")]
    public int checkpointsPerdidosPitStop; // Quantos Waypoints ele não está passando da pista por ter entrado no PitStop, para que quando saia do pit, seja incrementado nos waypoints para seguir.
    public int waypointIndexPiloto1; 
    public int waypointIndexPiloto2;
    public int waypointIndexPiloto3;
    public int waypointIndexPiloto4; // (PARA TODOS 'waypointIndexPilotoX') Qual é o index do PITSTOP do veiculo, exatamente o ponto em que ele para no pitstop.
    public int waypointIndexPiloto5;
    public int waypointIndexPiloto6;
    public int waypointIndexPiloto7;
    public int waypointIndexPiloto8;

    [Header("=========== CONFIGURAÇÃO DA PISTA ============")]
    [Tooltip("Duração do dia em segundos")] public int duracaoDoDia;

    [Header("=========== LOCAL INICIAL DOS CARROS - CORRIDA ============")]
    public Transform posicao_Grid_1;
    public Transform posicao_Grid_2;
    public Transform posicao_Grid_3;
    public Transform posicao_Grid_4;
    public Transform posicao_Grid_5;
    public Transform posicao_Grid_6;
    public Transform posicao_Grid_7;
    public Transform posicao_Grid_8;

    [Header("=========== LOCAL INICIAL DOS CARROS - CLASSIFICAÇÃO ============")]
    public Transform posicao_PitStop_Piloto1;
    public Transform posicao_PitStop_Piloto2;
    public Transform posicao_PitStop_Piloto3;
    public Transform posicao_PitStop_Piloto4;
    public Transform posicao_PitStop_Piloto5;
    public Transform posicao_PitStop_Piloto6;
    public Transform posicao_PitStop_Piloto7;
    public Transform posicao_PitStop_Piloto8;
    [HideInInspector] public Transform[] posicoesPitStop { get; private set; }

    [Header("=========== WAYPOINTS PISTA ============")]
    public Transform waypoint_Terra_PaiEsq;
    public Transform waypoint_Terra_PaiDir;
    public Transform waypoint_Terra_PaiCentro;
    public Transform waypoint_Terra_PaiPitStop;
    public int waypointsTotal;

    // Usado para preencher os waypoints com waypoints da pista escolhida e enviar ao piloto.
    [HideInInspector] public List<Waypoint> waypointsEsq, waypointsDir, waypointsCentro, waypoints_PitStop;

    // Distância acumulada da pista
    public float TotalTamanhoPista;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        // Inicializa todos os waypoints.
        InicializaWaypoints();
        waypointsTotal = waypointsCentro.Count;

        // Lista de posições dos pit stops.
        posicoesPitStop = new Transform[]
        {
            posicao_PitStop_Piloto1,
            posicao_PitStop_Piloto2,
            posicao_PitStop_Piloto3,
            posicao_PitStop_Piloto4,
            posicao_PitStop_Piloto5,
            posicao_PitStop_Piloto6,
            posicao_PitStop_Piloto7,
            posicao_PitStop_Piloto8
        };

        // Distância da pista, progresso.
        TotalTamanhoPista = 0f;

        for (int i = 1; i < waypointsCentro.Count; i++)
        {
            float dist = Vector3.Distance(
                waypointsCentro[i - 1].transform.position,
                waypointsCentro[i].transform.position
            );

            TotalTamanhoPista += dist;
        }

        // 🟡 Distância do último waypoint até o primeiro (fechar circuito)
        float distFinal = Vector3.Distance(
            waypointsCentro[waypointsCentro.Count - 1].transform.position,
            waypointsCentro[0].transform.position
        );

        TotalTamanhoPista += distFinal;

        // Aponta para o RaceManager iniciar a corrida.
        RaceManager.Instance.Inicializar();
    }

    // =============================================================================================================
    // ========================================== Inicializa Waypoints =============================================
    // =============================================================================================================
    private void InicializaWaypoints()
    {
        // Inicia os waypoints.
        waypointsEsq = new List<Waypoint>();
        waypointsDir = new List<Waypoint>();
        waypointsCentro = new List<Waypoint>();
        waypoints_PitStop = new List<Waypoint>();

        // Preenche os waypoints esquerda
        for (int i = 0; i < waypoint_Terra_PaiEsq.childCount; i++)
        {
            Transform childTransform = waypoint_Terra_PaiEsq.GetChild(i);
            Waypoint childWaypoint = childTransform.GetComponent<Waypoint>();
            if (childWaypoint != null)
            {
                childWaypoint.ID = i;
                waypointsEsq.Add(childWaypoint);
            }
        }
        // Preenche os waypoints direita
        for (int i = 0; i < waypoint_Terra_PaiDir.childCount; i++)
        {
            Transform childTransform = waypoint_Terra_PaiDir.GetChild(i);
            Waypoint childWaypoint = childTransform.GetComponent<Waypoint>();
            if (childWaypoint != null)
            {
                childWaypoint.ID = i;
                waypointsDir.Add(childWaypoint);
            }
        }
        // Preenche os waypoints central
        for (int i = 0; i < waypoint_Terra_PaiCentro.childCount; i++)
        {
            Transform childTransform = waypoint_Terra_PaiCentro.GetChild(i);
            Waypoint childWaypoint = childTransform.GetComponent<Waypoint>();
            if (childWaypoint != null)
            {
                childWaypoint.ID = i;
                waypointsCentro.Add(childWaypoint);
            }
        }

        // Preenche os waypoints de pitstop
        for (int i = 0; i < waypoint_Terra_PaiPitStop.childCount; i++)
        {
            Transform childTransform = waypoint_Terra_PaiPitStop.GetChild(i);
            Waypoint childWaypoint = childTransform.GetComponent<Waypoint>();
            if (childWaypoint != null)
            {
                childWaypoint.ID = i;
                waypoints_PitStop.Add(childWaypoint);
            }
        }
    }

    // =============================================================================================================
    // =========================================== Gets de Waypoints ===============================================
    // =============================================================================================================
    public List<Waypoint> GetListWaypointsEsq()
    {
        return waypointsEsq;
    }
    public List<Waypoint> GetListWaypointsCentro()
    {
        return waypointsCentro;
    }
    public List<Waypoint> GetListWaypointsDir()
    {
        return waypointsDir;
    }
    public List<Waypoint> GetListWaypointsPitStop()
    {
        return waypoints_PitStop;
    }
    public List<Waypoint> GetListWaypointsPitStop_InicioClassificacao(PilotoController controller)
    {
        controller.waypointIndex = GetPilotoParadaPitStopIndex(controller) + 1;
        return waypoints_PitStop;
    }
    public List<Waypoint> GetListWaypoints_InicioCorrida(PilotoController controller)
    {
        switch (controller.piloto.PosicaoGrid)
        {
            case 1 or 3 or 5 or 7:
                return waypointsDir;

            case 2 or 4 or 6 or 8:
                return waypointsEsq;

            default:
                Debug.Log("Retornou na nula");
                return null;
        }
    }

    public int GetPilotoParadaPitStopIndex(PilotoController controller)
    {
        // Index do pitstop.
        if (controller.piloto.ID == 1)
        {
            return waypointIndexPiloto1;
        }
        else if (controller.piloto.ID == 2)
        {
            return waypointIndexPiloto2;
        }
        else if (controller.piloto.ID == 3)
        {
            return waypointIndexPiloto3;
        }
        else if (controller.piloto.ID == 4)
        {
            return waypointIndexPiloto4;
        }
        else if (controller.piloto.ID == 5)
        {
            return waypointIndexPiloto5;
        }
        else if (controller.piloto.ID == 6)
        {
            return waypointIndexPiloto6;
        }
        else if (controller.piloto.ID == 7)
        {
            return waypointIndexPiloto7;
        }
        else if (controller.piloto.ID == 8)
        {
            return waypointIndexPiloto8;
        }
        else
            return -1;
    }

    // =============================================================================================================
    // ==================================== Funções adicionais para Waypoints ======================================
    // =============================================================================================================
    public void TeleportParaPitStop(PilotoController controller)
    {
        controller.carro.carroceria.carRigidBody.velocity = Vector3.zero;

        // Index do pitstop.
        if (controller.piloto.ID == 1)
        {
            controller.carro.transform.position = posicao_PitStop_Piloto1.position;
            controller.carro.transform.rotation = posicao_PitStop_Piloto1.rotation;
        }
        else if (controller.piloto.ID == 2)
        {
            controller.carro.transform.position = posicao_PitStop_Piloto2.position;
            controller.carro.transform.rotation = posicao_PitStop_Piloto2.rotation;
        }
        else if (controller.piloto.ID == 3)
        {
            controller.carro.transform.position = posicao_PitStop_Piloto3.position;
            controller.carro.transform.rotation = posicao_PitStop_Piloto3.rotation;
        }
        else if (controller.piloto.ID == 4)
        {
            controller.carro.transform.position = posicao_PitStop_Piloto4.position;
            controller.carro.transform.rotation = posicao_PitStop_Piloto4.rotation;
        }
        else if (controller.piloto.ID == 5)
        {
            controller.carro.transform.position = posicao_PitStop_Piloto5.position;
            controller.carro.transform.rotation = posicao_PitStop_Piloto5.rotation;
        }
        else if (controller.piloto.ID == 6)
        {
            controller.carro.transform.position = posicao_PitStop_Piloto6.position;
            controller.carro.transform.rotation = posicao_PitStop_Piloto6.rotation;
        }
        else if (controller.piloto.ID == 7)
        {
            controller.carro.transform.position = posicao_PitStop_Piloto7.position;
            controller.carro.transform.rotation = posicao_PitStop_Piloto7.rotation;
        }
        else if (controller.piloto.ID == 8)
        {
            controller.carro.transform.position = posicao_PitStop_Piloto8.position;
            controller.carro.transform.rotation = posicao_PitStop_Piloto8.rotation;
        }
    }

}
