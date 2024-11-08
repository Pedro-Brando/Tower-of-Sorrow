using UnityEngine;
using MoreMountains.CorgiEngine;

public class HodPhaseManager : MonoBehaviour
{
    public int CurrentPhase { get; private set; } = 1;

    private HodController _controller;
    private HodAbilityManager _abilityManager;
    private Health _health;

    [Header("Phase Thresholds")]
    public float Phase2HealthThreshold = 70f; // Exemplo de valor
    public float Phase3HealthThreshold = 40f;

    // Variável para armazenar o tempo de início da fase
    private float _phaseStartTime;

    // Propriedade para acessar o tempo de início da fase
    public float PhaseStartTime => _phaseStartTime;

    void Awake()
    {
        _controller = GetComponent<HodController>();
        _abilityManager = GetComponent<HodAbilityManager>();
        _health = GetComponent<Health>();

        if (_controller == null)
        {
            Debug.LogError("HodController component not found on Hod!");
        }
        if (_abilityManager == null)
        {
            Debug.LogError("HodAbilityManager component not found on Hod!");
        }
        if (_health == null)
        {
            Debug.LogError("Health component not found on Hod!");
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

        if (_abilityManager != null)
        {
            _abilityManager.UpdateAvailableAbilities();
        }
        else
        {
            Debug.LogError("_abilityManager is null in HodPhaseManager.SetPhase()!");
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

            case 4:
                // Fase de derrota
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
