using UnityEngine;
using System.Collections.Generic;

namespace MoreMountains.CorgiEngine
{
    public class MeteoroPool : MonoBehaviour
    {
        // Singleton instance para fácil acesso global
        public static MeteoroPool Instance;

        [Tooltip("Prefab do meteoro a ser usado no pool")]
        public GameObject MeteoroPrefab;

        [Tooltip("Número inicial de meteoros no pool")]
        public int PoolSize = 20;

        // Queue para gerenciar os meteoros disponíveis
        private Queue<GameObject> _pool = new Queue<GameObject>();

        void Awake()
        {
            // Implementação do Singleton
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            // Inicializa o pool com meteoros desativados
            for (int i = 0; i < PoolSize; i++)
            {
                GameObject meteoro = Instantiate(MeteoroPrefab);
                meteoro.SetActive(false);
                _pool.Enqueue(meteoro);
            }
        }

        /// <summary>
        /// Obtém um meteoro do pool. Se o pool estiver vazio, um novo meteoro é instanciado.
        /// </summary>
        /// <returns>Um GameObject meteoro</returns>
        public GameObject GetMeteoro()
        {
            if (_pool.Count > 0)
            {
                GameObject meteoro = _pool.Dequeue();
                meteoro.SetActive(true);
                return meteoro;
            }
            else
            {
                // Opcional: Instancia um meteoro adicional se o pool estiver vazio
                GameObject meteoro = Instantiate(MeteoroPrefab);
                meteoro.SetActive(true);
                return meteoro;
            }
        }

        /// <summary>
        /// Retorna um meteoro ao pool, desativando-o.
        /// </summary>
        /// <param name="meteoro">O GameObject meteoro a ser retornado</param>
        public void ReturnMeteoro(GameObject meteoro)
        {
            meteoro.SetActive(false);
            _pool.Enqueue(meteoro);
        }
    }
}
