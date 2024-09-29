using UnityEngine;
using MoreMountains.CorgiEngine;

namespace MoreMountains.CorgiEngine
{
    public class ConsumirVidaTester : MonoBehaviour
    {
        [Tooltip("Referência para o componente FuriaDoFogoFatuo do Uldric")]
        public ConsumirVida consumirVidaAbility;

        [Tooltip("Tecla para ativar a habilidade Fúria do Fogo Fátuo")]
        public KeyCode activationKey = KeyCode.F8;

        /// <summary>
        /// Verifica o input e ativa a habilidade
        /// </summary>
        void Update()
        {
            if (Input.GetKeyDown(activationKey))
            {
                if (consumirVidaAbility != null)
                {
                    Debug.Log("Ativando a habilidade Fúria do Fogo Fátuo!");
                    consumirVidaAbility.ActivateAbility();
                }
                else
                {
                    Debug.LogWarning("A habilidade FúriaDoFogoFatuo não está atribuída ao FuriaDoFogoFatuoTester!");
                }
            }
        }
    }
}
