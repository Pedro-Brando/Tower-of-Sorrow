using UnityEngine;
using System.Collections;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using System.Collections.Generic;

public class HodController : MonoBehaviour
{
    [Header("Hod Settings")]
    public GameObject Model; // Modelo 3D ou sprite de Hod
    public GameObject BarrierVisual; // Se houver algum visual de barreira
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

    private bool _isInvulnerable = false;

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
                Debug.LogError("Animator not found on Hod's model!");
            }
        }
        else
        {
            Debug.LogError("Model not assigned in the inspector!");
        }

        // Configurar movimentação
        if (_character != null)
        {
            _horizontalMovement = _character.FindAbility<CharacterHorizontalMovement>();
            if (_horizontalMovement == null)
            {
                Debug.LogError("CharacterHorizontalMovement ability not found on Hod!");
            }
        }
        else
        {
            Debug.LogError("Character component not found on Hod!");
        }

        _player = GameObject.FindGameObjectWithTag("Player");
        if (_player == null)
        {
            Debug.LogError("Player not found in the scene!");
        }

        if (_health != null)
        {
            _health.OnDeath += OnDeath;
        }
        else
        {
            Debug.LogError("Health component not found on Hod!");
        }

        DispersarAbility = GetComponent<Dispersar>();
        if (DispersarAbility == null)
        {
            Debug.LogError("Dispersar ability not found on HodController!");
        }

        // Iniciar na Fase 1
        _phaseManager.SetPhase(1);
    }

    void Update()
    {
        UpdateAnimatorState();
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
                // Implementar lógica de movimento da pré-fase ou Fase 1
                MoveForward();
                break;

            case 2:
                // Implementar lógica de movimento da Fase 2
                MoveForward();
                break;

            case 3:
                // Implementar lógica de movimento da Fase 3
                // Exemplo: enfrentar o jogador diretamente
                FacePlayer();
                break;
        }
    }

    public void SetVisible(bool visible)
    {
        if (Model != null)
        {
            Model.SetActive(visible);
        }
    }

    public void EndDispersar()
    {
        if (DispersarAbility != null)
        {
            DispersarAbility.EndDispersar();
        }
        else
        {
            Debug.LogError("DispersarAbility is not set in HodController!");
        }
    }

    public void StopMovement()
    {
        if (_horizontalMovement != null)
        {
            _horizontalMovement.MovementSpeed = 0;
        }
    }

    public void ResumeMovement()
    {
        if (_horizontalMovement != null)
        {
            _horizontalMovement.MovementSpeed = MoveSpeed;
        }
    }

    public void LookAtPlayer()
    {
        if (_player != null)
        {
            Vector3 direction = (_player.transform.position - transform.position).normalized;
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

    private void MoveForward()
    {
        if (_horizontalMovement != null)
        {
            _horizontalMovement.SetHorizontalMove(1f); // Move para a direita
            _character.MovementState.ChangeState(CharacterStates.MovementStates.Walking);
        }
    }

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

    private void OnDeath()
    {
        // Lógica quando Hod é derrotado
        Debug.Log("Hod defeated!");
        _phaseManager.SetPhase(4);

        // Implementar animação de morte ou eventos
    }
}
