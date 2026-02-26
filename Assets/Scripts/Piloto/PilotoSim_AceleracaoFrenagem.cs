using UnityEngine;

public class PilotoSim_AceleracaoFrenagem
{
    public PilotoSim piloto;

    // ==========================
    //    Variáveis de Acesso
    // ==========================

    public float Car_SpeedMS => d_veiculo.speedMS;          // em m/s
    public float Car_SpeedKM => d_veiculo.speedMS * 3.6f;   // em km/h
    public float Car_SpeedML => d_veiculo.speedMS;          // em milhas/h
    public float Car_Aceleracao => d_veiculo.aceleracao;    // m/s²

    public float P_ForcaAceleracao => d_piloto.forcaAceleracao;
    public float P_ForcaFrenagem => d_piloto.forcaFreio;

    // ==========================
    //          Structs
    // ==========================

    private struct DadosVeiculo
    {
        public float speedMS;    // velocidade real em m/s
        public float aceleracao; // aceleração aplicada no último frame
    }

    private struct DadosPiloto
    {
        public float forcaAceleracao; // 0–1 (input do “pé no acelerador”)
        public float forcaFreio;      // 0–1 (input de “pé no freio”)
    }

    private DadosVeiculo d_veiculo;
    private DadosPiloto d_piloto;

    // ==========================
    //         Construtor
    // ==========================

    public PilotoSim_AceleracaoFrenagem(PilotoSim piloto)
    {
        this.piloto = piloto;

        d_veiculo = new DadosVeiculo()
        {
            speedMS = 0
        };

        d_piloto = new DadosPiloto()
        {
            forcaAceleracao = 0,
            forcaFreio = 0
        };       
    }

    // ==========================
    //  MÉTODOS PROTEGIDOS
    // ==========================

    private void Set_Speed(float speed) => d_veiculo.speedMS = Mathf.Clamp(speed, 0f, Mathf.Infinity);

    // ==========================
    //   MÉTODOS PÚBLICOS EXTRA
    // ==========================

    // Dados do Veículo
    public void SetSpeedMS(float value)
    {
        d_veiculo.speedMS = Mathf.Max(0, value);
        Set_Speed(d_veiculo.speedMS);
    }


    // Dados Piloto - Quanto "Pisa" no Acelerador/Freio
    public void Set_ForcaAceleracao(float valor) => d_piloto.forcaAceleracao = Mathf.Clamp01(valor);
    public void Set_ForcaFreio(float valor) => d_piloto.forcaFreio = Mathf.Clamp01(valor);
}
