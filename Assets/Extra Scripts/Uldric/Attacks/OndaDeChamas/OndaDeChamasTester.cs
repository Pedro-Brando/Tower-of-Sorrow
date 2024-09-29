using UnityEngine;
using MoreMountains.CorgiEngine;

namespace MoreMountains.CorgiEngine
{
    public class OndaDeChamasTester : MonoBehaviour
    {
        [Tooltip("Referência para o componente FuriaDoFogoFatuo do Uldric")]
        public OndaDeChamas ondaDeChamasAbility;

        [Tooltip("Tecla para ativar a habilidade Fúria do Fogo Fátuo")]
        public KeyCode activationKey = KeyCode.F;

        /// <summary>
        /// Verifica o input e ativa a habilidade
        /// </summary>
        void Update()
        {
            if (Input.GetKeyDown(activationKey))
            {
                if (ondaDeChamasAbility != null)
                {
                    Debug.Log("Ativando a habilidade Fúria do Fogo Fátuo!");
                    ondaDeChamasAbility.ActivateAbility();
                }
                else
                {
                    Debug.LogWarning("A habilidade FúriaDoFogoFatuo não está atribuída ao FuriaDoFogoFatuoTester!");
                }
            }
        }
    }
}
