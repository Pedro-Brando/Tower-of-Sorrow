using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.CorgiEngine;

namespace MoreMountains.CorgiEngine
{
    [AddComponentMenu("Corgi Engine/Character/Abilities/Furia Do Fogo Fátuo")]
    public class FuriaDoFogoFatuo : CharacterAbility
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

        [Tooltip("Tempo de espera antes de permitir outro ataque")]
        public float Cooldown = 2f;

        [Tooltip("Delay entre o spawn de cada whisp")]
        public float SpawnDelay = 0.2f;

        [Tooltip("Velocidade dos whisps")]
        public float WhispSpeed = 5f; // Adicionado

        [Header("Pooler")]

        [Tooltip("Referência ao MMSimpleObjectPooler")]
        public MMSimpleObjectPooler simpleObjectPooler;

        private bool _canAttack = true;
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
            if (_canAttack)
            {
                StartCoroutine(FuriaDoFogoFatuoRoutine());
            }
        }

        /// <summary>
        /// Coroutine que gerencia a execução da Fúria do Fogo Fátuo
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator FuriaDoFogoFatuoRoutine()
        {
            _canAttack = false;

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

            // Espera o cooldown antes de permitir outro ataque
            yield return new WaitForSeconds(Cooldown);
            _canAttack = true;
        }

        /// <summary>
        /// Reseta a habilidade quando necessário
        /// </summary>
        public override void ResetAbility()
        {
            base.ResetAbility();
            _canAttack = true;
        }
    }
}
