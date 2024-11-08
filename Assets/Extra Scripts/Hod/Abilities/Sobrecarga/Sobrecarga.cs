using UnityEngine;
using System.Collections;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;

namespace MoreMountains.CorgiEngine
{
    [AddComponentMenu("Corgi Engine/Character/Abilities/Hod/Sobrecarga")]
    public class Sobrecarga : CharacterAbility, IHodAbility
    {
        [Header("Configurações da Sobrecarga")]
        public GameObject BeamPrefab;
        public int NumberOfBeams = 5;
        public float BeamSpreadAngle = 45f;
        public float CooldownDuration = 8f;
        public MMF_Player SobrecargaFeedback;

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

            StartCoroutine(SobrecargaRoutine());
        }

        private IEnumerator SobrecargaRoutine()
        {
            _abilityPermitted = false;
            _cooldownReady = false;

            // Parar movimento de Hod e virar para o jogador
            _hodController.StopMovement();
            _hodController.LookAtPlayer();

            // Iniciar feedback de Sobrecarga
            if (SobrecargaFeedback != null)
            {
                SobrecargaFeedback.PlayFeedbacks();
            }

            // Disparar feixes
            FireBeams();

            // Esperar um curto período antes de retomar o movimento
            yield return new WaitForSeconds(1f);

            // Retomar movimento
            _hodController.ResumeMovement();

            // Iniciar cooldown
            StartCoroutine(CooldownRoutine());

            // Notificar que a habilidade foi concluída
            OnAbilityCompleted?.Invoke();
        }

        private void FireBeams()
        {
            float angleStep = BeamSpreadAngle / (NumberOfBeams - 1);
            float startingAngle = -BeamSpreadAngle / 2;

            for (int i = 0; i < NumberOfBeams; i++)
            {
                float currentAngle = startingAngle + (angleStep * i);
                Quaternion rotation = Quaternion.Euler(0, 0, currentAngle);
                Instantiate(BeamPrefab, transform.position, rotation);
            }
        }

        private IEnumerator CooldownRoutine()
        {
            yield return new WaitForSeconds(CooldownDuration);
            _cooldownReady = true;
        }
    }
}
