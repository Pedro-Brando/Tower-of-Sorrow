using UnityEngine;
using System.Collections;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using System.Collections.Generic;

public class HodController : MonoBehaviour
{
    [Header("Hod Settings")]
    public GameObject Model; // Modelo 3D ou sprite de Hod
    public float MoveSpeed = 5f;
    public float FlySpeed = 5f;
    public Transform[] MovePoints; // Pontos de movimentação para o Hod

    private HodAbilityManager _attackManager;
    private HodPhaseManager _phaseManager;
    private Character _character;
    private CharacterHorizontalMovement _horizontalMovement;
    private Health _health;
    private Animator _animator;
    private GameObject _player;
    private CorgiController _controller;
    public int Damage { get; set; } = 1; // Defina conforme necessário
    private Dispersar dispersarAbility;
    private bool _isInvulnerable = false;

    // Referência direta ao Player
    public GameObject Player { get; private set; }

    // Evento para quando Hod é atacado
    public event System.Action OnAttacked;

    void Start()
    {
        // Inicializações básicas
        _attackManager = GetComponent<HodAbilityManager>();
        _phaseManager = GetComponent<HodPhaseManager>();
        _character = GetComponent<Character>();
        _health = GetComponent<Health>();
        _controller = GetComponent<CorgiController>();

        // Configurar o Animator
        if (Model != null)
        {
            _animator = Model.GetComponent<Animator>();
            if (_animator == null)
            {
                Debug.LogError("Animator não encontrado no modelo de Hod!");
            }
        }
        else
        {
            Debug.LogError("Model não atribuído no inspetor!");
        }

        // Configurar movimentação
        if (_character != null)
        {
            _horizontalMovement = _character.FindAbility<CharacterHorizontalMovement>();
            if (_horizontalMovement == null)
            {
                Debug.LogError("CharacterHorizontalMovement ability não encontrada no Hod!");
            }
        }
        else
        {
            Debug.LogError("Componente Character não encontrado no Hod!");
        }

        _player = GameObject.FindGameObjectWithTag("Player");
        if (_player == null)
        {
            Debug.LogError("Player não encontrado na cena!");
        }
        else
        {
            Player = _player;
        }

        if (_health != null)
        {
            _health.OnDeath += OnDeath;
        }
        else
        {
            Debug.LogError("Componente Health não encontrado no Hod!");
        }

        // Atribuir dispersarAbility
        dispersarAbility = GetComponent<Dispersar>();
        if (dispersarAbility == null)
        {
            Debug.LogError("Habilidade Dispersar não encontrada no HodController!");
        }

        // Iniciar na Fase 1
        _phaseManager.SetPhase(1);
    }

    void Update()
    {
        // UpdateAnimatorState();
        HandleMovement();
    }

    private void UpdateAnimatorState()
    {
        if (_animator == null) return;

        // Atualizar os parâmetros do Animator com base no estado atual do Hod
        _animator.SetBool("Idle", _character.MovementState.CurrentState == CharacterStates.MovementStates.Idle);
        _animator.SetBool("Walking", _character.MovementState.CurrentState == CharacterStates.MovementStates.Walking);
        _animator.SetBool("isFlying", _character.MovementState.CurrentState == CharacterStates.MovementStates.Flying);
        _animator.SetBool("Alive", _character.ConditionState.CurrentState == CharacterStates.CharacterConditions.Normal);
        _animator.SetBool("isDead", _character.ConditionState.CurrentState == CharacterStates.CharacterConditions.Dead);
    }

    private void HandleMovement()
    {
        switch (_phaseManager.CurrentPhase)
        {
            case 1:
                // Lógica de movimento da Fase 1
                
                break;

            case 2:
                // Lógica de movimento da Fase 2
                MoveForward();
                break;

            case 3:
                // Lógica de movimento da Fase 3
                // Exemplo: enfrentar o jogador diretamente
                FacePlayer();
                break;
        }
    }

    /// <summary>
    /// Define a visibilidade do modelo do Hod
    /// </summary>
    public void SetVisible(bool visible)
    {
        if (Model != null)
        {
            Model.SetActive(visible);
        }
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
    /// Método chamado para finalizar a habilidade Dispersar
    /// </summary>
    public void EndDispersar()
    {
        if (dispersarAbility != null)
        {
            dispersarAbility.EndDispersar();
        }
        else
        {
            Debug.LogError("dispersarAbility não está configurada no HodController!");
        }
    }

    /// <summary>
    /// Para o movimento do Hod
    /// </summary>
    public void StopMovement()
    {
        if (_horizontalMovement != null)
        {
            _horizontalMovement.MovementSpeed = 0;
            _horizontalMovement.SetHorizontalMove(0f);
        }
    }

    /// <summary>
    /// Retoma o movimento do Hod
    /// </summary>
    public void ResumeMovement()
    {
        if (_horizontalMovement != null)
        {
            _horizontalMovement.MovementSpeed = MoveSpeed;
            _horizontalMovement.SetHorizontalMove(1f); // Retomar movimento para a direita
        }
    }

    /// <summary>
    /// Faz o Hod olhar na direção do jogador
    /// </summary>
    public void LookAtPlayer()
    {
        if (Player != null)
        {
            Vector3 direction = (Player.transform.position - transform.position).normalized;
            if (direction.x > 0)
            {
                _character.Face(Character.FacingDirections.Right);
            }
            else
            {
                _character.Face(Character.FacingDirections.Left);
            }
        }
    }

    /// <summary>
    /// Move o Hod para a frente
    /// </summary>
    private void MoveForward()
    {
        if (_horizontalMovement != null)
        {
            _horizontalMovement.SetHorizontalMove(1f); // Move para a direita
            _character.MovementState.ChangeState(CharacterStates.MovementStates.Walking);
        }
    }

    /// <summary>
    /// Faz o Hod parar e olhar para o jogador
    /// </summary>
    private void FacePlayer()
    {
        // Parar movimento horizontal
        if (_horizontalMovement != null)
        {
            _horizontalMovement.SetHorizontalMove(0f);
        }

        // Virar em direção ao jogador
        LookAtPlayer();
    }

    /// <summary>
    /// Método chamado quando o Hod é derrotado
    /// </summary>
    private void OnDeath()
    {
        // Lógica quando Hod é derrotado
        Debug.Log("Hod derrotado!");
        _phaseManager.SetPhase(4);

        // Implementar animação de morte ou eventos
    }

    /// <summary>
    /// Método para sinalizar que Hod foi atacado
    /// </summary>
    public void OnHodAttacked()
    {
        // Lógica quando Hod é atacado
        OnAttacked?.Invoke();
    }

    // Outros métodos e interações conforme necessário
}
