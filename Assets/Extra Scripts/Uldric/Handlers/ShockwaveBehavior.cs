using UnityEngine;
using System.Collections;

public class ShockwaveBehavior : MonoBehaviour
{
    public float Duration = 1f;                    // Duração total da onda de choque
    public float Damage = 50f;                     // Dano causado pela onda de choque
    public float MaxRadius = 5f;                    // Raio máximo da onda de choque
    public LayerMask PlayerLayer;                   // Camada do jogador para detecção de dano

    private float _elapsedTime = 0f;               // Tempo decorrido desde o início da onda
    private float _currentRadius = 0f;             // Raio atual da onda

    void Start()
    {
        // Inicia a onda de choque
        StartCoroutine(ExpandShockwave());
    }

    IEnumerator ExpandShockwave()
    {
        while (_elapsedTime < Duration)
        {
            _elapsedTime += Time.deltaTime;
            _currentRadius = Mathf.Lerp(0f, MaxRadius, _elapsedTime / Duration);
            ApplyDamage();

            yield return null;
        }

        // Destroi a onda de choque após a duração
        Destroy(gameObject);
    }

    void ApplyDamage()
    {
        // Detecta jogadores dentro do raio atual da onda de choque
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _currentRadius, PlayerLayer);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(Damage * Time.deltaTime);
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Visualiza o raio atual da onda de choque no editor
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _currentRadius);
    }
}
