using UnityEngine;
using MoreMountains.CorgiEngine;

public class Fireball : MonoBehaviour
{
    public float Speed = 5f;
    public int Damage = 1;

    private Vector2 _direction;

    [Tooltip("Prefab da explosão ao colidir com o chão ou parede")]
    public GameObject ExplosionPrefab;

    
    public void Initialize(Vector2 direction)
    {
        _direction = direction.normalized;
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
        }
        else if (collision.CompareTag("Ground") || collision.CompareTag("Wall"))
        {
            // Instanciar explosão (opcional)
            if (ExplosionPrefab != null)
            {
                Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
            }
            // Desativar o whisp
            gameObject.SetActive(false);
        }
    }

    void OnBecameInvisible()
    {
        // Desativa a bola de fogo quando sair da tela para evitar desperdício de recursos
        gameObject.SetActive(false);
    }
}
