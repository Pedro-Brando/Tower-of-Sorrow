using UnityEngine;
using MoreMountains.CorgiEngine;

public class UldrichPhaseManager : MonoBehaviour
{
    public int CurrentPhase { get; private set; } = 1;

    private UldrichController _controller;
    private UldrichAttackManager _attackManager;
    private Health _health;

    [Header("Phase Thresholds")]
    public float Phase2HealthThreshold = 70f; // Valor de exemplo
    public float Phase3HealthThreshold = 0f;  // Transição para a Fase 3 quando a saúde chega a 0

    // Variável para armazenar o tempo de início da fase
    private float _phaseStartTime;

    // Propriedade para acessar o tempo de início da fase
    public float PhaseStartTime => _phaseStartTime;

    void Awake()
    {
        _controller = GetComponent<UldrichController>();
        _attackManager = GetComponent<UldrichAttackManager>();
        _health = GetComponent<Health>();

        if (_controller == null)
        {
            Debug.LogError("UldrichController component not found on Uldrich!");
        }
        if (_attackManager == null)
        {
            Debug.LogError("UldrichAttackManager component not found on Uldrich!");
        }
        if (_health == null)
        {
            Debug.LogError("Health component not found on Uldrich!");
        }
    }

    void Start()
    {
        _phaseStartTime = Time.time; // Define o tempo de início da Fase 1
    }

    void Update()
    {
        CheckPhaseTransition();
    }

    public void SetPhase(int phase)
    {
        CurrentPhase = phase;

        // Registra o tempo de início da nova fase
        _phaseStartTime = Time.time;

        if (_attackManager != null)
        {
            _attackManager.UpdateAvailableAbilities();
        }
        else
        {
            Debug.LogError("_attackManager is null in UldrichPhaseManager.SetPhase()!");
        }

        // Configurações adicionais para cada fase
        switch (CurrentPhase)
        {
            case 1:
                // Configurações iniciais para a Fase 1
                break;

            case 2:
                // Configurações para a Fase 2
                break;

            case 3:
                // Configurações para a Fase 3
                break;
        }
    }

    private void CheckPhaseTransition()
    {
        if (CurrentPhase == 1 && _health != null && _health.CurrentHealth <= Phase2HealthThreshold)
        {
            SetPhase(2);
        }
        else if (CurrentPhase == 2 && _health != null && _health.CurrentHealth <= Phase3HealthThreshold)
        {
            SetPhase(3);
        }
    }
}
