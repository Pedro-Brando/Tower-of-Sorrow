using UnityEngine;

public class OndaDeChoque : MonoBehaviour
{
    [Tooltip("Velocidade de propagação da onda")]
    public float WaveSpeed = 5f;

    private void OnEnable()
    {
        // Define a velocidade inicial
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = transform.up * WaveSpeed;
        }

        // Destroi a onda após 2 segundos para limpeza
        Destroy(gameObject, 2f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Platforms") || collision.collider.CompareTag("Ground"))
        {
            // Interrompe a propagação ao atingir uma parede ou plataforma
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
            }
            // Destroi a onda após colisão
            Destroy(gameObject);
        }
    }
}
