using UnityEngine;
using MoreMountains.CorgiEngine;

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

        // Desativa o Potentializer ao invés de destruí-lo
        gameObject.SetActive(false);
    }

    public void Revive()
    {
        // Reativa o Potentializer e reseta sua vida
        gameObject.SetActive(true);
        if (_health != null)
        {
            _health.SetHealth(_health.MaximumHealth, gameObject);
        }
        Debug.Log("Potentializer reviveu.");
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
