using UnityEngine;

public class MovimentoAtaque : MonoBehaviour
{
    public float velocidade = 10f;

    void Update()
    {
        // Move para baixo continuamente
        transform.Translate(Vector3.down * velocidade * Time.deltaTime);
    }
}
