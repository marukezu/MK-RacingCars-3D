using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostesDeLuz : MonoBehaviour
{

    public GameObject[] lampadas;

    

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        float segundos = TimeAndDate_Manager.Instance.GetSegundosAtuais();

        if (segundos > 61200 || (segundos >= 0 && segundos <= 21599)) // De noite
        {
            for (int i = 0; i < lampadas.Length; i++)
            {
                lampadas[i].gameObject.SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < lampadas.Length; i++)
            {
                lampadas[i].gameObject.SetActive(false);
            }
        }
    }
}
