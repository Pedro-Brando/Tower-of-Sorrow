using UnityEngine;
using System.Collections;
using MoreMountains.CorgiEngine;

public class OndaDeChoque : MonoBehaviour
{
    public float VelocidadeExpansao = 5f;
    public int Dano = 1;
    public LayerMask PlayerLayer;

    private float _alturaAlvo;
    private BoxCollider2D _ondaCollider;

    public void Initialize(BoxCollider2D ondaArea, float alturaAlvo)
    {
        _ondaCollider = GetComponent<BoxCollider2D>();
        _ondaCollider.size = new Vector2(ondaArea.bounds.size.x, 0);
        _ondaCollider.offset = new Vector2(0, _ondaCollider.size.y / 2);

        _alturaAlvo = alturaAlvo;
        StartCoroutine(ExpandirOnda());
    }

    private IEnumerator ExpandirOnda()
    {
        float alturaAtual = 0f;

        while (alturaAtual < _alturaAlvo)
        {
            float incremento = VelocidadeExpansao * Time.deltaTime;
            alturaAtual += incremento;

            _ondaCollider.size = new Vector2(_ondaCollider.size.x, alturaAtual);
            _ondaCollider.offset = new Vector2(0, _ondaCollider.size.y / 2);

            yield return null;
        }

        // Aguarda um curto período antes de destruir a onda
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Aplica dano ao jogador
        if (collision.gameObject.CompareTag("Player"))
        {
            Health playerHealth = collision.gameObject.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.Damage(Dano, gameObject, 0f, 0f, Vector2.zero);
            }
        }
    }
}
