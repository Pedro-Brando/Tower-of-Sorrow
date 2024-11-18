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
        [Tooltip("Prefab do feixe que será disparado.")]
        public GameObject BeamPrefab;

        [Tooltip("Número de feixes a serem disparados.")]
        public int NumberOfBeams = 5;

        [Tooltip("Ângulo total do cone de disparo (em graus).")]
        public float BeamSpreadAngle = 45f;

        [Tooltip("Tempo entre o disparo de cada feixe (em segundos).")]
        public float DelayBetweenBeams = 0.1f;

        [Tooltip("Duração do cooldown da habilidade (em segundos).")]
        public float CooldownDuration = 8f;

        [Tooltip("Feedback ao iniciar a habilidade Sobrecarga.")]
        public MMF_Player SobrecargaFeedback;

        [Header("Configurações de Dano")]
        [Tooltip("Dano que cada feixe causará.")]
        public int BeamDamage = 20;

        private bool _abilityPermitted = true;
        private bool _cooldownReady = true;
        public new bool AbilityPermitted => _abilityPermitted;
        public bool CooldownReady => _cooldownReady;
        public event System.Action OnAbilityCompleted;

        private HodController _hodController;
        private Transform _playerTransform;

        protected override void Initialization()
        {
            base.Initialization();
            _hodController = GetComponent<HodController>();
            if (_hodController == null)
            {
                Debug.LogError("Sobrecarga: HodController não encontrado no GameObject!");
            }

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                _playerTransform = player.transform;
            }
            else
            {
                Debug.LogError("Sobrecarga: Player não encontrado na cena!");
            }

            if (BeamPrefab == null)
            {
                Debug.LogError("Sobrecarga: BeamPrefab não foi atribuído!");
            }
        }

        public void ActivateAbility()
        {
            if (!_abilityPermitted || !_cooldownReady || _playerTransform == null)
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
            SobrecargaFeedback?.PlayFeedbacks();

            // Disparar feixes
            yield return StartCoroutine(FireBeams());

            // Esperar um curto período antes de retomar o movimento
            yield return new WaitForSeconds(1f);

            // Retomar movimento
            _hodController.ResumeMovement();

            // Iniciar cooldown
            StartCoroutine(CooldownRoutine());

            // Notificar que a habilidade foi concluída
            OnAbilityCompleted?.Invoke();
        }

        private IEnumerator FireBeams()
        {
            // Calcular a direção central (de Hod para o jogador)
            Vector3 directionToPlayer = (_playerTransform.position - transform.position).normalized;
            float centralAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

            // Calcular o ângulo de início para o cone de disparo
            float startingAngle = centralAngle - (BeamSpreadAngle / 2f);
            float angleStep = (BeamSpreadAngle) / (NumberOfBeams - 1);

            for (int i = 0; i < NumberOfBeams; i++)
            {
                float currentAngle = startingAngle + (angleStep * i);
                Quaternion rotation = Quaternion.Euler(0, 0, currentAngle);
                Vector3 spawnPosition = transform.position;

                // Instanciar o feixe com a rotação calculada
                GameObject beamInstance = Instantiate(BeamPrefab, spawnPosition, rotation);

                // Configurar o feixe
                BeamArc beamArc = beamInstance.GetComponent<BeamArc>();
                if (beamArc != null)
                {
                    beamArc.damage = BeamDamage;
                    // Opcional: ajustar outras propriedades do feixe, se necessário
                }
                else
                {
                    Debug.LogError("Sobrecarga: BeamArc script não encontrado no BeamPrefab!");
                }

                // Esperar antes de disparar o próximo feixe
                yield return new WaitForSeconds(DelayBetweenBeams);
            }

            yield return null;
        }

        private IEnumerator CooldownRoutine()
        {
            yield return new WaitForSeconds(CooldownDuration);
            _cooldownReady = true;
            _abilityPermitted = true;
        }
    }
}
