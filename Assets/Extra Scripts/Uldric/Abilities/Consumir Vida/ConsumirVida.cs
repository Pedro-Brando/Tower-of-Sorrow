using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;

namespace MoreMountains.CorgiEngine
{
    [AddComponentMenu("Corgi Engine/Character/Abilities/Consumir Vida")]
    public class ConsumirVida : CharacterAbility, IUldrichAbility
    {
        [Header("Configurações de Consumir Vida")]

        [Tooltip("Prefab do crosshair (mira)")]
        public GameObject CrosshairPrefab;

        [Tooltip("Prefab da explosão")]
        public GameObject ExplosionPrefab;

        [Tooltip("Número total de crosshairs a serem gerados")]
        public int TotalCrosshairs = 6;

        [Tooltip("Número máximo de crosshairs ativos simultaneamente")]
        public int MaxActiveCrosshairs = 3;

        [Tooltip("Tempo entre o spawn de crosshairs")]
        public float CrosshairSpawnDelay = 0.5f;

        [Tooltip("Tempo antes da explosão após o crosshair aparecer")]
        public float TimeBeforeExplosion = 1f;

        [Tooltip("Dano causado pela explosão")]
        public int ExplosionDamage = 1;

        [Tooltip("Layers que serão afetadas pela explosão")]
        public LayerMask DamageableLayers;

        [Header("Pooling")]

        [Tooltip("Pooler para os crosshairs")]
        public MMSimpleObjectPooler CrosshairPooler;

        [Tooltip("Pooler para as explosões")]
        public MMSimpleObjectPooler ExplosionPooler;

        [Header("Cooldown")]
        [Tooltip("Duração do cooldown da habilidade Consumir Vida")]
        public float CooldownDuration = 10f; // Ajuste conforme necessário

        [Header("Feedbacks MMF Player")]
        [Tooltip("Feedback ao iniciar a habilidade Consumir Vida")]
        public MMF_Player AbilityStartFeedback;

        [Tooltip("Feedback ao spawnar um crosshair")]
        public MMF_Player CrosshairSpawnFeedback;

        [Tooltip("Feedback ao ocorrer a explosão")]
        public MMF_Player ExplosionFeedback;

        [Tooltip("Feedback ao concluir a habilidade Consumir Vida")]
        public MMF_Player AbilityCompleteFeedback;

        [Header("Referências")]
        [Tooltip("Referência ao GameObject do jogador")]
        public GameObject PlayerReference;

        private bool _abilityInProgress = false;
        private float _lastActivationTime = -Mathf.Infinity; // Armazena o momento da última ativação

        /// <summary>
        /// Propriedade que indica se a habilidade está permitida (herdada de CharacterAbility)
        /// </summary>
        public new bool AbilityPermitted => base.AbilityPermitted;

        /// <summary>
        /// Propriedade que indica se o cooldown terminou e a habilidade está pronta para uso
        /// </summary>
        public bool CooldownReady => Time.time >= _lastActivationTime + CooldownDuration;

        // Evento para indicar quando a habilidade for concluída
        public event System.Action OnAbilityCompleted;

        /// <summary>
        /// Método público para ativar a habilidade Consumir Vida
        /// </summary>
        public void ActivateAbility()
        {
            if (!_abilityInProgress && AbilityAuthorized)
            {
                if (Time.time >= _lastActivationTime + CooldownDuration)
                {
                    _lastActivationTime = Time.time;
                    StartCoroutine(ConsumirVidaRoutine());
                }
                else
                {
                    // Opcional: Fornecer feedback indicando que a habilidade está em cooldown
                    float cooldownRemaining = (_lastActivationTime + CooldownDuration) - Time.time;
                    Debug.Log($"Consumir Vida está em cooldown. Tempo restante: {cooldownRemaining:F1} segundos.");
                }
            }
        }

        private IEnumerator ConsumirVidaRoutine()
        {
            _abilityInProgress = true;

            // Feedback ao iniciar a habilidade
            if (AbilityStartFeedback != null)
            {
                AbilityStartFeedback.PlayFeedbacks();
            }

            int crosshairsSpawned = 0;
            List<GameObject> activeCrosshairs = new List<GameObject>();

            while (crosshairsSpawned < TotalCrosshairs)
            {
                if (activeCrosshairs.Count < MaxActiveCrosshairs)
                {
                    // Verifica se a referência ao jogador está atribuída
                    if (PlayerReference == null)
                    {
                        Debug.LogError("PlayerReference não está atribuída!");
                        yield break;
                    }

                    // Posição do jogador
                    Vector2 spawnPosition = PlayerReference.transform.position;

                    // Obtém um crosshair do pool
                    GameObject crosshair = CrosshairPooler.GetPooledGameObject();
                    if (crosshair != null)
                    {
                        crosshair.transform.position = spawnPosition;
                        crosshair.transform.rotation = Quaternion.identity;
                        crosshair.SetActive(true);

                        // Feedback ao spawnar o crosshair
                        if (CrosshairSpawnFeedback != null)
                        {
                            CrosshairSpawnFeedback.PlayFeedbacks();
                        }

                        // Inicia a rotina da explosão
                        StartCoroutine(CrosshairRoutine(crosshair));

                        activeCrosshairs.Add(crosshair);
                        crosshairsSpawned++;
                    }
                    else
                    {
                        Debug.LogWarning("Nenhum crosshair disponível no pool!");
                    }
                }

                // Remove crosshairs inativos da lista
                activeCrosshairs.RemoveAll(item => !item.activeInHierarchy);

                yield return new WaitForSeconds(CrosshairSpawnDelay);
            }

            // Espera todos os crosshairs terminarem
            while (activeCrosshairs.Count > 0)
            {
                activeCrosshairs.RemoveAll(item => !item.activeInHierarchy);
                yield return null;
            }

            // Feedback ao concluir a habilidade
            if (AbilityCompleteFeedback != null)
            {
                AbilityCompleteFeedback.PlayFeedbacks();
            }

            // Aciona o evento indicando que a habilidade foi concluída
            OnAbilityCompleted?.Invoke();
            _abilityInProgress = false;
        }

        private IEnumerator CrosshairRoutine(GameObject crosshair)
        {
            // Aguarda o tempo antes da explosão
            yield return new WaitForSeconds(TimeBeforeExplosion);

            // Obtém uma explosão do pool
            GameObject explosion = ExplosionPooler.GetPooledGameObject();
            if (explosion != null)
            {
                explosion.transform.position = crosshair.transform.position;
                explosion.transform.rotation = Quaternion.identity;
                explosion.SetActive(true);

                // Feedback ao ocorrer a explosão
                if (ExplosionFeedback != null)
                {
                    ExplosionFeedback.PlayFeedbacks();
                }

                // Aplica dano às entidades na área da explosão
                ApplyDamageInArea(explosion.transform.position);

                // Desativa a explosão após o tempo definido (ajuste conforme a duração da animação)
                yield return new WaitForSeconds(1f);
                explosion.SetActive(false);
            }
            else
            {
                Debug.LogWarning("Nenhuma explosão disponível no pool!");
            }

            // Desativa o crosshair
            crosshair.SetActive(false);
        }

        /// <summary>
        /// Aplica dano às entidades na área da explosão
        /// </summary>
        /// <param name="explosionPosition">Posição da explosão</param>
        private void ApplyDamageInArea(Vector2 explosionPosition)
        {
            float explosionRadius = 1f; // Ajuste conforme o tamanho da sua explosão

            Collider2D[] hits = Physics2D.OverlapCircleAll(
                explosionPosition,
                explosionRadius,
                DamageableLayers);

            foreach (Collider2D hit in hits)
            {
                Health healthComponent = hit.GetComponent<Health>();
                if (healthComponent != null)
                {
                    healthComponent.Damage(ExplosionDamage, gameObject, 0f, 0f, transform.position);
                    Debug.Log($"{hit.name} recebeu dano da explosão.");
                }
            }
        }
    }
}
