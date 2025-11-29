using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class Painel_RACE_Painel_Placar : Panel
{
    public override PanelType Type => PanelType.RACE_PLACAR;

    public GameObject Painel_Pilotos;
    public Text Txt_voltaAtual_Total;

    private List<Container_Painel_Placar> Piloto_Containers = new List<Container_Painel_Placar>();

    public override void AbrirPainel(object param1 = null, object param2 = null, object param3 = null)
    {
        base.AbrirPainel(param1, param2, param3);

        if (PlayerSettings.campeonato != null)
        {
            // Inicia os containers de cada piloto.
            foreach (Piloto piloto in PlayerSettings.campeonato.pilotosParticipantes)
            {
                GameObject novoContainer = null;

                // Se for o jogador ou o piloto aliado, instancia um placar diferenciado.
                if (piloto == PlayerSettings.pilotoJogador || piloto == PlayerSettings.pilotoAliado)
                {
                    novoContainer = ContainerManager.Instance.InstantiateAndReturnContainer(ContainerManager.ContainerType.PANEL_PLACAR_PILOTOINFO_B, Painel_Pilotos);
                }
                else
                {
                    novoContainer = ContainerManager.Instance.InstantiateAndReturnContainer(ContainerManager.ContainerType.PANEL_PLACAR_PILOTOINFO_A, Painel_Pilotos);
                }

                // Ajusta o script do UI_Container_PlacarPiloto.
                Container_Painel_Placar novoContainerScript = novoContainer.GetComponent<Container_Painel_Placar>();
                novoContainerScript.controller = piloto.Carro.pilotoController;
                Piloto_Containers.Add(novoContainerScript);
            }
        }
    }

    public override void AtualizarPainel()
    {
        base.AtualizarPainel();

        AtualizarClassificacao();
    }

    private void AtualizarClassificacao()
    {
        // Atualiza a ordem dos UI_Containers para corrida (Baseada na posição).
        if (RaceManager.Instance.tipoDeCorrida == RaceManager.TipoDeCorrida.Corrida)
        {
            // Ordenar os containers com base na posição dos pilotos.
            Piloto_Containers = Piloto_Containers
            .OrderBy(container => container.controller.carro.pilotoController.posicaoAtual)
            .ToList();

            // Atualizar a hierarquia dos objetos no painel.
            for (int i = 0; i < Piloto_Containers.Count; i++)
            {
                Piloto_Containers[i].transform.SetSiblingIndex(i);
            }

            Txt_voltaAtual_Total.text = Piloto_Containers[0].controller.voltaAtual.ToString() + "/" + RaceManager.Instance.voltasTotais;
        }
        // Atualiza a ordem dos UI_Containers para classificação (Baseada na volta mais rápida).
        else if (RaceManager.Instance.tipoDeCorrida == RaceManager.TipoDeCorrida.Classificacao)
        {
            // Ordenar os containers com base na posição dos pilotos.
            Piloto_Containers = Piloto_Containers
            .OrderBy(container => container.controller.carro.pilotoController.tempoVoltaMaisRapida)
            .ToList();

            // Atualizar a hierarquia dos objetos no painel.
            for (int i = 0; i < Piloto_Containers.Count; i++)
            {
                Piloto_Containers[i].transform.SetSiblingIndex(i);
            }

            Txt_voltaAtual_Total.text = "N/A";
        }
    }
}
