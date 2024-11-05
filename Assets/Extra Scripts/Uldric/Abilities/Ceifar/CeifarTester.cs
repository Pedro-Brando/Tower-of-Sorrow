using UnityEngine;
using MoreMountains.CorgiEngine;

public class CeifarTester : MonoBehaviour
{
    [Tooltip("Referência ao componente Ceifar no Boss")]
    public Ceifar ceifarAbility;

    [Tooltip("Tecla para ativar o ataque Ceifar")]
    public KeyCode attackKey = KeyCode.C;

    void Update()
    {
        if (Input.GetKeyDown(attackKey))
        {
            if (ceifarAbility != null)
            {
                ceifarAbility.ActivateAbility();
            }
            else
            {
                Debug.LogError("CeifarAbility não está atribuído no CeifarTester!");
            }
        }
    }
}
