using UnityEngine;
using System.Collections;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;

namespace MoreMountains.CorgiEngine
{
    [AddComponentMenu("Corgi Engine/Character/Abilities/Hod/Invisibilidade")]
    public class Invisibilidade : CharacterAbility, IHodAbility
    {
        [Header("Configurações da Invisibilidade")]
        public float InvisibilityDuration = 10f;
        public float CooldownDuration = 15f;
        public MMF_Player InvisibilityFeedback;

        private bool _abilityPermitted = true;
        private bool _cooldownReady = true;
        public new bool AbilityPermitted => _abilityPermitted;
        public bool CooldownReady => _cooldownReady;
        public event System.Action OnAbilityCompleted;

        private HodController _hodController;

        protected override void Initialization()
        {
            base.Initialization();
            _hodController = GetComponent<HodController>();
        }

        public void ActivateAbility()
        {
            if (!_abilityPermitted || !_cooldownReady)
                return;

            StartCoroutine(InvisibilityRoutine());
        }

        private IEnumerator InvisibilityRoutine()
        {
            _abilityPermitted = false;
            _cooldownReady = false;

            // Ativar feedback de invisibilidade
            if (InvisibilityFeedback != null)
            {
                InvisibilityFeedback.PlayFeedbacks();
            }

            // Tornar Hod invisível
            _hodController.SetVisible(false);

            // Esperar a duração da invisibilidade
            yield return new WaitForSeconds(InvisibilityDuration);

            // Tornar Hod visível novamente
            _hodController.SetVisible(true);

            

            // Iniciar cooldown
            StartCoroutine(CooldownRoutine());

            // Notificar que a habilidade foi concluída
            OnAbilityCompleted?.Invoke();
        }

        private IEnumerator CooldownRoutine()
        {
            yield return new WaitForSeconds(CooldownDuration);
            _cooldownReady = true;
        }
    }
}
