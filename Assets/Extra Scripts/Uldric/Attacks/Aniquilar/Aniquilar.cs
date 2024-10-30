using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;
using Unity.VisualScripting;

namespace MoreMountains.CorgiEngine
{
    [AddComponentMenu("Corgi Engine/Character/Abilities/Aniquilar")]
    public class Aniquilar : CharacterAbility, IUldrichAbility
    {
        [Header("Configurações da Aniquilar")]

        [Tooltip("Tempo de carregamento antes de ativar a habilidade")]
        public float ChargeTime = 5f;

        [Tooltip("Área de efeito do golpe (GameObject com BoxCollider2D)")]
        public BoxCollider2D EffectArea;

        [Tooltip("Layers que serão afetadas pelo golpe")]
        public LayerMask TargetLayers;

        [Tooltip("Dano causado (use um valor alto para Hit Kill)")]
        public int Damage = 9999;

        [Header("Prefabs e Efeitos")]
        [Tooltip("Prefab do anúncio a ser instanciado durante o carregamento")]
        public GameObject AnnouncementPrefab;

        [Tooltip("Prefab do efeito de partículas na área de efeito")]
        public GameObject AreaEffectPrefab;

        [Header("Cooldown")]
        [Tooltip("Duração do cooldown da habilidade Aniquilar")]
        public float CooldownDuration = 30f; // Ajuste conforme necessário

        [Header("Feedbacks MMF Player")]
        [Tooltip("Feedback ao iniciar o carregamento da Aniquilar")]
        public MMF_Player ChargeFeedback;

        [Tooltip("Feedback ao aplicar o efeito da Aniquilar")]
        public MMF_Player EffectFeedback;

        [Tooltip("Feedback ao completar a habilidade da Aniquilar")]
        public MMF_Player AbilityCompleteFeedback;

        private float _lastActivationTime = -Mathf.Infinity; // Armazena o momento da última ativação

        private GameObject _announcementInstance;
        private GameObject _areaEffectInstance;


        /// <summary>
        /// Propriedade que indica se a habilidade está permitida (herdada de CharacterAbility)
        /// </summary>
        public new bool AbilityPermitted => base.AbilityPermitted;

        /// <summary>
        /// Propriedade que indica se o cooldown terminou e a habilidade está pronta para uso
        /// </summary>
        public bool CooldownReady => Time.time >= _lastActivationTime + CooldownDuration;

        // Evento para indicar quando a habilidade foi concluída
        public event System.Action OnAbilityCompleted;

        /// <summary>
        /// Método público para ativar a habilidade Aniquilar
        /// </summary>
        public void ActivateAbility()
        {
            if (AbilityAuthorized)
            {
                if (Time.time >= _lastActivationTime + CooldownDuration)
                {
                    _lastActivationTime = Time.time;
                    StartCoroutine(AniquilarRoutine());
                }
                else
                {
                    // Opcional: Fornecer feedback indicando que a habilidade está em cooldown
                    float cooldownRemaining = (_lastActivationTime + CooldownDuration) - Time.time;
                    Debug.Log($"Aniquilar está em cooldown. Tempo restante: {cooldownRemaining:F1} segundos.");
                }
            }
        }

        /// <summary>
        /// Coroutine que gerencia a execução da Aniquilar
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator AniquilarRoutine()
        {
            // Início do carregamento
            Debug.Log("Uldrich começou a carregar Aniquilar.");

            // Feedback ao iniciar o carregamento
            if (ChargeFeedback != null)
            {
                ChargeFeedback.PlayFeedbacks();
            }

            // Instancia o anúncio
            if (AnnouncementPrefab != null)
            {
                _announcementInstance = Instantiate(AnnouncementPrefab, transform.position, Quaternion.identity);
                // Opcional: parent do anúncio ao personagem ou a outro objeto
                //_announcementInstance.transform.SetParent(transform);
            }

            // Aguarda o tempo de carregamento
            yield return new WaitForSeconds(ChargeTime);

            // Fim do carregamento
            Debug.Log("Aniquilar ativada!");

            // Destrói o anúncio
            if (_announcementInstance != null)
            {
                Destroy(_announcementInstance);
            }

            // Feedback ao aplicar o efeito da habilidade
            if (EffectFeedback != null)
            {
                EffectFeedback.PlayFeedbacks();
            }

            // Aplica dano às entidades na área de efeito
            ApplyDamageInArea();

            // Instancia o efeito de partículas na área de efeito
            if (AreaEffectPrefab != null && EffectArea != null)
            {
                _areaEffectInstance = Instantiate(AreaEffectPrefab, EffectArea.bounds.center, Quaternion.identity);
                // Opcional: ajustar o tamanho do efeito para cobrir a área
            }

            // Acionar o evento indicando que a habilidade foi concluída
            OnAbilityCompleted?.Invoke();

            // Feedback ao completar a habilidade
            if (AbilityCompleteFeedback != null)
            {
                AbilityCompleteFeedback.PlayFeedbacks();
            }

            // Limpeza adicional, se necessário
            // Exemplo: destruir a área de efeito após algum tempo
            if (_areaEffectInstance != null)
            {
                yield return new WaitForSeconds(2f); // Duração do efeito de partículas
                Destroy(_areaEffectInstance);
            }
        }

        /// <summary>
        /// Aplica dano às entidades na área de efeito
        /// </summary>
        protected virtual void ApplyDamageInArea()
        {
            if (EffectArea == null)
            {
                Debug.LogError("EffectArea não está atribuída!");
                return;
            }

            // Obtém todos os colliders na área de efeito que estão nas layers alvo
            Collider2D[] hits = Physics2D.OverlapBoxAll(
                EffectArea.bounds.center,
                EffectArea.bounds.size,
                0f,
                TargetLayers);

            foreach (Collider2D hit in hits)
            {
                Health healthComponent = hit.GetComponent<Health>();
                if (healthComponent != null)
                {
                    // Aplica dano letal
                    healthComponent.Damage(Damage, gameObject, 0f, 0f, Vector3.zero);
                    Debug.Log($"{hit.name} recebeu dano de Aniquilar.");
                }
            }
        }

        /// <summary>
        /// Visualiza a área de efeito no editor
        /// </summary>
        protected virtual void OnDrawGizmosSelected()
        {
            if (EffectArea != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(EffectArea.bounds.center, EffectArea.bounds.size);
            }
        }
    }
}
