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
        // Feedback ao spawnar a onda de choque
        if (OndaDeChoqueFeedback != null)
        {
            OndaDeChoqueFeedback.PlayFeedbacks();
        }

        // Posição inicial da onda de choque na base da OndaDeChoqueArea
        Vector2 spawnPosition = new Vector2(OndaDeChoqueArea.bounds.center.x, OndaDeChoqueArea.bounds.min.y);

        // Instancia a onda de choque
        GameObject onda = Instantiate(OndaDeChoquePrefab, spawnPosition, Quaternion.identity);

        // Configura o controlador da onda de choque
        OndaDeChoque ondaController = onda.GetComponent<OndaDeChoque>();
        if (ondaController != null)
        {
            _alturaAtualOnda += IncrementoAlturaOnda;
            ondaController.Initialize(OndaDeChoqueArea, _alturaAtualOnda);
        }
        else
        {
            Debug.LogError("O prefab da onda de choque não possui o script OndaDeChoque!");
        }
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
