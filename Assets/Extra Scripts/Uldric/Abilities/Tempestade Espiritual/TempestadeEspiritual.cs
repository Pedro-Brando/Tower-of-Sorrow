using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;

namespace MoreMountains.CorgiEngine
{
    [AddComponentMenu("Assets/Extra Scripts/Uldric/Attacks/Tempestade Espiritual")]
    public class TempestadeEspiritual : CharacterAbility, IUldrichAbility
    {
        [Header("Configurações da Tempestade Espiritual")]

        [Tooltip("Prefab da linha diagonal transparente")]
        public GameObject DiagonalLinePrefab;

        [Tooltip("Prefab do meteoro que cairá pelas linhas")]
        public GameObject MeteoroPrefab;

        [Tooltip("Prefab do indicador de impacto no solo")]
        public GameObject GroundIndicatorPrefab;

        [Tooltip("Número de linhas diagonais")]
        public int NumberOfLines = 5;

        [Tooltip("Espaçamento horizontal entre as linhas diagonais")]
        public float LineSpacing = 2f;

        [Tooltip("Offset vertical para definir até onde as linhas devem ir abaixo do chão")]
        public float LineVerticalOffset = 1f;

        [Tooltip("Duração que as linhas permanecem na tela (em segundos)")]
        public float LinesDuration = 1f;

        [Tooltip("Tempo de delay antes dos meteoros caírem (em segundos)")]
        public float MeteoroDelay = 1f;

        [Tooltip("Dano aplicado pelo meteoro")]
        public float MeteoroDamage = 1f;

        [Tooltip("Cor das linhas diagonais")]
        public Color LineColor = new Color(1f, 1f, 1f, 0.5f);

        [Header("Configurações de Direção")]
        [Tooltip("Ângulo de inclinação das linhas em graus (0 = horizontal, 90 = vertical para cima)")]
        [Range(0f, 360f)]
        public float DirectionAngle = -45f;

        [Header("Referências")]
        [Tooltip("Referência para a área de spawn dos meteoros")]
        public MeteorSpawnArea meteorSpawnArea;

        [Header("Cooldown")]
        [Tooltip("Duração do cooldown da habilidade Tempestade Espiritual")]
        public float CooldownDuration = 15f;

        [Header("Feedbacks MMF Player")]
        [Tooltip("Feedback de linha spawnada usando MMF Player")]
        public MMF_Player LineSpawnFeedback;

        [Tooltip("Feedback de impacto do meteoro usando MMF Player")]
        public MMF_Player MeteoroImpactFeedback;

        private float _lastActivationTime = -Mathf.Infinity;
        protected Coroutine _tempestadeCoroutine;
        public bool IsExecuting { get; protected set; }

        private Vector2 directionVector;
        private float previousDirectionAngle;

        public new bool AbilityPermitted => base.AbilityPermitted;
        public bool CooldownReady => Time.time >= _lastActivationTime + CooldownDuration;

        public event System.Action OnAbilityCompleted;

        protected override void Initialization()
        {
            base.Initialization();
            UpdateDirectionVector();
            previousDirectionAngle = DirectionAngle;
        }

        void Update()
        {
            if (Mathf.Abs(DirectionAngle - previousDirectionAngle) > Mathf.Epsilon)
            {
                UpdateDirectionVector();
                previousDirectionAngle = DirectionAngle;

                Debug.Log($"DirectionAngle alterado para {DirectionAngle} graus. Vetor de direção atualizado para {directionVector}.");
            }
        }

        private void UpdateDirectionVector()
        {
            float radians = DirectionAngle * Mathf.Deg2Rad;
            directionVector = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)).normalized;
        }

        public void ActivateAbility()
        {
            if (AbilityAuthorized && !IsExecuting)
            {
                if (CooldownReady)
                {
                    _lastActivationTime = Time.time;
                    _tempestadeCoroutine = StartCoroutine(TempestadeRoutine());
                }
                else
                {
                    float cooldownRemaining = (_lastActivationTime + CooldownDuration) - Time.time;
                    Debug.Log($"Tempestade Espiritual está em cooldown. Tempo restante: {cooldownRemaining:F1} segundos.");
                }
            }
        }

        protected virtual IEnumerator TempestadeRoutine()
        {
            IsExecuting = true;
            Debug.Log("Tempestade Espiritual Iniciada!");

            List<GameObject> linhas = new List<GameObject>();
            float initialOffsetX = -((NumberOfLines - 1) * LineSpacing) / 2f;

            for (int i = 0; i < NumberOfLines; i++)
            {
                Vector3 spawnPosition = meteorSpawnArea.GetRandomSpawnPosition();
                float offsetX = initialOffsetX + i * LineSpacing;
                Vector3 adjustedSpawnPosition = spawnPosition + new Vector3(offsetX, 0f, 0f);
                Vector3 targetPosition = adjustedSpawnPosition + (Vector3)directionVector * LineVerticalOffset;

                Debug.Log($"Instanciando linha diagonal {i + 1} de {adjustedSpawnPosition} até {targetPosition}");

                GameObject linha = Instantiate(DiagonalLinePrefab, adjustedSpawnPosition, Quaternion.identity);
                LinhaDiagonal linhaScript = linha.GetComponent<LinhaDiagonal>();
                if (linhaScript != null)
                {
                    linhaScript.SetLineColor(LineColor);
                    linhaScript.SetLineEndpoints(adjustedSpawnPosition, targetPosition);
                }
                else
                {
                    Debug.LogWarning("O prefab da linha diagonal não possui o componente LinhaDiagonal!");
                }
                linhas.Add(linha);

                // Feedback de linha usando MMF Player
                if (LineSpawnFeedback != null)
                {
                    LineSpawnFeedback.PlayFeedbacks();
                }
            }

            Debug.Log($"Esperando {LinesDuration} segundos antes de lançar os meteoros.");
            yield return new WaitForSeconds(LinesDuration);

            foreach (GameObject linha in linhas)
            {
                Vector3 spawnPositionMeteoro = linha.transform.position;
                Vector3 targetPositionMeteoro = linha.GetComponent<LineRenderer>().GetPosition(1);

                Debug.Log($"Instanciando meteoro em {spawnPositionMeteoro} direcionado para {targetPositionMeteoro}");
                SpawnMeteoro(spawnPositionMeteoro, targetPositionMeteoro);
                yield return new WaitForSeconds(MeteoroDelay);
            }

            foreach (GameObject linha in linhas)
            {
                Destroy(linha);
            }

            Debug.Log("Tempestade Espiritual Finalizada.");
            _tempestadeCoroutine = null;
            IsExecuting = false;

            OnAbilityCompleted?.Invoke();
        }

        protected virtual void SpawnMeteoro(Vector3 startPosition, Vector3 targetPosition)
        {
            if (MeteoroPool.Instance != null)
            {
                GameObject meteoroInstance = MeteoroPool.Instance.GetMeteoro();
                meteoroInstance.transform.position = startPosition;
                meteoroInstance.transform.rotation = Quaternion.identity;

                Meteoro meteoroScript = meteoroInstance.GetComponent<Meteoro>();
                if (meteoroScript != null)
                {
                    meteoroScript.Initialize(targetPosition, MeteoroDamage);
                    Debug.Log($"Meteoro inicializado direcionando para {targetPosition} com dano {MeteoroDamage}");

                    // Feedback de Impacto usando MMF Player
                    if (MeteoroImpactFeedback != null)
                    {
                        MeteoroImpactFeedback.PlayFeedbacks();
                    }
                }
                else
                {
                    Debug.LogWarning("O prefab do meteoro não possui o componente Meteoro!");
                }
            }
            else
            {
                Debug.LogError("MeteoroPool.Instance não está definido. Assegure-se de que o MeteoroPool está presente na cena.");
            }
        }

        public override void ResetAbility()
        {
            base.ResetAbility();

            if (_tempestadeCoroutine != null)
            {
                StopCoroutine(_tempestadeCoroutine);
                _tempestadeCoroutine = null;
            }

            IsExecuting = false;
        }

        void OnDrawGizmosSelected()
        {
            if (DiagonalLinePrefab == null || meteorSpawnArea == null)
                return;

            Gizmos.color = Color.red;
            for (int i = 0; i < NumberOfLines; i++)
            {
                float initialOffsetX = -((NumberOfLines - 1) * LineSpacing) / 2f;
                float offsetX = initialOffsetX + i * LineSpacing;

                Vector3 spawnPosition = meteorSpawnArea.GetRandomSpawnPosition();
                Vector3 adjustedSpawnPosition = spawnPosition + new Vector3(offsetX, 0f, 0f);
                Vector3 targetPosition = adjustedSpawnPosition + (Vector3)directionVector * LineVerticalOffset;

                Gizmos.DrawLine(adjustedSpawnPosition, targetPosition);
                Gizmos.DrawSphere(adjustedSpawnPosition, 0.2f);
                Gizmos.DrawSphere(targetPosition, 0.2f);
            }
        }
    }
}
