using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerManager : MonoBehaviour
{
    public static ContainerManager Instance;

    public enum ContainerType
    {
        // RACE
        PANEL_PLACAR_PILOTOINFO_A,
        PANEL_PLACAR_PILOTOINFO_B,
        RADIO_MENSAGEM,
    }

    [Header("====== Containers ======")]
    [SerializeField] private GameObject Container_Placar_PilotoInfo_A;
    [SerializeField] private GameObject Container_Placar_PilotoInfo_B;
    [SerializeField] private GameObject Container_Radio_Mensagem;

    // Dicion�rio com todos os containers.
    private Dictionary<ContainerType, GameObject> containerDictionary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        InitializeDictionary();
    }

    private void InitializeDictionary()
    {
        containerDictionary = new Dictionary<ContainerType, GameObject>
        {
             // RACE
             { ContainerType.PANEL_PLACAR_PILOTOINFO_A, Container_Placar_PilotoInfo_A },
             { ContainerType.PANEL_PLACAR_PILOTOINFO_B, Container_Placar_PilotoInfo_B },
             { ContainerType.RADIO_MENSAGEM, Container_Radio_Mensagem },
        };
    }

    // ==========================================================================================
    // ========================= FUN��ES PARA MANUSEIO DOS CONTAINERS ===========================
    // ==========================================================================================
    public GameObject InstantiateAndReturnContainer(
        ContainerType containerType, GameObject containerDestiny)
    {
        GameObject newContainer = null;

        if (containerDictionary.TryGetValue(containerType, out GameObject container))
        {
            newContainer = Instantiate(container.gameObject, containerDestiny.transform);
        }

        return newContainer;
    }
    public GameObject InstantiateContainerInPositionAndReturnContainer(
    ContainerType containerType, Vector3 position)
    {
        GameObject newContainer = null;

        if (containerDictionary.TryGetValue(containerType, out GameObject container))
        {
            newContainer = Instantiate(container.gameObject, position, container.transform.rotation);
        }

        return newContainer;
    }
}
