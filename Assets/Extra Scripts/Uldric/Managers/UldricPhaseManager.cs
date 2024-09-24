using UnityEngine;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;

public class UldricPhaseManager : MonoBehaviour
{
    // Referências aos componentes principais
    protected Health _health;
    protected UldricController _uldricController;

    // Enum para representar as fases
    public enum UldricPhase { Phase1, Interphase, Phase2, Phase3 }
    public UldricPhase CurrentPhase = UldricPhase.Phase1;

    // Flags para garantir que as transições ocorram apenas uma vez
    protected bool Phase2Triggered = false;
    protected bool Phase3Triggered = false;

    void Start()
    {
        _health = GetComponent<Health>();
        _uldricController = GetComponent<UldricController>();

        // Iniciar a primeira fase
        StartPhase(UldricPhase.Phase1);
    }

    // Método chamado quando Uldric recebe dano
    public void CheckPhaseTransition(float currentHealth)
    {
        // Calcula a porcentagem de vida restante
        float healthPercentage = (currentHealth / _health.MaximumHealth) * 100f;

        if (CurrentPhase == UldricPhase.Phase1)
        {
            // Exemplo: Transição para a Fase 2 quando a vida estiver abaixo de 70%
            if (!Phase2Triggered && healthPercentage <= 70f)
            {
                Phase2Triggered = true;
                TransitionToPhase(UldricPhase.Phase2);
            }
        }
        else if (CurrentPhase == UldricPhase.Phase2)
        {
            // Exemplo: Transição para a Fase 3 quando a vida estiver abaixo de 30%
            if (!Phase3Triggered && healthPercentage <= 30f)
            {
                Phase3Triggered = true;
                TransitionToPhase(UldricPhase.Phase3);
            }
        }
    }

    // Método chamado quando o potencializador é destruído no plano espiritual
    public void OnPotentializerDestroyed()
    {
        if (CurrentPhase == UldricPhase.Phase1)
        {
            TransitionToPhase(UldricPhase.Interphase);
        }
    }

    // Método chamado após a Interfase terminar
    public void OnInterphaseEnded()
    {
        if (CurrentPhase == UldricPhase.Interphase)
        {
            // Retorna para a Fase 1 ou avança para a Fase 2, dependendo do design
            StartPhase(UldricPhase.Phase1);
        }
    }

    // Método chamado quando os espíritos são destruídos na Fase 3
    public void OnSpiritsDestroyed()
    {
        if (CurrentPhase == UldricPhase.Phase3)
        {
            // Uldric fica vulnerável para o golpe final
            _uldricController.SetState(UldricController.UldricState.Vulnerable);
        }
    }

    // Inicia uma fase específica
    public void StartPhase(UldricPhase phase)
    {
        CurrentPhase = phase;
        _uldricController.OnPhaseStart((int)phase);

        // Configurações específicas de cada fase
        switch (phase)
        {
            case UldricPhase.Phase1:
                // Configurações da Fase 1
                break;
            case UldricPhase.Interphase:
                // Configurações da Interfase
                break;
            case UldricPhase.Phase2:
                // Configurações da Fase 2
                break;
            case UldricPhase.Phase3:
                // Configurações da Fase 3
                break;
            default:
                break;
        }
    }

    // Realiza a transição para a próxima fase
    protected void TransitionToPhase(UldricPhase nextPhase)
    {
        // Chama o método de transição no UldricController
        _uldricController.OnPhaseTransition();

        // Pode adicionar efeitos ou animações de transição aqui

        // Inicia a próxima fase após qualquer atraso necessário
        StartPhase(nextPhase);
    }
}
