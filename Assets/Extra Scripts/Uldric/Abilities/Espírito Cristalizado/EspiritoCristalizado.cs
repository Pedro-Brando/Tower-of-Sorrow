using UnityEngine;
using System.Collections;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;

public class EspiritoCristalizado : CharacterAbility, IUldrichAbility
{
    [Header("Configurações da Habilidade")]

    [Tooltip("Prefab do cristal")]
    public GameObject CristalPrefab;

    [Tooltip("Área onde os cristais podem aparecer (BoxCollider2D)")]
    public BoxCollider2D SpawnArea;

    [Tooltip("Número de cristais a serem gerados por ativação")]
    public int NumberOfCrystals = 3;

    [Tooltip("Tempo entre o spawn de cada cristal")]
    public float SpawnDelay = 1f;

    [Tooltip("Tempo que o cristal fica flutuando antes de cair")]
    public float FloatTime = 2f;

    [Tooltip("Prefab do portal a ser instanciado quando o cristal for destruído")]
    public GameObject PortalPrefab;

    [Header("Cooldown")]
    [Tooltip("Duração do cooldown da habilidade Espírito Cristalizado")]
    public float CooldownDuration = 15f; // Ajuste conforme necessário

    [Header("Feedbacks MMF Player")]
    [Tooltip("Feedback ao iniciar a habilidade Espírito Cristalizado")]
    public MMF_Player AbilityStartFeedback;

    [Tooltip("Feedback ao spawnar um cristal")]
    public MMF_Player CrystalSpawnFeedback;

    [Tooltip("Feedback ao concluir a habilidade Espírito Cristalizado")]
    public MMF_Player AbilityCompleteFeedback;

    private float _lastActivationTime = -Mathf.Infinity; // Armazena o momento da última ativação

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
    /// Método público para ativar a habilidade Espírito Cristalizado
    /// </summary>
    public void ActivateAbility()
    {
        if (AbilityAuthorized)
        {
            if (Time.time >= _lastActivationTime + CooldownDuration)
            {
                _lastActivationTime = Time.time;
                StartCoroutine(SpawnCrystals());
            }
            else
            {
                // Opcional: Fornecer feedback indicando que a habilidade está em cooldown
                float cooldownRemaining = (_lastActivationTime + CooldownDuration) - Time.time;
                Debug.Log($"Espírito Cristalizado está em cooldown. Tempo restante: {cooldownRemaining:F1} segundos.");
            }
        }
    }

    private IEnumerator SpawnCrystals()
    {
        // Feedback ao iniciar a habilidade
        if (AbilityStartFeedback != null)
        {
            AbilityStartFeedback.PlayFeedbacks();
        }

        Debug.Log($"EspiritoCristalizado: Iniciando SpawnCrystals(). Número de cristais: {NumberOfCrystals}");

        for (int i = 0; i < NumberOfCrystals; i++)
        {
            SpawnCristal();
            yield return new WaitForSeconds(SpawnDelay);
        }

        Debug.Log("EspiritoCristalizado: SpawnCrystals() concluído.");

        // Feedback ao concluir a habilidade
        if (AbilityCompleteFeedback != null)
        {
            AbilityCompleteFeedback.PlayFeedbacks();
        }

        // Acionar o evento indicando que a habilidade foi concluída
        OnAbilityCompleted?.Invoke();
    }

    private void SpawnCristal()
    {
        // Seleciona uma posição aleatória dentro da área de spawn
        Vector2 spawnPosition = GetRandomPositionInArea();
        Debug.Log($"EspiritoCristalizado: Spawning cristal na posição {spawnPosition}");

        // Instancia o cristal
        GameObject cristal = Instantiate(CristalPrefab, spawnPosition, Quaternion.identity);
        Debug.Log("EspiritoCristalizado: Cristal instanciado.");

        // Feedback ao spawnar o cristal
        if (CrystalSpawnFeedback != null)
        {
            CrystalSpawnFeedback.PlayFeedbacks();
        }

        // Configura o controlador do cristal
        CristalController cristalController = cristal.GetComponent<CristalController>();
        if (cristalController != null)
        {
            cristalController.FloatTime = FloatTime;
            Debug.Log("EspiritoCristalizado: Parâmetros do cristal configurados.");
        }
        else
        {
            Debug.LogError("EspiritoCristalizado: O prefab do cristal não possui o script CristalController!");
        }
    }

    private Vector2 GetRandomPositionInArea()
    {
        if (SpawnArea == null)
        {
            Debug.LogError("EspiritoCristalizado: SpawnArea não está atribuída!");
            return Vector2.zero;
        }

        Bounds bounds = SpawnArea.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = bounds.max.y; // Topo da área

        return new Vector2(x, y);
    }

    protected virtual void OnDrawGizmosSelected()
    {
        if (SpawnArea != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(SpawnArea.bounds.center, SpawnArea.bounds.size);
        }
    }
}
