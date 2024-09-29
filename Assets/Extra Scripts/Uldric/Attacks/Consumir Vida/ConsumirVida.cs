using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
    [AddComponentMenu("Corgi Engine/Character/Abilities/Consumir Vida")]
    public class ConsumirVida : CharacterAbility
    {
        [Header("Configurações de Consumir Vida")]

        [Tooltip("Prefab do crosshair (mira)")]
        public GameObject CrosshairPrefab;

        [Tooltip("Prefab da explosão")]
        public GameObject ExplosionPrefab;

        [Tooltip("Área onde os crosshairs podem aparecer (BoxCollider2D)")]
        public BoxCollider2D SpawnArea;

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

        private bool _abilityInProgress = false;

        /// <summary>
        /// Método público para ativar a habilidade Consumir Vida
        /// </summary>
        public void ActivateAbility()
        {
            if (!_abilityInProgress)
            {
                StartCoroutine(ConsumirVidaRoutine());
            }
        }

        private IEnumerator ConsumirVidaRoutine()
        {
            _abilityInProgress = true;

            int crosshairsSpawned = 0;
            List<GameObject> activeCrosshairs = new List<GameObject>();

            while (crosshairsSpawned < TotalCrosshairs)
            {
                if (activeCrosshairs.Count < MaxActiveCrosshairs)
                {
                    // Seleciona uma posição aleatória dentro da área
                    Vector2 randomPosition = GetRandomPositionInArea();

                    // Obtém um crosshair do pool
                    GameObject crosshair = CrosshairPooler.GetPooledGameObject();
                    if (crosshair != null)
                    {
                        crosshair.transform.position = randomPosition;
                        crosshair.transform.rotation = Quaternion.identity;
                        crosshair.SetActive(true);

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

                // Aplica dano às entidades na área da explosão
                ApplyDamageInArea(explosion.transform.position);

                // Desativa a explosão após o tempo definido (assumindo que a animação dura 1 segundo)
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

        /// <summary>
        /// Retorna uma posição aleatória dentro do SpawnArea
        /// </summary>
        /// <returns>Posição aleatória dentro da área</returns>
        private Vector2 GetRandomPositionInArea()
        {
            if (SpawnArea == null)
            {
                Debug.LogError("SpawnArea não está atribuída!");
                return Vector2.zero;
            }

            Bounds bounds = SpawnArea.bounds;

            float x = Random.Range(bounds.min.x, bounds.max.x);
            float y = Random.Range(bounds.min.y, bounds.max.y);

            return new Vector2(x, y);
        }

        /// <summary>
        /// Visualiza a área de spawn no editor
        /// </summary>
        protected virtual void OnDrawGizmosSelected()
        {
            if (SpawnArea != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(SpawnArea.bounds.center, SpawnArea.bounds.size);
            }
        }
    }
}
