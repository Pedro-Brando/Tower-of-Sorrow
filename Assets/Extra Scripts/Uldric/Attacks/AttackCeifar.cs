using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Attacks/Ceifar")]
public class AttackCeifar : AttackPattern
{
    public GameObject SweepEffectPrefab;    // Prefab do efeito visual de varredura
    public float SweepDuration = 1.5f;      // Duração total da varredura
    public float SweepCooldown = 3f;        // Tempo de espera antes de poder realizar o próximo ataque
    public float SweepAngle = 90f;           // Ângulo total da varredura
    public float SweepRange = 5f;            // Alcance da varredura
    public float KnockbackForce = 5f;        // Força de repulsão aplicada ao jogador
    public int Damage = 20;                  // Dano causado pela varredura

    private Transform _uldricTransform;      // Referência ao transform de Uldric
    private Transform _playerTransform;       // Referência ao transform do jogador

    public override IEnumerator Execute()
    {
        // Obter referências ao Uldric e ao jogador
        if (_uldricTransform == null)
        {
            _uldricTransform = this.transform.parent; // Assumindo que o ScriptableObject está dentro de um GameObject filho de Uldric
        }

        if (_playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                _playerTransform = player.transform;
            }
            else
            {
                Debug.LogWarning("Player não encontrado! Certifique-se de que o jogador tem a tag 'Player'.");
                yield break;
            }
        }

        // Iniciar o efeito de varredura
        GameObject sweepEffect = Instantiate(SweepEffectPrefab, _uldricTransform.position, Quaternion.identity, _uldricTransform);
        Animator sweepAnimator = sweepEffect.GetComponent<Animator>();

        if (sweepAnimator != null)
        {
            sweepAnimator.SetTrigger("StartSweep");
        }

        // Calcular a direção da varredura com base na posição do jogador
        Vector3 direction = (_playerTransform.position - _uldricTransform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Girar Uldric para a direção do jogador
        _uldricTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        // Aguarda a duração da varredura
        yield return new WaitForSeconds(SweepDuration);

        // Detectar colisões na área da varredura
        Collider2D[] hits = Physics2D.OverlapCircleAll(_uldricTransform.position, SweepRange, LayerMask.GetMask("Player"));

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                // Aplicar dano ao jogador
                PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(Damage);
                }

                // Aplicar força de repulsão
                Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 knockbackDirection = (hit.transform.position - _uldricTransform.position).normalized;
                    rb.AddForce(knockbackDirection * KnockbackForce, ForceMode2D.Impulse);
                }
            }
        }

        // Destruir o efeito de varredura
        Destroy(sweepEffect);

        // Aguarda o cooldown antes de permitir o próximo ataque
        yield return new WaitForSeconds(SweepCooldown);
    }
}
