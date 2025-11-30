using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TimeAndDate_Manager : MonoBehaviour
{

    public static TimeAndDate_Manager Instance;

    [SerializeField] private Transform luzDirecional;
    [SerializeField] private int duracaoDiaEmSegundos;

    private int duracaoDoDia;
    private float segundos;
    private float multiplicador;

    private void Awake()
    {
        if (Instance == null) 
            Instance = this;
    }

    void Start()
    {
        segundos = 86400 / 2.3f; // para iniciar o jogo 10 da manha.

        SetDuracaoDia(duracaoDiaEmSegundos);
    }

    // Update is called once per frame
    void Update()
    {
        segundos += Time.deltaTime * multiplicador;

        if(segundos >= 86400)
        {
            segundos = 0;
        }

        ProcessarCeu();
    }

    private void ProcessarCeu()
    {
        float rotacaoX = Mathf.Lerp(-90, 270, segundos/86400);
        luzDirecional.rotation = Quaternion.Euler(rotacaoX, 0, 0);
    }

    // GET SETS
    public void SetDuracaoDia(int tempo)
    {
        duracaoDoDia = tempo;
        multiplicador = 86400 / duracaoDoDia;
    }

    public float GetSegundosAtuais()
    {
        return segundos;
    }

    public bool EstaDeNoite()
    {
        // 21600 = 6 da manha, 64800 = 6 da tarde.
        if(segundos > 21600 && segundos < 64800) // De dia
        {
            return false;
        }

        if(segundos >= 0 && segundos <= 21599) // De noite
        {
            return true;
        }

        return false;
    }
}
