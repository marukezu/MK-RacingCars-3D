using UnityEngine;

public class CarSimVisual : MonoBehaviour
{
    [Header("== Partes do Veiculo ==")]
    public CarSimVisual_Suspensao suspensao;
    public CarSimVisual_Carroceria carroceria;
    public CarSimVisual_Acessorios acessorios;
    public CarSimVisual_UpdateManager updateManager;

    // Piloto desse carro
    public PilotoSim piloto;

    // Referencia aos Managers
    public SplineManager spline;

    private void Awake()
    {
        suspensao ??= GetComponent<CarSimVisual_Suspensao>();
        carroceria ??= GetComponent<CarSimVisual_Carroceria>();
        acessorios ??= GetComponent<CarSimVisual_Acessorios>();
        updateManager ??= GetComponent<CarSimVisual_UpdateManager>();
    }

    public void AtualizarPosicaoNaPista()
    {
        if (spline == null || piloto == null) return;

        // 1️⃣ Pega posição e rotação da spline
        Vector3 posSpline = spline.GetPosition(piloto.SplineT);
        Quaternion rotSpline = spline.GetRotation(piloto.SplineT);

        // Mantem a altura atual (Y) do carro para não sobrescrever
        posSpline.y = transform.position.y;

        // 2️⃣ Aplica lateral offset do piloto
        Vector3 right = spline.GetRight(piloto.SplineT);
        posSpline += right * piloto.mod_PosLateral.offsetReal * 2f;

        // 3️⃣ Atualiza posição XZ
        transform.position = new Vector3(posSpline.x, transform.position.y, posSpline.z);

        // 4️⃣ Rotação base alinhada com spline
        Vector3 forward = Vector3.ProjectOnPlane(rotSpline * Vector3.forward, Vector3.up);
        transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
    }
}
