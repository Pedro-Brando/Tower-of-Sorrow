using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using System;
using MoreMountains.Feedbacks;

public class ImpactoEspiritual : CharacterAbility, IUldrichAbility
{
    [Header("Configurações do Impacto Espiritual")]

    [Tooltip("Prefab do meteoro")]
    public GameObject MeteoroPrefab;

    [Tooltip("Prefab da onda de choque")]
    public GameObject OndaDeChoquePrefab;

    [Tooltip("Área onde o meteoro cairá (BoxCollider2D)")]
    public BoxCollider2D MeteoroArea;

    [Tooltip("Área afetada pela onda de choque (BoxCollider2D)")]
    public BoxCollider2D OndaDeChoqueArea;

    [Tooltip("Tempo para o meteoro cair após ativar a habilidade")]
    public float MeteoroDelay = 1f;

    [Tooltip("Altura inicial da onda de choque")]
    public float AlturaInicialOnda = 2f;

    [Tooltip("Incremento de altura da onda a cada uso")]
    public float IncrementoAlturaOnda = 1f;

    [Tooltip("Largura alvo da onda de choque")]
    public float LarguraAlvoOnda = 10f; // Novo campo adicionado

    [Tooltip("Lista de plataformas a serem ativadas")]
    public List<GameObject> Plataformas;

    private int plataformasAtivadas = 0;

    private float _alturaAtualOnda;

    [Header("Cooldown")]
    [Tooltip("Duração do cooldown da habilidade Impacto Espiritual")]
    public float CooldownDuration = 10f; // Ajuste conforme necessário

    [Header("Feedbacks MMF Player")]
    [Tooltip("Feedback ao iniciar o Impacto Espiritual")]
    public MMF_Player AbilityStartFeedback;

    [Tooltip("Feedback ao spawnar o meteoro")]
    public MMF_Player MeteoroSpawnFeedback;

    [Tooltip("Feedback ao ocorrer o impacto do meteoro")]
    public MMF_Player MeteoroImpactFeedback;

    [Tooltip("Feedback ao spawnar a onda de choque")]
    public MMF_Player OndaDeChoqueFeedback;

    [Tooltip("Feedback ao ativar as plataformas")]
    public MMF_Player PlataformasAtivadasFeedback;

    [Tooltip("Prefab da partícula")]
    public GameObject ParticulaPrefab;

    public float VelocidadeParticula = 5f; // Velocidade de movimento das partículas

    private float _lastActivationTime = -Mathf.Infinity; // Armazena o momento da última ativação

    public UldrichPhaseManager _phaseManager;

    /// <summary>
    /// Propriedade que indica se a habilidade está permitida (herdada de CharacterAbility)
    /// </summary>
    public new bool AbilityPermitted => base.AbilityPermitted;

    /// <summary>
    /// Propriedade que indica se o cooldown terminou e a habilidade está pronta para uso
    /// </summary>
    public bool CooldownReady => Time.time >= _lastActivationTime + CooldownDuration;

    // Evento para indicar quando a habilidade foi concluída
    public event System.Action OnAbilityCompleted;

    private void Awake()
    {
        // Inicializa a altura atual da onda com a altura inicial definida
        _alturaAtualOnda = AlturaInicialOnda;
    }

    /// <summary>
    /// Método público para ativar a habilidade Impacto Espiritual
    /// </summary>
    public void ActivateAbility()
    {
        if (AbilityAuthorized)
        {
            if (Time.time >= _lastActivationTime + CooldownDuration)
            {
                _lastActivationTime = Time.time;
                StartCoroutine(ImpactoEspiritualRoutine());
            }
            else
            {
                // Opcional: Fornecer feedback indicando que a habilidade está em cooldown
                float cooldownRemaining = (_lastActivationTime + CooldownDuration) - Time.time;
                Debug.Log($"Impacto Espiritual está em cooldown. Tempo restante: {cooldownRemaining:F1} segundos.");
            }
        }
    }

    private IEnumerator ImpactoEspiritualRoutine()
    {
        // Feedback ao iniciar a habilidade
        if (AbilityStartFeedback != null)
        {
            AbilityStartFeedback.PlayFeedbacks();
        }

        // Aguarda o delay antes de cair o meteoro
        yield return new WaitForSeconds(MeteoroDelay);

        // Spawna o meteoro
        SpawnMeteoro();

        // A habilidade estará completa quando o meteoro atingir o chão e a onda de choque for gerada,
        // por isso a invocação do evento `OnAbilityCompleted` acontecerá após `OnMeteoroImpacto()` ser chamado
    }

    private void SpawnMeteoro()
    {
        // Feedback ao spawnar o meteoro
        if (MeteoroSpawnFeedback != null)
        {
            MeteoroSpawnFeedback.PlayFeedbacks();
        }

        // Posição do meteoro no topo da MeteoroArea
        Vector2 spawnPosition = new Vector2(MeteoroArea.bounds.center.x, MeteoroArea.bounds.max.y);

        // Instancia o meteoro
        GameObject meteoro = Instantiate(MeteoroPrefab, spawnPosition, Quaternion.identity);

        // Configura o controlador do meteoro
        MeteoroController meteoroController = meteoro.GetComponent<MeteoroController>();
        if (meteoroController != null)
        {
            meteoroController.Initialize(this);
        }
        else
        {
            Debug.LogError("O prefab do meteoro não possui o script MeteoroController!");
        }
    }

    /// <summary>
    /// Chamado pelo MeteoroController quando o meteoro atinge o chão
    /// </summary>
    public void OnMeteoroImpacto()
    {
        // Feedback ao ocorrer o impacto do meteoro
        if (MeteoroImpactFeedback != null)
        {
            MeteoroImpactFeedback.PlayFeedbacks();
        }

        // Spawna a onda de choque
        SpawnOndaDeChoque();

        // Se estiver na fase 1, ativa as plataformas
        if (_phaseManager.CurrentPhase == 1)
        {
            AtivarPlataformas();
        }

        // A habilidade foi concluída após o impacto do meteoro e o spawn da onda de choque
        OnAbilityCompleted?.Invoke();
    }

    private void SpawnOndaDeChoque()
    {
        // Posição de instanciação da onda de choque
        Vector2 spawnPosition = new Vector2(OndaDeChoqueArea.bounds.center.x, OndaDeChoqueArea.bounds.min.y);

        Debug.Log($"Spawnando OndaDeChoque na posição: {spawnPosition}");

        GameObject onda = Instantiate(OndaDeChoquePrefab, spawnPosition, Quaternion.identity);
        OndaDeChoque ondaController = onda.GetComponent<OndaDeChoque>();
        if (ondaController != null)
        {
            if (_alturaAtualOnda <= 0)
            {
                _alturaAtualOnda = AlturaInicialOnda;
                Debug.LogWarning($"_alturaAtualOnda estava <= 0. Foi redefinida para {AlturaInicialOnda}");
            }

            float LarguraAlvoOnda = 28f; // Ajuste conforme necessário

            ondaController.Initialize(LarguraAlvoOnda, _alturaAtualOnda);
            Debug.Log($"Inicializando OndaDeChoque com largura {LarguraAlvoOnda} e altura {_alturaAtualOnda}");
            _alturaAtualOnda += IncrementoAlturaOnda;

            // Instancia e move as partículas
            InstanciarParticulas(spawnPosition, LarguraAlvoOnda, ondaController);
        }
        else
        {
            Debug.LogError("O prefab da OndaDeChoque não possui o script OndaDeChoque!");
        }
    }

    private void InstanciarParticulas(Vector2 spawnPosition, float larguraAlvo, OndaDeChoque ondaController)
    {
        if (ParticulaPrefab != null)
        {

            Vector2 posicaoParticulas = new Vector2(
                spawnPosition.x,
                spawnPosition.y - (ondaController.AlturaOnda / 2f) + ondaController.OffsetVerticalParticulas
            );

            Debug.Log($"Instanciando partículas na posição: {posicaoParticulas}");

            // Instancia a partícula esquerda
            GameObject particulaEsquerda = Instantiate(ParticulaPrefab, posicaoParticulas, Quaternion.identity);
            // Instancia a partícula direita
            GameObject particulaDireita = Instantiate(ParticulaPrefab, posicaoParticulas, Quaternion.identity);

            // Inicia as corrotinas para mover as partículas
            StartCoroutine(MoverParticula(particulaEsquerda, Vector2.left, larguraAlvo / 2f, ondaController));
            StartCoroutine(MoverParticula(particulaDireita, Vector2.right, larguraAlvo / 2f, ondaController));
        }
        else
        {
            Debug.LogWarning("ParticulaPrefab não está atribuído no inspetor.");
        }
    }


    private IEnumerator MoverParticula(GameObject particula, Vector2 direcao, float distanciaMaxima, OndaDeChoque ondaController)
    {
        float distanciaPercorrida = 0f;
        Vector3 escalaInicial = particula.transform.localScale;
        Vector3 escalaFinal = escalaInicial * 2f; // Ajuste conforme necessário

        while (distanciaPercorrida < distanciaMaxima)
        {
            float deslocamento = VelocidadeParticula * Time.deltaTime;
            particula.transform.Translate(direcao * deslocamento);
            distanciaPercorrida += deslocamento;

            // Calcula a fração da distância percorrida
            float fracaoPercorrida = distanciaPercorrida / distanciaMaxima;

            // Sincroniza a escala da partícula com a largura atual da onda
            if (ondaController != null)
            {
                float larguraAtual = ondaController.ObterLarguraAtual() / ondaController.LarguraAlvo; // Fração da largura
                float escala = Mathf.Lerp(1f, 2f, larguraAtual); // Ajuste conforme necessário
                particula.transform.localScale = escalaInicial * escala;
            }

            yield return null;
        }

        // Destrói a partícula ao chegar no final
        Destroy(particula);
    }



    public void ResetarAlturaOnda()
    {
        _alturaAtualOnda = AlturaInicialOnda;
        Debug.Log("Altura da onda de choque foi resetada pelo Extinção da Alma.");
    }

    private void AtivarPlataformas()
    {
        // Quantidade de plataformas a ativar por chamada
        int quantidadeParaAtivar = 2;

        // Ativando as plataformas de acordo com a quantidade definida
        for (int i = plataformasAtivadas; i < plataformasAtivadas + quantidadeParaAtivar && i < Plataformas.Count; i++)
        {
            Plataformas[i].SetActive(true);
        }

        // Feedback ao ativar as plataformas
        if (PlataformasAtivadasFeedback != null)
        {
            PlataformasAtivadasFeedback.PlayFeedbacks();
        }

        // Atualizando a quantidade de plataformas já ativadas
        plataformasAtivadas += quantidadeParaAtivar;
    }

    // Método público para resetar o número de plataformas ativadas
    public void ResetarPlataformasAtivadas()
    {
        plataformasAtivadas = 0;
        Debug.Log("Plataformas ativadas foram resetadas pelo Extinção da Alma.");
    }

    protected virtual void OnDrawGizmosSelected()
    {
        if (MeteoroArea != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(MeteoroArea.bounds.center, MeteoroArea.bounds.size);
        }

        if (OndaDeChoqueArea != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(OndaDeChoqueArea.bounds.center, OndaDeChoqueArea.bounds.size);
        }
    }
}
