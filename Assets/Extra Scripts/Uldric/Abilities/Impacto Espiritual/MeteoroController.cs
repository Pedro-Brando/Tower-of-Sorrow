using UnityEngine;
using MoreMountains.CorgiEngine;

public class MeteoroController : MonoBehaviour
{
    public float VelocidadeQueda = 10f;
    public int Dano = 1;
    public LayerMask GroundLayer;

    private ImpactoEspiritual _impactoEspiritual;

    /// <summary>
    /// Inicializa o meteoro com a referência ao script principal
    /// </summary>
    public void Initialize(ImpactoEspiritual impactoEspiritual)
    {
        _impactoEspiritual = impactoEspiritual;
    }

    void Update()
    {
        // Movimento de queda
        transform.Translate(Vector2.down * VelocidadeQueda * Time.deltaTime);

        // Verifica se colidiu com o chão
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, VelocidadeQueda * Time.deltaTime, GroundLayer);
        if (hit.collider != null)
        {
            OnImpacto();
        }
    }

    private void OnImpacto()
    {
        // Notifica o script principal
        _impactoEspiritual.OnMeteoroImpacto();

        // Destrói o meteoro
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Aplica dano ao jogador ao contato
        if (collision.gameObject.CompareTag("Player"))
        {
            Health playerHealth = collision.gameObject.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.Damage(Dano, gameObject, 0f, 0f, Vector2.zero);
            }
        }
    }
}
