using UnityEngine;
using MoreMountains.CorgiEngine;
using MoreMountains.Feedbacks; // Importar namespace para MMFeedbacks

public class Explosao : MonoBehaviour
{
    [Header("Configurações da Explosão")]
    public float raio = 3f;
    public float dano = 50f;
    public LayerMask camadaAlvo; // Defina as camadas que podem ser afetadas

    [Header("Configurações do Dano e Knockback")]
    public GameObject instigator; // Referência ao boss ou ao objeto que causou a explosão
    public float flickerDuration = 0.5f; // Duração do efeito de piscada
    public float invincibilityDuration = 1f; // Duração da invulnerabilidade após o dano
    public enum KnockbackStyles { NoKnockback, SetForce, AddForce }
    public KnockbackStyles knockbackStyle = KnockbackStyles.AddForce;
    public Vector2 knockbackForce = new Vector2(10f, 5f); // Força de knockback a ser aplicada

    [Header("Feedbacks")]
    public MMFeedbacks feedbacksExplosao; // Referência a um MMFeedbacks configurado no Inspector

    void Start()
    {
        AtivarExplosao();
    }

    void AtivarExplosao()
    {
        // Executar feedbacks visuais e sonoros da explosão, se configurados
        if (feedbacksExplosao != null)
        {
            // Utilize o método PlayFeedbacks() em vez de Play()
            feedbacksExplosao.PlayFeedbacks(transform.position);
        }

        // Detectar colisores dentro do raio
        Collider2D[] colisores = Physics2D.OverlapCircleAll(transform.position, raio, camadaAlvo);

        foreach (Collider2D col in colisores)
        {
            Health health = col.GetComponent<Health>();

            if (health != null)
            {
                // Calcular a direção do dano (do centro da explosão para o alvo)
                Vector3 damageDirection = (col.transform.position - transform.position).normalized;

                // Chamar o método Damage com todos os parâmetros necessários
                health.Damage(dano, instigator, flickerDuration, invincibilityDuration, damageDirection);

                // Adicionar Knockback
                AplicarKnockback(col.gameObject, damageDirection);
            }
            else
            {
                Debug.Log("Nenhum componente Health encontrado em: " + col.name);
            }
        }

        // Opcional: Adicionar efeito de fade-out ou destruir após um tempo
        Destroy(gameObject, 1f);
    }

    void AplicarKnockback(GameObject alvo, Vector3 direcao)
    {
        if (knockbackStyle == KnockbackStyles.NoKnockback)
        {
            return;
        }

        CorgiController corgiController = alvo.GetComponent<CorgiController>();
        if (corgiController != null)
        {
            Vector2 force = direcao * knockbackForce;

            switch (knockbackStyle)
            {
                case KnockbackStyles.SetForce:
                    corgiController.SetForce(force);
                    break;
                case KnockbackStyles.AddForce:
                    corgiController.AddForce(force);
                    break;
            }
        }
        else
        {
            Debug.LogWarning("CorgiController não encontrado no objeto: " + alvo.name);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, raio);
    }
}
