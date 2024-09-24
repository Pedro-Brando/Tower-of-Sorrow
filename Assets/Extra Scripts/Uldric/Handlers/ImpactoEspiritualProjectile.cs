using UnityEngine;

public class ImpactoEspiritualProjectile : MonoBehaviour
{
    public float Lifetime = 5f;                    // Tempo de vida do projétil antes de ser destruído
    public LayerMask GroundLayer;                  // Camada do chão para detecção de colisão
    public GameObject ShockwavePrefab;             // Prefab da onda de choque

    private Vector3 _direction;                    // Direção do movimento
    private float _speed;                          // Velocidade do projétil

    void Start()
    {
        // Destroi o projétil após seu tempo de vida
        Destroy(gameObject, Lifetime);
    }

    void Update()
    {
        // Move o projétil na direção definida
        transform.Translate(_direction * _speed * Time.deltaTime, Space.World);
    }

    // Inicializa a direção e a velocidade do projétil
    public void Initialize(Vector3 direction, float speed)
    {
        _direction = direction.normalized;
        _speed = speed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Detecta colisão com o chão
        if (((1 << other.gameObject.layer) & GroundLayer) != 0)
        {
            // Instanciar a onda de choque na posição de impacto
            if (ShockwavePrefab != null)
            {
                Instantiate(ShockwavePrefab, transform.position, Quaternion.identity);
            }

            // Destroi o projétil após gerar a onda de choque
            Destroy(gameObject);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Visualiza a direção do projétil no editor
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, _direction * 1f);
    }
}
