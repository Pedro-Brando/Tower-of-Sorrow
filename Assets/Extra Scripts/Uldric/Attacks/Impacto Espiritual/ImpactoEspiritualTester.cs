using UnityEngine;
using MoreMountains.CorgiEngine;

namespace MoreMountains.CorgiEngine
{
    public class ImpactoEspiritualTester : MonoBehaviour
    {
        [Tooltip("Referência para o componente ImpactoEspiritual do Uldric")]
        public ImpactoEspiritual impactoEspiritualAbility;

        [Tooltip("Tecla para ativar a habilidade Fúria do Fogo Fátuo")]
        public KeyCode activationKey = KeyCode.F10;

        /// <summary>
        /// Verifica o input e ativa a habilidade
        /// </summary>
        void Update()
        {
            if (Input.GetKeyDown(activationKey))
            {
                if (impactoEspiritualAbility != null)
                {
                    Debug.Log("Ativando a habilidade Fúria do Fogo Fátuo!");
                    impactoEspiritualAbility.ActivateAbility();
                }
                else
                {
                    Debug.LogWarning("A habilidade FúriaDoFogoFatuo não está atribuída ao ImpactoEspiritualTester!");
                }
            }
        }
    }
}
