using UnityEngine;
using MoreMountains.CorgiEngine;

namespace MoreMountains.CorgiEngine
{
    public class EspiritoCristalizadoTester : MonoBehaviour
    {
        [Tooltip("Referência para o componente FuriaDoFogoFatuo do Uldric")]
        public EspiritoCristalizado espiritoCristalizadoAbility;

        [Tooltip("Tecla para ativar a habilidade Fúria do Fogo Fátuo")]
        public KeyCode activationKey = KeyCode.F9;

        /// <summary>
        /// Verifica o input e ativa a habilidade
        /// </summary>
        void Update()
        {
            if (Input.GetKeyDown(activationKey))
            {
                if (espiritoCristalizadoAbility != null)
                {
                    Debug.Log("Ativando a habilidade Fúria do Fogo Fátuo!");
                    espiritoCristalizadoAbility.ActivateAbility();
                }
                else
                {
                    Debug.LogWarning("A habilidade tempestadeDeFogoFatuo não está atribuída ao FuriaDoFogoFatuoTester!");
                }
            }
        }
    }
}
