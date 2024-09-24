using UnityEngine;
using System.Collections;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;

public class UldricAttackManager : MonoBehaviour
{
    // Referências aos scripts e componentes necessários
    private UldricController _uldricController;
    private UldricPhaseManager _phaseManager;

    // Lista de ataques disponíveis em cada fase
    public List<AttackPattern> Phase1Attacks;
    public List<AttackPattern> InterphaseAttacks;
    public List<AttackPattern> Phase2Attacks;
    public List<AttackPattern> Phase3Attacks;

    // Ataque atual
    private AttackPattern _currentAttack;

    // Controle de tempo entre ataques
    public float TimeBetweenAttacks = 2f;
    private float _attackTimer = 0f;

    // Indica se Uldric está executando um ataque
    private bool _isAttacking = false;

    void Start()
    {
        _uldricController = GetComponent<UldricController>();
        _phaseManager = GetComponent<UldricPhaseManager>();
    }

    void Update()
    {
        // Se Uldric não está no estado de ataque, não faz nada
        if (_uldricController.CurrentState != UldricController.UldricState.Attacking)
            return;

        // Se já está executando um ataque, não inicia outro
        if (_isAttacking)
            return;

        // Atualiza o timer entre ataques
        _attackTimer -= Time.deltaTime;
        if (_attackTimer <= 0f)
        {
            // Seleciona e inicia um novo ataque
            StartCoroutine(ExecuteAttack());
            _attackTimer = TimeBetweenAttacks;
        }
    }

    // Coroutine para executar o ataque
    private IEnumerator ExecuteAttack()
    {
        _isAttacking = true;

        // Seleciona um ataque com base na fase atual
        List<AttackPattern> availableAttacks = GetAttacksForCurrentPhase();

        // Seleciona um ataque aleatoriamente
        _currentAttack = availableAttacks[Random.Range(0, availableAttacks.Count)];

        // Executa o ataque
        yield return StartCoroutine(_currentAttack.Execute());

        _isAttacking = false;
    }

    // Retorna a lista de ataques disponíveis na fase atual
    private List<AttackPattern> GetAttacksForCurrentPhase()
    {
        switch (_phaseManager.CurrentPhase)
        {
            case UldricPhaseManager.UldricPhase.Phase1:
                return Phase1Attacks;
            case UldricPhaseManager.UldricPhase.Interphase:
                return InterphaseAttacks;
            case UldricPhaseManager.UldricPhase.Phase2:
                return Phase2Attacks;
            case UldricPhaseManager.UldricPhase.Phase3:
                return Phase3Attacks;
            default:
                return Phase1Attacks;
        }
    }
}
