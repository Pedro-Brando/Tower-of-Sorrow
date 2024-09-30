using UnityEngine;
using System.Collections;
using MoreMountains.CorgiEngine;

namespace MoreMountains.CorgiEngine
{
    public class Ceifar : CharacterAbility, IUldrichAbility
    {
        [Header("Configurações do Ceifar")]
        
        [Tooltip("Prefab da foice mágica")]
        public GameObject ScythePrefab;

        [Tooltip("Prefab da animação de corte")]
        public GameObject ScytheSlashPrefab; // Prefab contendo a animação de corte

        [Tooltip("Tempo antes de realizar o ataque após a foice aparecer")]
        public float AttackDelay = 0.5f;

        [Tooltip("Tempo durante o qual o ataque está ativo")]
        public float FlickerDamageTime = 0.5f;

        [Tooltip("Dano causado pelo ataque Ceifar")]
        public float Damage = 1f;

        [Tooltip("Referência à área de ataque do Ceifar")]
        public GameObject CeifarArea; // Referência ao GameObject da área do ataque

        protected Collider2D _ceifarCollider; // Referência ao Collider2D para controle

        [Header("Cooldown")]
        [Tooltip("Duração do cooldown da habilidade Ceifar")]
        public float CooldownDuration = 5f; // Duração do cooldown em segundos

        private float _lastActivationTime = -Mathf.Infinity; // Armazena o momento da última ativação

        /// <summary>
        /// Propriedade que indica se a habilidade está permitida (herdada de CharacterAbility)
        /// </summary>
        public new bool AbilityPermitted => base.AbilityPermitted;

        /// <summary>
        /// Propriedade que indica se o cooldown terminou e a habilidade está pronta para uso
        /// </summary>
        public bool CooldownReady => Time.time >= _lastActivationTime + CooldownDuration;

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
                    _ceifarCollider.enabled = false; // Desativa o collider no início
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
            if (!AbilityAuthorized)
            {
                return;
            }

            if (Time.time >= _lastActivationTime + CooldownDuration)
            {
                _lastActivationTime = Time.time;
                StartCoroutine(CeifarRoutine());
            }
            else
            {
                // Opcional: Fornecer feedback indicando que a habilidade está em cooldown
                float cooldownRemaining = (_lastActivationTime + CooldownDuration) - Time.time;
                Debug.Log($"Ceifar está em cooldown. Tempo restante: {cooldownRemaining:F1} segundos.");
            }
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
                        slashAnimator.Play("SlashAnimation"); // Certifique-se de que o nome corresponde ao da animação
                    }

                    Destroy(slashInstance, 1f); // Ajuste o tempo conforme a duração da animação
                }

                // Tempo durante o qual o ataque está ativo
                yield return new WaitForSeconds(FlickerDamageTime);

                _ceifarCollider.enabled = false; // Desativa o collider após o ataque
            }
        }

        /// <summary>
        /// Detecta colisões com o jogador e aplica dano
        /// </summary>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_ceifarCollider != null && _ceifarCollider.enabled && collision.CompareTag("Player"))
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
