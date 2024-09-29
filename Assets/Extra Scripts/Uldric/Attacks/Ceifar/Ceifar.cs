using UnityEngine;
using System.Collections;
using MoreMountains.CorgiEngine;

namespace MoreMountains.CorgiEngine
{
    public class Ceifar : CharacterAbility
    {
        [Header("Configurações do Ceifar")]
        
        [Tooltip("Prefab da foice mágica")]
        public GameObject ScythePrefab;

        [Tooltip("Prefab da animação de corte")]
        public GameObject ScytheSlashPrefab; // Prefab contendo a animação de corte

        [Tooltip("Tempo antes de realizar o ataque após a foice aparecer")]
        public float AttackDelay = 0.5f;
    [Tooltip("Tempo antes de realizar o ataque após a foice aparecer")]
        public float FlickerDamageTime = 0.5f;
        [Tooltip("Dano causado pelo ataque Ceifar")]
        public float Damage = 1f;

        [Tooltip("Referência à área de ataque do Ceifar")]
        public GameObject CeifarArea; // Referência ao GameObject da área do BoxCollider2D

        protected Collider2D _ceifarCollider; // Referência ao BoxCollider2D para controle

        /// <summary>
        /// Inicialização da habilidade Ceifar
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();

            if (CeifarArea != null)
            {
                _ceifarCollider = CeifarArea.GetComponent<Collider2D>();
                if (_ceifarCollider != null)
                {
                    _ceifarCollider.enabled = false; // Desativar o collider no início
                }
                else
                {
                    Debug.LogWarning("Nenhum Collider2D encontrado na CeifarArea!");
                }
            }
            else
            {
                Debug.LogWarning("CeifarArea não está atribuído!");
            }
        }

        /// <summary>
        /// Ativa o ataque Ceifar
        /// </summary>
        public void ActivateAbility()
        {
            StartCoroutine(CeifarRoutine());
        }

        /// <summary>
        /// Corrotina para gerenciar o ataque Ceifar
        /// </summary>
        protected virtual IEnumerator CeifarRoutine()
        {
            // Exibe a foice mágica acima do Uldric
            if (ScythePrefab != null)
            {
                GameObject scythe = Instantiate(ScythePrefab, CeifarArea.transform.position + Vector3.up, Quaternion.identity);
                Destroy(scythe, AttackDelay + 1f); // Destrói a foice após o ataque
            }

            // Espera antes de realizar o ataque
            yield return new WaitForSeconds(AttackDelay);

            // Ativa o collider da área de ataque
            if (_ceifarCollider != null)
            {
                _ceifarCollider.enabled = true; // Ativa o collider

                // Instancia a animação de corte na CeifarArea
                if (ScytheSlashPrefab != null)
                {
                    GameObject slashInstance = Instantiate(ScytheSlashPrefab, CeifarArea.transform.position, Quaternion.identity);
                    
                    // Toca a animação de corte
                    Animator slashAnimator = slashInstance.GetComponent<Animator>();
                    if (slashAnimator != null)
                    {
                        slashAnimator.Play("SlashAnimation"); // O nome da animação deve coincidir com a animação de corte no Animator
                    }

                    Destroy(slashInstance, 1f); // Destrói o prefab após a animação terminar (ajuste o tempo conforme necessário)
                }

                yield return new WaitForSeconds(FlickerDamageTime); // Tempo durante o qual o ataque está ativo
                _ceifarCollider.enabled = false; // Desativa o collider após o ataque
            }
        }

        /// <summary>
        /// Detecta colisões com o jogador e aplica dano
        /// </summary>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_ceifarCollider.enabled && collision.CompareTag("Player")) // O dano só é aplicado quando o collider está ativo
            {
                Health playerHealth = collision.GetComponent<Health>();
                if (playerHealth != null)
                {
                    playerHealth.Damage(Damage, gameObject, flickerDuration: 0.1f, invincibilityDuration: 1f, damageDirection: transform.position, typedDamages: null);
                    Debug.Log($"Ceifar causou {Damage} de dano ao jogador!");
                }
            }
        }
    }
}
