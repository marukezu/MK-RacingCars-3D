using System.Collections;
using UnityEngine;
using static PilotoController;

// =============================================================================================================
// =================== CLASSE RESPONSÁVEL PELA LÓGICA DE CONDUÇAO DO CARRO DURANTE A CORRIDA ===================
// =============================================================================================================

public class PilotoController_Corrida : MonoBehaviour
{
    public PilotoController controller;

    private void Update()
    {
        if (RaceManager.Instance.tipoDeCorrida == RaceManager.TipoDeCorrida.Corrida)
            ComportamentoIA();
    }

    // =============================================================================================================
    // ================================================== CORE =====================================================
    // =============================================================================================================
    private void ComportamentoIA()
    {
        if (controller.faixaAtual == Faixas.PitStop) // Se estiver seguindo waypoints de pitstop
        {
            ConduzirPeloPitStop();
        }

        else if (controller.faixaAtual != Faixas.PitStop) // Se NÃO estiver seguindo waypoints de pitstop
        {
            ConduzirPelaPista();
        }
    }

    // =============================================================================================================
    // ========================================= COMPORTAMENTO CONDUÇÃO ============================================
    // =============================================================================================================
    private void ConduzirPelaPista()
    {
        // Responsável pela inteligência ao acelerar.
        AcelerarFrear();

        // Responsável pela inteligência ao virar o volante.
        VirarVeiculo();
        
        // Responsável por manter o carro na faixa do centro.
        ManterNoCentro();

        // Responsável pela inteligência para ultrapassar o carro a frente.
        Ultrapassar();

        // Escolhe o fator de direção e motor na pista se o Auto_Mode do piloto estiver ativado.
        EscolherFatorDirecao();
        EscolherFatorMotor();

        // Verifica niveis de Pneu e Combustivel se é necessário realizar uma parada.
        VerificarSeNecessitaPararNoPitStop();

        // Atualiza comandos do Carro.
        controller.carro.comandos.CarroAcelerar(controller.potenciaAceleracao);
        controller.carro.comandos.CarroFrear(controller.potenciaFreio);
        controller.carro.comandos.CarroVirar(controller.potenciaGiroDirecao);
    }
    private void ConduzirPeloPitStop()
    {

        // Responsável pela inteligência ao acelerar/frear  ======== ****
        AcelerarFrear();

        // Responsável pela inteligência ao virar o volante  ======== ****
        VirarVeiculo();

        // Escolhe modos de motor/direção.
        EscolherFatorDirecao();
        EscolherFatorMotor();

        // Atualiza comandos do Carro.
        controller.carro.comandos.CarroAcelerar(controller.potenciaAceleracao);
        controller.carro.comandos.CarroFrear(controller.potenciaFreio);
        controller.carro.comandos.CarroVirar(controller.potenciaGiroDirecao);

    }

    // ==================================================================================================================
    // ===================================== Comportamentos de Condução - (I.A.) ========================================
    // ==================================================================================================================
    private void AcelerarFrear()
    {
        float velocidadeCarro = controller.carro.carroceria.velocidadeKMAtual;
        float velocidadeWaypoint = 0f;

        // Se o carro a frente estiver próximo ganha 5% de bonus de velocidade no waypoint. (Facilitar Ultrapassagens)
        if (controller.carroFrente != controller.carro && controller.distanciaCarroFrente < 25f)
        {
            velocidadeWaypoint = controller.waypointAtual.GetWaypointSpeed(controller) + 5f;
        }
        else
        {
            velocidadeWaypoint = controller.waypointAtual.GetWaypointSpeed(controller);
        }

        float novapotenciaAceleracao = 0f;
        float novapotenciaFrenagem = 0f;

        // ===============
        // Aceleração, redução de Frenagem

        if (velocidadeCarro < velocidadeWaypoint)
        {
            novapotenciaAceleracao = 1f;

            // Fator de "o quanto esta virando o volante".
            float fatorGiroVolante = Mathf.Abs(controller.potenciaGiroDirecao);

            // Adiciona a potencia dependendo do esterco do volante.
            novapotenciaAceleracao = Mathf.Clamp01((Mathf.Clamp(1f - ((fatorGiroVolante * 0.5f)), 0.7f, 1f)));

            // Fator de inclinação - subida ou descida.
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
                if (velocidadeWaypoint * 0.8f < velocidadeCarro)
                    novapotenciaAceleracao *= 0.3f;
            }
        }

        // ================
        // Frenagem, Redução de aceleração

        // Se o carro estiver mais rapido que a velocidade imposta pelo waypoint.
        if (velocidadeCarro > velocidadeWaypoint)
        {
            // Calcular a razão de velocidade
            float razaoVelocidade = Mathf.Clamp01((velocidadeCarro / velocidadeWaypoint) - 0.5f);

            // Freia com limite.
            novapotenciaFrenagem = Mathf.Clamp(razaoVelocidade, 0, 0.5f);
        }
        else
        {
            controller.potenciaFreio = 0f;
        }

        // Se o carro a frente já existir e estiver na mesma faixa que eu
        if (controller.carroFrente != null && controller.carroFrente.pilotoController.faixaAtual == controller.faixaAtual)
        {
            // Checa se tem carro a frente próximo.
            if (controller.distanciaCarroFrente <= 1.5f)
            {
                // Reduzir a aceleração com base na distância.
                novapotenciaAceleracao *= 0.9f;
            }
            else if (controller.distanciaCarroFrente <= 4f)
            {
                // Reduzir a aceleração com base na distância.
                novapotenciaAceleracao *= 0.95f;
            }
            else if (controller.distanciaCarroFrente <= 8f)
            {
                // Reduzir a aceleração com base na distância.
                novapotenciaAceleracao *= 0.975f;
            }
        }
        
        
        // Pega referência das Suspensões do veiculo
        controller.carro.carroceria.suspensaoFL.wheelCollider.brakeTorque = 0f;
        controller.carro.carroceria.suspensaoFR.wheelCollider.brakeTorque = 0f;
        controller.carro.carroceria.suspensaoRL.wheelCollider.brakeTorque = 0f;
        controller.carro.carroceria.suspensaoRR.wheelCollider.brakeTorque = 0f;

        // Se estiver parado no pitstop ou a largada ainda não tiver sido autorizada
        if (controller.piloto.EquipePitStop.ParadoNoPitStop || !controller.foiDadoLargada)
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

        // Calcular a distância até o próximo waypoint
        float distanceToWaypoint = dir.magnitude;

        // Definir a distância máxima para iniciar a virada suave (pode ser ajustada conforme necessário)
        float maxTurnDistance = 5f;

        // Calcular a intensidade da rotação com base na distância
        float turnIntensity = Mathf.Clamp01(distanceToWaypoint / maxTurnDistance);

        // Ajustar a potência do giro com base na intensidade calculada
        controller.potenciaGiroDirecao = Mathf.Clamp((dir.x / dir.magnitude) * turnIntensity, -1f, 1f);
    }

    private void Ultrapassar()
    {
        // Verifica condições básicas
        if (controller.carroFrente == null || !controller.foiDadoLargada || controller.trocandoFaixa)
            return;

        // Se o veiculo estiver entre o primeiro e o terceiro Waypoint e estiver na PRIMEIRA volta (Largada)
        if (controller.waypointIndex <= 2 && controller.voltaAtual <= 1)
            return;

        // Se o carro a frente está muito longe (é ignorado)
        if (controller.RetornaDistanciaCarroFrente() > 3.5f)
            return;
        
        // Se eu não estiver mais rápido que ele (não devo trocar de faixa)
        if ((controller.carro.carroceria.velocidadeKMAtual - controller.carroFrente.carroceria.velocidadeKMAtual) < 0.5f)
            return;

        // Se ele não estiver na mesma faixa que eu (não há necessidade em alterar faixa)
        if (controller.carroFrente.pilotoController.faixaAtual != controller.faixaAtual)
            return;

        // Verificar qual faixa é a melhor opção
        Faixas faixaEscolhida = controller.EscolherMelhorFaixa();

        // Se já estou nela, não troca
        if (faixaEscolhida == controller.faixaAtual)
            return;

        // Antes de trocar, verifica segurança (sua função atual)
        if (controller.VerificarSePodeTrocarFaixa(faixaEscolhida))
        {
            controller.AlternarFaixa(faixaEscolhida, 2.5f);
            controller.AvancarProximoWaypoint(controller.waypointAtual.ID + 1);
        }
    }

    private void ManterNoCentro()
    {
        // Se o veiculo estiver entre o primeiro e o terceiro Waypoint e estiver na PRIMEIRA volta (Largada)
        if (controller.waypointIndex <= 2 && controller.voltaAtual <= 1)
            return;

    }

    // =============================================================================================================
    // ============================ Controle de Potência do carro (MOTOR e DIREÇÃO) =================================
    // =============================================================================================================
    private void EscolherFatorDirecao()
    {
        if (!controller.piloto.AutoMode) return;

        // Modo de direção PADRAO.
        controller.carro.comandos.SetModoDirecao(Carro.TipoDeConducao.Equilibrado);

        // SE ESTA NO PITSTOP.
        if (controller.faixaAtual == Faixas.PitStop)
        {
            controller.carro.comandos.SetModoDirecao(Carro.TipoDeConducao.Economia);
            return;
        }

        // Por volta.
        if (controller.voltaAtual <= 1)
        {
            controller.carro.comandos.SetModoDirecao(Carro.TipoDeConducao.Agressivo);
            return;
        }

        // Pelo desgasto dos pneus.
        if (controller.carro.carroceria.GetCondicaoMediaPneus() <= 20)
        {
            controller.carro.comandos.SetModoDirecao(Carro.TipoDeConducao.Economia);
            return;
        }
        else if (controller.carro.carroceria.GetCondicaoMediaPneus() <= 35)
        {
            controller.carro.comandos.SetModoDirecao(Carro.TipoDeConducao.Equilibrado);
            return;
        }

        // Se o carro atrás está colando.
        if (controller.distanciaCarroAtras <= 7)
        {
            controller.carro.comandos.SetModoDirecao(Carro.TipoDeConducao.Agressivo);
            return;
        }

        // Se o carro a frente está a x metros, próximo.
        if (controller.distanciaCarroFrente <= 15)
        {
            controller.carro.comandos.SetModoDirecao(Carro.TipoDeConducao.Forte);
            return;
        }
    }

    private void EscolherFatorMotor()
    {
        if (!controller.piloto.AutoMode) return;

        // Modo padrão.
        controller.carro.comandos.SetModoMotor(Carro.TipoDeMotor.Equilibrado);

        // SE ESTA NO PITSTOP.
        if (controller.faixaAtual == Faixas.PitStop)
        {
            controller.carro.comandos.SetModoMotor(Carro.TipoDeMotor.Economia);
            return;
        }

        // Por volta.
        if (controller.voltaAtual <= 1)
        {
            controller.carro.comandos.SetModoMotor(Carro.TipoDeMotor.Agressivo);
            return;
        }

        // Pela falta de combustivel.
        if (controller.carro.carroceria.combustivelAtual <= 25)
        {
            controller.carro.comandos.SetModoMotor(Carro.TipoDeMotor.Equilibrado);
            return;
        }

        // Se o carro de trás está a x metros, próximo.
        if (controller.distanciaCarroAtras <= 3)
        {
            controller.carro.comandos.SetModoMotor(Carro.TipoDeMotor.Agressivo);
            return;
        }

        // Se o carro a frente está a x metros, próximo.
        if (controller.distanciaCarroFrente <= 7)
        {
            controller.carro.comandos.SetModoMotor(Carro.TipoDeMotor.Forte);
            return;
        }
    }

    // =============================================================================================================
    // ==================================== Estratégia de PitStop do Piloto ========================================
    // =============================================================================================================
    public void VerificarSeNecessitaPararNoPitStop()
    {
        // Se é o jogador ou o piloto aliado retorna.
        if (controller.piloto.ID == 1 || controller.piloto.ID == 2) return;

        // Se já tem uma parada programada, retorna.
        if (controller.piloto.EquipePitStop.PitStopNestaVolta) return;

        // ===========================================

        // Se a condição dos pneus está próxima de começar causar queda de desempenho.
        if (controller.carro.carroceria.GetCondicaoMediaPneus() < 30)
        {
            // Se estiver faltando mais de 3 voltas, ele para.
            if (RaceManager.Instance.voltasTotais - controller.voltaAtual > 2)
            {
                controller.piloto.EquipePitStop.PitStopNestaVolta = true;
                controller.piloto.EquipePitStop.VaiTrocarPneu = true;
                controller.piloto.EquipePitStop.Reparar = true;
            }
        }

        // Se a condição do combustivel está critica, prestes a terminar.
        if (controller.carro.carroceria.combustivelAtual <= 20)
        {
            // Se estiver faltando mais de 3 voltas, ele para.
            if (RaceManager.Instance.voltasTotais - controller.voltaAtual > 2)
            {
                controller.piloto.EquipePitStop.PitStopNestaVolta = true;
                controller.piloto.EquipePitStop.VaiTrocarPneu = true;
                controller.piloto.EquipePitStop.Reparar = true;
            }
        }

        // ===========================================
        // Seleção de pneus.

        // Se o jogador for mais forte que eu.
        if (PlayerSettings.pilotoJogador.HabilidadeAceleracao >= controller.piloto.HabilidadeAceleracao)
        {
            // Se ainda tiver pneus macios.
            if (controller.piloto.PneuMacioQuantidade > 0)
            {
                controller.piloto.EquipePitStop.TipoPneu = Pneu.TipoPneu.Macio;
            }
            // Se ainda tiver pneus medios.
            else if (controller.piloto.PneuMedioQuantidade > 0)
            {
                controller.piloto.EquipePitStop.TipoPneu = Pneu.TipoPneu.Medio;
            }
        }
        // Se o jogador for mais fraco que eu.
        else
        {
            // Se ainda tiver pneus medios.
            if (controller.piloto.PneuMedioQuantidade > 0)
            {
                controller.piloto.EquipePitStop.TipoPneu = Pneu.TipoPneu.Medio;
            }
            // Se ainda tiver pneus macios.
            else if (controller.piloto.PneuMacioQuantidade > 0)
            {
                controller.piloto.EquipePitStop.TipoPneu = Pneu.TipoPneu.Macio;
            }
        }
    }

    // Ao entrar na parada do seu pitstop.
    public void PararNoPitStop()
    {
        // Teleporta para a posição correta do pitstop.
        PistaManager.Instance.TeleportParaPitStop(controller);

        controller.piloto.EquipePitStop.ParadoNoPitStop = true;
        controller.piloto.EquipePitStop.PitStopNestaVolta = false;
        controller.estaNaPista = false;
        RaceManager.Instance.carrosNaPista--;

        // Index do pitstop.
        controller.AvancarProximoWaypoint(controller.waypointAtual.ID);

        // Freia o carro.
        controller.potenciaAceleracao = 0f;
        controller.potenciaFreio = 1f;
        StartCoroutine(RealizarPitStop());
    }

    // Se for para fazer reparo no carro.
    public IEnumerator RealizarPitStop()
    {
        controller.piloto.EquipePitStop.ConsertandoNoPit = true;
        yield return controller.piloto.EquipePitStop.RealizarReparo(controller.carro, this);

        controller.piloto.EquipePitStop.ParadoNoPitStop = false;
        controller.estaNaPista = true;
        RaceManager.Instance.carrosNaPista++;
    }
}
