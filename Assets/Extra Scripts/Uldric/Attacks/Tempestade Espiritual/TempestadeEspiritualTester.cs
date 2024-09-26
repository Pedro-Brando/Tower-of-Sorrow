 using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    public class TempestadeEspiritualTester : MonoBehaviour
    {
        [Tooltip("Referência para a habilidade TempestadeEspiritual")]
        public TempestadeEspiritual TempestadeEspiritualAbility;

        [Tooltip("Tecla para ativar a habilidade")]
        public KeyCode ActivationKey = KeyCode.T;

        void Update()
        {
            if (Input.GetKeyDown(ActivationKey))
            {
                if (TempestadeEspiritualAbility != null)
                {
                    TempestadeEspiritualAbility.ActivateAbility();
                    Debug.Log("Tempestade Espiritual Ativada!");
                }
                else
                {
                    Debug.LogError("TempestadeEspiritualAbility não está atribuída no Inspector.");
                }
            }
        }
    }
}
