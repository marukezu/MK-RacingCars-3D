using UnityEngine;

public class Carro : MonoBehaviour
{

    // Enums
    public enum Modelo
    {
        Lanzia,
        LanziaStradale,
        RinaultAlpine,

        // Mini Cupa's
        MiniCupa_RED,
        MiniCupa_BLUE,
        MiniCupa_GREEN,
        MiniCupa_YELLOW,
    }
    public enum TipoTracao
    {
        Frontal,
        Traseira,
        QuatroPorQuatro // Nome mais intuitivo para "4x4"
    }

    public enum TipoDeConducao // gasto Pneus
    {
        Economia,
        Equilibrado,
        Forte,
        Agressivo
    }

    public enum TipoDeMotor // gasto Gasolina
    {
        Economia,
        Equilibrado,
        Forte,
        Agressivo
    }

    private void Awake()
    {
        carCollider = GetComponent<BoxCollider>();
    }

    public Modelo modelo { get; set; }
    public BoxCollider carCollider;
    public Carroceria carroceria;
    public Motor_E_Marcha motorMarcha;
    public Carro_Comandos comandos;
    public Carro_Upgrades upgrades;
    public Carro_DetectorColisao detectorColisao;
    public PilotoController pilotoController;

    public void InicializarParaClassificacao(Piloto piloto)
    {
        // 
        upgrades = piloto.CarroUpgrades;
        piloto.Carro = this;
        piloto.EquipePitStop.EntrouNoPitStopNessaVolta = true;
        piloto.EquipePitStop.ParadoNoPitStop = true;
        piloto.EquipePitStop.ConsertandoNoPit = false;
        carroceria.NovosPneus(piloto.EquipePitStop.TipoPneu);
        carroceria.combustivelAtual = 100;
        pilotoController.piloto = piloto;
        pilotoController.tempoVoltaMaisRapida = 599.999f;

        // Retorna os waypoints do pitstop e j� no indice certo para avan�ar no pit.
        pilotoController.AlterarWaypointsParaSeguir(PistaManager.Instance.GetListWaypointsPitStop_InicioClassificacao(pilotoController));
        pilotoController.waypointAtual = pilotoController.waypointsParaSeguir[pilotoController.waypointIndex];
        pilotoController.faixaAtual = pilotoController.waypointAtual.faixa;
    }

    public void InicializarParaCorrida(Piloto piloto) 
    {
        upgrades = piloto.CarroUpgrades;
        piloto.Carro = this;
        piloto.EquipePitStop.EntrouNoPitStopNessaVolta = false;
        piloto.EquipePitStop.ParadoNoPitStop = false;
        piloto.EquipePitStop.PitStopNestaVolta = false;
        piloto.EquipePitStop.ConsertandoNoPit = false;
        pilotoController.piloto = piloto;
        pilotoController.foiDadoLargada = false;
        carroceria.NovosPneus(piloto.EquipePitStop.TipoPneu);
        carroceria.combustivelAtual = 100;
        pilotoController.tempoVoltaMaisRapida = 599.999f;

        // Retorna os waypoints da corrida na faixa correta e no indice certo.
        pilotoController.AlterarWaypointsParaSeguir(PistaManager.Instance.GetListWaypoints_InicioCorrida(pilotoController)); // Retorna Waypoints Dir ou Esq dependendo da posição do piloto.
        pilotoController.waypointIndex = 0;
        pilotoController.waypointAtual = pilotoController.waypointsParaSeguir[pilotoController.waypointIndex];
        pilotoController.faixaAtual = pilotoController.waypointAtual.faixa;
    }
}
