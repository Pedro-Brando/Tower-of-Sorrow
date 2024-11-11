using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;

namespace MoreMountains.CorgiEngine
{
    [AddComponentMenu("Corgi Engine/Character/Abilities/Hod/Desmoronar")]
    public class Desmoronar : CharacterAbility, IHodAbility
    {
        [Header("Configurações do Desmoronar")]

        [Tooltip("Lista de prefabs dos escombros que caem do teto")]
        public List<GameObject> DebrisPrefabs;

        [Tooltip("Intervalo entre o spawn dos escombros")]
        public float SpawnInterval = 0.5f;

        [Tooltip("Número total de escombros a serem spawnados")]
        public int TotalDebris = 10;

        [Tooltip("Velocidade dos escombros ao cair")]
        public float DebrisSpeed = 5f;

        [Tooltip("Chance de um escombro se tornar permanente (0 a 1)")]
        public float PermanentDebrisChance = 0.25f;

        [Tooltip("Duração do cooldown da habilidade")]
        public float CooldownDuration = 5f;

        [Header("Feedbacks MMF Player")]
        [Tooltip("Feedback ao iniciar a habilidade Desmoronar")]
        public MMF_Player DesmoronarFeedback;

        private float _lastActivationTime = -Mathf.Infinity;

        public GameObject _player;
        private HodController _hodController;

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
        /// Método público para ativar a habilidade Desmoronar
        /// </summary>
        public void ActivateAbility()
        {
            if (AbilityAuthorized && CooldownReady)
            {
                _lastActivationTime = Time.time;
                StartCoroutine(DesmoronarRoutine());
            }
        }

        /// <summary>
        /// Coroutine que gerencia a execução da habilidade Desmoronar
        /// </summary>
        private IEnumerator DesmoronarRoutine()
        {
            // Início da habilidade
            Debug.Log("Hod começou a usar Desmoronar.");

            // Feedback ao iniciar a habilidade
            if (DesmoronarFeedback != null)
            {
                DesmoronarFeedback.PlayFeedbacks();
            }

            // Executar o spawn dos escombros
            for (int i = 0; i < TotalDebris; i++)
            {
                SpawnDebris();
                yield return new WaitForSeconds(SpawnInterval);
            }

            // Acionar o evento indicando que a habilidade foi concluída
            OnAbilityCompleted?.Invoke();
        }

        /// <summary>
        /// Spawna um escombro em uma posição aleatória acima do jogador
        /// </summary>
        private void SpawnDebris()
        {
            if (DebrisPrefabs == null || DebrisPrefabs.Count == 0)
            {
                Debug.LogError("A lista DebrisPrefabs está vazia! Não é possível spawnar escombros.");
                return;
            }

            Vector3 spawnPosition = GetRandomSpawnPositionAbovePlayer();

            // Seleciona um prefab aleatório da lista
            int randomIndex = Random.Range(0, DebrisPrefabs.Count);
            GameObject selectedPrefab = DebrisPrefabs[randomIndex];

            GameObject debris = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
            Rigidbody2D rb = debris.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.velocity = Vector2.down * DebrisSpeed;
            }

            // Determinar se o escombro será permanente
            bool isPermanent = UnityEngine.Random.value <= PermanentDebrisChance;
            if (isPermanent)
            {
                debris.AddComponent<PermanentObstacle>();
            }
            else
            {
                Destroy(debris, 5f); // Destrói o escombro após 5 segundos
            }
        }

        /// <summary>
        /// Obtém uma posição aleatória acima do jogador para spawnar o escombro
        /// </summary>
        /// <returns>Posição de spawn</returns>
        private Vector3 GetRandomSpawnPositionAbovePlayer()
        {
            if (_player != null)
            {
                Vector3 playerPosition = _player.transform.position;
                float randomX = playerPosition.x + UnityEngine.Random.Range(-5f, 5f);
                float spawnY = playerPosition.y + 10f; // Altura acima do jogador
                return new Vector3(randomX, spawnY, playerPosition.z);
            }
            else
            {
                // Caso o jogador não seja encontrado, spawnar em uma posição padrão
                return transform.position + new Vector3(0f, 10f, 0f);
            }
        }

        /// <summary>
        /// Visualiza a área de spawn dos escombros no editor
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            if (_player != null)
            {
                Gizmos.color = Color.gray;
                Vector3 playerPosition = _player.transform.position;
                Vector3 center = playerPosition + new Vector3(0f, 10f, 0f);
                Vector3 size = new Vector3(10f, 1f, 0f); // Área de 10 unidades em X
                Gizmos.DrawWireCube(center, size);
            }
        }
    }

    /// <summary>
    /// Componente para marcar o escombro como obstáculo permanente
    /// </summary>
    public class PermanentObstacle : MonoBehaviour
    {
        private void Start()
        {
            // Ajustar o objeto para ser um obstáculo permanente
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.isKinematic = true;
            }

            Collider2D collider = GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.isTrigger = false;
            }

            // Alterar a layer para "Obstacles" ou outra layer adequada
            gameObject.layer = LayerMask.NameToLayer("MovingPlatforms");
        }
    }
}
