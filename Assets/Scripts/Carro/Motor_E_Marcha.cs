using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Motor_E_Marcha : MonoBehaviour
{
    public Carro carro;

    [Header("============== CONFIGURAÇÃO DO MOTOR ==============")]
    public AnimationCurve torqueCurva;
    public float torqueMotor, torqueInjetado;

    [Header("============== ADIÇÃO DE POTÊNCIA ==============")]
    public float fatorTurbo = 5f; // Aumenta em até 5% a potência do motor.
    public float duracaoTurbo = 0f;
    public float fatorDRS = 5f; // Aumenta em 5% a potência do motor.
    public bool turboAtivado;
    public bool DRSAtivado;

    [Header("============== CONTROLE DE MARCHAS ==============")]
    public float[] totalMarchas;
    public int marchaAtual;
    public float motorMinRPM, motorMaxRPM;
    public float motorAtualRPM;
    public float tempoTrocaMarcha; // Tempo do carro pra trocar de marcha, definido no inspector.
    private float delayTrocaMarcha = 0f; // Delay, usado para calcular o valor do delay.

    private void Update()
    {
        // Temporizador do Delay entre as trocas de marcha
        delayTrocaMarcha -= Time.deltaTime;
        if (delayTrocaMarcha <= 0f) { delayTrocaMarcha = 0; };

        // Ativação/destivação do Turbo.
        duracaoTurbo -= Time.deltaTime;
        if (duracaoTurbo <= 0) { turboAtivado = false; }
        else if (duracaoTurbo > 0) { turboAtivado = true; }


        CalcularTorqueMotor();
        AtualizaMarchas();
        MarchaAutomatica();
    }

    private void CalcularTorqueMotor() // potencia Valor entre 0 até 1f
    {

        // Avalia a curva de torque com base na proporção REAL do RPM
        float atualRPM = torqueCurva.Evaluate(motorAtualRPM);

        //float torqueAtual = Mathf.Lerp(0, torqueMotor, multiplicadorTorque);
        torqueInjetado = torqueMotor;

        // ======= Torques Adicionais ========
        // Calculo habilidade do piloto
        float habilidadeSuavizada = Mathf.Pow(carro.pilotoController.piloto.HabilidadeAceleracao / 100f, 0.3f) * 100f;
        float fatorHabilidadeAceleracao_Piloto = (torqueInjetado * (habilidadeSuavizada / 100)) - torqueInjetado;

        // Calculo tipo Pneu.
        float fatorTipoPneu = (torqueInjetado * carro.carroceria.pneuFL.fatorPotencia) - torqueInjetado;

        // TURBO
        float fatorTurbo = 0f;
        if (turboAtivado)
        {
            fatorTurbo = (torqueInjetado * (this.fatorTurbo / 100 + 1)) - torqueInjetado;
        }

        // DRS
        float fatorDRS = 0f;
        if (DRSAtivado)
        {
            fatorDRS = (torqueInjetado * (this.fatorDRS / 100 + 1)) - torqueInjetado;
        }

        // Vácuo carro a frente.
        Carro carroFrente = carro.pilotoController.carroFrente;
        float fatorVacuo = 0f;
        if (carroFrente != null)
        {
            if (carro.pilotoController.distanciaCarroFrente <= 25f && carroFrente.pilotoController.faixaAtual == carro.pilotoController.faixaAtual)
            {
                fatorVacuo = torqueInjetado * 0.05f;
            }
        }

        // Calculo dos Modos de Motor e Direção.
        float fatorModoMotor = (torqueInjetado * carro.carroceria.GetFatorModoMotor("potencia")) - torqueInjetado;
        float fatorModoDirecao = (torqueInjetado * carro.carroceria.GetFatorModoDirecao("potencia")) - torqueInjetado;

        // Adiciona as potências adicionais evitando multiplicativos.
        torqueInjetado += fatorHabilidadeAceleracao_Piloto + fatorTipoPneu + fatorTurbo + fatorDRS + fatorModoMotor + fatorModoDirecao + fatorVacuo;

        // ===== Penalidades pelo desgasto dos Pneus ======
        if (carro.carroceria.GetCondicaoMediaPneus() < 30)
        {
            float penalidadeAtual = Mathf.Clamp(carro.carroceria.GetCondicaoMediaPneus() / 30, 0.9f, 1f);
            torqueInjetado *= penalidadeAtual;
        }

        // ==================
        // Se o combustivel acabar, o carro recebe apenas 50% do torque total que receberia.
        if (carro.carroceria.combustivelAtual <= 0)
        {
            torqueInjetado *= 0.5f;
        }

        // ==================
        // Engasgo ao trocar de marcha
        if (delayTrocaMarcha > 0.001f)
        {
            torqueInjetado = torqueInjetado * 0.25f;
        }

        // ==================
        // Corte de giro. Se o RPM atual ultrapassar o máximo, corta todo torque do motor.
        if (motorAtualRPM >= motorMaxRPM)
        {
            torqueInjetado = 0;
        }

        // ==================
        // torque é multiplicado pelo RPM do carro (0-1).
        torqueInjetado *= atualRPM;

        // Adiciona o torque conforme acelera.
        torqueInjetado *= carro.comandos.potenciaAceleracao;
    }

    // ======================================================================================
    // ================================= CONTROLE DE MARCHA  ================================
    // ======================================================================================
    private void AtualizaMarchas()
    {
        // Ponto morto
        if (carro.carroceria.velocidadeKMAtual < 1)
        {
            marchaAtual = 0;
            motorAtualRPM = motorMinRPM;
            return;
        }
        else
        {
            motorAtualRPM = carro.carroceria.velocidadeKMAtual * totalMarchas[marchaAtual] * 10f;
        }
    }

    private void MarchaAutomatica()
    {

        if (delayTrocaMarcha > 0)
            return;

        // RPMAtual chegando em 85% do motorMaxRPM, Sobe uma marcha.
        if (motorAtualRPM > motorMaxRPM * 0.85f && marchaAtual < totalMarchas.Length - 1)
        {
            marchaAtual++;
            delayTrocaMarcha = tempoTrocaMarcha;
        }

        // Ta acelerando e a rotação ta baixa, joga uma pra baixo.
        if ((carro.comandos.potenciaAceleracao > 0.1f) && (motorAtualRPM < motorMaxRPM * 0.4f) && (marchaAtual > 1))

        {
            marchaAtual--;
            delayTrocaMarcha = tempoTrocaMarcha;
        }

        // RPMAtual cai para menos que o motorMinRPM, Reduz uma marcha.
        if (motorAtualRPM < motorMinRPM && marchaAtual >= 1)
        {
            marchaAtual--;
            delayTrocaMarcha = tempoTrocaMarcha;
        }

        // Não está acelerando e o RPMAtual está baixo, reduz uma marcha.
        if (carro.comandos.potenciaAceleracao <= 0 && motorAtualRPM < motorMaxRPM * 0.5f && marchaAtual > 1)
        {
            marchaAtual--;
            delayTrocaMarcha = tempoTrocaMarcha;
        }
    }
}
