using UnityEngine;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using System.Collections;

public class HodCopy : MonoBehaviour
{
    [Header("Configurações do HodCopy")]
    [Tooltip("Determina se esta cópia é a verdadeira Hod.")]
    [SerializeField] private bool isTrueHod = false;

    [Tooltip("Referência ao controlador de Hod original.")]
    [SerializeField] private HodController originalHodController;

    [Header("Configurações de Ataque")]
    [Tooltip("Prefab do feixe de energia.")]
    [SerializeField] private GameObject beamPrefab;

    [Tooltip("Número de feixes a serem disparados.")]
    [SerializeField] private int numberOfBeams = 1;

    [Tooltip("Ângulo de dispersão dos feixes.")]
    [SerializeField] private float beamSpreadAngle = 0f;

    [Tooltip("Tempo entre disparos de feixes.")]
    [SerializeField] private float beamFireInterval = 0.5f;

    [Tooltip("Referência ao componente Health.")]
    [SerializeField] private Health _health;

    [Tooltip("Referência ao Corgi Controller.")]
    [SerializeField] private CorgiController _controller;

    private bool canFire = true;

    /// <summary>
    /// Inicializa a cópia de Hod.
    /// </summary>
    /// <param name="isTrueHod">Se a cópia é a verdadeira Hod.</param>
    /// <param name="hodController">Referência ao controlador original de Hod.</param>
    public void Initialize(bool isTrueHod, HodController hodController = null)
    {
        this.isTrueHod = isTrueHod;
        originalHodController = hodController;

        if (isTrueHod)
        {
            // Configurações específicas para a verdadeira Hod
            // Por exemplo, reativar habilidades, animações, etc.
            // Exemplo: Ativar o Animator, se necessário
            Animator animator = GetComponent<Animator>();
            if (animator != null)
            {
                animator.enabled = true;
            }
        }
        else
        {
            // Configurações para cópias falsas
            // Alterar aparência, desativar certas habilidades, etc.
            SetAsFalseCopy();
        }
    }

    
    /// <summary>
    /// Configura a cópia como falsa, alterando sua aparência ou comportamento.
    /// </summary>
    private void SetAsFalseCopy()
    {
        // Alterar a cor ou outros aspectos visuais para diferenciar a cópia
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = Color.blue; // Exemplo: mudar a cor para azul
        }

        // Desativar componentes ou habilidades que não devem estar ativos nas cópias
        // Por exemplo, desativar scripts de movimentação
        CharacterHorizontalMovement movement = GetComponent<CharacterHorizontalMovement>();
        if (movement != null)
        {
            movement.enabled = false;
        }

        // Desativar o Animator, se presente
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.enabled = false;
        }
    }

    public void BecomeInvulnerable()
    {
        _health.DamageDisabled();
    }

    public void BecomeVulnerable()
    {
        _health.DamageDisabled();
    }



    public void Canalizar()
    {
        _controller.enabled = false;
    }

    public void PararCanalizar()
    {
        _controller.enabled = true;
    }

    /// <summary>
    /// Método para disparar feixes de energia.
    /// </summary>
    public void FireBeams()
    {
        if (!canFire) return;

        StartCoroutine(FireBeamsRoutine());
    }

    /// <summary>
    /// Coroutine para gerenciar o disparo de feixes com intervalo.
    /// </summary>
    private IEnumerator FireBeamsRoutine()
    {
        canFire = false;

        float angleStep = beamSpreadAngle / (numberOfBeams - 1);
        float startingAngle = -beamSpreadAngle / 2;

        for (int i = 0; i < numberOfBeams; i++)
        {
            float currentAngle = startingAngle + (angleStep * i);
            Quaternion rotation = Quaternion.Euler(0, 0, currentAngle);
            GameObject beam = Instantiate(beamPrefab, transform.position, rotation);

            Beam beamScript = beam.GetComponent<Beam>();
            if (beamScript != null)
            {
                Vector3 direction = rotation * Vector3.right; // Assumindo que o feixe inicialmente aponta para a direita
                beamScript.SetDirection(direction);

                if (!isTrueHod)
                {
                    beamScript.SetAsFalseBeam(); // Feixes das cópias não causam dano
                }
                else
                {
                    beamScript.SetDamage(originalHodController.Damage); // Definir o dano correto para a verdadeira Hod
                }
            }

            yield return new WaitForSeconds(beamFireInterval);
        }

        canFire = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerAttack"))
        {
            if (isTrueHod)
            {
                // Jogador atacou a verdadeira Hod
                originalHodController?.OnHodAttacked();
            }
            else
            {
                // Jogador atacou uma cópia falsa
                Destroy(gameObject);
                // Aplicar feedback visual ou sonoro se necessário
            }
        }
    }

    // Outros métodos e interações conforme necessário
}
