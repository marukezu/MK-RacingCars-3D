using UnityEngine;

public class Suspensao : MonoBehaviour
{
    private Carro carro;

    public Transform wheelMesh; // Parte visual da roda.
    public WheelCollider wheelCollider; // WheelCollider associado.
    private float wheelRadius; // Raio da roda (em metros)
    public float velocidadeLinear;

    // Booleanas
    public bool isGrounded = false;
    public bool isFrontal = false; // Se é uma suspensão frontal do carro.

    private void Awake()
    {
        // Inicia os componentes
        carro = GetComponentInParent<Carro>();
        wheelCollider = GetComponent<WheelCollider>();
    }

    private void Update()
    {
        // checa se esta em contato com o chao
        isGrounded = wheelCollider.isGrounded;

        // Pega o raio da roda.
        wheelRadius = wheelCollider.radius;

        // Aceleração que vem do torque do Motor.
        switch (carro.carroceria.tracao)
        {
            case Carro.TipoTracao.Frontal:
                if (isFrontal)
                    wheelCollider.motorTorque = carro.motorMarcha.torqueInjetado;
                break;

            case Carro.TipoTracao.Traseira:
                if (!isFrontal)
                    wheelCollider.motorTorque = carro.motorMarcha.torqueInjetado;
                break;

            case Carro.TipoTracao.QuatroPorQuatro:
                wheelCollider.motorTorque = carro.motorMarcha.torqueInjetado;
                break;
        }

        // Sincroniza o visual da roda com o WheelCollider
        AtualizarWheelMesh();

        // Atualiza a velocidade linear do pneu.
        GetVelocidadeLinear();
    }

    private void FixedUpdate()
    {
        if (!isGrounded)
        {
            // A roda perdeu contato com o chão.
            // Aplicar força para manter a roda no solo.
            Rigidbody rb = carro.GetComponent<Rigidbody>();

            // Posição do centro do WheelCollider
            Vector3 wheelPosition = wheelCollider.transform.position;

            // Força para baixo
            Vector3 downwardForce = Vector3.down * 100000f; // Ajuste a magnitude conforme necessário.

            // Aplica a força no ponto do WheelCollider
            rb.AddForceAtPosition(downwardForce, wheelPosition, ForceMode.Force);
        }
    }


    private void GetVelocidadeLinear()
    {
        // Obtemos o valor de RPM do WheelCollider
        float rpm = wheelCollider.rpm;

        // Calculamos a velocidade linear
        velocidadeLinear = (rpm * 2 * Mathf.PI * wheelRadius) / 60;
    }

    private void AtualizarWheelMesh()
    {
        // Obtém a posição e rotação do WheelCollider
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);

        // Atualiza a posição e rotação da parte visual da roda
        wheelMesh.position = pos;
        wheelMesh.rotation = rot;
    }
}
