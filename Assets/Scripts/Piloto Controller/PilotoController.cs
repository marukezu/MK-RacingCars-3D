using System.Collections.Generic;
using UnityEngine;

// ==============================================================================
// == CLASSE RESPONSÁVEL POR GERIR WAYPOINTS E DADOS DA CORRIDA SOBRE O PILOTO ==
// ==============================================================================

public class PilotoController : MonoBehaviour
{

    public enum Faixas
    {
        Esquerda,
        Centro,
        Direita,
        PitStop
    }

    [Header("============== Entrada de Dados ==============")]
    public Carro carro;
    public PilotoController_Classificacao controllerClassificacao;
    public PilotoController_Corrida controllerCorrida;
    public Piloto piloto;

    [Header("============== Dados de Pista/Condução ==============")]
    public bool foiDadoLargada;
    public bool iniciouClassificacao;
    public bool estaNaPista;
    public int posicaoAtual; // Posição na corrida, 1°, 2°, 3°, etc...
    public int voltaAtual;
    public string NomePilotoFrente;
    public Carro carroFrente;
    public string NomePilotoAtras;
    public Carro carroAtras;
    public float distanciaCarroFrente;
    public float distanciaCarroAtras;

    [Header("============== Tempos - Checkpoints ==============")]
    public float tempoVoltaAtual;
    public float tempoUltimaVolta;
    public float tempoTotalVoltas;
    public float tempoVoltaMaisRapida;
    public float tempoDiferencaCarroFrente;
    public int checkpointsPassados;

    [Header("============== Temporizadores ==============")]
    public float tempoEntreSaidasClassificatoria = 5f;

    [Header("============== Controle de Faixa ==============")]
    public Faixas faixaAtual;
    public float delayTrocandoFaixa = 0f;
    public bool trocandoFaixa;

    [Header("============== Controle de Waypoints ==============")]
    public int waypointIndex = 0;
    public List<Waypoint> waypointsParaSeguir;
    public Waypoint waypointAtual;

    [Header("============== Forças impostas ao carro pelo piloto ==============")]
    public float potenciaAceleracao;           // 0-1f
    public float potenciaFreio;                // 0-1f
    public float potenciaGiroDirecao;          // 0-1f

    private void Awake()
    {
        waypointsParaSeguir = new List<Waypoint>();
    }

    void Update()
    {
        TemporizadoresContadores();
        SetPosicao();

        // Acende e apaga o farol conforme a hora do dia.
        carro.comandos.SetFarolFrontal(TimeAndDate_Manager.Instance.EstaDeNoite());
    }

    private void TemporizadoresContadores()
    {
        // Delay Trocando de Faixa.
        delayTrocandoFaixa -= Time.deltaTime;
        if (delayTrocandoFaixa <= 0) { trocandoFaixa = false; }
        if (delayTrocandoFaixa > 0) { trocandoFaixa = true; }

        // Se alguma sess�o estiver acontecendo.
        if (RaceManager.Instance.corridaEmAndamento || RaceManager.Instance.classificacaoEmAndamento)
        {
            if (!piloto.EquipePitStop.ParadoNoPitStop)
            {
                // Contador do tempo da volta atual.
                tempoVoltaAtual += Time.deltaTime;

                // Atualiza pilotos a frente e atrás e tempos de diferença.
                (carroFrente, carroAtras) = RetornaCarroFrenteAtras();

                if (carroFrente == null)
                    carroFrente = carro;

                if (carroAtras == null)
                    carroAtras = carro;

                // Calcula a distancia do carro a frente e atras.
                distanciaCarroFrente = RetornaDistanciaCarroFrente();
                distanciaCarroAtras = RetornaDistanciaCarroAtras();

                // Calcula o tempo diferença do carro a frente.
                float tempoDiferencaInstantanea = CalculaTempoDeDiferenca();
                tempoDiferencaCarroFrente = Mathf.Lerp(tempoDiferencaCarroFrente, tempoDiferencaInstantanea, 0.5f * Time.deltaTime);
            }

            // Contador para o piloto ir classificar.
            tempoEntreSaidasClassificatoria -= Time.deltaTime;
        }
    }

    public float CalculaTempoDeDiferenca()
    {
        // Velocidade do carro em km/h
        float velocidadeEmKmPorHora = carro.carroceria.velocidadeKMAtual;

        // Convertendo a velocidade para metros por segundo
        float velocidadeEmMetrosPorSegundo = velocidadeEmKmPorHora * 1000f / 3600f;

        // Calculando o tempo de diferença entre os carros
        float tempoDiferenca = distanciaCarroFrente / velocidadeEmMetrosPorSegundo;

        if (posicaoAtual == 1)
            return 0;

        return tempoDiferenca;
    }

    // =============================================================================================================
    // ============================= Calculo Distância dos carros A Frente e Atrás =================================
    // =============================================================================================================
    private (Carro, Carro) RetornaCarroFrenteAtras()
    {
        Carro carroFrente = null;
        Carro carroAtras = null;

        foreach (Piloto outro in PlayerSettings.campeonato.pilotosParticipantes)
        {
            if (outro == piloto) continue;

            // posição local do outro carro
            Vector3 outroLocal = transform.InverseTransformPoint(outro.Carro.transform.position);

            bool outroNaFrente = outroLocal.z > 0f;
            bool outroAtras = outroLocal.z < 0f;

            // ------------------------------
            //     CARRO À FRENTE
            // ------------------------------
            if (outroNaFrente)
            {
                if (carroFrente == null)
                {
                    carroFrente = outro.Carro;
                }
                else
                {
                    Vector3 frenteLocal = transform.InverseTransformPoint(carroFrente.transform.position);

                    // menor z positivo = mais perto na frente
                    if (outroLocal.z < frenteLocal.z)
                        carroFrente = outro.Carro;
                }
            }

            // ------------------------------
            //     CARRO ATRÁS
            // ------------------------------
            if (outroAtras)
            {
                if (carroAtras == null)
                {
                    carroAtras = outro.Carro;
                }
                else
                {
                    Vector3 atrasLocal = transform.InverseTransformPoint(carroAtras.transform.position);

                    // maior z negativo = mais perto atrás
                    if (outroLocal.z > atrasLocal.z)
                        carroAtras = outro.Carro;
                }
            }
        }

        return (carroFrente, carroAtras);
    }

    public float RetornaDistanciaCarroFrente()
    {
        if (carroFrente == null) return 999f;

        // Pontos reais no mundo
        Vector3 minhaFrente = carro.carroceria.pontoBicoCarro.position;
        Vector3 traseiraOutro = carroFrente.carroceria.pontoTraseiroCarro.position;

        // Converte a traseira do outro carro para meu espaço local
        Vector3 outroLocal = transform.InverseTransformPoint(traseiraOutro);

        // Converte a minha frente também para meu espaço local
        Vector3 minhaFrenteLocal = transform.InverseTransformPoint(minhaFrente);

        // Distância ao longo do eixo Z (minha frente até traseira dele)
        float distanciaZ = outroLocal.z - minhaFrenteLocal.z;

        return Mathf.Max(distanciaZ, 0f);
    }

    public float RetornaDistanciaCarroAtras()
    {
        if (carroAtras == null) return 999f;

        Vector3 minhaTraseira = carro.carroceria.pontoTraseiroCarro.position;
        Vector3 frenteOutro = carroAtras.carroceria.pontoBicoCarro.position;

        Vector3 outroLocal = transform.InverseTransformPoint(frenteOutro);
        Vector3 minhaTraseiraLocal = transform.InverseTransformPoint(minhaTraseira);

        // para carro atrás, a distância correta é:
        float distanciaZ = minhaTraseiraLocal.z - outroLocal.z;

        return Mathf.Max(distanciaZ, 0f);
    }

    private float DistanciaCarrosMesmaFaixa(Faixas faixa)
    {
        float melhorDistancia = 9999f;

        Vector3 minhaFrenteWorld = carro.carroceria.pontoBicoCarro.position;
        Vector3 minhaFrenteLocal = transform.InverseTransformPoint(minhaFrenteWorld);

        foreach (Piloto outro in PlayerSettings.campeonato.pilotosParticipantes)
        {
            if (outro == piloto) continue;
            if (outro.Carro == null) continue;

            // Apenas carros na faixa que queremos avaliar
            if (outro.Carro.pilotoController.faixaAtual != faixa)
                continue;

            // Pegar a traseira do outro carro
            Vector3 traseiraOutroWorld = outro.Carro.carroceria.pontoTraseiroCarro.position;
            Vector3 traseiraOutroLocal = transform.InverseTransformPoint(traseiraOutroWorld);

            float distanciaZ = traseiraOutroLocal.z - minhaFrenteLocal.z;

            // Carro atrás ou muito perto: ignora
            if (distanciaZ <= 0)
                continue;

            // Procurar o carro mais próximo
            if (distanciaZ < melhorDistancia)
                melhorDistancia = distanciaZ;
        }

        return melhorDistancia;
    }

    public bool VerificarSePodeTrocarFaixa(Faixas faixaDestino)
    {
        // Parâmetros ajustáveis
        float maxCheckDistance = 7f;   // raio inicial pra filtrar
        float frenteThreshold = 4f;    // distância perigosa à frente (minha frente <-> traseira do outro)
        float trasThreshold = 6f;      // distância perigosa atrás (minha traseira <-> frente do outro)
        float frontBackTolerance = 0f; // margem para considerar "na frente" ou "atrás"

        // Posição do meu carro (world points das referências do carro)
        Vector3 meuBicoWorld = carro.carroceria.pontoBicoCarro.transform.position;
        Vector3 minhaTraseiraWorld = carro.carroceria.pontoTraseiroCarro.transform.position;

        foreach (Piloto outro in PlayerSettings.campeonato.pilotosParticipantes)
        {
            if (outro == this.piloto) continue;
            if (outro.Carro == null) continue;

            // --- 1) usa o centro do outro pra decidir se ele está grosso modo à frente/atrás no meu sistema local
            Vector3 outroCentroWorld = outro.Carro.transform.position; // pode ser transform.position do root
            Vector3 outroCentroLocal = transform.InverseTransformPoint(outroCentroWorld);

            // se muito longe do alcance geral, ignora já
            Vector2 centroLocal2D = new Vector2(outroCentroLocal.x, outroCentroLocal.z);
            if (centroLocal2D.magnitude > maxCheckDistance)
                continue;

            bool outroTaNaFrente = outroCentroLocal.z > frontBackTolerance;   // está pra frente do meu nariz
            bool outroTaAtras = outroCentroLocal.z < -frontBackTolerance;     // está atrás do meu carro

            // --- 2) escolhe quais pontos comparar (world)
            Vector3 pontoOutroParaCompararWorld;
            Vector3 meuPontoParaCompararWorld;

            if (outroTaNaFrente)
            {
                // compara: minha frente (bico)  <->  traseira dele
                pontoOutroParaCompararWorld = outro.Carro.carroceria.pontoTraseiroCarro.transform.position;
                meuPontoParaCompararWorld = meuBicoWorld;
            }
            else if (outroTaAtras)
            {
                // compara: minha traseira  <->  frente dele (bico)
                pontoOutroParaCompararWorld = outro.Carro.carroceria.pontoBicoCarro.transform.position;
                meuPontoParaCompararWorld = minhaTraseiraWorld;
            }
            else
            {
                // Se o outro está aproximadamente no lado (local.z ≈ 0), tratar como lateral:
                // medimos a distância lateral entre os pontos centrais (segurança)
                Vector3 outroLocalAsIs = transform.InverseTransformPoint(outroCentroWorld);
                Vector2 lateralVec = new Vector2(outroLocalAsIs.x, outroLocalAsIs.z);
                float lateralDist = lateralVec.magnitude;
                if (lateralDist > maxCheckDistance) continue;

                // Para o caso lateral, usamos centro <-> meu centro
                pontoOutroParaCompararWorld = outro.Carro.transform.position;
                meuPontoParaCompararWorld = transform.position;
            }

            // --- 3) transforma ambos os pontos para o meu espaço local e calcula vetor entre eles
            Vector3 localOutro = transform.InverseTransformPoint(pontoOutroParaCompararWorld);
            Vector3 localMeuRef = transform.InverseTransformPoint(meuPontoParaCompararWorld);

            Vector2 diff = new Vector2(localOutro.x - localMeuRef.x, localOutro.z - localMeuRef.z);
            float distanciaEntrePontos = diff.magnitude;

            // --- 4) aplica thresholds dependendo se está à frente/atrás/lateral
            if (outroTaNaFrente)
            {
                if (distanciaEntrePontos <= frenteThreshold)
                {
                    // Se outro próximo na faixa destino -> bloqueia
                    if (outro.Carro.pilotoController.faixaAtual == faixaDestino)
                        return false;
                }
            }
            else if (outroTaAtras)
            {
                if (distanciaEntrePontos <= trasThreshold)
                {
                    // Se outro próximo na faixa destino -> bloqueia
                    if (outro.Carro.pilotoController.faixaAtual == faixaDestino)
                        return false;
                }
            }
            else
            {
                // lateral: considerar uma distância curta para bloquear a troca (evitar colisões ao cortar)
                float lateralThreshold = 2.5f;
                if (distanciaEntrePontos <= lateralThreshold)
                {
                    if (outro.Carro.pilotoController.faixaAtual == faixaDestino)
                        return false;
                }
            }
        }

        // se passou por todos sem bloquear, pode trocar
        return true;
    }

    public Faixas EscolherMelhorFaixa()
    {
        // Se estou na esquerda → única opção possível é voltar ao centro
        if (faixaAtual == Faixas.Esquerda)
        {
            return Faixas.Centro;
        }

        // Se estou na direita → única opção possível é voltar ao centro
        if (faixaAtual == Faixas.Direita)
        {
            return Faixas.Centro;
        }

        // Se estou no centro → avaliar esquerda e direita
        float dEsquerda = DistanciaCarrosMesmaFaixa(Faixas.Esquerda);
        float dDireita = DistanciaCarrosMesmaFaixa(Faixas.Direita);

        // Escolhe a faixa com maior espaço livre
        if (dEsquerda > dDireita)
            return Faixas.Esquerda;
        else
            return Faixas.Direita;
    }


    // =============================================================================================================
    // ========================================= Controle dos Waypoints ============================================
    // =============================================================================================================
    public void AlternarFaixa(Faixas faixa, float delayTrocaFaixa)
    {
        if (faixa == Faixas.Esquerda)
        {
            AlterarWaypointsParaSeguir(PistaManager.Instance.GetListWaypointsEsq());
            
        }
        else if (faixa == Faixas.Centro)
        {
            AlterarWaypointsParaSeguir(PistaManager.Instance.GetListWaypointsCentro());
        }
        else if (faixa == Faixas.Direita)
        {
            AlterarWaypointsParaSeguir(PistaManager.Instance.GetListWaypointsDir());
        }

        else if (faixa == Faixas.PitStop) // PitStop
        {
            AlterarWaypointsParaSeguir(PistaManager.Instance.GetListWaypointsPitStop());
            waypointIndex = 0;
        }

        delayTrocandoFaixa = delayTrocaFaixa;
        waypointAtual = waypointsParaSeguir[waypointIndex];
        faixaAtual = waypointAtual.faixa;
    }

    public void AlterarWaypointsParaSeguir(List<Waypoint> novosWaypoints)
    {
        waypointsParaSeguir.Clear();
        waypointsParaSeguir.AddRange(novosWaypoints);

        // SetFaixaOffSet(faixaAtual);
    }    

    public void AvancarProximoWaypoint(int waypointID)
    {
        // Incrementa o waypoint atual
        waypointIndex = waypointID + 1;

        // Verifica se atingiu o final da lista de waypoints
        if (waypointIndex >= waypointsParaSeguir.Count)
        {
            // Reinicia o índice para voltar ao início
            waypointIndex = 0;
        }

        waypointAtual = waypointsParaSeguir[waypointIndex];
    }

    // =============================================================================================================
    // ========================= Calcula a Posição do Piloto na CORRIDA e CLASSIFICAÇÃO ============================
    // =============================================================================================================
    public void SetPosicao()
    {
        if (PlayerSettings.campeonato == null || carro == null) return;

        int posicao = 1;

        foreach (Piloto piloto in PlayerSettings.campeonato.pilotosParticipantes)
        {
            if (piloto == this.piloto) continue;

            var pcOutro = piloto.Carro.pilotoController;

            // 1) Compara voltas
            if (pcOutro.voltaAtual > voltaAtual)
            {
                posicao++;
                continue;
            }
            else if (pcOutro.voltaAtual < voltaAtual)
            {
                continue;
            }

            // 2) Compara checkpoints
            if (pcOutro.checkpointsPassados > checkpointsPassados)
            {
                posicao++;
                continue;
            }
            else if (pcOutro.checkpointsPassados < checkpointsPassados)
            {
                continue;
            }

            // 3) Mesma volta e mesmo checkpoint → decide pela posição física real na pista
            Vector3 pontoFrontalOutro = pcOutro.carro.carroceria.pontoBicoCarro.position;
            Vector3 outroNoMeu = transform.InverseTransformPoint(pontoFrontalOutro);

            if (outroNoMeu.z > 0f)
            {
                posicao++;
            }
        }

        posicaoAtual = posicao;
    }

}