using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    public class MeteorSpawnArea : MonoBehaviour
    {
        [Tooltip("Altura acima da área de spawn onde os meteoros começam a cair")]
        public float SpawnHeightOffset = 0f;

        private float areaWidth;

        void Start()
        {
            // Calcula a largura da área baseada na escala do Sprite Renderer
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                areaWidth = sr.bounds.size.x;
            }
            else
            {
                Debug.LogError("MeteorSpawnArea requer um SpriteRenderer para calcular a largura.");
                areaWidth = 10f; // Valor padrão
            }
        }

        /// <summary>
        /// Retorna uma posição aleatória ao longo do topo da área de spawn no quadrante direito
        /// </summary>
        /// <returns>Posição de spawn para o meteoro</returns>
        public Vector3 GetRandomSpawnPosition()
        {
            float halfWidth = areaWidth / 2f;
            // Garante que randomX seja sempre positivo ou zero para estar no quadrante direito
            float randomX = Random.Range(0f, halfWidth);
            Vector3 spawnPosition = new Vector3(transform.position.x + randomX, transform.position.y + SpawnHeightOffset, transform.position.z);
            return spawnPosition;
        }
    }
}
