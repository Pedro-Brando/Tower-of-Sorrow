using UnityEngine;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;

public class HodCopy : MonoBehaviour
{
    private bool _isTrueHod = false;
    private HodController _hodController;

    [Header("Feixes Configuração")]
    public GameObject BeamPrefab;
    public int NumberOfBeams = 5;
    public float BeamSpreadAngle = 45f;

    public void Initialize(bool isTrueHod, HodController hodController = null)
    {
        _isTrueHod = isTrueHod;
        _hodController = hodController;
    }

    public void FireBeams()
    {
        // Implementar disparo de feixes pela cópia
        float angleStep = BeamSpreadAngle / (NumberOfBeams - 1);
        float startingAngle = -BeamSpreadAngle / 2;

        for (int i = 0; i < NumberOfBeams; i++)
        {
            float currentAngle = startingAngle + (angleStep * i);
            Quaternion rotation = Quaternion.Euler(0, 0, currentAngle);
            GameObject beam = Instantiate(BeamPrefab, transform.position, rotation);

            // Marcar o feixe como falso, se necessário
            Beam beamScript = beam.GetComponent<Beam>();
            if (beamScript != null)
            {
                beamScript.SetAsFalseBeam();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerAttack"))
        {
            if (_isTrueHod)
            {
                // Jogador acertou o verdadeiro Hod
                if (_hodController != null)
                {
                    // Aplicar dano ao Hod
                    Health hodHealth = _hodController.GetComponent<Health>();
                    if (hodHealth != null)
                    {
                        hodHealth.Damage(10, gameObject, 0f, 0f, Vector3.zero);
                    }
                    else
                    {
                        Debug.LogError("Health component not found on HodController!");
                    }

                    // Destruir cópias e finalizar habilidade
                    _hodController.EndDispersar();
                }
            }
            else
            {
                // Jogador acertou uma cópia
                // Pode causar dano ao jogador ou outro efeito
                // Exemplo: destruir a cópia e aplicar um feedback visual
                Destroy(gameObject);
                // Opcional: Aplicar um efeito visual ou sonoro aqui
            }
        }
    }
}
