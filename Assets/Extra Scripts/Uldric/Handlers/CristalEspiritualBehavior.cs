using UnityEngine;
using System.Collections;

public class CristalEspiritualBehavior : MonoBehaviour
{
    public float Lifetime = 10f;                  // Tempo que o cristal permanecerá no chão
    public GameObject PortalPrefab;               // Prefab do portal que será gerado
    public GameObject DestructionEffectPrefab;    // Prefab do efeito visual ao destruir o cristal

    private float _elapsedTime = 0f;              // Tempo decorrido desde o spawn do cristal

    void Start()
    {
        // Inicia a destruição automática do cristal após sua duração
        Destroy(gameObject, Lifetime);
    }

    void Update()
    {
        _elapsedTime += Time.deltaTime;

        if (_elapsedTime >= Lifetime)
        {
            // Opcional: Adicionar efeitos antes de destruir o cristal
            if (DestructionEffectPrefab != null)
            {
                Instantiate(DestructionEffectPrefab, transform.position, Quaternion.identity);
            }

            // Destroi o cristal
            Destroy(gameObject);
        }
    }

    // Método chamado ao destruir o cristal manualmente (por exemplo, pelo jogador)
    public void OnDestroyed()
    {
        // Instanciar o portal ao ser destruído
        if (PortalPrefab != null)
        {
            Instantiate(PortalPrefab, transform.position, Quaternion.identity);
        }

        // Opcional: Instanciar efeitos de destruição
        if (DestructionEffectPrefab != null)
        {
            Instantiate(DestructionEffectPrefab, transform.position, Quaternion.identity);
        }

        // Destroi o cristal
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Detecta se o cristal foi atingido pelo jogador (assumindo que o jogador tem uma tag "Player")
        if (collision.gameObject.CompareTag("Player"))
        {
            // Pode implementar lógica para o cristal reagir ao contato com o jogador
        }
    }

    void OnDestroy()
    {
        // Pode adicionar lógica adicional ao destruir o cristal, se necessário
    }
}
