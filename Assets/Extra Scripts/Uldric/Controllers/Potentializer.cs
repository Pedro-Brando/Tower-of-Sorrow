using UnityEngine;
using MoreMountains.CorgiEngine; // Se estiver usando o Corgi Engine

public class Potentializer : MonoBehaviour
{
    public UldrichController uldrichController;

    private Health _health;

    void Awake()
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

    void Start()
    {
        _health = GetComponent<Health>();
        if (_health == null)
        {
            Debug.LogError("Health component não encontrado no Potentializer!");
        }
        else
        {
            _health.OnDeath += OnDeath;
        }
    }

    private void OnDeath()
    {
        Debug.Log("Potentializer morreu.");
        if (uldrichController != null)
        {
            uldrichController.OnPotentializerDestroyed();
        }
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        // Certifique-se de remover o evento para evitar leaks de memória
        if (_health != null)
        {
            _health.OnDeath -= OnDeath;
        }
    }
}
