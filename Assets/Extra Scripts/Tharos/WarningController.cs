using UnityEngine;
using MoreMountains.Feedbacks;

public class WarningController : MonoBehaviour
{
    private Animator animator;

    [Header("Feedbacks")]
    public MMFeedbacks feedbacksWarning; // Referência a um MMFeedbacks configurado no Inspector

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Activate");
            if (feedbacksWarning != null)
            {
                feedbacksWarning.PlayFeedbacks(transform.position);
                Debug.Log("Feedbacks da explosão ativados.");
            }
            else
            {
                Debug.LogWarning("MMFeedbacks não está atribuído em Explosao.cs.");
            }
            // Opcional: Desativar o aviso após a animação
            Destroy(gameObject, 1f); // Ajuste o tempo conforme a duração da animação
        }
        else
        {
            Debug.LogError("Animator não encontrado no Warning GameObject.");
        }
    }
}
