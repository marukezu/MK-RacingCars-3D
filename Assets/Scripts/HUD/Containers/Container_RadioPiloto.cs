using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Container_RadioPiloto : MonoBehaviour
{
    public Piloto piloto;

    public Image IMG_Piloto;
    public Text Txt_PilotoNome;
    public Text Txt_PilotoFala;

    private void Start()
    {
        // Destroi depois de alguns segundos.
        Destroy(gameObject, 5f);

        // Se a mensagem for enviada pelo Piloto 1.
        if (piloto.PilotoNumero == Piloto.NumeroDoPiloto.Um)
        {
            IMG_Piloto.sprite = PlayerSettings.pilotoJogador.PilotoSprite;
            Txt_PilotoNome.text = PlayerSettings.pilotoJogador.PilotoNome;
            Txt_PilotoFala.text = PlayerSettings.pilotoJogador.GetFalaPeloEstado();
        }

        // Se a mensagem for enviada pelo Piloto 2.
        else if (piloto.PilotoNumero == Piloto.NumeroDoPiloto.Dois)
        {
            IMG_Piloto.sprite = PlayerSettings.pilotoAliado.PilotoSprite;
            Txt_PilotoNome.text = PlayerSettings.pilotoAliado.PilotoNome;
            Txt_PilotoFala.text = PlayerSettings.pilotoAliado.GetFalaPeloEstado();
        }
    }
}
