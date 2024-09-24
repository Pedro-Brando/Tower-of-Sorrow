using UnityEngine;

public class FireballBehavior : MonoBehaviour
{
    public float Damage = 20f;                  // Dano causado ao jogador
    public float Lifetime = 5f;                  // Tempo de vida da bola de fogo
    public LayerMask PlayerLayer;                // Camada do jogador para detecção de colisão

    private Vector3 _direction;                  // Direção do movimento
    private float _speed;                        // Velocidade da bola de fogo

    void Start()
    {
        // Destroi a bola de fogo após seu tempo de vida
        Destroy(gameObject, Lifetime);
    }

    void Update()
    {
        // Move a bola de fogo na direção definida
        transform.Translate(_direction * _speed * Time.deltaTime, Space.World);
    }

    // Inicializa a direção e a velocidade da bola de fogo
    public void Initialize(Vector3 direction, float speed)
    {
        _direction = direction.normalized;
        _speed = speed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(Damage);
            }

            // Opcional: Adicionar efeitos visuais ou sonoros ao colidir
            // Exemplo: Instantiate(ImpactEffectPrefab, transform.position, Quaternion.identity);

            // Destroi a bola de fogo após colidir com o jogador
            Destroy(gameObject);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Visualiza a trajetória da bola de fogo no editor
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, _direction * 1f);
    }
}
