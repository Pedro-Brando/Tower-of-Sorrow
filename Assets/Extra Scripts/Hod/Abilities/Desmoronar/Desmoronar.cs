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

        [Tooltip("Velocidade dos escombros ao cair (unidades por segundo)")]
        public float DebrisFallSpeed = 5f;

        [Tooltip("Chance de um escombro se tornar permanente (0 a 1)")]
        public float PermanentDebrisChance = 0.25f;

        [Tooltip("Duração do cooldown da habilidade")]
        public float CooldownDuration = 5f;

        [Tooltip("Área onde os escombros podem aparecer (BoxCollider2D)")]
        public BoxCollider2D SpawnArea;

        [Header("Feedbacks MMF Player")]
        [Tooltip("Feedback ao iniciar a habilidade Desmoronar")]
        public MMF_Player DesmoronarFeedback;

        private float _lastActivationTime = -Mathf.Infinity;

        private GameObject _player;
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

        protected override void Initialization()
        {
            base.Initialization();
            _player = GameObject.FindGameObjectWithTag("Player");
            if (_player == null)
            {
                Debug.LogError("Player não encontrado na cena!");
            }

            _hodController = GetComponent<HodController>();
            if (_hodController == null)
            {
                Debug.LogError("HodController não encontrado no GameObject!");
            }

            if (DebrisPrefabs == null || DebrisPrefabs.Count == 0)
            {
                Debug.LogError("A lista DebrisPrefabs está vazia! Por favor, atribua pelo menos um prefab.");
            }

            if (SpawnArea == null)
            {
                Debug.LogError("SpawnArea não está atribuída! Por favor, atribua um BoxCollider2D.");
            }
        }

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
        /// Spawna um escombro em uma posição aleatória dentro da área definida
        /// </summary>
        private void SpawnDebris()
        {
            if (DebrisPrefabs == null || DebrisPrefabs.Count == 0)
            {
                Debug.LogError("A lista DebrisPrefabs está vazia! Não é possível spawnar escombros.");
                return;
            }

            Vector3 spawnPosition = GetRandomPositionInArea();

            // Seleciona um prefab aleatório da lista
            int randomIndex = Random.Range(0, DebrisPrefabs.Count);
            GameObject selectedPrefab = DebrisPrefabs[randomIndex];

            // Instancia o escombro
            GameObject debris = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);

            // Determinar se o escombro será permanente
            bool isPermanent = UnityEngine.Random.value <= PermanentDebrisChance;

            // Adicionar e configurar o script DebrisFaller para controlar a queda
            DebrisFaller debrisFaller = debris.AddComponent<DebrisFaller>();
            debrisFaller.FallSpeed = DebrisFallSpeed;
            debrisFaller.IsPermanent = isPermanent;

            // Definir a layer dos escombros para evitar colisões entre si
            debris.layer = LayerMask.NameToLayer("Platforms");

            // Configurar o Collider e Rigidbody com base na permanência
            Collider2D debrisCollider = debris.GetComponent<Collider2D>();
            if (debrisCollider != null)
            {
                if (isPermanent)
                {
                    debrisCollider.isTrigger = false; // Collider sólido para obstáculos
                }
                else
                {
                    debrisCollider.isTrigger = true; // Collider como Trigger para detecção de colisões
                }
            }
            else
            {
                Debug.LogError("Prefab de escombro não possui um Collider2D!");
            }

            Rigidbody2D rb = debris.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.isKinematic = true; // Tornar kinematic, pois a queda é controlada via código
                rb.gravityScale = 0f; // Desativar a gravidade
            }
            else
            {
                Debug.LogError("Prefab de escombro não possui um Rigidbody2D!");
            }
        }

        /// <summary>
        /// Obtém uma posição aleatória dentro da área de spawn
        /// </summary>
        /// <returns>Posição de spawn</returns>
        private Vector3 GetRandomPositionInArea()
        {
            if (SpawnArea == null)
            {
                Debug.LogError("SpawnArea não está atribuída!");
                return Vector3.zero;
            }

            Bounds bounds = SpawnArea.bounds;

            float x = Random.Range(bounds.min.x, bounds.max.x);
            float y = bounds.max.y; // Topo da área

            return new Vector3(x, y, 0f);
        }

        /// <summary>
        /// Método para finalizar a habilidade Desmoronar antecipadamente ou após a duração
        /// </summary>
        public void EndDispersar()
        {
            // Destruir todas as cópias na cena
            foreach (DebrisFaller debrisFaller in FindObjectsOfType<DebrisFaller>())
            {
                Destroy(debrisFaller.gameObject);
            }

            // Fazer Hod reaparecer na posição original ou em uma posição específica
            _hodController.SetVisible(true);

            // Iniciar o cooldown
            StartCoroutine(CooldownRoutine());
        }

        /// <summary>
        /// Inicia o cooldown da habilidade
        /// </summary>
        private IEnumerator CooldownRoutine()
        {
            yield return new WaitForSeconds(CooldownDuration);
            // Cooldown terminado
        }

        /// <summary>
        /// Visualiza a área de spawn dos escombros no editor
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            if (SpawnArea != null)
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawWireCube(SpawnArea.bounds.center, SpawnArea.bounds.size);
            }
        }

        /// <summary>
        /// Componente que controla a queda dos escombros e interações com o jogador
        /// </summary>
        public class DebrisFaller : MonoBehaviour
        {
            [Tooltip("Velocidade da queda (unidades por segundo)")]
            public float FallSpeed = 5f;

            [Tooltip("Determina se o escombro é permanente")]
            public bool IsPermanent = false;

            // Removido: [Tooltip("Referência ao jogador para aplicar dano")]
            // public GameObject Player;

            private bool _hasLanded = false;

            void Update()
            {
                if (!_hasLanded)
                {
                    // Move o escombro para baixo de forma consistente
                    transform.Translate(Vector3.down * FallSpeed * Time.deltaTime);
                }
            }

            private void OnTriggerEnter2D(Collider2D collision)
            {
                if (_hasLanded) return;

                Debug.Log($"{gameObject.name} entrou em trigger com {collision.gameObject.name} ({collision.tag}).");

                if (collision.CompareTag("Ground") || collision.CompareTag("Wall"))
                {
                    _hasLanded = true;
                    Debug.Log($"{gameObject.name} colidiu com Ground ou Wall.");

                    if (!IsPermanent)
                    {
                        // Destruir o escombro após atingir o chão
                        Destroy(gameObject, 5f);
                        Debug.Log($"{gameObject.name} será destruído em 5 segundos.");
                    }
                }
                else if (collision.CompareTag("Player"))
                {
                    _hasLanded = false; // Corrigido para true
                    Debug.Log($"{gameObject.name} colidiu com Player.");

                    // Aplicar dano ao jogador
                    Health playerHealth = collision.GetComponent<Health>();
                    if (playerHealth != null)
                    {
                        Debug.Log($"{gameObject.name} está causando dano ao jogador.");
                        playerHealth.Damage(1f, gameObject, 0.1f, 0.5f, Vector3.zero); // Ajuste o valor de dano conforme necessário
                    }
                    else
                    {
                        Debug.LogError("Health component não encontrado no jogador!");
                    }
                    // Destruir o escombro imediatamente após causar dano
                    Destroy(gameObject);
                    Debug.Log($"{gameObject.name} foi destruído após causar dano.");
                }
            }
        }
    }
}
