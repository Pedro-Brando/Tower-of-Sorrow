using UnityEngine;

/// <summary>
/// Componente para marcar o escombro como obstáculo permanente
/// </summary>
public class PermanentObstacle : MonoBehaviour
{
    private void Start()
    {
        // Ajustar o objeto para ser um obstáculo permanente
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.isTrigger = false;
        }

        // Alterar a layer para "Obstacles" ou outra layer adequada
        gameObject.layer = LayerMask.NameToLayer("Obstacles");
    }
}