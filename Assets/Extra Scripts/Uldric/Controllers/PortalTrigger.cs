using UnityEngine;

public class PortalTrigger : MonoBehaviour
{
    private UldrichController _uldRichController;

    void Start()
    {
        _uldRichController = FindObjectOfType<UldrichController>();
        if (_uldRichController == null)
        {
            Debug.LogError("UldrichController não encontrado na cena!");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _uldRichController.PlayerEnteredSpiritualWorld();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _uldRichController.PlayerExitedSpiritualWorld();
        }
    }
}
