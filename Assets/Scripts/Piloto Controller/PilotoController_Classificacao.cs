using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static PilotoController;

public class PilotoController_Classificacao : MonoBehaviour
{
    public PilotoController controller;

    private void Update()
    {
        if (RaceManager.Instance.tipoDeCorrida == RaceManager.TipoDeCorrida.Classificacao)
            ComportamentoIA();
    }

    // =============================================================================================================
    // ================================================== CORE =====================================================
    // =============================================================================================================
    private void ComportamentoIA()
    {
        ConduzirPelaPista();
    }

    // =============================================================================================================
    // ========================================= COMPORTAMENTO CONDU��O ============================================
    // =============================================================================================================
    private void ConduzirPelaPista()
    {
        // Respons�vel pela intelig�ncia ao acelerar.
        AcelerarFrear();

        // Respons�vel pela intelig�ncia ao virar o volante.
        VirarVeiculo();

        // Responsável pela lógica de sair do pit e ir classificar.
        IrClassificar();

        // Respons�vel por verificar se deu as voltas da classifica��o e retornar ao Pit.
        EscolherModoDirecaoEMotor();

        // Lógica para ultrapassar e ir ao centro durante a classificação.
        Ultrapassar();

        // Lógica para manter o carro nas faixas.
        ManterEmFaixas();

        // Atualiza comandos do Carro.
        controller.carro.comandos.CarroAcelerar(controller.potenciaAceleracao);
        controller.carro.comandos.CarroFrear(controller.potenciaFreio);
        controller.carro.comandos.CarroVirar(controller.potenciaGiroDirecao);
    }

    // ==================================================================================================================
    // ======================================= Comportamentos de Pista - (I.A.) =========================================
    // ==================================================================================================================
    private void AcelerarFrear()
    {
        float velocidadeCarro = controller.carro.carroceria.velocidadeKMAtual;
        float velocidadeWaypoint = controller.waypointAtual.GetWaypointSpeed(controller);
        
        float novapotenciaAceleracao = 0f;
        float novapotenciaFrenagem = 0f;

        // ===============
        // Aceleração

        if (velocidadeCarro < velocidadeWaypoint)
        {
            novapotenciaAceleracao = 1f;

            // Fator de "o quanto est� virando o volante".
            float fatorGiroVolante = Mathf.Abs(controller.potenciaGiroDirecao);

            if (!controller.waypointAtual.waypointPitStop)
            {
                // Adiciona a pot�ncia dependendo do ester�o do volante.
                novapotenciaAceleracao = Mathf.Clamp01((Mathf.Clamp(1f - ((fatorGiroVolante * 0.5f)), 0.7f, 1f)));
            }

            // Fator de inclina��o - subida ou descida.
            float inclinacao = controller.carro.carroceria.carRigidBody.transform.eulerAngles.x;
            float minInclinacao = 270f;
            float maxInclinacao = 350f;
            float fatorInclinacao = (maxInclinacao - inclinacao) / (maxInclinacao - minInclinacao);
            fatorInclinacao = Mathf.Clamp01(fatorInclinacao); // Garante que o valor esteja entre 0 e 1.

            // Subida com baixa velocidade, acelera mais forte.
            if ((inclinacao < 350f && inclinacao > 270f))
            {
                if (controller.carro.carroceria.velocidadeKMAtual <= 35)
                {
                    novapotenciaAceleracao += Mathf.Clamp01(fatorInclinacao * 2f);
                }
                if (controller.carro.carroceria.velocidadeKMAtual <= 25)
                {
                    novapotenciaAceleracao += Mathf.Clamp01(fatorInclinacao * 3f);
                }
                if (controller.carro.carroceria.velocidadeKMAtual <= 15)
                {
                    novapotenciaAceleracao += Mathf.Clamp01(fatorInclinacao * 4f);
                }
                if (controller.carro.carroceria.velocidadeKMAtual <= 0)
                {
                    novapotenciaAceleracao += Mathf.Clamp01(fatorInclinacao * 5f);
                }
            }

            // Se estiver no pitstop 
            if (controller.waypointAtual.waypointPitStop)
            {
                if (velocidadeWaypoint * 0.95f < velocidadeCarro)
                    novapotenciaAceleracao *= 0.2f;
            }
        }

        // ================
        // Frenagem

        // Se o carro estiver mais rapido que a velocidade imposta pelo waypoint.
        if (velocidadeCarro > velocidadeWaypoint)
        {
            // Calcular a raz�o de velocidade
            float razaoVelocidade = Mathf.Clamp01((velocidadeCarro / velocidadeWaypoint) - 0.5f);

            // Escalar a potenciaFreio com base na raz�o de velocidade
            novapotenciaFrenagem = Mathf.Clamp(razaoVelocidade, 0, 0.5f);
        }
        else
        {
            novapotenciaFrenagem = 0f;
        }

        // Checa se tem carro a frente pr�ximo.
        if (controller.voltaAtual == 2) // Se estiver na volta rápida.
        {
            if (controller.distanciaCarroFrente <= 2f)
            {
                // Reduzir a acelera��o com base na dist�ncia.
                novapotenciaAceleracao *= 0.85f;
            }
            else if (controller.distanciaCarroFrente <= 6f)
            {
                // Reduzir a acelera��o com base na dist�ncia.
                novapotenciaAceleracao *= 0.9f;
            }
            else if (controller.distanciaCarroFrente <= 10f)
            {
                // Reduzir a acelera��o com base na dist�ncia.
                novapotenciaAceleracao *= 0.95f;
            }
        }
        else // Voltas de saida e entrada.
        {
            if (controller.distanciaCarroFrente <= 2f) 
            {
                // Reduzir a acelera��o com base na dist�ncia.
                novapotenciaAceleracao *= 0.5f;
            }
            else if (controller.distanciaCarroFrente <= 6f)
            {
                // Reduzir a acelera��o com base na dist�ncia.
                novapotenciaAceleracao *= 0.55f;
            }
            else if (controller.distanciaCarroFrente <= 10f)
            {
                // Reduzir a acelera��o com base na dist�ncia.
                novapotenciaAceleracao *= 0.7f;
            }
        }
        

        controller.carro.carroceria.suspensaoFL.wheelCollider.brakeTorque = 0f;
        controller.carro.carroceria.suspensaoFR.wheelCollider.brakeTorque = 0f;
        controller.carro.carroceria.suspensaoRL.wheelCollider.brakeTorque = 0f;
        controller.carro.carroceria.suspensaoRR.wheelCollider.brakeTorque = 0f;

        if (controller.piloto.EquipePitStop.ParadoNoPitStop)
        {
            novapotenciaAceleracao = 0f;
            novapotenciaFrenagem = 1f;
            controller.carro.carroceria.suspensaoFL.wheelCollider.brakeTorque = 100f;
            controller.carro.carroceria.suspensaoFR.wheelCollider.brakeTorque = 100f;
            controller.carro.carroceria.suspensaoRL.wheelCollider.brakeTorque = 100f;
            controller.carro.carroceria.suspensaoRR.wheelCollider.brakeTorque = 100f;
        }

        // ===============
        // Aplica a potencia final.
        controller.potenciaAceleracao = novapotenciaAceleracao;
        controller.potenciaFreio = novapotenciaFrenagem;
    }

    private void VirarVeiculo()
    {
        Vector3 waypointPosition = new Vector3(controller.waypointAtual.transform.position.x, transform.position.y, controller.waypointAtual.transform.position.z);

        Vector3 dir = transform.InverseTransformPoint(waypointPosition);

        // Calcular a dist�ncia at� o pr�ximo waypoint
        float distanceToWaypoint = dir.magnitude;

        // Definir a dist�ncia m�xima para iniciar a virada suave (pode ser ajustada conforme necess�rio)
        float maxTurnDistance = 5f;

        // Calcular a intensidade da rota��o com base na dist�ncia
        float turnIntensity = Mathf.Clamp01(distanceToWaypoint / maxTurnDistance);

        // Ajustar a pot�ncia do giro com base na intensidade calculada
        controller.potenciaGiroDirecao = Mathf.Clamp((dir.x / dir.magnitude) * turnIntensity, -1f, 1f);
    }

    private void Ultrapassar()
    {
        // Só funciona fora do pitstop.
        if (controller.piloto.EquipePitStop.EntrouNoPitStopNessaVolta) return;

        if (controller.voltaAtual == 1) // Volta de saída.
        {
            // Todo
        }
        else if (controller.voltaAtual == 2) // Volta de classificação.
        {
            // Todo
        }
        else if (controller.voltaAtual == 3) // Volta de retorno.
        {
            // Todo
        }
    }

    private void ManterEmFaixas()
    {
        // Só funciona fora do pitstop.
        if (controller.piloto.EquipePitStop.EntrouNoPitStopNessaVolta) return;

        // Volta de saída.
        if (controller.voltaAtual == 1)
        {
            // Se for o waypoint de inicio de volta rapida.
            if (controller.waypointAtual.inicioVoltaRapida)
            {
                // Se posiciona na faixa do centro para iniciar a classificação.
                if (controller.faixaAtual != Faixas.Centro)
                {
                    controller.AlternarFaixa(Faixas.Centro, 1.5f);
                }
            }
        }

        // Volta de classificação.
        else if (controller.voltaAtual == 2)
        {
            // Se não estiver no centro
            if (controller.faixaAtual != Faixas.Centro)
            {
                // Vai para o centro.
                controller.AlternarFaixa(Faixas.Centro, 1.5f);
            }
        }

        // Volta de retorno.
        else if (controller.voltaAtual == 3)
        {
            // Se não estiver na direita 
            if (controller.faixaAtual != Faixas.Direita)
            {
                // Vai para direita.
                controller.AlternarFaixa(Faixas.Direita, 1.5f);
            }
        }
    }

    private void IrClassificar()
    {
        // Se for o jogador ou o aliado, retorna.
        if(controller.piloto == PlayerSettings.pilotoAliado || controller.piloto == PlayerSettings.pilotoJogador) return;

        // Se o contador de tempo entre as saidas classificatorias forem <= 0.
        if (controller.tempoEntreSaidasClassificatoria <= 0)
        {
            // Se a sessão já está terminando, fica nos boxes.
            if (RaceManager.Instance.tempoTotalClassificacao <= 150) return;
            // Se o tempo da classificação está acabando e ele ainda não pontuou, ele sai.
            if (RaceManager.Instance.tempoTotalClassificacao <= 180 && controller.tempoUltimaVolta < 5)
            {
                controller.piloto.EquipePitStop.ParadoNoPitStop = false;
                controller.iniciouClassificacao = true;
                RaceManager.Instance.carrosNaPista++;
                return;
            }

            // Se ainda tem tempo, sorteia a chance pra sair.
            int sorteio = Random.Range(0, 100);
            if (sorteio <= 7) // 7% de chance de ir.
            {
                controller.piloto.EquipePitStop.ParadoNoPitStop = false;
                controller.iniciouClassificacao = true;
                RaceManager.Instance.carrosNaPista++;
            }

            controller.tempoEntreSaidasClassificatoria = 5f; // Tempo para o piloto (IA) tentar sair novamente do pit para classificar.
        }
    }

    private void EscolherModoDirecaoEMotor()
    {
        if (!controller.iniciouClassificacao) return;

        if (!controller.piloto.AutoMode) return;

        // Saindo do pit, até a largada.
        if (controller.voltaAtual == 0)
        {
            controller.carro.comandos.SetModoDirecao(Carro.TipoDeConducao.Economia);
            controller.carro.comandos.SetModoMotor(Carro.TipoDeMotor.Economia);
        }

        // Volta de saida. 
        else if (controller.voltaAtual == 1)
        {
            // Se estiver a alguns Waypoints da 2° volta, Inicia volta forte.
            if (controller.faixaAtual == Faixas.Esquerda)
            {
                controller.carro.comandos.SetModoDirecao(Carro.TipoDeConducao.Economia);
                controller.carro.comandos.SetModoMotor(Carro.TipoDeMotor.Economia);
            }
            else if (controller.faixaAtual == Faixas.Centro)
            {
                controller.carro.comandos.SetModoDirecao(Carro.TipoDeConducao.Agressivo);
                controller.carro.comandos.SetModoMotor(Carro.TipoDeMotor.Agressivo);
            }
        }

        // Terceira volta, volta de retorno.
        else if (controller.voltaAtual == 3)
        {
            controller.carro.comandos.SetModoDirecao(Carro.TipoDeConducao.Economia);
            controller.carro.comandos.SetModoMotor(Carro.TipoDeMotor.Economia);
            controller.piloto.EquipePitStop.PitStopNestaVolta = true;
        }
    }

    public void PararNoPitStop()
    {
        // Teleporta para a posição correta do pitstop.
        PistaManager.Instance.TeleportParaPitStop(controller);

        controller.piloto.EquipePitStop.ParadoNoPitStop = true;
        controller.piloto.EquipePitStop.PitStopNestaVolta = false;
        controller.voltaAtual = 0;
        controller.iniciouClassificacao = false;
        controller.estaNaPista = false;
        controller.tempoVoltaAtual = 0;
        RaceManager.Instance.carrosNaPista--;

        // Index do pitstop.
        controller.AvancarProximoWaypoint(controller.waypointAtual.ID);

        // Freia o carro
        controller.potenciaAceleracao = 0f;
        controller.potenciaFreio = 1f;

        // Se tem reparo programado.
        if (controller.piloto.EquipePitStop.ParadoNoPitStop && controller.piloto.EquipePitStop.Reparar)
        {
            StartCoroutine(RealizarPitStop());
        }
    }

    public IEnumerator RealizarPitStop()
    {
        controller.piloto.EquipePitStop.ConsertandoNoPit = true;
        yield return controller.piloto.EquipePitStop.RealizarReparo(controller.carro, this);
    }
}