using UnityEngine;
using System.Collections;
using MoreMountains.Tools; // Namespace do Corgi Engine
using MoreMountains.CorgiEngine;

namespace MoreMountains.CorgiEngine
{
    [AddComponentMenu("Corgi Engine/Character/Abilities/Tempestade De Fogo Fátuo")]
    public class TempestadeDeFogoFatuo : CharacterAbility
    {
        [Header("Configurações da Tempestade de Fogo Fátuo")]

        [Tooltip("Número de whisps que serão instanciados de cada lado")]
        public int NumberOfWhispsPerSide = 10;

        [Tooltip("Espaçamento vertical entre os whisps")]
        public float WhispSpacing = 1.0f;

        [Tooltip("Prefab do whisp")]
        public GameObject WhispPrefab;

        [Tooltip("Referência ao MMSimpleObjectPooler")]
        public MMSimpleObjectPooler WhispPooler;

        [Tooltip("Velocidade dos whisps")]
        public float WhispSpeed = 5f;

        [Tooltip("Dano causado pelos whisps ao colidir com o jogador")]
        public float WhispDamage = 1f;

        [Tooltip("Delay entre o spawn de cada whisp")]
        public float SpawnDelay = 0.1f;

        [Tooltip("Tempo de espera antes de permitir outro ataque")]
        public float Cooldown = 5f;

        [Header("Referências de Spawn")]

        [Tooltip("Transform que define a posição de spawn dos whisps da esquerda")]
        public Transform LeftSpawnPoint;

        [Tooltip("Transform que define a posição de spawn dos whisps da direita")]
        public Transform RightSpawnPoint;

        private bool _canAttack = true;

        /// <summary>
        /// Método público para ativar a habilidade
        /// </summary>
        public void ActivateAbility()
        {
            if (_canAttack)
            {
                StartCoroutine(TempestadeDeFogoFatuoRoutine());
            }
        }

        /// <summary>
        /// Coroutine que gerencia a execução da habilidade Tempestade de Fogo Fátuo
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator TempestadeDeFogoFatuoRoutine()
        {
            _canAttack = false;

            Debug.Log("Tempestade de Fogo Fátuo iniciada!");

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

            // Espera o cooldown antes de permitir outro ataque
            yield return new WaitForSeconds(Cooldown);
            _canAttack = true;

            Debug.Log("Tempestade de Fogo Fátuo concluída.");
        }

        /// <summary>
        /// Método para spawnar um whisp
        /// </summary>
        /// <param name="spawnPosition">Posição de spawn</param>
        /// <param name="direction">Direção do movimento</param>
        /// <param name="lado">Identificador do lado (para logs)</param>
        private void SpawnWhisp(Vector3 spawnPosition, Vector2 direction, string lado)
        {
            if (WhispPooler != null && WhispPrefab != null)
            {
                GameObject whisp = WhispPooler.GetPooledGameObject();
                if (whisp != null)
                {
                    whisp.transform.position = spawnPosition;
                    whisp.transform.rotation = Quaternion.identity; // Resetar rotação, se necessário

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
                }
                else
                {
                    Debug.LogWarning("Nenhum whisp disponível no pool!");
                }
            }
            else
            {
                Debug.LogError("WhispPooler ou WhispPrefab não está atribuído no Inspector!");
            }
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
