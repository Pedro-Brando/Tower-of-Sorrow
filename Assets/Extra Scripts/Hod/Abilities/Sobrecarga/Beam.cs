using MoreMountains.CorgiEngine;
using UnityEngine;

public class Beam : MonoBehaviour
{
    private Vector3 direction;
    private float speed = 10f;
    private float lifetime = 5f;
    private int damage = 0;
    private bool isFalseBeam = false;

    private void Start()
    {
        // Define a destruição automática após a vida útil
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // Move o feixe na direção definida
        transform.position += direction * speed * Time.deltaTime;
    }

    /// <summary>
    /// Define a direção do feixe
    /// </summary>
    /// <param name="dir">Direção do feixe</param>
    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    /// <summary>
    /// Define o dano causado pelo feixe
    /// </summary>
    /// <param name="dmg">Quantidade de dano</param>
    public void SetDamage(int dmg)
    {
        damage = dmg;
    }

    /// <summary>
    /// Marca o feixe como falso, impedindo-o de causar dano
    /// </summary>
    public void SetAsFalseBeam()
    {
        isFalseBeam = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isFalseBeam)
        {
            Health playerHealth = collision.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.Damage(damage, gameObject, 0f, 0f, Vector3.zero);
            }
            else
            {
                Debug.LogError("Health component não encontrado no jogador!");
            }

            // Destruir o feixe após colidir
            Destroy(gameObject);
        }
    }
}
