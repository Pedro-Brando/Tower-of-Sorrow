using UnityEngine;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;

public class Fireball : MonoBehaviour
{
    public float Speed = 5f;
    public int Damage = 1;

    private Vector2 _direction;

    [Tooltip("Prefab da explosão ao colidir com o chão ou parede")]
    public GameObject ExplosionPrefab;

    [Header("Feedbacks MMF Player")]
    [Tooltip("Feedback ao ocorrer a colisão da Fireball com o chão ou parede")]
    public MMF_Player CollisionFeedback;

    [Tooltip("Feedback que toca em loop até a destruição da Fireball")]
    public MMF_Player LoopingFeedback;

    public void Initialize(Vector2 direction)
    {
        _direction = direction.normalized;

        // Iniciar o feedback em loop quando a Fireball é ativada
        if (LoopingFeedback != null)
        {
            LoopingFeedback.PlayFeedbacks();
        }
    }

    void Update()
    {
        transform.Translate(_direction * Speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Health playerHealth = collision.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.Damage(Damage, gameObject, 0.1f, 0.1f, transform.position);
            }
            // Não desativa o whisp
            // Feedback ao ocorrer a colisão
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

            // Feedback ao ocorrer a colisão
            if (CollisionFeedback != null)
            {
                CollisionFeedback.PlayFeedbacks();
            }

            // Parar o feedback em loop antes de desativar o objeto
            if (LoopingFeedback != null)
            {
                LoopingFeedback.StopFeedbacks();
            }

            // Desativar a bola de fogo
            gameObject.SetActive(false);
        }
    }

    void OnBecameInvisible()
    {
        // Parar o feedback em loop quando a bola de fogo sai da tela
        if (LoopingFeedback != null)
        {
            LoopingFeedback.StopFeedbacks();
        }

        // Desativa a bola de fogo quando sair da tela para evitar desperdício de recursos
        gameObject.SetActive(false);
    }
}
