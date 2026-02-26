using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static PilotoSim_Ultrapassagem;

public class SimulationEngine
{
    public List<PilotoSim> pilotos => PilotosDataBase.Pilotos_CampeonatoAtual;

    public SplineManager splineManager;
    public RaceManager raceManager;

    // Módulos do Simulation Engine
    public SE_Modulo_AceleracaoFrenagem mod_AceleracaoFrenagem;
    public SE_Modulo_Ultrapassagem mod_Ultrapassagem;
    public SE_Modulo_ModosConducao mod_Conducao;
    public SE_Modulo_Mentalidade mod_Mentalidade;
    public SE_Modulo_PosicionamentoLateral mod_PosLateral;

    public SimulationEngine(SplineManager spline, RaceManager raceManager)
    {
        this.splineManager = spline;
        this.raceManager = raceManager;

        mod_AceleracaoFrenagem = new SE_Modulo_AceleracaoFrenagem(this);
        mod_Ultrapassagem = new SE_Modulo_Ultrapassagem(this);
        mod_Conducao = new SE_Modulo_ModosConducao(this);
        mod_Mentalidade = new SE_Modulo_Mentalidade(this);
        mod_PosLateral = new SE_Modulo_PosicionamentoLateral(this);
    }

    // Pré Tick -> Primeiro método da ordem de execução lógica, pega informações, agrupa -> envia para os módulos
    private void PreTick(float dt, List<PilotoSim> ordenados)
    {       
        // 1. Seta posição na corrida (1º, 2º, 3º...)
        SetPilotosPosition(ordenados);
    }

    // Módulo de ultrapassagem -> Responsável por gerir a lógica das ultrapassagens.
    private void Tick_Mod_Ultrapassagem(float dt, List<PilotoSim> ordenados)
    {
        for (int idx = 0; idx < ordenados.Count; idx++)
        {
            PilotoSim piloto = ordenados[idx];

            piloto.mod_Ultrapassagem.Tick(dt);
            mod_Ultrapassagem.TentarUltrapassagem(piloto, idx, ordenados, dt);
            mod_Ultrapassagem.Checar_Se_Conseguiu_Ultrapassar(piloto);
        }
    }

    // Módulo de Modos de Condução -> Responsável por gerir os Modos de Condução/Consumos do veículo e gestão da IA (escolhendo modos).
    private void Tick_Mod_ModosConducao(float dt, List<PilotoSim> ordenados)
    {
        for (int idx = 0; idx < ordenados.Count; idx++)
        {
            PilotoSim piloto = ordenados[idx];

            mod_Conducao.Atualizar_Conducao(piloto, dt);
            mod_Conducao.Atualizar_Desgaste_Pneus(piloto, dt);
            mod_Conducao.Atualizar_Consumo_Combustivel(piloto, dt);
            mod_Conducao.IA_Selecionar_ModosConducao(piloto, ordenados, idx);
        }        
    }

    // Módulo Mentalidade -> Responsável por gerir a mentalidade do piloto durante a corrida
    private void Tick_Mod_Mentalidade(float dt, List<PilotoSim> ordenados)
    {
        for (int idx = 0; idx < ordenados.Count; idx++)
        {
            PilotoSim piloto = ordenados[idx];

            piloto.mod_Mentalidade.Atualizar(dt);
            mod_Mentalidade.IA_SelecionarMentalidade(piloto, idx, ordenados, dt);
        }
    }

    // Módulo Aceleração e Frenagem -> Responsável por gerir Motor/Marcha do veiculo e Frenagem, evitar colisões frontais.
    private void Tick_Mod_AceleracaoFrenagem(float dt, List<PilotoSim> ordenados)
    {
        for (int idx = 0; idx < ordenados.Count; idx++)
        {
            PilotoSim piloto = ordenados[idx];

            mod_AceleracaoFrenagem.CalcularVelocidadeFinal(piloto, idx, ordenados, dt);
        }        
    }

    // Módulo Posicionamento Lateral -> Responsável pela lógica em manter a posição lateral dos carros
    private void Tick_Mod_PosicionamentoLateral(float dt, List<PilotoSim> ordenados)
    {
        for (int idx = 0; idx < ordenados.Count; idx++)
        {
            PilotoSim piloto = ordenados[idx];

            piloto.mod_PosLateral.Tick(dt);
            mod_PosLateral.AtualizarFaixaCarro(piloto, idx, ordenados, dt);
        }
    }

    // Pós tick -> Responsável por gerir a parte visual do jogo, após todos os calculos.
    public void PosTick(float dt, List<PilotoSim> ordenados)
    {
        for (int idx = 0; idx < ordenados.Count; idx++)
        {
            PilotoSim piloto = ordenados[idx];

            AtualizarPosicaoVisual(piloto, dt);
            AtualizarPosicaoReal(piloto, dt);
        }
    }

    public void Tick(float dt)
    {
        // 1. Ordena pilotos pela distância
        var ordenados = pilotos.OrderByDescending(p => p.DistanceTotal).ToList();

        // 2. Ticks por Ordem de importância.
        PreTick(dt, ordenados);
        Tick_Mod_Ultrapassagem(dt, ordenados);
        Tick_Mod_ModosConducao(dt, ordenados);
        Tick_Mod_Mentalidade(dt, ordenados);
        Tick_Mod_AceleracaoFrenagem(dt, ordenados);
        Tick_Mod_PosicionamentoLateral(dt, ordenados);
        PosTick(dt, ordenados);
    }

    private void SetPilotosPosition(List<PilotoSim> ordenados)
    {
        for (int i = 0; i < ordenados.Count; i++)
        {
            if (ordenados[i].DistanceTotal > 0)
                ordenados[i].Set_PosicaoAtual(i + 1);
        }
    }

    private void AtualizarPosicaoVisual(PilotoSim p, float dt)
    {
        p.Set_PrevDistance(p.Distance);  // salvar antes do update
        float oldDistance = p.Distance;

        // Atualizar distância visual (cíclica)
        p.Set_Distance(oldDistance + p.mod_acelFrenagem.Car_SpeedMS * dt);

        if (p.Distance >= splineManager.TotalLength)
        {
  
            p.Set_Distance(oldDistance -= splineManager.TotalLength);
        }

        // Atualizar splineT
        p.Set_SplineT(splineManager.SplineTFromDistance(p.Distance));
    }

    private void AtualizarPosicaoReal(PilotoSim p, float dt)
    {
        if (!p.PassouLargadaPrimeiraVez)
        {
            if (p.Distance < p.PrevDistance)
                p.Set_PassouLargadaPrimeiraVez(true);
        }

        if (p.PassouLargadaPrimeiraVez)
        {
            float oldDistance = p.DistanceTotal;
            p.Set_DistanceTotal(oldDistance + p.mod_acelFrenagem.Car_SpeedMS * dt);
        }
    }
}