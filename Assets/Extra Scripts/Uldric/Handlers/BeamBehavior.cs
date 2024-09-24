using UnityEngine;

public class BeamBehavior : MonoBehaviour
{
    public float Range = 15f;            // Alcance do feixe
    public float Width = 0.5f;            // Largura do feixe
    public float Damage = 100f;           // Dano causado pelo feixe
    public LayerMask PlayerLayer;         // Camada do jogador para detecção de colisão

    private Vector3 _direction;           // Direção do feixe
    private float _speed = 0f;            // Velocidade do feixe (não se move, apenas existe)
    private float _currentRange = 0f;     // Alcance atual percorrido pelo feixe

    private LineRenderer _lineRenderer;    // Componente para desenhar o feixe visualmente

    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        if (_lineRenderer == null)
        {
            _lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        // Configurar o LineRenderer
        _lineRenderer.startWidth = Width;
        _lineRenderer.endWidth = Width;
        _lineRenderer.positionCount = 2;
        _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        _lineRenderer.startColor = Color.red;
        _lineRenderer.endColor = Color.yellow;
    }

    void Update()
    {
        // Atualizar a posição do feixe
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, transform.position + _direction * Range);

        // Detectar colisão com o jogador
        RaycastHit2D hit = Physics2D.Raycast(transform.position, _direction, Range, PlayerLayer);
        if (hit.collider != null)
        {
            // Aplicar dano ao jogador
            PlayerHealth playerHealth = hit.collider.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(Damage * Time.deltaTime);
            }

            // Opcional: aplicar efeitos visuais ou sonoros adicionais
        }
    }

    // Inicializar a direção e os parâmetros do feixe
    public void Initialize(Vector3 direction, float range, float width, float damage)
    {
        _direction = direction.normalized;
        Range = range;
        Width = width;
        Damage = damage;
    }

    void OnDrawGizmosSelected()
    {
        // Visualizar o feixe no editor
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + _direction * Range);
        Gizmos.DrawWireSphere(transform.position + _direction * Range, Width / 2f);
    }
}
