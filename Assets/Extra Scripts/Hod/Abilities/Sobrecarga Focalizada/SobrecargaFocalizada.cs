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

        [Tooltip("Atraso entre o disparo de cada feixe")]
        public float DelayBetweenBeams = 0.5f;

        [Header("Feedbacks MMF Player")]
        [Tooltip("Feedback ao iniciar a habilidade Sobrecarga Focalizada")]
        public MMF_Player SobrecargaFocalizadaFeedback;

        [Header("Configurações de Área")]
        [Tooltip("BoxCollider2D que define a área de efeito da habilidade.")]
        [SerializeField] private BoxCollider2D areaCollider;

        [Header("Configurações de Dano")]
        [Tooltip("Dano que cada feixe de Hod causará.")]
        [SerializeField] private int hodBeamDamage = 1;

        private float _lastActivationTime = -Mathf.Infinity;
        private HodController _hodController;
        public GameObject _player;

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
                Debug.LogError("HodController não encontrado no GameObject!");
            }

            _player = GameObject.FindGameObjectWithTag("Player");
            if (_player == null)
            {
                Debug.LogError("Player não encontrado na cena!");
            }

            if (areaCollider == null)
            {
                Debug.LogError("BoxCollider2D não foi atribuído em SobrecargaFocalizada!");
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
            SobrecargaFocalizadaFeedback?.PlayFeedbacks();

            // Escolher posições particionadas na área definida
            Vector3[] positions = GetPartitionedPositions();

            // Criar indicadores
            List<GameObject> indicators = new List<GameObject>();
            foreach (var pos in positions)
            {
                GameObject indicator = Instantiate(IndicatorPrefab, pos, Quaternion.identity);
                indicators.Add(indicator);
            }

            // Esperar antes de disparar os feixes
            yield return new WaitForSeconds(DelayBeforeBeam);

            // Disparar feixes verticais com atraso entre cada um
            foreach (var pos in positions)
            {
                GameObject beamInstance = Instantiate(VerticalBeamPrefab, pos, Quaternion.identity);
                Beam beamScript = beamInstance.GetComponent<Beam>();

                if (beamScript != null)
                {
                    // Definir direção do feixe (assumindo para cima)
                    beamScript.SetDirection(Vector3.up);

                    // Definir dano do feixe
                    beamScript.SetDamage(hodBeamDamage);
                }
                else
                {
                    Debug.LogError("Beam script não encontrado no VerticalBeamPrefab!");
                }

                // Esperar antes de disparar o próximo feixe
                yield return new WaitForSeconds(DelayBetweenBeams);
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
        /// Obtém posições particionadas igualmente ao longo do eixo X dentro da área definida pelo BoxCollider2D para os feixes
        /// </summary>
        /// <returns>Array de posições</returns>
        private Vector3[] GetPartitionedPositions()
        {
            Vector3[] positions = new Vector3[NumberOfBeams];

            if (areaCollider == null)
            {
                Debug.LogError("BoxCollider2D não está atribuído!");
                return positions;
            }

            Bounds bounds = areaCollider.bounds;

            float totalWidth = bounds.size.x;
            float partitionWidth = totalWidth / NumberOfBeams;

            float startX = bounds.min.x + partitionWidth / 2f; // Iniciar no centro da primeira partição
            float yPos = bounds.center.y; // Utilizar o centro vertical da área

            for (int i = 0; i < NumberOfBeams; i++)
            {
                float xPos = startX + i * partitionWidth;
                positions[i] = new Vector3(xPos, yPos, 0f);
            }

            return positions;
        }

        /// <summary>
        /// Visualiza a área de efeito no editor usando Gizmos
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            if (areaCollider != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(areaCollider.bounds.center, areaCollider.bounds.size);
            }
        }
    }
}
