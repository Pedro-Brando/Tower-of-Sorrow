using UnityEngine;
using System.Collections;
using MoreMountains.CorgiEngine;
using MoreMountains.Feedbacks;

public class CristalController : MonoBehaviour
{
    [Header("Configurações do Cristal")]

    [Tooltip("Tempo que o cristal fica flutuando antes de cair")]
    public float FloatTime = 2f;

    [Tooltip("Prefab do portal a ser instanciado quando o cristal for destruído")]
    public GameObject PortalPrefab;

    [Header("Feedbacks MMF Player")]
    [Tooltip("Feedback ao iniciar a flutuação do cristal")]
    public MMF_Player FloatStartFeedback;

    [Tooltip("Feedback ao iniciar a queda do cristal")]
    public MMF_Player FallStartFeedback;

    [Tooltip("Feedback ao aterrissar no chão")]
    public MMF_Player LandingFeedback;

    [Tooltip("Feedback ao cristal ser destruído")]
    public MMF_Player DeathFeedback;

    private Rigidbody2D _rigidbody2D;
    private Health _healthComponent;
    private bool _hasLanded = false;
    public float Damage = 1f;

    void Start()
    {
        Debug.Log("CristalController: Start() chamado.");

        // Obtém referências aos componentes
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _healthComponent = GetComponent<Health>();

        if (_rigidbody2D == null)
        {
            Debug.LogError("CristalController: Rigidbody2D não encontrado no Cristal!");
            return;
        }

        if (_healthComponent == null)
        {
            Debug.LogError("CristalController: Health não encontrado no Cristal!");
            return;
        }

        // Desativa a gravidade inicialmente
        _rigidbody2D.gravityScale = 0f;
        Debug.Log("CristalController: Gravidade desativada.");

        // Inicia a rotina do cristal
        StartCoroutine(CristalRoutine());
        Debug.Log("CristalController: Coroutine CristalRoutine() iniciada.");

        // Assina o evento de morte
        _healthComponent.OnDeath += OnDeath;
        Debug.Log("CristalController: Evento OnDeath assinado.");
    }

    private IEnumerator CristalRoutine()
    {
        Debug.Log("CristalController: Coroutine CristalRoutine() iniciada com sucesso.");

        // Feedback ao iniciar a flutuação do cristal
        if (FloatStartFeedback != null)
        {
            FloatStartFeedback.PlayFeedbacks();
        }

        // Espera o tempo de flutuação
        Debug.Log($"CristalController: Aguardando {FloatTime} segundos antes de ativar a gravidade.");
        yield return new WaitForSeconds(FloatTime);

        // Ativa a gravidade para iniciar a queda
        _rigidbody2D.gravityScale = 1f;
        Debug.Log("CristalController: Gravidade ativada, cristal começará a cair.");

        // Feedback ao iniciar a queda
        if (FallStartFeedback != null)
        {
            FallStartFeedback.PlayFeedbacks();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"CristalController: OnCollisionEnter2D chamado. Cristal colidiu com {collision.gameObject.name} com a tag {collision.gameObject.tag} e layer {LayerMask.LayerToName(collision.gameObject.layer)}");
        if (collision.CompareTag("Player"))
        {
            Health playerHealth = collision.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.Damage(Damage, gameObject, 0.1f, 0.1f, transform.position);
            }
        }
        else if (collision.CompareTag("Ground") || collision.CompareTag("Wall"))
        {
            Debug.Log("CristalController: Cristal aterrissou no chão.");
            OnLanding();
        }
    }

    private void OnLanding()
    {
        Debug.Log("CristalController: OnLanding() chamado.");

        _hasLanded = true;

        // Desativa a gravidade
        _rigidbody2D.gravityScale = 0f;
        _rigidbody2D.velocity = Vector2.zero;
        _rigidbody2D.isKinematic = true;
        Debug.Log("CristalController: Gravidade desativada após aterrissagem.");

        // Feedback ao aterrissar no chão
        if (LandingFeedback != null)
        {
            LandingFeedback.PlayFeedbacks();
        }

        // Opcional: você pode adicionar lógica aqui se quiser que o cristal faça algo ao aterrissar
    }

    private void OnDeath()
    {
        Debug.Log("CristalController: OnDeath() chamado. Cristal será destruído.");

        // Feedback ao cristal ser destruído
        if (DeathFeedback != null)
        {
            DeathFeedback.PlayFeedbacks();
        }

        // Instancia o portal
        if (PortalPrefab != null)
        {
            PortalPrefab.SetActive(true);
            Debug.Log("CristalController: Portal instanciado na posição do cristal.");
        }
        else
        {
            Debug.LogWarning("CristalController: PortalPrefab não atribuído.");
        }

        // Remove a assinatura do evento para evitar erros
        _healthComponent.OnDeath -= OnDeath;
        Debug.Log("CristalController: Evento OnDeath desassociado.");
    }

    private void OnDestroy()
    {
        Debug.Log("CristalController: OnDestroy() chamado.");

        // Remove a assinatura do evento
        if (_healthComponent != null)
        {
            _healthComponent.OnDeath -= OnDeath;
            Debug.Log("CristalController: Evento OnDeath desassociado no OnDestroy().");
        }
    }
}
