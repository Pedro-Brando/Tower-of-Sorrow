using MoreMountains.CorgiEngine;
using MoreMountains.Feedbacks;
using UnityEngine;

public class BossLaser2D : MonoBehaviour
{
    [Header("Configurações do Laser")]
    public LineRenderer lineRenderer;
    public Transform olho; // Posição do olho de onde o laser sai
    public float alcance = 15f; // Distância do laser
    public float angulo = 120f; // Ângulo do laser
    public LayerMask camadaAlvo; // Camadas que o laser pode atingir
    public float taxaDano = 1f; // Dano por segundo
    public int dano = 10; // Quantidade de dano

    [Header("Configurações de Dano")]
    public float flickerDuration = 0.1f; // Duração do efeito de piscada
    public float invincibilityDuration = 0.1f; // Duração da invulnerabilidade após o dano

    [Header("Configurações de Feedback")]
    public MMFeedbacks danoFeedbacks; // Feedbacks a serem reproduzidos ao causar dano

    private float timer = 0f;
    private GameObject instigator; // Referência ao Boss que está causando o dano

    void Start()
    {
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

        DesativarLaser();

        // Definir o instigator como o Boss (assumindo que este script está no filho do Boss)
        instigator = transform.root.gameObject;
    }

    void Update()
    {
        if (lineRenderer.enabled)
        {
            timer += Time.deltaTime;
            if (timer >= 1f / taxaDano)
            {
                AplicarDano();
                timer = 0f;
            }
        }
    }

    public void AtivarLaser(bool paraEsquerda)
    {
        if (olho == null)
        {
            Debug.LogError("Referência ao Transform 'olho' não está atribuída no Inspector.");
            return;
        }

        // Definir a direção com base na rotação
        float direcao = paraEsquerda ? -1f : 1f;
        float anguloOffset = angulo / 2f;

        // Calcular o ângulo total para a direção do laser
        float anguloTotal = direcao * anguloOffset;

        // Converter o ângulo para radianos
        float anguloRad = anguloTotal * Mathf.Deg2Rad;

        // Calcular a direção do laser usando funções trigonométricas
        Vector2 direcaoVector = new Vector2(Mathf.Cos(anguloRad), Mathf.Sin(anguloRad));

        // Definir os pontos de início e fim do laser
        Vector2 pontoInicio = olho.position;
        Vector2 pontoFim = pontoInicio + direcaoVector * alcance;

        lineRenderer.SetPosition(0, pontoInicio);
        lineRenderer.SetPosition(1, pontoFim);
        lineRenderer.enabled = true;

        Debug.Log("Laser ativado: " + (paraEsquerda ? "Esquerda" : "Direita"));
    }

    public void DesativarLaser()
    {
        lineRenderer.enabled = false;
        timer = 0f;
        Debug.Log("Laser desativado.");
    }

    void AplicarDano()
    {
        if (olho == null)
        {
            Debug.LogError("Referência ao Transform 'olho' não está atribuída no Inspector.");
            return;
        }

        // Direção do laser
        Vector2 direcao = (lineRenderer.GetPosition(1) - lineRenderer.GetPosition(0)).normalized;

        // Realizar Raycast para detectar objetos
        RaycastHit2D[] hits = Physics2D.RaycastAll(olho.position, direcao, alcance, camadaAlvo);

        foreach (RaycastHit2D hit in hits)
        {
            // Verificar se o objeto possui o componente Health
            Health health = hit.collider.GetComponent<Health>();
            if (health != null)
            {
                // Chamar o método Damage do CorgiEngine Health
                health.Damage(dano, instigator, flickerDuration, invincibilityDuration, direcao);

                // Reproduzir feedback de dano, se configurado
                if (danoFeedbacks != null)
                {
                    danoFeedbacks.PlayFeedbacks(hit.collider.transform.position);
                }

                Debug.Log("Dano aplicado a: " + hit.collider.name);
            }

            // Opcional: Adicionar knockback ou outros efeitos
            // Por exemplo, você pode adicionar knockback aqui se desejar
        }
    }

    void OnDrawGizmosSelected()
    {
        if (lineRenderer != null && lineRenderer.enabled)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(lineRenderer.GetPosition(0), lineRenderer.GetPosition(1));
        }
    }
}
