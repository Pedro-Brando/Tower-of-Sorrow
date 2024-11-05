using UnityEngine;
using MoreMountains.CorgiEngine;

namespace MoreMountains.CorgiEngine
{
    public class CeifarAttackArea : MonoBehaviour
    {
        [Tooltip("Dano aplicado ao jogador")]
        public float Damage = 20f;

        [Tooltip("GameObject que realizou o ataque")]
        public GameObject Attacker;

        /// <summary>
        /// Define o dano da área de ataque
        /// </summary>
        public void SetDamage(float damage)
        {
            Damage = damage;
        }

        /// <summary>
        /// Define o atacante da área de ataque
        /// </summary>
        public void SetAttacker(GameObject attacker)
        {
            Attacker = attacker;
        }

        /// <summary>
        /// Detecta colisões com o jogador para aplicar dano
        /// </summary>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Verifica se o objeto colidido é o jogador
            if (collision.CompareTag("Player"))
            {
                Health playerHealth = collision.GetComponent<Health>();
                if (playerHealth != null)
                {
                    // Aplica dano ao jogador
                    playerHealth.Damage(Damage, Attacker, flickerDuration: 0.1f, invincibilityDuration: 1f, damageDirection: transform.position, typedDamages: null);
                    Debug.Log($"Ceifar causou {Damage} de dano ao jogador {collision.gameObject.name}.");
                }
            }
        }
    }
}
