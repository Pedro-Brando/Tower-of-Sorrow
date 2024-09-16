using UnityEngine;
using MoreMountains.CorgiEngine;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;

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
    public Vector2 knockbackForce = new Vector2(20f, 10f); // Força de knockback a ser aplicada

    [Header("Feedbacks")]
    public MMFeedbacks feedbacksExplosao; // Referência a um MMFeedbacks configurado no Inspector

    private Animator animator; // Referência ao Animator
    private bool explodiu = false;

    void Start()
    {
        // Inicializar o Animator
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator não encontrado no attackPrefab.");
        }

        // Remover a chamada de AtivarExplosao() do Start()
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("OnTriggerEnter2D chamado para: " + other.gameObject.name);
        // Verificar se a colisão é com a camada alvo
        if (((1 << other.gameObject.layer) & camadaAlvo) != 0)
        {
            AtivarExplosao();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("OnCollisionEnter2D chamado para: " + collision.gameObject.name);
        // Se estiver usando colisão física (Is Trigger = false)
        if (((1 << collision.gameObject.layer) & camadaAlvo) != 0)
        {
            AtivarExplosao();
        }
    }

    void AtivarExplosao()
    {
        if (explodiu) return;
        explodiu = true;
        Debug.Log("AtivarExplosao() chamado para: " + gameObject.name);

        // Acionar a animação de Impacto
        if (animator != null)
        {
            animator.SetTrigger("ImpactTrigger");
            Debug.Log("Trigger 'ImpactTrigger' acionado no Animator.");
        }
        else
        {
            Debug.LogWarning("Animator não está atribuído em Explosao.cs.");
        }

        // Executar feedbacks visuais e sonoros da explosão, se configurados
        if (feedbacksExplosao != null)
        {
            feedbacksExplosao.PlayFeedbacks(transform.position);
            Debug.Log("Feedbacks da explosão ativados.");
        }
        else
        {
            Debug.LogWarning("MMFeedbacks não está atribuído em Explosao.cs.");
        }

        // A lógica de dano será chamada via Animation Event na ImpactAnim
    }

    // Método chamado via Animation Event durante ImpactAnim
    public void AplicarDano()
    {
        Debug.Log("AplicarDano() chamado via Animation Event.");

        // Detectar colisores dentro do raio após o impacto
        Collider2D[] colisores = Physics2D.OverlapCircleAll(transform.position, raio, camadaAlvo);
        Debug.Log($"Número de objetos detectados na explosão: {colisores.Length}");

        foreach (Collider2D col in colisores)
        {
            Health health = col.GetComponent<Health>();

            if (health != null)
            {
                // Calcular a direção do dano (do centro da explosão para o alvo)
                Vector3 damageDirection = (col.transform.position - transform.position).normalized;
                Debug.Log($"Aplicando dano e Knockback em {col.name} com direção {damageDirection}");

                // Chamar o método Damage com todos os parâmetros necessários
                health.Damage(dano, instigator, flickerDuration, invincibilityDuration, damageDirection);

                // Adicionar Knockback
                AplicarKnockback(col.gameObject, damageDirection);
            }
            else
            {
                Debug.Log($"Nenhum componente Health encontrado em: {col.name}");
            }
        }
    }

    public void DestruirObjeto()
    {
        Debug.Log("DestruirObjeto() chamado. Destruindo: " + gameObject.name);
        Destroy(gameObject);
    }

    void AplicarKnockback(GameObject alvo, Vector3 direcao)
    {
        if (knockbackStyle == KnockbackStyles.NoKnockback)
        {
            Debug.Log("Knockback desativado.");
            return;
        }

        CorgiController corgiController = alvo.GetComponent<CorgiController>();
        if (corgiController != null)
        {
            // Converter direção para Vector2 para consistência com física 2D
            Vector2 direcao2D = new Vector2(direcao.x, direcao.y).normalized;
            Vector2 force = direcao2D * knockbackForce;
            Debug.Log($"Aplicando Knockback: Força = {force}, Estilo = {knockbackStyle}");

            switch (knockbackStyle)
            {
                case KnockbackStyles.SetForce:
                    corgiController.SetForce(force);
                    Debug.Log("Força de Knockback definida.");
                    break;
                case KnockbackStyles.AddForce:
                    corgiController.AddForce(force);
                    Debug.Log("Força de Knockback adicionada.");
                    break;
            }

            // Opcional: Ajustar flags de jump se necessário
            Character character = corgiController.gameObject.MMGetComponentNoAlloc<Character>();
            if (character != null)
            {
                CharacterJump characterJump = character.FindAbility<CharacterJump>();
                if (characterJump != null)
                {
                    characterJump.SetCanJumpStop(false);
                    characterJump.SetJumpFlags();
                    Debug.Log("Flags de Jump ajustadas para Knockback.");
                }
            }
        }
        else
        {
            Debug.LogWarning($"CorgiController não encontrado no objeto: {alvo.name}");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, raio);
    }
}
