using UnityEngine;
using static PilotoController;

public class Waypoint : MonoBehaviour
{
    public int ID;

    [Header("====== PISTA PRINCIPAL ======")]
    public Faixas faixa;
    public float velocidadeWaypoint;
    public bool curva; // se esse waypoint está em uma curva
    public bool waypointLargada; // se esse waypoint é o waypoint da largada/chegada.
    public bool entradaPitStop; // Usado na pista de corrida, pra "trocar" o waypoint para o do pitstop.
    public bool inicioVoltaRapida; // para classificação, se esse ponto for o inicio de volta rápida o piloto ativará o máximo de uso do carro (pneus e motor).
    public bool saidaPitStop; // Se esse waypoint é o waypoint final do pitstop

    [Header("====== PIT-STOP ======")]
    public bool waypointPitStop; // Se esse waypoint é um waypoint de pitstop
    public bool entradaDeGaragem; // Se for entrada de uma garagem (parada de pit).
    public bool paradaPitStop;  // Simboliza a parada do pitstop, onde o carro para para arrumar.
    public bool ultimoWaypointPitStop; // Ultimo waypoint do pitstop, depois desse, troca para pista.


    [Header("====== PIT-STOP - Waypoint Entrada de Garagem ======")]
    public bool entrada_pit__piloto1;
    public bool entrada_pit__piloto2;
    public bool entrada_pit__piloto3;
    public bool entrada_pit__piloto4;
    public bool entrada_pit__piloto5;
    public bool entrada_pit__piloto6;
    public bool entrada_pit__piloto7;
    public bool entrada_pit__piloto8;

    public float GetWaypointSpeed(PilotoController pilotoController)
    {
        return (velocidadeWaypoint * 1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        // ===========
        if (other.gameObject.CompareTag("Carro"))
        {
            // Pega o piloto no trigger.
            PilotoController controller = other.GetComponent<Carro>().pilotoController;

            if (RaceManager.Instance.tipoDeCorrida == RaceManager.TipoDeCorrida.Classificacao)
            {
                MarcarPassagemClassificacao(controller);
            }

            else if (RaceManager.Instance.tipoDeCorrida == RaceManager.TipoDeCorrida.Corrida)
            {
                MarcarPassagemCorrida(controller);
            }
        }
    }


    private void MarcarPassagemClassificacao(PilotoController controller)
    {
        if (controller.piloto.EquipePitStop.ParadoNoPitStop) return;

        // Checar se deve entrar no pitstop nesse waypoint.
        if (controller.piloto.EquipePitStop.PitStopNestaVolta && entradaPitStop)
        {
            controller.AlternarFaixa(Faixas.PitStop, 1.5f);
            controller.piloto.EquipePitStop.EntrouNoPitStopNessaVolta = true;
            return;
        }

        // Se estou dentro do pitstop.
        if (waypointPitStop && controller.piloto.EquipePitStop.EntrouNoPitStopNessaVolta)
        {

            if (entradaDeGaragem)
            {
                // Se for a entrada do pitstop desse carro. (entrada_pit__piloto1)
                if (ChecarEntradaPitStop(controller))
                {
                    controller.AvancarProximoWaypoint(ID);
                    return;
                }

                controller.AvancarProximoWaypoint(ID + 2);
                return;
            }   

            // Controla o ponto de parada.
            if (paradaPitStop)
            {
                if (controller.piloto.EquipePitStop.PitStopNestaVolta)
                {
                    controller.controllerClassificacao.PararNoPitStop();
                    return;
                }
            }

            // Se for o ultimo waypoint do pitstop.
            if (ultimoWaypointPitStop)
            {
                // Altera para o waypoint da pista.
                controller.AlternarFaixa(Faixas.Esquerda, 1.5f);

                // Seta o index correto do proximo waypoint
                foreach (Waypoint waypoint in PistaManager.Instance.GetListWaypointsEsq())
                {
                    if (waypoint.saidaPitStop)
                    {
                        controller.AvancarProximoWaypoint(waypoint.ID - 1);
                        break;
                    }
                }                  

                return;
            }

            controller.AvancarProximoWaypoint(ID);
            return;
        }

        // Avança waypoint
        controller.AvancarProximoWaypoint(ID);
        controller.checkpointsPassados++;

        // Se é o waypoint da largada
        if (waypointLargada)
        {
            // Atualiza o gasto de combustível por volta.
            controller.carro.carroceria.AtualizaConsumoPorVolta();

            // Se ele acabou de sair do pitstop.
            if (controller.piloto.EquipePitStop.EntrouNoPitStopNessaVolta)
            {
                // Desativa o acabou de sair do pit.
                controller.piloto.EquipePitStop.EntrouNoPitStopNessaVolta = false;

                // Envia a mensagem por rádio.
                // PanelManager.Instance.InstanciarNovaMensagemDeRadio(controller.piloto, Piloto.EstadoDoPiloto.VoltaSaida);
            }

            // Se fez a volta mais rapida.
            if ((controller.tempoVoltaAtual < controller.tempoVoltaMaisRapida || controller.tempoVoltaMaisRapida == 0f) && controller.voltaAtual > 0)
            {
                controller.tempoVoltaMaisRapida = controller.tempoVoltaAtual;
            }

            // Avança 1 volta.
            controller.tempoTotalVoltas += controller.tempoUltimaVolta;
            controller.voltaAtual++;
            controller.tempoUltimaVolta = controller.tempoVoltaAtual;
            controller.tempoVoltaAtual = 0;


            // Colocado após, para ser computado a volta atual antes da verificação.
            // Acabou de iniciar a volta rápida.
            if (controller.voltaAtual == 2)
            {
                // Envia a mensagem por rádio.
                // PanelManager.Instance.InstanciarNovaMensagemDeRadio(controller.piloto, Piloto.EstadoDoPiloto.VoltaRapida);
            }

            if (controller.voltaAtual == 3)
            {
                // Envia a mensagem por rádio.
                // PanelManager.Instance.InstanciarNovaMensagemDeRadio(controller.piloto, Piloto.EstadoDoPiloto.VoltaEntrada);
            }
        }     
    }

    private void MarcarPassagemCorrida(PilotoController controller)
    {
        if (controller.piloto.EquipePitStop.ParadoNoPitStop) return;

        // Checar se deve entrar no pitstop nesse waypoint.
        if (controller.piloto.EquipePitStop.PitStopNestaVolta && entradaPitStop)
        {
            controller.AlternarFaixa(Faixas.PitStop, 1f);
            controller.piloto.EquipePitStop.EntrouNoPitStopNessaVolta = true;
            return;
        }

        // Se estou dentro do pitstop.
        if (waypointPitStop && controller.piloto.EquipePitStop.EntrouNoPitStopNessaVolta)
        {

            if (entradaDeGaragem)
            {
                // Se for a entrada do pitstop desse carro. (entrada_pit__piloto1)
                if (ChecarEntradaPitStop(controller))
                {
                    controller.AvancarProximoWaypoint(ID);
                    return;
                }

                controller.AvancarProximoWaypoint(ID + 2);
                return;
            }

            // Controla o ponto de parada.
            if (paradaPitStop)
            {
                if (controller.piloto.EquipePitStop.PitStopNestaVolta)
                {
                    controller.controllerCorrida.PararNoPitStop();
                    return;
                }
            }

            // Se for o ultimo waypoint do pitstop.
            if (ultimoWaypointPitStop)
            {
                // Altera para o waypoint da pista.
                controller.AlternarFaixa(Faixas.Esquerda, 1.5f);

                // Seta o index correto do proximo waypoint
                foreach (Waypoint waypoint in PistaManager.Instance.GetListWaypointsEsq())
                {
                    if (waypoint.saidaPitStop)
                    {
                        controller.AvancarProximoWaypoint(waypoint.ID - 1);
                        break;
                    }
                }

                // Adiciona os checkpoints que o carro não passa por ter entrado no pitstop.
                controller.checkpointsPassados += PistaManager.Instance.checkpointsPerdidosPitStop;
                return;
            }

            controller.AvancarProximoWaypoint(ID);
            return;
        }

        // Avança waypoint
        controller.AvancarProximoWaypoint(ID);
        controller.checkpointsPassados++;

        // Se é o waypoint da largada
        if (waypointLargada)
        {
            // Atualiza o gasto de combustível por volta.
            controller.carro.carroceria.AtualizaConsumoPorVolta();

            // Se ele acabou de sair do pitstop.
            if (controller.piloto.EquipePitStop.EntrouNoPitStopNessaVolta)
            {
                // Desativa o acabou de sair do pit.
                controller.piloto.EquipePitStop.EntrouNoPitStopNessaVolta = false;
            }

            // Se fez a volta mais rapida.
            if ((controller.tempoVoltaAtual < controller.tempoVoltaMaisRapida || controller.tempoVoltaMaisRapida == 0f) && controller.voltaAtual > 0)
            {
                controller.tempoVoltaMaisRapida = controller.tempoVoltaAtual;
            }

            // Avança 1 volta.
            controller.tempoTotalVoltas += controller.tempoUltimaVolta;
            controller.voltaAtual++;
            controller.tempoUltimaVolta = controller.tempoVoltaAtual;
            controller.tempoVoltaAtual = 0;
        }
    }

    private bool ChecarEntradaPitStop(PilotoController controller)
    {
        if (controller.piloto.ID == 1)
        {
            if (entrada_pit__piloto1)
                return true;
        }
        else if (controller.piloto.ID == 2)
        {
            if (entrada_pit__piloto2)
                return true;
        }
        else if (controller.piloto.ID == 3)
        {
            if (entrada_pit__piloto3)
                return true;
        }
        else if (controller.piloto.ID == 4)
        {
            if (entrada_pit__piloto4)
                return true;
        }
        else if (controller.piloto.ID == 5)
        {
            if (entrada_pit__piloto5)
                return true;
        }
        else if (controller.piloto.ID == 6)
        {
            if (entrada_pit__piloto6)
                return true;
        }
        else if (controller.piloto.ID == 7)
        {
            if (entrada_pit__piloto7)
                return true;
        }
        else if (controller.piloto.ID == 8)
        {
            if (entrada_pit__piloto8)
                return true;
        }

        return false;
    }
}
