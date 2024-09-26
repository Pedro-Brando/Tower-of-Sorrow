using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    [RequireComponent(typeof(LineRenderer))]
    public class LinhaDiagonal : MonoBehaviour
    {
        private LineRenderer _lineRenderer;

        /// <summary>
        /// Inicializa o LineRenderer
        /// </summary>
        void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            if (_lineRenderer == null)
            {
                Debug.LogError("LinhaDiagonal requer um componente LineRenderer!");
            }
            else
            {
                // Configurações iniciais do LineRenderer
                _lineRenderer.positionCount = 2;
                _lineRenderer.startWidth = 0.05f;
                _lineRenderer.endWidth = 0.05f;
                _lineRenderer.useWorldSpace = true; // Usar World Space para conectar pontos diferentes
                _lineRenderer.alignment = LineAlignment.TransformZ;
            }
        }

        /// <summary>
        /// Define a direção da linha diagonal (pode ser usada para ajustar inclinação se necessário)
        /// </summary>
        /// <param name="right">Se true, a linha vai para a direita; caso contrário, para a esquerda</param>
        public void SetLineDirection(bool right)
        {
            // Atualmente, este método não altera a direção, pois as posições são definidas diretamente.
            // Você pode adicionar lógica adicional aqui se desejar alterar a inclinação com base na direção.
        }

        /// <summary>
        /// Define a cor da linha diagonal
        /// </summary>
        /// <param name="color">Cor desejada</param>
        public void SetLineColor(Color color)
        {
            if (_lineRenderer == null) return;

            _lineRenderer.startColor = color;
            _lineRenderer.endColor = color;
        }

        /// <summary>
        /// Define os pontos de início e fim da linha diagonal
        /// </summary>
        /// <param name="startPoint">Ponto inicial</param>
        /// <param name="endPoint">Ponto final</param>
        public void SetLineEndpoints(Vector3 startPoint, Vector3 endPoint)
        {
            if (_lineRenderer == null) return;

            _lineRenderer.SetPosition(0, startPoint);
            _lineRenderer.SetPosition(1, endPoint);
        }

        /// <summary>
        /// Desenha gizmos para visualizar a linha diagonal no Editor
        /// </summary>
        void OnDrawGizmosSelected()
        {
            if (_lineRenderer == null)
                return;

            Gizmos.color = Color.green;
            Vector3 start = _lineRenderer.GetPosition(0);
            Vector3 end = _lineRenderer.GetPosition(1);
            Gizmos.DrawLine(start, end);
        }
    }
}
