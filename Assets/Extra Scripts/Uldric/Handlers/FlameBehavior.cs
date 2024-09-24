using UnityEngine;

public class FlameBehavior : MonoBehaviour
{
    public float Speed = 3f;              // Velocidade de propagação da chama
    public float DamagePerSecond = 10f;   // Dano por segundo causado pela chama
    public float Lifetime = 7f;            // Tempo de vida da chama

    private Vector3 _direction;            // Direção da propagação
    private float _damageTimer = 0f;       // Timer para dano contínuo

    void Update()
    {
        // Mover a chama na direção definida
        transform.Translate(_direction * Speed * Time.deltaTime, Space.World);

        // Atualizar o timer de dano
        _damageTimer += Time.deltaTime;
        if (_damageTimer >= 1f)
        {
            ApplyDamage();
            _damageTimer = 0f;
        }
    }

    // Inicializar a direção e velocidade da chama
    public void Initialize(Vector3 direction, float speed, float damagePerSecond)
    {
        _direction = direction.normalized;
        Speed = speed;
        DamagePerSecond = damagePerSecond;
    }

    // Aplicar dano ao jogador se estiver dentro do alcance
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(DamagePerSecond);
            }
        }
    }

    // Opcional: Adicionar efeitos de partículas ou som ao colidir com o jogador
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Exemplo: adicionar efeito de partículas
            // Instantiate(HitEffectPrefab, transform.position, Quaternion.identity);
        }
    }
}