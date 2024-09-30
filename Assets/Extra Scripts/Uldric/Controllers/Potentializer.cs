using UnityEngine;

public class Potentializer : MonoBehaviour
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
            uldrichController.OnPotentializerDestroyed();
        }
    }

    // Implementar lógica de dano para destruir o potencializador
}
