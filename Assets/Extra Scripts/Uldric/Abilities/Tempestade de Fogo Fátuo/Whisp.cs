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

        [Tooltip("Feedback contínuo enquanto o Whisp está ativo")]
        public MMF_Player LoopingFeedback;

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

            // Iniciar o feedback contínuo enquanto o Whisp está ativo
            if (LoopingFeedback != null)
            {
                Debug.Log("Iniciando feedback contínuo do Whisp");
                LoopingFeedback.PlayFeedbacks();
            }
        }

        /// <summary>
        /// Método chamado quando o objeto é desativado
        /// </summary>
        void OnDisable()
        {
            // Parar o feedback contínuo quando o Whisp for desativado
            if (LoopingFeedback != null)
            {
                Debug.Log("Parando feedback contínuo do Whisp");
                LoopingFeedback.StopFeedbacks();
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
                Debug.Log("Whisp colidiu com jogador");
                Health playerHealth = collision.GetComponent<Health>();
                if (playerHealth != null)
                {
                    playerHealth.Damage(Damage, gameObject, 0.1f, 0.1f, transform.position);
                }
                if (CollisionFeedback != null)
                {
                    Debug.Log("Tocando feedback de colisão");
                    CollisionFeedback.PlayFeedbacks();
                }
            }
            else if (collision.CompareTag("Ground") || collision.CompareTag("Wall"))
            {
                // Feedback ao colidir com o chão ou parede
                Debug.Log("Whisp colidiu com chão ou parede");
                if (CollisionFeedback != null)
                {
                    Debug.Log("Tocando feedback de colisão");
                    CollisionFeedback.PlayFeedbacks();
                }
                // Instanciar explosão (opcional)
                if (ExplosionPrefab != null)
                {
                    Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
                }

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
