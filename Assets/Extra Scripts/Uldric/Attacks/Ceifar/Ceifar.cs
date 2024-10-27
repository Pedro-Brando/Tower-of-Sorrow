using UnityEngine;
using System.Collections;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using System;
using MoreMountains.Feedbacks;

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

        [Header("Feedbacks MMF Player")]
        [Tooltip("Feedback ao iniciar o cast do Ceifar")]
        public MMF_Player CastFeedback;

        [Tooltip("Feedback ao realizar o ataque do Ceifar")]
        public MMF_Player AttackFeedback;

        [Tooltip("Feedback do aviso na tela antes do ataque Ceifar ser realizado")]
        public MMF_Player WarningFeedback;

        private float _lastActivationTime = -Mathf.Infinity; // Armazena o momento da última ativação

        /// <summary>
        /// Propriedade que indica se a habilidade está permitida (herdada de CharacterAbility)
        /// </summary>
        public new bool AbilityPermitted => base.AbilityPermitted;

        /// <summary>
        /// Propriedade que indica se o cooldown terminou e a habilidade está pronta para uso
        /// </summary>
        public bool CooldownReady => Time.time >= _lastActivationTime + CooldownDuration;

        public event Action OnAbilityCompleted;

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
            // Feedback do aviso na tela
            if (WarningFeedback != null)
            {
                WarningFeedback.PlayFeedbacks();
            }

            // Exibe a foice mágica como "warning" na tela
            if (ScythePrefab != null)
            {
                Vector3 spawnPosition = Camera.main.transform.position + Camera.main.transform.forward * 2;
                
                // Adiciona um offset ao componente Y para ajustar a posição vertical da foice
                float yOffset = 0f; // Ajuste este valor para obter o offset desejado
                spawnPosition.y -= yOffset;

                GameObject scytheWarning = Instantiate(ScythePrefab, spawnPosition, Quaternion.identity);
                scytheWarning.transform.SetParent(Camera.main.transform); // Faz a foice seguir a câmera como um aviso
                
                // Configurar o Animator da foice para usar UnscaledTime, assim ele não é afetado pelo timeScale
                Animator scytheAnimator = scytheWarning.GetComponent<Animator>();
                if (scytheAnimator != null)
                {
                    scytheAnimator.updateMode = AnimatorUpdateMode.UnscaledTime; // Isso faz a animação continuar mesmo quando timeScale for 0
                }

                Destroy(scytheWarning, 1f); // Destrói a foice após o tempo do aviso (ajustado para UnscaledTime)
            }

            // Feedback ao iniciar o cast do Ceifar
            if (CastFeedback != null)
            {
                CastFeedback.PlayFeedbacks();
            }

            // Espera antes de realizar o ataque
            yield return new WaitForSeconds(AttackDelay);

            // Feedback ao realizar o ataque do Ceifar
            if (AttackFeedback != null)
            {
                AttackFeedback.PlayFeedbacks();
            }

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
                        slashAnimator.updateMode = AnimatorUpdateMode.UnscaledTime; // Garante que a animação também não seja afetada
                        slashAnimator.Play("SlashAnimation"); // Certifique-se de que o nome corresponde ao da animação
                    }

                    Destroy(slashInstance, 1f); // Ajuste o tempo conforme a duração da animação
                }

                // Tempo durante o qual o ataque está ativo
                yield return new WaitForSeconds(FlickerDamageTime);

                _ceifarCollider.enabled = false; // Desativa o collider após o ataque
                OnAbilityCompleted?.Invoke();
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
