using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Carro;

public class Carro_Comandos : MonoBehaviour
{
    public Carro carro;

    [Header("============== POTÊNCIAS APLICADAS ==============")]
    public float potenciaAceleracao, potenciaFrenagem, potenciaVolante;

    private void Start()
    {
        // Inicia os modos de direção e motor conforme o inicial do piloto.
        SetModoDirecao(carro.pilotoController.piloto.ModoDirecaoInicial);
        SetModoMotor(carro.pilotoController.piloto.ModoMotorInicial);
    }

    private void Update()
    {
        AcionarFarolFreio();
    }

    // ======================================================================================
    // ========================= Entrada de Comandos Do Veiculo =============================
    // ======================================================================================
    public void CarroAcelerar(float potencia)
    {
        potenciaAceleracao = potencia;
    }
    public void CarroFrear(float potencia)
    {
        potenciaFrenagem = potencia;
    }
    public void CarroVirar(float potencia) // potencia Valor entre -1f até 1f
    {
        potenciaVolante = potencia;
    }
    public void CarroSetarTurbo(bool option)
    {
        carro.motorMarcha.turboAtivado = option;
    }
    public void CarroSetarDRS(bool option)
    {
        carro.motorMarcha.DRSAtivado = option;
    }
    public void SetModoMotor(TipoDeMotor option)
    {
        carro.carroceria.modoMotor = option;
    }
    public void SetModoDirecao(TipoDeConducao option)
    {
        carro.carroceria.modoDirecao = option;
    }

    // ======================================================================================
    // =========================== LIGAR/DESLIGAR PERIFÉRICOS ===============================
    // ======================================================================================
    public void SetFarolFrontal(bool option)
    {
        carro.carroceria.FarolFrontalDir.gameObject.SetActive(option);
        carro.carroceria.FarolFrontalEsq.gameObject.SetActive(option);
    }
    private void AcionarFarolFreio()
    {
        if (potenciaFrenagem > 0.1f)
        {
            carro.carroceria.FarolFreioDir.gameObject.SetActive(true);
            carro.carroceria.FarolFreioEsq.gameObject.SetActive(true);
        }
        else
        {
            carro.carroceria.FarolFreioDir.gameObject.SetActive(false);
            carro.carroceria.FarolFreioEsq.gameObject.SetActive(false);
        }
    }
}
