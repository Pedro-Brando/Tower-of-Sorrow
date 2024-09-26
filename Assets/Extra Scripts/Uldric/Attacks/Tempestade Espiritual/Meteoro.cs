using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.CorgiEngine;

namespace MoreMountains.CorgiEngine
{
    public class Meteoro : MonoBehaviour
    {
        [Tooltip("Velocidade de queda do meteoro")]
        public float DropSpeed = 10f;

        [Tooltip("Dano aplicado ao jogador")]
        public float Damage = 1f;

        [Tooltip("Prefab da explosão do meteoro")]
        public GameObject ExplosaoPrefab;

        private Rigidbody2D _rigidbody;
        private bool _hasExploded = false;

        /// <summary>
        /// Inicializa o meteoro com direção e velocidade
        /// </summary>
        /// <param name="targetPosition">Posição alvo (chão)</param>
        /// <param name="damageAmount">Quantidade de dano</param>
        public void Initialize(Vector3 targetPosition, float damageAmount)
        {
            Damage = damageAmount;
            _rigidbody = GetComponent<Rigidbody2D>();
            if (_rigidbody == null)
            {
                Debug.LogError("Meteoro requer um componente Rigidbody2D!");
                return;
            }

            // Calcula a direção para o chão
            Vector3 direction = (targetPosition - transform.position).normalized;
            Debug.Log($"Meteoro inicializado com direção {direction} e velocidade {DropSpeed}");

            // Define a velocidade do meteoro
            _rigidbody.velocity = new Vector2(direction.x, direction.y) * DropSpeed;

            // Opcional: Rotacionar o meteoro para dar efeito visual
            float randomRotation = Random.Range(0f, 360f);
            transform.rotation = Quaternion.Euler(0f, 0f, randomRotation);

            _hasExploded = false;

            // Inicia uma coroutine para retornar o meteoro ao pool após um tempo, se não tiver colidido
            StartCoroutine(ReturnToPoolAfterDelay(5f));
        }

        /// <summary>
        /// Detecta colisões com o jogador e o chão para aplicar dano e explodir
        /// </summary>
        /// <param name="collision">Informações da colisão</param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_hasExploded)
                return;

            if (collision.CompareTag("Player"))
            {
                Health playerHealth = collision.GetComponent<Health>();
                if (playerHealth != null)
                {
                    playerHealth.Damage(Damage, gameObject, flickerDuration: 0.1f, invincibilityDuration: 1f, damageDirection: transform.position, typedDamages: null);
                    Debug.Log($"Meteoro causou {Damage} de dano ao jogador!");
                }

                // Explodir e retornar ao pool
                Explodir();
            }
            else if (collision.CompareTag("Ground"))
            {
                // Explodir e retornar ao pool
                Explodir();
            }
            else
            {
                Debug.Log($"Meteoro colidiu com {collision.gameObject.name}, que não é o jogador ou chão.");
            }
        }

        /// <summary>
        /// Método para explodir o meteoro
        /// </summary>
        private void Explodir()
        {
            if (ExplosaoPrefab != null)
            {
                Instantiate(ExplosaoPrefab, transform.position, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("ExplosaoPrefab não está atribuído no meteoro!");
            }

            _hasExploded = true;

            // Retornar o meteoro ao pool
            if (MeteoroPool.Instance != null)
            {
                MeteoroPool.Instance.ReturnMeteoro(gameObject);
            }
            else
            {
                Debug.LogError("MeteoroPool.Instance não está definido. Assegure-se de que o MeteoroPool está presente na cena.");
                // Como fallback, desative o meteoro para evitar erros
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Coroutine para retornar o meteoro ao pool após um delay
        /// </summary>
        /// <param name="delay">Tempo em segundos para esperar antes de retornar</param>
        /// <returns></returns>
        private IEnumerator ReturnToPoolAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (!_hasExploded)
            {
                Debug.Log($"Meteoro não colidiu após {delay} segundos. Retornando ao pool.");
                if (MeteoroPool.Instance != null)
                {
                    MeteoroPool.Instance.ReturnMeteoro(gameObject);
                }
                else
                {
                    Debug.LogError("MeteoroPool.Instance não está definido. Assegure-se de que o MeteoroPool está presente na cena.");
                    // Como fallback, desative o meteoro para evitar erros
                    gameObject.SetActive(false);
                }
            }
        }
    }
}
