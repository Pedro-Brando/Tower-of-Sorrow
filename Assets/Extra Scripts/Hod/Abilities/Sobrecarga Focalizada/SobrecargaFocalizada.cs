using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;

namespace MoreMountains.CorgiEngine
{
    [AddComponentMenu("Corgi Engine/Character/Abilities/Hod/SobrecargaFocalizada")]
    public class SobrecargaFocalizada : CharacterAbility, IHodAbility
    {
        [Header("Configurações da Sobrecarga Focalizada")]

        [Tooltip("Prefab do indicador visual que aparece antes dos feixes")]
        public GameObject IndicatorPrefab;

        [Tooltip("Prefab do feixe vertical que é disparado")]
        public GameObject VerticalBeamPrefab;

        [Tooltip("Número de feixes a serem disparados")]
        public int NumberOfBeams = 3;

        [Tooltip("Duração do cooldown da habilidade")]
        public float CooldownDuration = 10f;

        [Tooltip("Atraso antes de disparar os feixes após os indicadores aparecerem")]
        public float DelayBeforeBeam = 1.5f;

        [Header("Feedbacks MMF Player")]
        [Tooltip("Feedback ao iniciar a habilidade Sobrecarga Focalizada")]
        public MMF_Player SobrecargaFocalizadaFeedback;

        private float _lastActivationTime = -Mathf.Infinity;
        private HodController _hodController;
        private GameObject _player;

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

        protected override void Initialization()
        {
            base.Initialization();
            _hodController = GetComponent<HodController>();
            if (_hodController == null)
            {
                Debug.LogError("HodController not found on the GameObject!");
            }

            _player = GameObject.FindGameObjectWithTag("Player");
            if (_player == null)
            {
                Debug.LogError("Player not found in the scene!");
            }
        }

        /// <summary>
        /// Método público para ativar a habilidade Sobrecarga Focalizada
        /// </summary>
        public void ActivateAbility()
        {
            if (AbilityAuthorized && CooldownReady)
            {
                _lastActivationTime = Time.time;
                StartCoroutine(SobrecargaFocalizadaRoutine());
            }
        }

        /// <summary>
        /// Coroutine que gerencia a execução da habilidade Sobrecarga Focalizada
        /// </summary>
        /// <returns></returns>
        private IEnumerator SobrecargaFocalizadaRoutine()
        {
            // Início da habilidade
            Debug.Log("Hod começou a usar Sobrecarga Focalizada.");

            // Feedback ao iniciar a habilidade
            if (SobrecargaFocalizadaFeedback != null)
            {
                SobrecargaFocalizadaFeedback.PlayFeedbacks();
            }

            // Escolher posições aleatórias para os feixes
            Vector3[] positions = GetRandomPositions();

            // Criar indicadores
            List<GameObject> indicators = new List<GameObject>();
            foreach (var pos in positions)
            {
                GameObject indicator = Instantiate(IndicatorPrefab, pos, Quaternion.identity);
                indicators.Add(indicator);
            }

            // Esperar antes de disparar os feixes
            yield return new WaitForSeconds(DelayBeforeBeam);

            // Disparar feixes verticais
            foreach (var pos in positions)
            {
                Instantiate(VerticalBeamPrefab, pos, Quaternion.identity);
            }

            // Destruir os indicadores após o uso
            foreach (var indicator in indicators)
            {
                if (indicator != null)
                {
                    Destroy(indicator);
                }
            }

            // Acionar o evento indicando que a habilidade foi concluída
            OnAbilityCompleted?.Invoke();
        }

        /// <summary>
        /// Obtém posições aleatórias no chão para os feixes
        /// </summary>
        /// <returns>Array de posições</returns>
        private Vector3[] GetRandomPositions()
        {
            Vector3[] positions = new Vector3[NumberOfBeams];
            float minX = -10f; // Defina o mínimo X possível
            float maxX = 10f;  // Defina o máximo X possível
            float groundY = _player.transform.position.y; // Supondo que o chão está na mesma altura do jogador

            for (int i = 0; i < NumberOfBeams; i++)
            {
                float randomX = Random.Range(minX, maxX);
                positions[i] = new Vector3(randomX, groundY, 0f);
            }
            return positions;
        }

        /// <summary>
        /// Visualiza as posições dos feixes no editor
        /// </summary>
        public void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Vector3[] positions = GetRandomPositions();

            foreach (var pos in positions)
            {
                Gizmos.DrawWireSphere(pos, 0.5f);
            }
        }
    }
}
