using MoreMountains.CorgiEngine;
using UnityEngine;

public class BeamArc : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    [Tooltip("Velocidade do feixe.")]
    public float speed = 10f;

    [Tooltip("Gravidade que afeta o feixe.")]
    public float gravity = -9.81f;

    [Header("Configurações de Dano")]
    [Tooltip("Dano que o feixe causará ao jogador.")]
    public int damage = 20;

    private Rigidbody2D rb;
    private Vector2 velocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("BeamArc: Rigidbody2D não encontrado no feixe!");
        }
    }

    private void Start()
    {
        // Determina a direção com base na rotação atual do feixe
        Vector2 direction = transform.right.normalized;

        // Inicializa a velocidade do feixe
        velocity = direction * speed + Vector2.up * Mathf.Abs(gravity) * 0.5f;

        // Define a velocidade inicial no Rigidbody2D
        rb.velocity = velocity;
    }

    private void FixedUpdate()
    {
        // Aplicar a gravidade manualmente para criar o arco
        velocity += Vector2.up * gravity * Time.fixedDeltaTime;
        rb.velocity = new Vector2(rb.velocity.x, velocity.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Health playerHealth = collision.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.Damage(damage, gameObject, 0f, 0f, Vector3.zero);
            }
            else
            {
                Debug.LogError("BeamArc: Componente Health não encontrado no jogador!");
            }

            // Destruir o feixe após colidir
            Destroy(gameObject);
        }
    }
}
