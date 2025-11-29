using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    // Controle da Câmera.
    public CinemachineFreeLook mainCamera;
    public float velocidadePerseguicaoCamera = 0.5f;

    // Bools de controle.
    [HideInInspector] public bool podeTrocarCamera = false;
    private bool controleCameraAtivado = false; // Para "rodar" a camera e ter o visual em volta.

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Update()
    {
        AtualizaBotoesCamera();
    }

    public void Inicializar()
    {
        // Pega a referência do piloto em foco
        GameManager.Instance.SetPilotoEmFoco(PlayerSettings.pilotoJogador);

        // Inicializa camera.
        podeTrocarCamera = true;

        // Ajusta a velocidade de perseguição da camera.
        for (int i = 0; i < 3; i++)
        {
            var rig = mainCamera.GetRig(i);
            if (rig != null)
            {
                var transposer = rig.GetCinemachineComponent<CinemachineTransposer>();
                if (transposer != null)
                {
                    transposer.m_XDamping = velocidadePerseguicaoCamera;
                    transposer.m_YDamping = velocidadePerseguicaoCamera;
                    transposer.m_ZDamping = velocidadePerseguicaoCamera;
                }
            }
        }
    }

    // =============================================================================================================
    // ================================================= Câmera ====================================================
    // =============================================================================================================
    public CinemachineFreeLook GetCamera()
    {
        return mainCamera;
    }

    public void AtualizarCamera(Carro carro)
    {
        if (!podeTrocarCamera && carro == null) return;

        mainCamera.LookAt = carro.gameObject.transform;
        mainCamera.Follow = carro.gameObject.transform;
    }

    public void AtualizaBotoesCamera()
    {
        // Verifica se o botão direito do mouse está sendo pressionado
        if (Input.GetMouseButtonDown(1)) // O botão direito do mouse tem o código 1
        {
            controleCameraAtivado = true;
        }

        // Verifica se o botão direito do mouse foi solto
        if (Input.GetMouseButtonUp(1))
        {
            controleCameraAtivado = false;
        }

        if (controleCameraAtivado)
        {
            mainCamera.m_XAxis.m_InputAxisName = "Mouse X";
            mainCamera.m_XAxis.m_MaxSpeed = 3;
        }
        else
        {
            mainCamera.m_XAxis.m_InputAxisName = "";
            mainCamera.m_XAxis.m_MaxSpeed = 0;
        }
    }
}
