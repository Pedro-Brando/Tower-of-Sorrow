using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.CorgiEngine;
using MoreMountains.Feedbacks;

namespace MoreMountains.CorgiEngine
{
    [AddComponentMenu("Corgi Engine/Character/Abilities/Tempestade De Fogo Fátuo")]
    public class TempestadeDeFogoFatuo : CharacterAbility, IUldrichAbility
    {
        [Header("Configurações da Tempestade de Fogo Fátuo")]

        [Tooltip("Número de whisps que serão instanciados de cada lado")]
        public int NumberOfWhispsPerSide = 10;

        [Tooltip("Espaçamento vertical entre os whisps")]
        public float WhispSpacing = 1.0f;

        [Tooltip("Velocidade dos whisps")]
        public float WhispSpeed = 5f;

        [Tooltip("Dano causado pelos whisps ao colidir com o jogador")]
        public float WhispDamage = 1f;

        [Tooltip("Delay entre o spawn de cada whisp")]
        public float SpawnDelay = 0.1f;

        [Tooltip("Prefab do whisp")]
        public GameObject WhispPrefab;

        [Tooltip("Referência ao MMSimpleObjectPooler")]
        public MMSimpleObjectPooler WhispPooler;

        [Header("Referências de Spawn")]

        [Tooltip("Transform que define a posição de spawn dos whisps da esquerda")]
        public Transform LeftSpawnPoint;

        [Tooltip("Transform que define a posição de spawn dos whisps da direita")]
        public Transform RightSpawnPoint;

        [Header("Cooldown")]
        [Tooltip("Duração do cooldown da habilidade")]
        public float CooldownDuration = 10f; // Ajuste conforme necessário

        [Header("Feedbacks MMF Player")]
        [Tooltip("Feedback ao iniciar o cast da Tempestade de Fogo Fátuo")]
        public MMF_Player CastFeedback;

        [Tooltip("Feedback ao spawnar cada whisp da Tempestade de Fogo Fátuo")]
        public MMF_Player WhispSpawnFeedback;

        [Tooltip("Feedback ao completar a habilidade da Tempestade de Fogo Fátuo")]
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
        /// Método público para ativar a habilidade
        /// </summary>
        public void ActivateAbility()
        {
            if (AbilityAuthorized)
            {
                if (Time.time >= _lastActivationTime + CooldownDuration)
                {
                    _lastActivationTime = Time.time;
                    StartCoroutine(TempestadeDeFogoFatuoRoutine());
                }
                else
                {
                    // Fornecer feedback indicando que a habilidade está em cooldown
                    float cooldownRemaining = (_lastActivationTime + CooldownDuration) - Time.time;
                    Debug.Log($"Tempestade de Fogo Fátuo está em cooldown. Tempo restante: {cooldownRemaining:F1} segundos.");
                }
            }
        }

        /// <summary>
        /// Coroutine que gerencia a execução da habilidade Tempestade de Fogo Fátuo
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator TempestadeDeFogoFatuoRoutine()
        {
            Debug.Log("Tempestade de Fogo Fátuo iniciada!");

            // Feedback ao iniciar o cast da habilidade
            if (CastFeedback != null)
            {
                CastFeedback.PlayFeedbacks();
            }

            // Calcular a posição inicial e espaçamento vertical
            float totalSpacing = (NumberOfWhispsPerSide - 1) * WhispSpacing;
            float initialOffsetY = -totalSpacing / 2;

            // Spawn dos whisps da esquerda
            for (int i = 0; i < NumberOfWhispsPerSide; i++)
            {
                Vector3 spawnPosition = LeftSpawnPoint.position + new Vector3(0, initialOffsetY + i * WhispSpacing, 0);
                Vector2 direction = Vector2.right * WhispSpeed;

                SpawnWhisp(spawnPosition, direction, "Esquerda");
                yield return new WaitForSeconds(SpawnDelay);
            }

            // Spawn dos whisps da direita
            for (int i = 0; i < NumberOfWhispsPerSide; i++)
            {
                Vector3 spawnPosition = RightSpawnPoint.position + new Vector3(0, initialOffsetY + i * WhispSpacing, 0);
                Vector2 direction = Vector2.left * WhispSpeed;

                SpawnWhisp(spawnPosition, direction, "Direita");
                yield return new WaitForSeconds(SpawnDelay);
            }

            Debug.Log("Tempestade de Fogo Fátuo concluída.");

            // Feedback ao completar a habilidade
            if (AbilityCompleteFeedback != null)
            {
                AbilityCompleteFeedback.PlayFeedbacks();
            }

            // A habilidade foi concluída, aciona o evento
            OnAbilityCompleted?.Invoke();
        }

        /// <summary>
        /// Método para spawnar um whisp
        /// </summary>
        /// <param name="spawnPosition">Posição de spawn</param>
        /// <param name="direction">Direção do movimento</param>
        /// <param name="lado">Identificador do lado (para logs)</param>
        private void SpawnWhisp(Vector3 spawnPosition, Vector2 direction, string lado)
        {
            if (WhispPooler != null)
            {
                GameObject whisp = WhispPooler.GetPooledGameObject();
                if (whisp != null)
                {
                    whisp.transform.position = spawnPosition;
                    whisp.transform.rotation = Quaternion.identity;

                    // Reativa o whisp
                    whisp.SetActive(true);

                    // Configura o dano e a direção no script Whisp
                    Whisp whispScript = whisp.GetComponent<Whisp>();
                    if (whispScript != null)
                    {
                        whispScript.Damage = WhispDamage;
                        whispScript.Initialize(direction);
                        Debug.Log($"Whisp {lado} spawnado em {spawnPosition} com direção {direction}.");
                    }
                    else
                    {
                        Debug.LogWarning("O prefab do whisp não possui o componente Whisp!");
                    }

                    // Feedback ao spawnar cada whisp
                    if (WhispSpawnFeedback != null)
                    {
                        WhispSpawnFeedback.PlayFeedbacks();
                    }
                }
                else
                {
                    Debug.LogWarning("Nenhum whisp disponível no pool!");
                }
            }
            else
            {
                Debug.LogError("WhispPooler não está atribuído no Inspector!");
            }
        }
    }
}
