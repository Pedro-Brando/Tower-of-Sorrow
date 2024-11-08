using UnityEngine;

public class Beam : MonoBehaviour
{
    private bool _isFalseBeam = false;

    public void SetAsFalseBeam()
    {
        _isFalseBeam = true;
        // Alterar a aparência do feixe, por exemplo, mudar a cor
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_isFalseBeam && collision.CompareTag("Player"))
        {
            // Causar dano ao jogador
        }
        // Se for um feixe falso, não faz nada
    }
}
