using UnityEngine;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;

public class UldricController : Character
{
    // Referências aos componentes principais
    protected CharacterBehavior _characterBehavior;
    protected Animator _animator;
    protected UldricPhaseManager _phaseManager;
    protected UldricAttackManager _attackManager;

    // Estados de Uldric
    public enum UldricState { Idle, Attacking, Vulnerable, Transitioning }
    public UldricState CurrentState = UldricState.Idle;

    protected override void Awake()
    {
        base.Awake();
        _characterBehavior = GetComponent<CharacterBehavior>();
        _animator = GetComponent<Animator>();
        _phaseManager = GetComponent<UldricPhaseManager>();
        _attackManager = GetComponent<UldricAttackManager>();
    }

    protected override void Start()
    {
        base.Start();
        // Iniciar a primeira fase
        _phaseManager?.StartPhase(1);
    }

    protected override void Update()
    {
        base.Update();
        // Gerenciar comportamentos baseados no estado atual
        switch (CurrentState)
        {
            case UldricState.Idle:
                // Comportamento de espera
                break;
            case UldricState.Attacking:
                // Gerenciar ataques através do AttackManager
                _attackManager?.HandleAttacks();
                break;
            case UldricState.Vulnerable:
                // Comportamento quando vulnerável
                break;
            case UldricState.Transitioning:
                // Transição entre fases
                break;
        }
    }

    // Método chamado quando Uldric recebe dano
    public override void Damage(DamageType damageType, float damage, GameObject instigator)
    {
        base.Damage(damageType, damage, instigator);

        // Gatilho da animação de dano
        _animator?.SetTrigger("Hit");

        // Verificar se é hora de mudar de fase
        _phaseManager?.CheckPhaseTransition(_health.CurrentHealth);
    }

    // Método chamado quando Uldric morre
    public override void Kill()
    {
        base.Kill();

        // Gatilho da animação de morte
        _animator?.SetTrigger("Death");

        // Desabilitar componentes após a morte
        _characterBehavior.enabled = false;
        _attackManager.enabled = false;

        // Iniciar rotina de morte
        StartCoroutine(DeathRoutine());
    }

    // Rotina para ações após a morte
    protected virtual IEnumerator DeathRoutine()
    {
        // Esperar a animação de morte terminar
        yield return new WaitForSeconds(2f);
        // Notificar o sistema que Uldric foi derrotado
        LevelManager.Instance.DefeatBoss("Uldric");
        // Destruir o objeto
        Destroy(gameObject);
    }

    // Método para alterar estados
    public void SetState(UldricState newState)
    {
        CurrentState = newState;
        // Atualizar animações conforme o estado
        _animator?.SetInteger("State", (int)newState);
    }

    // Chamado pelo PhaseManager para iniciar uma nova fase
    public void OnPhaseStart(int phaseNumber)
    {
        switch (phaseNumber)
        {
            case 1:
                SetState(UldricState.Attacking);
                break;
            case 2:
                SetState(UldricState.Vulnerable);
                break;
            case 3:
                SetState(UldricState.Attacking);
                break;
        }
    }

    // Chamado pelo PhaseManager para transições
    public void OnPhaseTransition()
    {
        SetState(UldricState.Transitioning);
        // Adicionar efeitos ou animações de transição se necessário
    }
}
