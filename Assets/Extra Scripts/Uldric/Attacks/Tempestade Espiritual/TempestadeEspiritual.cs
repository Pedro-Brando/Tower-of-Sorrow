using UnityEngine;
using System.Collections; // Necessário para IEnumerator e corrotinas
using System.Collections.Generic;
using MoreMountains.CorgiEngine;

namespace MoreMountains.CorgiEngine
{
    [AddComponentMenu("Assets/Extra Scripts/Uldric/Attacks/Tempestade Espiritual")]
    public class TempestadeEspiritual : CharacterAbility
    {
        [Header("Configurações da Tempestade Espiritual")]
        
        [Tooltip("Prefab da linha diagonal transparente")]
        public GameObject DiagonalLinePrefab;

        [Tooltip("Prefab do meteoro que cairá pelas linhas")]
        public GameObject MeteoroPrefab;

        [Tooltip("Número de linhas diagonais")]
        public int NumberOfLines = 5;

        [Tooltip("Espaçamento horizontal entre as linhas diagonais")]
        public float LineSpacing = 2f;

        [Tooltip("Offset vertical para definir até onde as linhas devem ir abaixo do chão")]
        public float LineVerticalOffset = 1f; // Nova variável adicionada

        [Tooltip("Duração que as linhas permanecem na tela (em segundos)")]
        public float LinesDuration = 1f;

        [Tooltip("Tempo de delay antes dos meteoros caírem (em segundos)")]
        public float MeteoroDelay = 1f;

        [Tooltip("Dano aplicado pelo meteoro")]
        public float MeteoroDamage = 1f;

        [Tooltip("Cor das linhas diagonais")]
        public Color LineColor = new Color(1f, 1f, 1f, 0.5f); // Branco semi-transparente

        [Header("Configurações de Direção")]
        
        [Tooltip("Ângulo de inclinação das linhas em graus (0 = horizontal, 90 = vertical para cima)")]
        [Range(0f, 360f)]
        public float DirectionAngle = -45f; // Exemplo: -45 graus para direção para a direita e para baixo

        [Header("Referências")]
        [Tooltip("Referência para a área de spawn dos meteoros")]
        public MeteorSpawnArea meteorSpawnArea;

        protected Coroutine _tempestadeCoroutine;

        // Propriedade para verificar se a habilidade está sendo executada
        public bool IsExecuting { get; protected set; }

        private Vector2 directionVector;
        private float previousDirectionAngle;

        /// <summary>
        /// Inicialização da habilidade
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();

            if (DiagonalLinePrefab == null)
            {
                Debug.LogError("DiagonalLinePrefab não está atribuído no Inspector de TempestadeEspiritual!");
            }

            if (MeteoroPrefab == null)
            {
                Debug.LogError("MeteoroPrefab não está atribuído no Inspector de TempestadeEspiritual!");
            }

            // Assegurar que o MeteoroPool está presente na cena
            if (MeteoroPool.Instance == null)
            {
                Debug.LogError("MeteoroPool.Instance não está definido. Adicione um GameObject com o script MeteoroPool na cena.");
            }

            // Verificar a referência ao MeteorSpawnArea
            if (meteorSpawnArea == null)
            {
                Debug.LogError("MeteorSpawnArea não está atribuído no Inspector de TempestadeEspiritual!");
            }

            // Converter o ângulo em vetor de direção
            UpdateDirectionVector();

            // Inicializar o valor anterior do ângulo
            previousDirectionAngle = DirectionAngle;
        }

        /// <summary>
        /// Atualiza a direção das linhas se o DirectionAngle for modificado
        /// </summary>
        void Update()
        {
            if (Mathf.Abs(DirectionAngle - previousDirectionAngle) > Mathf.Epsilon)
            {
                UpdateDirectionVector();
                previousDirectionAngle = DirectionAngle;

                Debug.Log($"DirectionAngle alterado para {DirectionAngle} graus. Vetor de direção atualizado para {directionVector}.");
            }
        }

        /// <summary>
        /// Atualiza o vetor de direção baseado no DirectionAngle
        /// </summary>
        private void UpdateDirectionVector()
        {
            float radians = DirectionAngle * Mathf.Deg2Rad;
            directionVector = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)).normalized;
        }

        /// <summary>
        /// Processa a habilidade
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();

            // Não usamos AbilityAuthorized para ativar a habilidade manualmente
        }

        /// <summary>
        /// Método público para ativar a habilidade
        /// </summary>
        public void ActivateAbility()
        {
            if (IsExecuting)
                return;

            _tempestadeCoroutine = StartCoroutine(TempestadeRoutine());
        }

        /// <summary>
        /// Coroutine que gerencia a execução da tempestade espiritual
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator TempestadeRoutine()
        {
            IsExecuting = true;
            Debug.Log("Tempestade Espiritual Iniciada!");

            // Criar as linhas diagonais
            List<GameObject> linhas = new List<GameObject>();

            // Espaçamento inicial
            float initialOffsetX = -((NumberOfLines - 1) * LineSpacing) / 2f;

            for (int i = 0; i < NumberOfLines; i++)
            {
                // Obter uma posição de spawn aleatória ao longo do topo da área de spawn
                Vector3 spawnPosition = meteorSpawnArea.GetRandomSpawnPosition();

                // Calcular o deslocamento horizontal baseado no índice da linha
                float offsetX = initialOffsetX + i * LineSpacing;

                // Aplicar o deslocamento horizontal ao spawnPosition para espalhar as linhas horizontalmente
                Vector3 adjustedSpawnPosition = spawnPosition + new Vector3(offsetX, 0f, 0f);

                // Encontrar a posição do chão diretamente abaixo da posição de spawn ajustada
                Vector3 groundPosition = FindGroundPosition(adjustedSpawnPosition);

                // Aplicar a direção comum e o offset vertical ao targetPosition
                Vector3 targetPosition = adjustedSpawnPosition + (Vector3)directionVector * LineVerticalOffset;

                Debug.Log($"Instanciando linha diagonal {i + 1} de {adjustedSpawnPosition} até {targetPosition}");

                // Instanciar a linha diagonal
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
            }

            // Esperar a duração das linhas
            Debug.Log($"Esperando {LinesDuration} segundos antes de lançar os meteoros.");
            yield return new WaitForSeconds(LinesDuration);

            // Instanciar os meteoros
            foreach (GameObject linha in linhas)
            {
                Vector3 spawnPositionMeteoro = linha.transform.position;
                Vector3 targetPositionMeteoro = linha.GetComponent<LineRenderer>().GetPosition(1);

                Debug.Log($"Instanciando meteoro em {spawnPositionMeteoro} direcionado para {targetPositionMeteoro}");
                SpawnMeteoro(spawnPositionMeteoro, targetPositionMeteoro);
                yield return new WaitForSeconds(MeteoroDelay);
            }

            // Destruir as linhas diagonais
            foreach (GameObject linha in linhas)
            {
                Destroy(linha);
            }

            Debug.Log("Tempestade Espiritual Finalizada.");
            _tempestadeCoroutine = null;
            IsExecuting = false;
        }

        /// <summary>
        /// Calcula a posição do chão usando Raycast
        /// </summary>
        /// <param name="startPosition">Posição inicial do Raycast</param>
        /// <returns>Posição do chão</returns>
        protected virtual Vector3 FindGroundPosition(Vector3 startPosition)
        {
            RaycastHit2D hit = Physics2D.Raycast(startPosition, Vector2.down, Mathf.Infinity, LayerMask.GetMask("Ground"));
            if (hit.collider != null)
            {
                return hit.point;
            }
            else
            {
                Debug.LogWarning("Nenhum chão encontrado abaixo de " + startPosition + ". Usando posição padrão.");
                // Retorna uma posição padrão se nenhum chão for encontrado
                return new Vector3(startPosition.x, startPosition.y - Camera.main.orthographicSize, startPosition.z);
            }
        }

        /// <summary>
        /// Instancia e inicializa um meteoro na posição especificada
        /// </summary>
        /// <param name="startPosition">Posição de spawn do meteoro (topo)</param>
        /// <param name="targetPosition">Posição alvo do meteoro (chão)</param>
        protected virtual void SpawnMeteoro(Vector3 startPosition, Vector3 targetPosition)
        {
            if (MeteoroPool.Instance != null)
            {
                GameObject meteoroInstance = MeteoroPool.Instance.GetMeteoro();
                meteoroInstance.transform.position = startPosition;
                meteoroInstance.transform.rotation = Quaternion.identity; // Resetar rotação, se necessário

                Meteoro meteoroScript = meteoroInstance.GetComponent<Meteoro>();
                if (meteoroScript != null)
                {
                    meteoroScript.Initialize(targetPosition, MeteoroDamage);
                    Debug.Log($"Meteoro inicializado direcionando para {targetPosition} com dano {MeteoroDamage}");
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

        /// <summary>
        /// Reseta a habilidade quando a personagem morre ou revive
        /// </summary>
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

        /// <summary>
        /// Desenha gizmos para visualizar as linhas diagonais no Editor
        /// </summary>
        void OnDrawGizmosSelected()
        {
            if (DiagonalLinePrefab == null || meteorSpawnArea == null)
                return;

            Gizmos.color = Color.red;
            for (int i = 0; i < NumberOfLines; i++)
            {
                // Calcular o deslocamento horizontal para espalhar as linhas
                float initialOffsetX = -((NumberOfLines - 1) * LineSpacing) / 2f;
                float offsetX = initialOffsetX + i * LineSpacing;

                // Obter uma posição de spawn aleatória ao longo do topo da área de spawn
                Vector3 spawnPosition = meteorSpawnArea.GetRandomSpawnPosition();

                // Ajustar a posição de spawn com o deslocamento horizontal
                Vector3 adjustedSpawnPosition = spawnPosition + new Vector3(offsetX, 0f, 0f);

                // Aplicar a direção comum e o offset vertical ao targetPosition
                Vector3 targetPosition = adjustedSpawnPosition + (Vector3)directionVector * LineVerticalOffset;

                Gizmos.DrawLine(adjustedSpawnPosition, targetPosition);
                Gizmos.DrawSphere(adjustedSpawnPosition, 0.2f);
                Gizmos.DrawSphere(targetPosition, 0.2f);
            }
        }
    }
}
