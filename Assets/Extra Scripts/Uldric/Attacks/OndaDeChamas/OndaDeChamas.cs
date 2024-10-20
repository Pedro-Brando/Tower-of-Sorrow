using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;

namespace MoreMountains.CorgiEngine
{
    [AddComponentMenu("Corgi Engine/Character/Abilities/Onda De Chamas")]
    public class OndaDeChamas : CharacterAbility, IUldrichAbility
    {
        [Header("Configurações da Onda de Chamas")]

        [Tooltip("Prefab da bola de fogo")]
        public GameObject FireballPrefab;

        [Tooltip("Pooler para as bolas de fogo")]
        public MMSimpleObjectPooler FireballPooler;

        [Tooltip("Número de paredes por uso da habilidade")]
        public int NumberOfWalls = 5;

        [Tooltip("Delay entre as paredes")]
        public float WallDelay = 1f;

        [Tooltip("Velocidade das bolas de fogo")]
        public float FireballSpeed = 5f;

        [Tooltip("Dano de cada bola de fogo")]
        public int FireballDamage = 1;

        [Tooltip("Altura (número de linhas) da parede")]
        public int WallHeight = 5;

        [Tooltip("Espaçamento vertical entre as bolas de fogo")]
        public float VerticalSpacing = 1f;

        [Tooltip("Ponto de spawn das paredes")]
        public Transform WallSpawnPoint;

        private List<int[]> _patterns;

        [Header("Cooldown")]
        [Tooltip("Duração do cooldown da habilidade Onda de Chamas")]
        public float CooldownDuration = 15f; // Ajuste conforme necessário

        [Header("Feedbacks MMF Player")]
        [Tooltip("Feedback ao iniciar a habilidade Onda de Chamas")]
        public MMF_Player AbilityStartFeedback;

        [Tooltip("Feedback ao spawnar cada parede de chamas")]
        public MMF_Player WallSpawnFeedback;

        [Tooltip("Feedback ao concluir a habilidade Onda de Chamas")]
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

        protected override void Initialization()
        {
            base.Initialization();

            // Inicializa os padrões
            _patterns = new List<int[]>
            {
                // Padrão 1: Ficar parado para desviar (abertura na linha inferior)
                new int[] { 0, 1, 1, 1, 1 },
                // Padrão 2: Um pulo para desviar (abertura no meio)
                new int[] { 1, 0, 0, 1, 1 },
                // Padrão 3: Pulo duplo para desviar (abertura na linha superior)
                new int[] { 1, 1, 1, 1, 0 }
            };

            // Verifica se o Pooler está atribuído
            if (FireballPooler == null)
            {
                Debug.LogError("FireballPooler não está atribuído no OndaDeChamas!");
            }

            // Verifica se o WallSpawnPoint está atribuído
            if (WallSpawnPoint == null)
            {
                Debug.LogError("WallSpawnPoint não está atribuído no OndaDeChamas!");
            }
        }

        public void ActivateAbility()
        {
            if (AbilityAuthorized)
            {
                if (Time.time >= _lastActivationTime + CooldownDuration)
                {
                    _lastActivationTime = Time.time;
                    StartCoroutine(OndaDeChamasRoutine());
                }
                else
                {
                    // Opcional: Fornecer feedback indicando que a habilidade está em cooldown
                    float cooldownRemaining = (_lastActivationTime + CooldownDuration) - Time.time;
                    Debug.Log($"Onda de Chamas está em cooldown. Tempo restante: {cooldownRemaining:F1} segundos.");
                }
            }
        }

        private IEnumerator OndaDeChamasRoutine()
        {
            // Feedback ao iniciar a habilidade
            if (AbilityStartFeedback != null)
            {
                AbilityStartFeedback.PlayFeedbacks();
            }

            for (int i = 0; i < NumberOfWalls; i++)
            {
                SpawnWall();
                yield return new WaitForSeconds(WallDelay);
            }

            // Feedback ao concluir a habilidade
            if (AbilityCompleteFeedback != null)
            {
                AbilityCompleteFeedback.PlayFeedbacks();
            }

            // A habilidade está completa após todas as paredes serem geradas
            OnAbilityCompleted?.Invoke();
        }

        private void SpawnWall()
        {
            // Seleciona um padrão aleatório
            int patternIndex = Random.Range(0, _patterns.Count);
            int[] pattern = _patterns[patternIndex];

            Vector3 spawnPosition = WallSpawnPoint.position;

            for (int i = 0; i < WallHeight; i++)
            {
                if (pattern[i] == 1)
                {
                    Vector3 fireballPosition = spawnPosition + new Vector3(0, i * VerticalSpacing, 0);
                    GameObject fireball = FireballPooler.GetPooledGameObject();
                    if (fireball != null)
                    {
                        fireball.transform.position = fireballPosition;
                        fireball.transform.rotation = Quaternion.identity;
                        fireball.SetActive(true);

                        // Inicializa a bola de fogo
                        Fireball fireballScript = fireball.GetComponent<Fireball>();
                        if (fireballScript != null)
                        {
                            fireballScript.Speed = FireballSpeed;
                            fireballScript.Damage = FireballDamage;
                            Vector2 direction = Vector2.left; // Ajuste conforme a direção desejada
                            fireballScript.Initialize(direction);
                        }
                        else
                        {
                            Debug.LogWarning("O prefab da bola de fogo não possui o script Fireball.");
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Nenhuma bola de fogo disponível no pool!");
                    }
                }
            }

            // Feedback ao spawnar cada parede de chamas
            if (WallSpawnFeedback != null)
            {
                WallSpawnFeedback.PlayFeedbacks();
            }
        }
    }
}
