using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.CorgiEngine;
public class Meteoro : MonoBehaviour
{
    [Tooltip("Referência ao ImpactoEspiritual")]
    public ImpactoEspiritual impactoEspiritual;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            Debug.Log("Meteoro colidiu com o chão.");
            // Gera a onda de choque
            impactoEspiritual.GerarOndaDeChoque(transform.position);
            // Desativa o meteoro
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Meteoro tocou o jogador.");
            Health playerHealth = collision.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.Damage(impactoEspiritual.Damage, gameObject, 0.1f, 1f, transform.position, null);
            }
            // Desativa o meteoro após causar dano
            gameObject.SetActive(false);
        }
    }
}
