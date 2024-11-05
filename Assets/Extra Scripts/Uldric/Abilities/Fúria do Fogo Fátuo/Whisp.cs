using UnityEngine;
using MoreMountains.CorgiEngine; // Assegure-se de que esta namespace está correta

public class Whisp : MonoBehaviour
{
    [Header("Configurações do Fire Fatuo")]
    [Tooltip("Velocidade de descida do Fire Fatuo")]
    public float FallSpeed = 2f;

    [Tooltip("Prefab da explosão ao tocar o chão")]
    public GameObject ExplosionPrefab;

    [Tooltip("Dano causado ao colidir com o jogador")]
    public float Damage = 1f;

    private Rigidbody2D _rb;
    private bool _hasCollided = false;

    /// <summary>
    /// Inicialização do Fire Fatuo
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
        Debug.Log("Whisp ativado.");
        // Resetar variáveis
        _hasCollided = false;

        // Configurar a velocidade de descida
        if (_rb != null)
        {
            _rb.velocity = Vector2.down * FallSpeed;
            Debug.Log($"Velocidade de descida definida para: {_rb.velocity}");
        }
    }

    /// <summary>
    /// Detecta colisões com o chão para instanciar a explosão e desativar o whisp
    /// </summary>
    /// <param name="collision">Informações da colisão</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Whisp trigger com: {collision.tag}");
        if (collision.CompareTag("Player"))
        {
            Health playerHealth = collision.GetComponent<Health>();
            if (playerHealth != null)
            {
                Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
                playerHealth.Damage(Damage, gameObject, flickerDuration: 0.1f, invincibilityDuration: 1f, damageDirection: transform.position, typedDamages: null);
                Debug.Log($"Meteoro causou {Damage} de dano ao jogador!");
            }

            ReturnToPool();
            Debug.Log("Whisp desativado após colidir com o jogador.");
        }
        else if (collision.CompareTag("Ground"))
        {
            _hasCollided = true;
            if (ExplosionPrefab != null)
            {
                Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
                Debug.Log("Explosão instanciada ao colidir com o chão.");
            }
            ReturnToPool();
            Debug.Log("Whisp desativado após colidir com o chão.");
        }
        else
        {
            Debug.Log($"Whisp colidiu com {collision.gameObject.name}, que não é o jogador ou chão.");
        }
    }

    /// <summary>
    /// Retorna o objeto ao estado desativado
    /// </summary>
    private void ReturnToPool()
    {
        // Desativa o objeto
        gameObject.SetActive(false);
    }
}
