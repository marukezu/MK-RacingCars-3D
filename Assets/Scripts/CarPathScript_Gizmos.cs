using UnityEngine;

public class CarPathScript_Gizmos : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        // Obtém os waypoints como filhos do objeto pai
        Transform[] waypoints = GetComponentsInChildren<Transform>();

        // Verifica se há pelo menos dois waypoints para desenhar uma linha
        if (waypoints.Length >= 2)
        {
            // Define a cor da linha do gizmo
            Gizmos.color = Color.blue;

            // Desenha uma linha entre os waypoints
            for (int i = 1; i < waypoints.Length; i++)
            {
                Gizmos.DrawLine(waypoints[i - 1].position, waypoints[i].position);
                Gizmos.DrawWireSphere(waypoints[i].position, 0.3f);
            }
        }
    }
}
