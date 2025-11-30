using System.Collections.Generic;
using UnityEngine;
using static PilotoController;

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

    [Header("============== Tempos - Checkpoints ==============")]
    public float tempoVoltaAtual;
    public float tempoUltimaVolta;
    public float tempoTotalVoltas;
    public float tempoVoltaMaisRapida;
    public float tempoDiferencaCarroFrente = 0;
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

                // Calcula o tempo diferença do carro a frente.
                float tempoDiferencaInstantanea = CalculaTempoDeDiferenca();
                tempoDiferencaCarroFrente = Mathf.Max(Mathf.Lerp(tempoDiferencaCarroFrente, tempoDiferencaInstantanea, 1f * Time.deltaTime), 0);
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
        float tempoDiferenca = RetornaDistanciaCarroFrenteWAYPOINTS() / velocidadeEmMetrosPorSegundo;

        if (posicaoAtual == 1)
            return 0;

        return tempoDiferenca;
    }

    // =============================================================================================================
    // ============================= Calculo Distância dos carros A Frente e Atrás =================================
    // =============================================================================================================
    public float RetornaDistanciaCarroFrenteWAYPOINTS()
    {
        // Pega o piloto mais próximo à frente (qualquer faixa)
        Piloto pilotoFrente = null;

        foreach (Piloto outro in PlayerSettings.campeonato.pilotosParticipantes)
        {
            if (outro == piloto) continue;
            if (outro.Carro == null) continue;

            // Se for o carro da minha frente, popula.
            if (outro.Carro.pilotoController.posicaoAtual == posicaoAtual - 1)
                pilotoFrente = outro;
        }

        if (pilotoFrente == null)
            return 0f; // Se não houver carro à frente, retorna 0

        // Pega os waypoints que meu carro está seguindo (faixa atual)
        List<Waypoint> wps = PistaManager.Instance.waypointsCentro;
        if (wps == null || wps.Count == 0)
            return 0f;

        Vector3 minhaPos = carro.transform.position;
        Vector3 posOutro = pilotoFrente.Carro.transform.position;

        // Encontrar waypoint mais próximo do meu carro e do carro à frente
        int indexInicio = 0;
        int indexFim = 0;
        float menorDistInicio = float.MaxValue;
        float menorDistFim = float.MaxValue;

        for (int i = 0; i < wps.Count; i++)
        {
            float distInicio = Vector3.Distance(minhaPos, wps[i].transform.position);
            if (distInicio < menorDistInicio)
            {
                menorDistInicio = distInicio;
                indexInicio = i;
            }

            float distFim = Vector3.Distance(posOutro, wps[i].transform.position);
            if (distFim < menorDistFim)
            {
                menorDistFim = distFim;
                indexFim = i;
            }
        }

        // Ajusta caso o índice do carro à frente seja "menor" (loop da pista)
        if (indexFim < indexInicio)
            indexFim += wps.Count;

        // Soma distância do meu carro até o primeiro waypoint
        float distanciaTotal = Vector3.Distance(minhaPos, wps[indexInicio % wps.Count].transform.position);

        // Soma distâncias entre waypoints
        for (int i = indexInicio; i < indexFim; i++)
        {
            Waypoint atual = wps[i % wps.Count];
            Waypoint proximo = wps[(i + 1) % wps.Count];
            distanciaTotal += Vector3.Distance(atual.transform.position, proximo.transform.position);
        }

        // Soma distância do último waypoint até o carro à frente
        distanciaTotal += Vector3.Distance(wps[indexFim % wps.Count].transform.position, posOutro);

        return distanciaTotal;
    }

    public float RetornaDistanciaCarro_FRENTE_MesmaFaixa()
    {
        // Pega o piloto mais próximo à frente na mesma faixa
        Piloto pilotoFrente = RetornaPilotoMaisProximo_FRENTE_MesmaFaixa();
        if (pilotoFrente == null)
            return 999f;

        // Pontos do carro
        Vector3 minhaFrente = carro.carroceria.pontoBicoCarro.position;
        Vector3 traseiraOutro = pilotoFrente.Carro.carroceria.pontoTraseiroCarro.position;

        // Converte para meu espaço local
        Vector3 outroLocal = transform.InverseTransformPoint(traseiraOutro);
        Vector3 minhaFrenteLocal = transform.InverseTransformPoint(minhaFrente);

        // Distância ao longo do eixo Z
        float distanciaZ = outroLocal.z - minhaFrenteLocal.z;

        return Mathf.Max(distanciaZ, 0f);
    }

    public float RetornaDistanciaCarro_ATRAS_MesmaFaixa()
    {
        // Pega o piloto mais próximo à frente na mesma faixa
        Piloto pilotoAtras = RetornaPilotoMaisProximo_ATRAS_MesmaFaixa();
        if (pilotoAtras == null || pilotoAtras.Carro == null)
            return 999f;

        Vector3 minhaTraseira = carro.carroceria.pontoTraseiroCarro.position;
        Vector3 frenteOutro = pilotoAtras.Carro.carroceria.pontoBicoCarro.position;

        Vector3 frenteOutroLocal = transform.InverseTransformPoint(frenteOutro);
        Vector3 minhaTraseiraLocal = transform.InverseTransformPoint(minhaTraseira);

        float distanciaZ = minhaTraseiraLocal.z - frenteOutroLocal.z;

        return Mathf.Max(distanciaZ, 0f);
    }

    public Piloto RetornaPilotoMaisProximo_FRENTE_MesmaFaixa() // Esse método ele retorna o piloto mais próximo e que está na frente, da faixa que vem por parâmetro.
    {
        float melhorDistancia = 9999f;
        Piloto pilotoProximo = null;

        // Pega a frente do meu carro
        Vector3 minhaFrenteWorld = carro.carroceria.pontoBicoCarro.position;
        Vector3 minhaFrenteLocal = transform.InverseTransformPoint(minhaFrenteWorld);

        foreach (Piloto outro in PlayerSettings.campeonato.pilotosParticipantes)
        {
            if (outro == piloto) continue;
            if (outro.Carro == null) continue;

            // Se o piloto estiver atrás de mim, ignora.
            if (outro.Carro.pilotoController.posicaoAtual > posicaoAtual)
                continue;

            // Se o piloto estiver em faixa diferente da minha, ignora.
            if (outro.Carro.pilotoController.faixaAtual != faixaAtual)
                continue;

            // Pegar a traseira do outro carro
            Vector3 traseiraOutroWorld = outro.Carro.carroceria.pontoTraseiroCarro.position;
            Vector3 traseiraOutroLocal = transform.InverseTransformPoint(traseiraOutroWorld);

            float distanciaZ = traseiraOutroLocal.z - minhaFrenteLocal.z;

            // Procurar o carro mais próximo
            if (distanciaZ < melhorDistancia)
            {
                melhorDistancia = distanciaZ;
                pilotoProximo = outro;
            }
        }

        return pilotoProximo;
    }

    public Piloto RetornaPilotoMaisProximo_ATRAS_MesmaFaixa() // Esse método ele retorna o piloto mais próximo e que está atrás, da faixa que vem por parâmetro.
    {
        float melhorDistancia = 9999f;
        Piloto pilotoProximo = null;

        // Pega a frente do meu carro
        Vector3 minhaTraseiraWorld = carro.carroceria.pontoTraseiroCarro.position;
        Vector3 minhaTraseiraLocal = transform.InverseTransformPoint(minhaTraseiraWorld);

        foreach (Piloto outro in PlayerSettings.campeonato.pilotosParticipantes)
        {
            if (outro == piloto) continue;
            if (outro.Carro == null) continue;

            // Se o piloto estiver na frente de mim, ignora.
            if (outro.Carro.pilotoController.posicaoAtual < posicaoAtual)
                continue;

            // Se o piloto estiver em faixa diferente da minha, ignora.
            if (outro.Carro.pilotoController.faixaAtual != faixaAtual)
                continue;

            // Pegar a traseira do outro carro
            Vector3 frenteOutroWorld = outro.Carro.carroceria.pontoBicoCarro.position;
            Vector3 frenteOutroLocal = transform.InverseTransformPoint(frenteOutroWorld);

            float distanciaZ = frenteOutroLocal.z - minhaTraseiraLocal.z;

            // Procurar o carro mais próximo
            if (distanciaZ < melhorDistancia)
            {
                melhorDistancia = distanciaZ;
                pilotoProximo = outro;
            }
        }

        return pilotoProximo;
    }

    private float RetornaDistanciaMaisProximaCarro_FaixaSelecionavel(Faixas faixa) // Esse método ele retorna a distância do carro mais próximo da faixa que vem por parâmetro.
    {
        float melhorDistancia = 9999f;

        // Pega a frente do meu carro
        Vector3 minhaFrenteWorld = carro.carroceria.pontoBicoCarro.position;
        Vector3 minhaFrenteLocal = transform.InverseTransformPoint(minhaFrenteWorld);

        foreach (Piloto outro in PlayerSettings.campeonato.pilotosParticipantes)
        {
            if (outro == piloto) continue;
            if (outro.Carro == null) continue;

            // Se o carro estiver atrás de mim, ignora.
            if (outro.Carro.pilotoController.posicaoAtual > posicaoAtual)
                continue;

            // Se outro estiver em faixa diferente da minha, ignora.
            if (outro.Carro.pilotoController.faixaAtual != faixa)
                continue;

            // Pegar a traseira do outro carro
            Vector3 traseiraOutroWorld = outro.Carro.carroceria.pontoTraseiroCarro.position;
            Vector3 traseiraOutroLocal = transform.InverseTransformPoint(traseiraOutroWorld);

            float distanciaZ = traseiraOutroLocal.z - minhaFrenteLocal.z;

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
        float frenteThreshold = 5f;    // distância perigosa à frente (minha frente <-> traseira do outro)
        float trasThreshold = 5f;      // distância perigosa atrás (minha traseira <-> frente do outro)
        float lateralThreshold = 2.5f; // distancia perigosa lateral
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

    public Faixas EscolherMelhorFaixa() // Escolhe a melhor faixa para trocar, baseando-se no método "RetornaDistanciaMaisProximaCarroMesmaFaixa"
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
        float dEsquerda = RetornaDistanciaMaisProximaCarro_FaixaSelecionavel(Faixas.Esquerda);
        float dDireita = RetornaDistanciaMaisProximaCarro_FaixaSelecionavel(Faixas.Direita);

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