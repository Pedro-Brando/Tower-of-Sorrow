using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;

namespace MoreMountains.CorgiEngine
{
    [AddComponentMenu("Corgi Engine/Character/Abilities/Hod/Dispersar")]
    public class Dispersar : CharacterAbility, IHodAbility
    {
        #region Serialized Fields

        [Header("Configurações do Dispersar")]

        [Tooltip("Prefab das cópias de Hod.")]
        [SerializeField] private GameObject hodCopyPrefab;

        [Tooltip("Duração da habilidade antes de terminar automaticamente (em segundos).")]
        [SerializeField] private float abilityDuration = 5f;

        [Tooltip("Duração do cooldown da habilidade (em segundos).")]
        [SerializeField] private float cooldownDuration = 10f;

        [Tooltip("Posições onde Hod e suas cópias aparecerão.")]
        [SerializeField] private Transform[] positions;

        [Tooltip("Feedback visual e sonoro ao iniciar a habilidade Dispersar.")]
        [SerializeField] private MMF_Player dispersarFeedback;

        [Header("Configurações de Dano")]

        [Tooltip("Dano causado ao jogador se a habilidade não for cancelada.")]
        [SerializeField] private int damageToPlayer = 10;

        [Header("Configurações de Tempo")]

        [Tooltip("Tempo para o jogador cancelar a habilidade ao atacar Hod (em segundos).")]
        [SerializeField] private float cancelDuration = 5f;

        #endregion

        #region Private Fields

        private float lastActivationTime = -Mathf.Infinity;

        public bool CooldownReady => Time.time >= lastActivationTime + cooldownDuration;

        public new bool AbilityPermitted => base.AbilityPermitted;

        public event Action OnAbilityCompleted;

        private HodController hodController;
        private List<GameObject> copies = new List<GameObject>();
        private GameObject trueHodInstance;

        private Coroutine dispersarRoutineCoroutine;

        private bool isAbilityActive = false;

        private float abilityTimer = 0f;
        private bool isCancelled = false;

        #endregion

        #region Initialization

        protected override void Initialization()
        {
            base.Initialization();

            // Referências necessárias
            hodController = GetComponent<HodController>();
            if (hodController == null)
            {
                Debug.LogError("HodController não encontrado no GameObject!");
            }

            if (hodCopyPrefab == null)
            {
                Debug.LogError("HodCopyPrefab não atribuído!");
            }

            if (positions == null || positions.Length == 0)
            {
                Debug.LogError("Nenhuma posição definida para as cópias de Hod!");
            }
        }

        #endregion

        #region Ability Activation

        /// <summary>
        /// Método público para ativar a habilidade Dispersar
        /// </summary>
        public void ActivateAbility()
        {
            if (AbilityAuthorized && CooldownReady && !isAbilityActive)
            {
                isAbilityActive = true;
                lastActivationTime = Time.time;
                dispersarRoutineCoroutine = StartCoroutine(DispersarRoutine());
            }
        }

        /// <summary>
        /// Coroutine que gerencia a execução da habilidade Dispersar
        /// </summary>
        private IEnumerator DispersarRoutine()
        {
            Debug.Log("Hod começou a usar Dispersar.");

            // Play feedbacks
            dispersarFeedback?.PlayFeedbacks();

            // Make Hod invisible and stop movement
            hodController.SetVisible(false);
            hodController.StopMovement();

            // Create copies
            CreateCopies();

            // Start cancel timer
            abilityTimer = 0f;
            isCancelled = false;

            // Subscribe to HodController's OnAttacked event
            if (hodController != null)
            {
                hodController.OnAttacked += HandleHodAttacked;
            }

            // Wait for cancel duration or until cancelled
            while (abilityTimer < cancelDuration && !isCancelled)
            {
                abilityTimer += Time.deltaTime;
                yield return null;

                // Optional: You can add logic here to allow for early termination if needed
            }

            // Unsubscribe from HodController's OnAttacked event
            if (hodController != null)
            {
                hodController.OnAttacked -= HandleHodAttacked;
            }

            if (isCancelled)
            {
                Debug.Log("Hod foi atacado. Cancelando Dispersar.");
                CancelDispersar();
            }
            else
            {
                Debug.Log("Tempo de cancelamento expirou. Aplicando dano ao jogador.");
                // ApplyDamageToPlayer();
                EndDispersar();
            }
        }

        #endregion

        #region Ability Behavior

        /// <summary>
        /// Cria as cópias de Hod e posiciona-as nas posições definidas
        /// </summary>
        private void CreateCopies()
        {
            // Selecionar aleatoriamente qual posição terá a verdadeira Hod
            int trueHodIndex = UnityEngine.Random.Range(0, positions.Length);

            for (int i = 0; i < positions.Length; i++)
            {
                // Instanciar cópia
                GameObject copy = Instantiate(hodCopyPrefab, positions[i].position, Quaternion.identity);
                HodCopy hodCopyScript = copy.GetComponent<HodCopy>();

                if (hodCopyScript != null)
                {
                    if (i == trueHodIndex)
                    {
                        // Inicializar como verdadeira Hod
                        hodCopyScript.Initialize(true, hodController);
                        trueHodInstance = copy;
                    }
                    else
                    {
                        // Inicializar como cópia falsa
                        hodCopyScript.Initialize(false);
                        // Alterar aparência da cópia falsa, se necessário
                        ApplyFalseCopyVisuals(copy);
                    }
                }

                copies.Add(copy);
            }
        }

        /// <summary>
        /// Aplica alterações visuais às cópias falsas para diferenciá-las da verdadeira Hod
        /// </summary>
        /// <param name="copy">A cópia de Hod falsa.</param>
        private void ApplyFalseCopyVisuals(GameObject copy)
        {
            // Exemplo: mudar a cor do Sprite Renderer para vermelho
            SpriteRenderer sr = copy.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = Color.red;
            }

            // Adicionar outros detalhes visuais conforme necessário
        }

        /// <summary>
        /// Cancela a habilidade Dispersar após Hod ser atacado
        /// </summary>
        private void CancelDispersar()
        {
            // Parar a coroutine da habilidade, se ainda estiver rodando
            if (dispersarRoutineCoroutine != null)
            {
                StopCoroutine(dispersarRoutineCoroutine);
            }

            // Destruir cópias
            DestroyCopies();

            // Fazer Hod reaparecer e iniciar fuga
            hodController.SetVisible(true);
            hodController.ResumeMovement();
            // hodController.StartFleeing();

            // Marcar habilidade como inativa
            isAbilityActive = false;

            // Iniciar cooldown
            StartCoroutine(CooldownRoutine());

            // Notificar que a habilidade foi concluída
            OnAbilityCompleted?.Invoke();
        }

        /// <summary>
        /// Aplica dano ao jogador após a duração da habilidade
        /// </summary>
        private void ApplyDamageToPlayer()
        {
            // Utilizar referência direta ao jogador, já obtida no HodController
            if (hodController != null && hodController.Player != null)
            {
                Health playerHealth = hodController.Player.GetComponent<Health>();
                if (playerHealth != null)
                {
                    playerHealth.Damage(damageToPlayer, gameObject, 0f, 0f, Vector3.zero);
                }
                else
                {
                    Debug.LogError("Componente Health não encontrado no Player!");
                }
            }
            else
            {
                Debug.LogError("Player não encontrado no HodController!");
            }
        }

        /// <summary>
        /// Finaliza a habilidade Dispersar após a duração e aplica as ações finais
        /// </summary>
        public void EndDispersar()
        {
            // Destruir cópias
            DestroyCopies();

            // Fazer Hod reaparecer e iniciar fuga
            hodController.SetVisible(true);
            hodController.ResumeMovement();
            // hodController.StartFleeing();

            // Iniciar cooldown
            StartCoroutine(CooldownRoutine());

            // Marcar habilidade como inativa
            isAbilityActive = false;

            // Notificar que a habilidade foi concluída
            OnAbilityCompleted?.Invoke();
        }

        /// <summary>
        /// Destroi todas as cópias criadas durante a habilidade
        /// </summary>
        private void DestroyCopies()
        {
            foreach (var copy in copies)
            {
                if (copy != null)
                {
                    Destroy(copy);
                }
            }
            copies.Clear();
        }

        /// <summary>
        /// Coroutine para gerenciar o cooldown da habilidade
        /// </summary>
        private IEnumerator CooldownRoutine()
        {
            Debug.Log("Cooldown da habilidade Dispersar iniciado.");
            yield return new WaitForSeconds(cooldownDuration);
            Debug.Log("Cooldown da habilidade Dispersar concluído.");
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handler chamado quando Hod é atacado
        /// </summary>
        private void HandleHodAttacked()
        {
            isCancelled = true;
        }

        #endregion

        #region Interface Implementation

        /// <summary>
        /// Método para lidar com mudanças de fase, se necessário
        /// </summary>
        /// <param name="currentPhase">A fase atual do jogo.</param>
        public void HandlePhase(int currentPhase)
        {
            // Implementar lógica específica para fases, se necessário
            // Por exemplo, ajustar número de cópias, posições, etc.
            // Este método pode ser expandido conforme as necessidades das fases
        }

        #endregion
    }
}
