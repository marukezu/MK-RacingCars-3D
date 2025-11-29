using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carro_DetectorColisao : MonoBehaviour
{
    public Carro carro;

    [Header("====== Transforms - Pontos de Raycast ======")]
    public Transform raycast_Frontal;

    public Transform raycast_Direita_Frontal;
    public Transform raycast_Direita_Central;
    public Transform raycast_Direita_Traseira;

    public Transform raycast_Esquerda_Frontal;
    public Transform raycast_Esquerda_Central;
    public Transform raycast_Esquerda_Traseira;

    [Header("====== Carros - Colisões ======")]
    public Carro carro_Frontal;
    public Carro carro_Direita_Frontal;
    public Carro carro_Direita_Central;
    public Carro carro_Direita_Traseira;

    public Carro carro_Esquerda_Frontal;
    public Carro carro_Esquerda_Central;
    public Carro carro_Esquerda_Traseira;

    [Header("====== Booleans - Colisões ======")]
    public bool colisaoFrontal;

    public bool colisao_Direita_Frontal;
    public bool colisao_Direita_Centro;
    public bool colisao_Direita_Traseira;

    public bool colisao_Esquerda_Frontal;
    public bool colisao_Esquerda_Centro;
    public bool colisao_Esquerda_Traseira;

    private void Update()
    {
        DetectarColisao();
    }

    private void DetectarColisao()
    {
        RaycastHit hit;

        // Teste de colisão frontal
        if (Physics.Raycast(raycast_Frontal.position, transform.forward, out hit, 10f) && hit.collider.CompareTag("Carro"))
        {
            colisaoFrontal = true;
            carro_Frontal = hit.collider.GetComponent<Carro>(); // Preenche o componente Carro
        }
        else
        {
            colisaoFrontal = false;
            carro_Frontal = null; // Reseta se nenhuma colisão for detectada
        }

        // Teste de colisão à direita - Frontal
        if (Physics.Raycast(raycast_Direita_Frontal.position, transform.right, out hit, 10f) && hit.collider.CompareTag("Carro"))
        {
            colisao_Direita_Frontal = true;
            carro_Direita_Frontal = hit.collider.GetComponent<Carro>();
        }
        else
        {
            colisao_Direita_Frontal = false;
            carro_Direita_Frontal = null;
        }

        // Teste de colisão à direita - Central
        if (Physics.Raycast(raycast_Direita_Central.position, transform.right, out hit, 10f) && hit.collider.CompareTag("Carro"))
        {
            colisao_Direita_Centro = true;
            carro_Direita_Central = hit.collider.GetComponent<Carro>();
        }
        else
        {
            colisao_Direita_Centro = false;
            carro_Direita_Central = null;
        }

        // Teste de colisão à direita - Traseira
        if (Physics.Raycast(raycast_Direita_Traseira.position, transform.right, out hit, 10f) && hit.collider.CompareTag("Carro"))
        {
            colisao_Direita_Traseira = true;
            carro_Direita_Traseira = hit.collider.GetComponent<Carro>();
        }
        else
        {
            colisao_Direita_Traseira = false;
            carro_Direita_Traseira = null;
        }

        // Teste de colisão à esquerda - Frontal
        if (Physics.Raycast(raycast_Esquerda_Frontal.position, -transform.right, out hit, 10f) && hit.collider.CompareTag("Carro"))
        {
            colisao_Esquerda_Frontal = true;
            carro_Esquerda_Frontal = hit.collider.GetComponent<Carro>();
        }
        else
        {
            colisao_Esquerda_Frontal = false;
            carro_Esquerda_Frontal = null;
        }

        // Teste de colisão à esquerda - Central
        if (Physics.Raycast(raycast_Esquerda_Central.position, -transform.right, out hit, 10f) && hit.collider.CompareTag("Carro"))
        {
            colisao_Esquerda_Centro = true;
            carro_Esquerda_Central = hit.collider.GetComponent<Carro>();
        }
        else
        {
            colisao_Esquerda_Centro = false;
            carro_Esquerda_Central = null;
        }

        // Teste de colisão à esquerda - Traseira
        if (Physics.Raycast(raycast_Esquerda_Traseira.position, -transform.right, out hit, 10f) && hit.collider.CompareTag("Carro"))
        {
            colisao_Esquerda_Traseira = true;
            carro_Esquerda_Traseira = hit.collider.GetComponent<Carro>();
        }
        else
        {
            colisao_Esquerda_Traseira = false;
            carro_Esquerda_Traseira = null;
        }

        // Debug para visualização no Editor
        DebugRays();
    }


    private void DebugRays()
    {
        // Frontal
        Debug.DrawRay(raycast_Frontal.position, transform.forward * 10f, Color.red);

        // Direita
        Debug.DrawRay(raycast_Direita_Frontal.position, transform.right * 10f, Color.blue);
        Debug.DrawRay(raycast_Direita_Central.position, transform.right * 10f, Color.blue);
        Debug.DrawRay(raycast_Direita_Traseira.position, transform.right * 10f, Color.blue);

        // Esquerda
        Debug.DrawRay(raycast_Esquerda_Frontal.position, -transform.right * 10f, Color.green);
        Debug.DrawRay(raycast_Esquerda_Central.position, -transform.right * 10f, Color.green);
        Debug.DrawRay(raycast_Esquerda_Traseira.position, -transform.right * 10f, Color.green);
    }
}
