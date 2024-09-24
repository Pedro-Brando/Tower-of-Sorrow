using UnityEngine;
using System.Collections;

public class ConsumirVidaMarker : MonoBehaviour
{
    public float MarkerRadius = 1.5f;           // Raio da área de marcação
    public LayerMask PlayerLayer;               // Camada do jogador para detecção de dano
    public GameObject DamageEffectPrefab;        // Prefab do efeito visual de dano (opcional)

    private float _activationDelay;              // Tempo antes de ativar o dano
    private float _damageAmount;                 // Quantidade de dano a ser aplicada
    private Transform _playerTransform;          // Referência ao transform do jogador

    void Start()
    {
        // Opcional: Configurar visualmente a marcação (por exemplo, um círculo)
        // Pode ser feito via SpriteRenderer ou outro método
    }

    // Inicializa a marcação com os parâmetros necessários
    public void Initialize(float activationDelay, float damageAmount, Transform playerTransform)
    {
        _activationDelay = activationDelay;
        _damageAmount = damageAmount;
        _playerTransform = playerTransform;

        // Inicia a coroutine que gerencia o delay e a aplicação do dano
        StartCoroutine(ActivateDamage());
    }

    IEnumerator ActivateDamage()
    {
        // Aguarda o delay antes de ativar o dano
        yield return new WaitForSeconds(_activationDelay);

        // Verifica se o jogador está dentro do raio da marcação
        Collider2D hit = Physics2D.OverlapCircle(transform.position, MarkerRadius, PlayerLayer);
        if (hit != null && hit.CompareTag("Player"))
        {
            // Aplicar dano ao jogador
            PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(_damageAmount);
            }

            // Opcional: Instanciar um efeito visual de dano
            if (DamageEffectPrefab != null)
            {
                Instantiate(DamageEffectPrefab, transform.position, Quaternion.identity);
            }
        }

        // Opcional: Remover a marcação após a ativação
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        // Visualiza o raio da marcação no editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, MarkerRadius);
    }
}
