using UnityEngine;
using static Carro;
using static Pneu;

public class Carroceria : MonoBehaviour
{
    [Header("============== SCRIPTS / PARTES DO CARRO / COMPONENTES ==============")]
    public Carro carro;
    public Rigidbody carRigidBody;

    [Header("============== SUSPENSÕES ==============")]
    public Suspensao suspensaoFL;
    public Suspensao suspensaoFR;
    public Suspensao suspensaoRL;
    public Suspensao suspensaoRR;

    [Header("============== PNEUS ==============")]
    public Pneu pneuFL;
    public Pneu pneuFR;
    public Pneu pneuRL;
    public Pneu pneuRR;

    [Header("============== Pontos do Carro ==============")]
    public Transform pontoBicoCarro;
    public Transform pontoTraseiroCarro;
    public Transform pontoAerofolio;
    public Transform pontoDeMassa;

    [Header("============== Farois Frontais ==============")]
    public GameObject FarolFrontalDir;
    public GameObject FarolFrontalEsq;

    [Header("============== Farois Traseiros ==============")]
    public GameObject FarolFreioDir;
    public GameObject FarolFreioEsq;

    [Header("============== Dados do veiculo ==============")]
    public TipoTracao tracao;
    public float pesoVeiculo;
    public float raioGeometria = 5f;
    public float distanciaEntreEixos = 2.4f;
    public float distanciaRodasTraseiras = 1.6f;
    public float anguloAckermanFL, anguloAckermanFR;
    public Vector3 velocidadeCarroceria;
    public float velocidadeKMAtual = 0f;

    [Header("============== MODO DE DIREÇÃO ==============")]
    public TipoDeConducao modoDirecao;

    [Header("============== MODO DE MOTOR ==============")]
    public TipoDeMotor modoMotor;
    public float combustivelAtual = 100;
    private float gasolinaConsumoBasal = 0.055f;
    public float combustivelVoltaPassada = 100f;
    public float consumoCombustivelPorVolta;
    public bool deltaPositivo;

    private void Awake()
    {
        // Inicia o RigidBody
        carRigidBody.mass = pesoVeiculo;
        carRigidBody.centerOfMass = pontoDeMassa.localPosition;
    }

    private void Update()
    {
        // Impede o carro de capotar.
        Vector3 currentRotation = transform.eulerAngles;
        float rotationLimit = 15f;

        // Ajusta a rotação para o intervalo correto (-180 a 180) e limita apenas X e Z
        currentRotation.x = Mathf.Clamp(NormalizeAngle(currentRotation.x), -rotationLimit, rotationLimit);
        currentRotation.z = Mathf.Clamp(NormalizeAngle(currentRotation.z), -rotationLimit, rotationLimit);

        // Aplica a rotação de volta ao Transform
        transform.eulerAngles = currentRotation;

        // Velocidade da carroceria 
        velocidadeCarroceria = transform.InverseTransformDirection(carRigidBody.velocity);

        // Simulação do freio.
        float drag = 0.1f + (carro.comandos.potenciaFrenagem * 2f);
        carRigidBody.drag = drag;

        // Calcula a velocidade instantânea
        float velocidadeInstantanea = carRigidBody.velocity.magnitude * 3.6f;

        // Suaviza a transição para o valor atual
        velocidadeKMAtual = Mathf.Lerp(velocidadeKMAtual, velocidadeInstantanea, 0.1f);

        // Outros métodos no Update
        VirarRodasGeometriaAckerman();

        // Gasto de pneus
        if (pneuFL != null) pneuFL.AtualizaCondicaoPneu();
        if (pneuFR != null) pneuFR.AtualizaCondicaoPneu();
        if (pneuRL != null) pneuRL.AtualizaCondicaoPneu();
        if (pneuRR != null) pneuRR.AtualizaCondicaoPneu();

        // Gasto de combustível
        AtualizaConsumoCombustivel();
    }

    float NormalizeAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        if (angle < -180f) angle += 360f;
        return angle;
    }

    // Usado para virar a roda conforme o volante gira usando a Geometria Ackerman.
    private void VirarRodasGeometriaAckerman()
    {
        if (carro.comandos.potenciaVolante < 0)
        {
            anguloAckermanFL = Mathf.Rad2Deg * Mathf.Atan2(distanciaEntreEixos, raioGeometria - (distanciaRodasTraseiras / 2));
            anguloAckermanFR = Mathf.Rad2Deg * Mathf.Atan2(distanciaEntreEixos, raioGeometria + (distanciaRodasTraseiras / 2));
        }
        else if (carro.comandos.potenciaVolante > 0)
        {
            anguloAckermanFL = Mathf.Rad2Deg * Mathf.Atan2(distanciaEntreEixos, raioGeometria + (distanciaRodasTraseiras / 2));
            anguloAckermanFR = Mathf.Rad2Deg * Mathf.Atan2(distanciaEntreEixos, raioGeometria - (distanciaRodasTraseiras / 2));
        }
        else if (carro.comandos.potenciaVolante == 0)
        {
            anguloAckermanFL = 0;
            anguloAckermanFR = 0;
        }

        suspensaoFL.wheelCollider.steerAngle = anguloAckermanFL * carro.comandos.potenciaVolante;
        suspensaoFR.wheelCollider.steerAngle = anguloAckermanFR * carro.comandos.potenciaVolante;
    }

    // ======================================================================================
    // ============================== Consumo de Combustível  ===============================
    // ======================================================================================
    public void AtualizaConsumoCombustivel()
    {
        // Se estiver acelerando inicia o gasto do combustivel.
        float consumo = (carro.comandos.potenciaAceleracao * GetFatorModoMotor("gasto")) * gasolinaConsumoBasal * Time.deltaTime;
        combustivelAtual = Mathf.Clamp(combustivelAtual - consumo, 0f, 100f);

    }

    public void AtualizaConsumoPorVolta()
    {
        if (combustivelVoltaPassada != 0)
        {
            consumoCombustivelPorVolta = combustivelVoltaPassada - combustivelAtual;
            float combustivelNecessario = (RaceManager.Instance.voltasTotais - carro.pilotoController.voltaAtual) * consumoCombustivelPorVolta;
            deltaPositivo = combustivelAtual > combustivelNecessario;
        }
        combustivelVoltaPassada = combustivelAtual;
    }

    public float GetFatorModoMotor(string tipo) // GASOLINA
    {
        if (tipo == "potencia")
        {
            switch (modoMotor)
            {
                case TipoDeMotor.Economia: return 0.96f;    // Baixa
                case TipoDeMotor.Equilibrado: return 1f;    // Média
                case TipoDeMotor.Forte: return 1.03f;        // Forte
                case TipoDeMotor.Agressivo: return 1.06f;    // Agressivo
                default: return 1f;
            }
        }
        else if (tipo == "gasto")
        {
            switch (modoMotor)
            {
                case TipoDeMotor.Economia: return 0.8f;     // Baixa
                case TipoDeMotor.Equilibrado: return 1f;    // Média
                case TipoDeMotor.Forte: return 1.25f;        // Forte
                case TipoDeMotor.Agressivo: return 1.50f;    // Agressivo
                default: return 1f;
            }
        }
        return 1f;
    }

    public float GetFatorModoDirecao(string tipo) // PNEUS
    {
        if (tipo == "potencia")
        {
            switch (carro.carroceria.modoDirecao)
            {
                case TipoDeConducao.Economia: return 0.96f;     // Baixa
                case TipoDeConducao.Equilibrado: return 1f;     // Média
                case TipoDeConducao.Forte: return 1.03f;        // Forte
                case TipoDeConducao.Agressivo: return 1.06f;    // Ultrapassagem
                default: return 1f;
            }
        }
        else if (tipo == "gasto")
        {
            switch (carro.carroceria.modoDirecao)
            {
                case TipoDeConducao.Economia: return 0.8f;     // Baixa
                case TipoDeConducao.Equilibrado: return 1f;     // Média
                case TipoDeConducao.Forte: return 1.25f;       // Forte
                case TipoDeConducao.Agressivo: return 1.50f;    // Agressivo
                default: return 1f;
            }
        }
        return 1f;
    }

    public float GetCondicaoMediaPneus()
    {
        if (pneuFL == null || pneuFR == null || pneuRL == null || pneuRR == null) return 0;

        return (pneuFL.durabilidadeAtual + pneuFR.durabilidadeAtual + pneuRL.durabilidadeAtual + pneuRR.durabilidadeAtual) / 4;
    }

    public void NovosPneus(TipoPneu tipoPneu)
    {
        pneuFL = new Pneu(tipoPneu, carro, Pneu.SlotPneu.FL);
        pneuFR = new Pneu(tipoPneu, carro, Pneu.SlotPneu.FR);
        pneuRL = new Pneu(tipoPneu, carro, Pneu.SlotPneu.RL);
        pneuRR = new Pneu(tipoPneu, carro, Pneu.SlotPneu.RR);
    }
}
