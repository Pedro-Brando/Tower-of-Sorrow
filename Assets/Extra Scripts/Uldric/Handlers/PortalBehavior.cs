using UnityEngine;

public class PortalBehavior : MonoBehaviour
{
    public float PortalLifetime = 30f;            // Tempo que o portal ficará aberto antes de fechar automaticamente
    public GameObject PortalCloseEffectPrefab;    // Prefab do efeito visual ao fechar o portal

    private bool _isOpen = true;                   // Estado do portal

    void Start()
    {
        // Inicia a contagem regressiva para fechar o portal
        Invoke("ClosePortal", PortalLifetime);
    }

    // Método para abrir o portal
    public void OpenPortal()
    {
        _isOpen = true;
        // Implementar lógica visual e sonora para abrir o portal, se necessário
    }

    // Método para fechar o portal
    public void ClosePortal()
    {
        _isOpen = false;

        // Instanciar o efeito de fechamento do portal
        if (PortalCloseEffectPrefab != null)
        {
            Instantiate(PortalCloseEffectPrefab, transform.position, Quaternion.identity);
        }

        // Destroi o portal após o efeito de fechamento
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_isOpen && other.CompareTag("Player"))
        {
            // Teleporta o jogador para o mundo espiritual
            TeleportPlayerToSpiritualWorld(other.gameObject.transform);
        }
    }

    void TeleportPlayerToSpiritualWorld(Transform playerTransform)
    {
        // Implementar a lógica de teleportação para o mundo espiritual
        // Por exemplo, mover o jogador para uma posição específica no mundo espiritual

        // Exemplo:
        Vector3 spiritualSpawnPoint = new Vector3(10f, 0f, 0f); // Defina o ponto de spawn no mundo espiritual
        playerTransform.position = spiritualSpawnPoint;

        // Opcional: Adicionar efeitos visuais ou sonoros durante a teleportação
    }
}
