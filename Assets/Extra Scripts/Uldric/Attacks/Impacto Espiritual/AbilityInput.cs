using UnityEngine;
using MoreMountains.CorgiEngine;

public class AbilityInput : MonoBehaviour
{
    [Tooltip("Referência à habilidade Impacto Espiritual")]
    public ImpactoEspiritual impactoEspiritual;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            impactoEspiritual.AtivarHabilidade();
        }
    }
}
