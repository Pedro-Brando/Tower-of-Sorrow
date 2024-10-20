using UnityEngine;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;

namespace MoreMountains.CorgiEngine
{
    public class Whisp : MonoBehaviour
    {
        [Header("Configurações do Whisp")]
        [Tooltip("Direção e velocidade do whisp")]
        public Vector2 Velocity;

        [Tooltip("Dano causado ao colidir com o jogador")]
        public float Damage = 1f;

        [Tooltip("Prefab da explosão ao colidir com o chão ou parede")]
        public GameObject ExplosionPrefab;

        [Header("Feedbacks MMF Player")]
        [Tooltip("Feedback ao colidir com o chão ou parede")]
        public MMF_Player CollisionFeedback;

        private Rigidbody2D _rb;

        /// <summary>
        /// Inicialização do Whisp
        /// </summary>
        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            if (_rb == null)
            {
                Debug.LogError("Whisp precisa de um Rigidbody2D!");
            }
        }

        /// <summary>
        /// Método chamado quando o objeto é ativado (spawnado)
        /// </summary>
        void OnEnable()
        {
            // Configurar a velocidade do whisp
            if (_rb != null)
            {
                _rb.velocity = Velocity;
            }
        }

        /// <summary>
        /// Detecta colisões com o jogador, chão e paredes
        /// </summary>
        /// <param name="collision">Informações da colisão</param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                Health playerHealth = collision.GetComponent<Health>();
                if (playerHealth != null)
                {
                    playerHealth.Damage(Damage, gameObject, 0.1f, 0.1f, transform.position);
                }
                if (CollisionFeedback != null)
                {
                    CollisionFeedback.PlayFeedbacks();
                }
            }
            else if (collision.CompareTag("Ground") || collision.CompareTag("Wall"))
            {
                // Instanciar explosão (opcional)
                if (ExplosionPrefab != null)
                {
                    Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
                }

                // Feedback ao colidir com o chão ou parede


                // Desativar o whisp
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Inicializa o whisp com a direção e velocidade desejadas
        /// </summary>
        /// <param name="direction">Direção do movimento</param>
        public void Initialize(Vector2 direction)
        {
            Velocity = direction;
            if (_rb != null)
            {
                _rb.velocity = Velocity;
            }
        }
    }
}
