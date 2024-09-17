using System.Collections;
using UnityEngine;

public class BossController2D : MonoBehaviour
{
    private Animator animator;
    private BossLaser2D bossLaser;

    [Header("Configurações de Ataque")]
    public float intervaloAtaque = 5f; // Tempo entre ataques
    public float tempoAntesDeAtacar = 2f; // Tempo de preparação antes do ataque

    void Start()
    {
        // Obtém o componente Animator anexado ao Boss
        animator = GetComponent<Animator>();
        
        // Obtém o componente BossLaser2D que está nos filhos do Boss
        bossLaser = GetComponentInChildren<BossLaser2D>();

        // Verifica se o Animator foi encontrado
        if (animator == null)
            Debug.LogError("Animator não encontrado no Boss.");

        // Verifica se o BossLaser2D foi encontrado
        if (bossLaser == null)
            Debug.LogError("BossLaser2D não encontrado nos filhos do Boss.");

        // Inicia a rotina de ataques
        StartCoroutine(RotinaAtaques());
    }

    IEnumerator RotinaAtaques()
    {
        while (true)
        {
            // Aguarda o intervalo de ataque
            yield return new WaitForSeconds(intervaloAtaque);
            
            // Tempo de preparação antes do ataque
            yield return new WaitForSeconds(tempoAntesDeAtacar);
            
            // Alterna direção aleatoriamente
            bool paraEsquerda = Random.value > 0.5f;
            if (paraEsquerda)
                animator.SetTrigger("VirarEsquerdaTrigger");
            else
                animator.SetTrigger("VirarDireitaTrigger");
        }
    }

    // Métodos chamados via Animation Events
    /// <summary>
    /// Ativa o laser na direção especificada.
    /// Chamado via Animation Event nas animações de virar.
    /// </summary>
    /// <param name="paraEsquerda">Se true, ativa o laser para a esquerda; caso contrário, para a direita.</param>
    public void AtivarLaser(bool paraEsquerda)
    {
        if (bossLaser != null)
        {
            bossLaser.AtivarLaser(paraEsquerda);
        }
        else
        {
            Debug.LogError("BossLaser2D não está atribuído.");
        }
    }

    /// <summary>
    /// Desativa o laser.
    /// Chamado via Animation Event após o ataque.
    /// </summary>
    public void DesativarLaser()
    {
        if (bossLaser != null)
        {
            bossLaser.DesativarLaser();
        }
        else
        {
            Debug.LogError("BossLaser2D não está atribuído.");
        }
    }
}
