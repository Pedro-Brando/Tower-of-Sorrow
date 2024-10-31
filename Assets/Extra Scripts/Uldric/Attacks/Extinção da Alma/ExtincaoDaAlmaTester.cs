using UnityEngine;
using MoreMountains.CorgiEngine;

namespace MoreMountains.CorgiEngine
{
    public class ExtincaoDaAlmaTester : MonoBehaviour
    {
        [Tooltip("Referência para o componente FuriaDoFogoFatuo do Uldric")]
        public ExtincaoDaAlma extincaoDaAlmaAbility;

        [Tooltip("Tecla para ativar a habilidade Fúria do Fogo Fátuo")]
        public KeyCode activationKey = KeyCode.F6;

        /// <summary>
        /// Verifica o input e ativa a habilidade
        /// </summary>
        void Update()
        {
            if (Input.GetKeyDown(activationKey))
            {
                if (extincaoDaAlmaAbility != null)
                {
                    Debug.Log("Ativando a habilidade Fúria do Fogo Fátuo!");
                    extincaoDaAlmaAbility.ActivateAbility();
                }
                else
                {
                    Debug.LogWarning("A habilidade tempestadeDeFogoFatuo não está atribuída ao FuriaDoFogoFatuoTester!");
                }
            }
        }
    }
}
