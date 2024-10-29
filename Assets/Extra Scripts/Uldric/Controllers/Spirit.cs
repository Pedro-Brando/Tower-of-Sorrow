using UnityEngine;

public class Spirit : MonoBehaviour
{
    public UldrichController uldrichController;

    void Start()
    {
        if (uldrichController == null)
        {
            uldrichController = FindObjectOfType<UldrichController>();
            if (uldrichController == null)
            {
                Debug.LogError("UldrichController não encontrado na cena!");
            }
        }
    }

    void OnDestroy()
    {
        if (uldrichController != null)
        {
            uldrichController.OnSpiritDeath();
        }
    }

    // Implementar lógica de dano para destruir o espírito
}
