using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSimVisual_Acessorios : MonoBehaviour
{
    public CarSimVisual carSimVisual;

    [Header("====== Farois Frontais ======")]
    public GameObject farolFrontalDir;
    public GameObject farolFrontalEsq;

    [Header("====== Farois Traseiros ======")]
    public GameObject farolTraseiroDir;
    public GameObject farolTraseiroEsq;

    [Header("====== Particle - Nitro ======")]
    public GameObject particleNitro;

    private void Awake()
    {
        carSimVisual ??= GetComponent<CarSimVisual>();
    }

    public void Atualizar()
    {

    }

    public void Set_FaroisFrontais(bool value)
    {
        farolFrontalDir.SetActive(value);
        farolFrontalEsq.SetActive(value);
    }

    public void Set_FaroisTraseiros(bool value) 
    {
        farolTraseiroDir.SetActive(value);
        farolTraseiroEsq.SetActive(value);
    }
}
