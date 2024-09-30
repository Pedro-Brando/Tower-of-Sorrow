using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.CorgiEngine;

namespace MoreMountains.CorgiEngine
{
    [AddComponentMenu("Corgi Engine/Character/Abilities/Furia Do Fogo Fátuo")]
    public class FuriaDoFogoFatuo : CharacterAbility, IUldrichAbility
    {
        [Header("Configurações da Fúria do Fogo Fátuo")]

        [Tooltip("Número de whisps que serão instanciados em uma linha")]
        public int NumberOfWhisps = 5;

        [Tooltip("Espaçamento horizontal entre os whisps")]
        public float WhispSpacing = 2f;

        [Tooltip("BoxCollider2D que define a área de spawn dos whisps")]
        public BoxCollider2D SpawnAreaCollider;

        [Tooltip("Dano causado pelo whisp ao colidir com o jogador")]
        public float Damage = 1f;

        [Tooltip("Direção da linha de spawn (True para da esquerda para a direita, False para da direita para a esquerda)")]
        public bool SpawnDirectionLeftToRight = true;

        [Tooltip("Delay entre o spawn de cada whisp")]
        public float SpawnDelay = 0.2f;

        [Tooltip("Velocidade dos whisps")]
        public float WhispSpeed = 5f;

        [Header("Pooler")]

        [Tooltip("Referência ao MMSimpleObjectPooler")]
        public MMSimpleObjectPooler simpleObjectPooler;

        [Header("Cooldown")]
        [Tooltip("Duração do cooldown da habilidade Fúria do Fogo Fátuo")]
        public float CooldownDuration = 5f; // Ajuste conforme necessário

        private float _lastActivationTime = -Mathf.Infinity; // Armazena o momento da última ativação

        /// <summary>
        /// Propriedade que indica se a habilidade está permitida (herdada de CharacterAbility)
        /// </summary>
        public new bool AbilityPermitted => base.AbilityPermitted;

        /// <summary>
        /// Propriedade que indica se o cooldown terminou e a habilidade está pronta para uso
        /// </summary>
        public bool CooldownReady => Time.time >= _lastActivationTime + CooldownDuration;

        private Transform _playerTransform;

        /// <summary>
        /// Inicialização da habilidade
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();

            if (simpleObjectPooler == null)
            {
                Debug.LogError("MMSimpleObjectPooler não está atribuído no FuriaDoFogoFatuo!");
            }

            if (SpawnAreaCollider == null)
            {
                Debug.LogError("SpawnAreaCollider não está atribuído no FuriaDoFogoFatuo!");
            }

            // Busca o transform do player
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                _playerTransform = player.transform;
            }
            else
            {
                Debug.LogWarning("Player com tag 'Player' não encontrado na cena!");
            }
        }

        /// <summary>
        /// Método público para ativar a habilidade Fúria do Fogo Fátuo
        /// </summary>
        public void ActivateAbility()
        {
            if (AbilityAuthorized)
            {
                if (Time.time >= _lastActivationTime + CooldownDuration)
                {
                    _lastActivationTime = Time.time;
                    StartCoroutine(FuriaDoFogoFatuoRoutine());
                }
                else
                {
                    // Opcional: Fornecer feedback indicando que a habilidade está em cooldown
                    float cooldownRemaining = (_lastActivationTime + CooldownDuration) - Time.time;
                    Debug.Log($"Fúria do Fogo Fátuo está em cooldown. Tempo restante: {cooldownRemaining:F1} segundos.");
                }
            }
        }

        /// <summary>
        /// Coroutine que gerencia a execução da Fúria do Fogo Fátuo
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator FuriaDoFogoFatuoRoutine()
        {
            // Calcula o espaçamento inicial baseado na direção
            float initialOffsetX = SpawnDirectionLeftToRight ? -((NumberOfWhisps - 1) * WhispSpacing) / 2f : ((NumberOfWhisps - 1) * WhispSpacing) / 2f;

            // Obtém as bordas do BoxCollider2D para definir as posições de spawn
            Vector3 colliderCenter = SpawnAreaCollider.bounds.center;
            Vector3 colliderSize = SpawnAreaCollider.bounds.size;

            for (int i = 0; i < NumberOfWhisps; i++)
            {
                // Calcula a posição de cada whisp dentro da área de spawn
                float xOffset = initialOffsetX + i * WhispSpacing * (SpawnDirectionLeftToRight ? 1 : -1);
                float yPosition = colliderCenter.y + colliderSize.y / 2f; // Topo da área de spawn

                Vector3 whispPosition = new Vector3(colliderCenter.x + xOffset, yPosition, 0f);

                // Calcula a direção em direção ao jogador
                Vector3 directionToPlayer = (_playerTransform.position - whispPosition).normalized;

                // Calcula o ângulo para rotacionar o whisp (opcional)
                float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
                Quaternion rotation = Quaternion.Euler(0, 0, angle - 90f); // Ajuste conforme a orientação do prefab

                // Recupera um whisp do pool
                GameObject whisp = simpleObjectPooler.GetPooledGameObject();
                if (whisp != null)
                {
                    whisp.transform.position = whispPosition;
                    whisp.transform.rotation = rotation;

                    // Reativa o whisp
                    whisp.SetActive(true);

                    // Configura o dano e a direção no script Whisp
                    Whisp whispScript = whisp.GetComponent<Whisp>();
                    if (whispScript != null)
                    {
                        whispScript.Damage = Damage;
                        whispScript.Initialize(directionToPlayer * WhispSpeed); // Define a direção e velocidade
                    }

                    Debug.Log($"Whisp {i + 1} spawnado em {whispPosition} em direção ao jogador.");
                }
                else
                {
                    Debug.LogWarning("Nenhum Whisp disponível no pool!");
                }

                // Espera o delay configurado antes de spawnar o próximo whisp
                yield return new WaitForSeconds(SpawnDelay);
            }
        }
    }
}
