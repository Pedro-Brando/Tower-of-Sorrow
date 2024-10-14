using UnityEngine;

public class BackgroundSwitcher : MonoBehaviour
{
    [Tooltip("Novo Sprite para ser usado no background")]
    public Sprite NovoBackground;

    [Tooltip("Referência ao GameObject que contém o Sprite Renderer do background")]
    public GameObject BgObject;

    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        if (BgObject != null)
        {
            // Obtém o SpriteRenderer do GameObject 'bg'
            _spriteRenderer = BgObject.GetComponent<SpriteRenderer>();

            if (_spriteRenderer == null)
            {
                Debug.LogError("O GameObject especificado não possui um SpriteRenderer!");
            }
        }
        else
        {
            Debug.LogError("BgObject não foi atribuído no inspetor!");
        }
    }

    // Método chamado quando outro collider entra no trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se o objeto que entrou é o player
        if (other.CompareTag("Player"))
        {
            TrocarBackground();
        }
    }

    // Troca o sprite do background
    private void TrocarBackground()
    {
        if (_spriteRenderer != null && NovoBackground != null)
        {
            _spriteRenderer.sprite = NovoBackground;
            Debug.Log("Background trocado com sucesso!");
        }
        else
        {
            Debug.LogError("SpriteRenderer ou NovoBackground não estão corretamente atribuídos!");
        }
    }
}
