using UnityEngine;
using MoreMountains.CorgiEngine;

namespace MoreMountains.CorgiEngine
{
    public class FuriaDoFogoFatuoTester : MonoBehaviour
    {
        [Tooltip("Referência para o componente FuriaDoFogoFatuo do Uldric")]
        public FuriaDoFogoFatuo furiaDoFogoFatuoAbility;

        [Tooltip("Tecla para ativar a habilidade Fúria do Fogo Fátuo")]
        public KeyCode activationKey = KeyCode.F;

        /// <summary>
        /// Verifica o input e ativa a habilidade
        /// </summary>
        void Update()
        {
            if (Input.GetKeyDown(activationKey))
            {
                if (furiaDoFogoFatuoAbility != null)
                {
                    Debug.Log("Ativando a habilidade Fúria do Fogo Fátuo!");
                    furiaDoFogoFatuoAbility.ActivateAbility();
                }
                else
                {
                    Debug.LogWarning("A habilidade FúriaDoFogoFatuo não está atribuída ao FuriaDoFogoFatuoTester!");
                }
            }
        }
    }
}
