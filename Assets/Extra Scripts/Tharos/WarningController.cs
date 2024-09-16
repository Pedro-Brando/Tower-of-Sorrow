using UnityEngine;

public class WarningController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Activate");
            // Opcional: Desativar o aviso após a animação
            Destroy(gameObject, 1f); // Ajuste o tempo conforme a duração da animação
        }
        else
        {
            Debug.LogError("Animator não encontrado no Warning GameObject.");
        }
    }
}
