using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSimVisual_UpdateManager : MonoBehaviour
{
    [HideInInspector] public CarSimVisual carSimVisual;

    private void Awake()
    {
        carSimVisual ??= GetComponent<CarSimVisual>();
    }

    void Update()
    {
        // CarSimVisual
        carSimVisual.AtualizarPosicaoNaPista();

        // Suspensoes
        carSimVisual.suspensao.AtualizarPosicao();
        carSimVisual.suspensao.AtualizarRotacao();

        // Carroceria
        carSimVisual.carroceria.AtualizarPosicaoCarroceria();
        carSimVisual.carroceria.AtualizarRotacaoCarroceria();

        // Acessórios
        carSimVisual.acessorios.Atualizar();
    }
}
