using UnityEngine;

public class WorldZoneTrigger : MonoBehaviour
{
    [Tooltip("Defina este valor como true se esta área representa o mundo espiritual.")]
    public bool IsSpiritualWorldZone;

    [Tooltip("Referência ao UldrichController na cena.")]
    public UldrichController UldrichController;

    private void Start()
    {
        if (UldrichController == null)
        {
            // Tenta encontrar o UldrichController na cena
            UldrichController = FindObjectOfType<UldrichController>();
            if (UldrichController == null)
            {
                Debug.LogError("UldrichController não encontrado na cena!");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se o objeto que entrou é o jogador
        if (other.CompareTag("Player"))
        {
            if (UldrichController != null)
            {
                if (IsSpiritualWorldZone)
                {
                    UldrichController.PlayerEnteredSpiritualWorld();
                    Debug.Log("Jogador entrou na área do mundo espiritual.");
                }
                else
                {
                    UldrichController.PlayerExitedSpiritualWorld();
                    Debug.Log("Jogador entrou na área do mundo físico.");
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Verifica se o objeto que saiu é o jogador
        if (other.CompareTag("Player"))
        {
            if (UldrichController != null)
            {
                if (IsSpiritualWorldZone)
                {
                    UldrichController.PlayerExitedSpiritualWorld();
                    Debug.Log("Jogador saiu da área do mundo espiritual.");
                }
                else
                {
                    UldrichController.PlayerEnteredSpiritualWorld();
                    Debug.Log("Jogador saiu da área do mundo físico.");
                }
            }
        }
    }
}
