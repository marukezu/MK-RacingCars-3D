using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostesDeLuz : MonoBehaviour
{
    public GameObject[] lampadas;

    void Update()
    {
        for (int i = 0; i < lampadas.Length; i++)
        {
            lampadas[i].gameObject.SetActive(TimeAndDate_Manager.Instance.EstaDeNoite());
        }
    }
}
