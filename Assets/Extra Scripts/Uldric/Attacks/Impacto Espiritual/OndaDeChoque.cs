using UnityEngine;
using System.Collections;
using MoreMountains.CorgiEngine;
using MoreMountains.Feedbacks;

public class OndaDeChoque : MonoBehaviour
{
    public float VelocidadeExpansao = 5f;
    public int Dano = 1;

    private float _larguraAlvo;
    private BoxCollider2D _ondaCollider;

    public MMF_Player ExpandFeedback;
    public MMF_Player ImpactFeedback;

    private Vector2 _offsetInicial;
    private float _larguraInicial;
    private float _alturaOnda; 

    public void Initialize(float larguraAlvo, float alturaOnda)
    {
        _ondaCollider = GetComponent<BoxCollider2D>();

        // Configura o collider com largura inicial pequena e altura definida
        _larguraInicial = 0.1f; // Largura inicial pequena
        _alturaOnda = alturaOnda; // Altura da onda definida externamente

        _ondaCollider.size = new Vector2(_larguraInicial, _alturaOnda);
        _ondaCollider.offset = new Vector2(0f, -1.5f); // Ajusta o offset vertical para posicionar a onda corretamente

        // Armazena o offset inicial
        _offsetInicial = _ondaCollider.offset;

        if (ExpandFeedback != null)
        {
            ExpandFeedback.PlayFeedbacks();
        }

        Debug.Log($"OndaDeChoque inicializada: tamanho {_ondaCollider.size}, posição {transform.position}");

        _larguraAlvo = larguraAlvo;
        StartCoroutine(ExpandirOnda());
    }

    public float AlturaOnda => _alturaOnda; // Propriedade para acessar a altura da onda
    public float OffsetVerticalParticulas = -1.5f; // Ajuste conforme necessário

    public float LarguraAlvo => _larguraAlvo;

    public float ObterLarguraAtual()
    {
        return _ondaCollider.size.x;
    }

    public float ObterPosicaoBase()
{
    return transform.position.y + _ondaCollider.offset.y - (_ondaCollider.size.y / 2f);
}


    private IEnumerator ExpandirOnda()
    {
        float larguraAtual = _larguraInicial;

        while (larguraAtual < _larguraAlvo)
        {
            float incremento = VelocidadeExpansao * Time.deltaTime * 2f; // Multiplica por 2 para expansão bilateral
            larguraAtual += incremento;

            // Atualiza o tamanho do collider
            _ondaCollider.size = new Vector2(larguraAtual, _alturaOnda);

            // O offset x permanece o mesmo, pois a expansão é simétrica
            _ondaCollider.offset = _offsetInicial;

            yield return null;
        }

        // Aguarda um curto período antes de destruir a onda
        yield return new WaitForSeconds(0.5f);

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Health playerHealth = collision.gameObject.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.Damage(Dano, gameObject, 0f, 0f, Vector2.zero);

                if (ImpactFeedback != null)
                {
                    ImpactFeedback.PlayFeedbacks();
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (_ondaCollider != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(_ondaCollider.bounds.center, _ondaCollider.bounds.size);
        }
    }
}
