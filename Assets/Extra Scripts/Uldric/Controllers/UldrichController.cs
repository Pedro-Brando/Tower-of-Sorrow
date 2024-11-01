using UnityEngine;
using System.Collections;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using System.Collections.Generic;

public class UldrichController : MonoBehaviour
{

    [Header("Uldrich Settings")]
    public GameObject BarrierVisual; // Representação visual da barreira
    public Potentializer potentializer; // Referência ao objeto Potentializer


    [Header("Spiritual World Settings")]
    public float MaxTimeInSpiritualWorld = 10f; // Tempo máximo que o jogador pode ficar no mundo espiritual antes de Uldrich lançar Extinção da Alma

    [Header("Espírito Container")]
    [Tooltip("GameObject que contém todos os espíritos como filhos")]
    public GameObject EspiritosContainer; // Referência ao GameObject que contém todos os espíritos


    private UldrichAttackManager _attackManager;
    private UldrichPhaseManager _phaseManager;
    private Character _character; // Referência ao script Character de Uldrich
    private CharacterHorizontalMovement _horizontalMovement; // Referência à habilidade de movimento horizontal
    private Health _health;
    private GameObject _player; // Referência ao jogador
    private BoxCollider2D _collider2D;


    private bool _isInvulnerable = true;
    public bool _playerInSpiritualWorld = false;
    private float _spiritualWorldTimer = 0f;
    private bool _potentializerDestroyed = false;
    private bool _espiritosAtivados = false;
    private bool _vidaRestaurada = false; // Flag para controle da restauração da vida
    private bool _vulneravelNaFase3 = false;
    private int _spiritsDestroyed = 0;
    public float FlySpeed = 5f; // Velocidade de voo ajustável
    private CorgiController _controller; // Referência ao CorgiController


    [Header("Model Settings")]
    public GameObject Model; // Referência ao GameObject que contém o modelo e o Animator
    private Animator _animator; // Referência ao Animator do modelo


    [Header("Portals Settings")]
    public List<GameObject> Portais; // Lista de todos os portais na cena



    void Start()
    {
        // Inicializações básicas
        _attackManager = GetComponent<UldrichAttackManager>();
        _phaseManager = GetComponent<UldrichPhaseManager>();
        _character = GetComponent<Character>();
        _health = GetComponent<Health>();


        // Pegando o Animator do GameObject Model
        if (Model != null)
        {
            _animator = Model.GetComponent<Animator>();
            if (_animator == null)
            {
                Debug.LogError("Animator não encontrado no GameObject Model!");
            }
        }
        else
        {
            Debug.LogError("Model não foi atribuído no inspetor!");
        }


        _animator = GetComponentInChildren<Animator>(); // Supondo que o Animator esteja no GameObject filho "Model".
        if (_animator == null)
        {
            Debug.LogError("Animator not found on Uldric's model!");
        }

        // Inicializa o Container de Espíritos, garantindo que eles estejam desativados no início
        if (EspiritosContainer != null)
        {
            EspiritosContainer.SetActive(false);
            _espiritosAtivados = false;
        }
        else
        {
            Debug.LogWarning("EspiritosContainer não foi atribuído no inspetor!");
        }

        if (BarrierVisual != null)
        {
            BarrierVisual.SetActive(false);
        }

        if (_character != null)
        {
            _horizontalMovement = _character.FindAbility<CharacterHorizontalMovement>();
            if (_horizontalMovement == null)
            {
                Debug.LogError("CharacterHorizontalMovement ability not found on Uldrich!");
            }
        }
        else
        {
            Debug.LogError("Character component not found on Uldrich!");
        }

        if (_health != null)
        {
            _health.OnDeath += OnDeath;
        }
        else
        {
            Debug.LogError("Health component not found on Uldrich!");
        }

        _player = GameObject.FindGameObjectWithTag("Player");
        if (_player == null)
        {
            Debug.LogError("Player not found in the scene!");
        }

        _controller = GetComponent<CorgiController>();
        if (_controller == null)
        {
            Debug.LogError("CorgiController não encontrado no Uldrich!");
        }

        if (potentializer == null)
        {
            potentializer = FindObjectOfType<Potentializer>();
            if (potentializer == null)
            {
                Debug.LogError("Potentializer não encontrado na cena!");
            }
        }

        // Iniciar na Fase 1
        _phaseManager.SetPhase(1);
        BecomeInvulnerable();
        Debug.Log("Barreira aplicada.");
    }

    void Update()
    {
        UpdateAnimatorState();
        HandleMovement();

        if (_playerInSpiritualWorld && _phaseManager.CurrentPhase < 2)
        {
            _spiritualWorldTimer += Time.deltaTime;
            if (_spiritualWorldTimer >= MaxTimeInSpiritualWorld )
            {
                CastExtincaoDaAlma();
                _spiritualWorldTimer = 0f;
            }
        }
        else
        {
            _spiritualWorldTimer = 0f;
        }
    }


    private void UpdateAnimatorState()
    {
        if (_animator == null) return;

        // Atualizar os parâmetros do Animator com base no estado atual do Uldric
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
                if (_isInvulnerable)
                {
                    FlyToPoint(new Vector2(-38, -4));
                }
                else
                {
                    MoveAwayFromPlayer();
                }
                break;

            case 2:
                if (_isInvulnerable)
                {
                    BecomeVulnerable(); // Garante que Uldrich se torne vulnerável ao entrar na fase 2
                }
                MoveAwayFromPlayer();
                break;

            case 3:
                if (!_espiritosAtivados)
                {
                    ActivateEspiritos();
                }

                if (_spiritsDestroyed >= 2 && !_vulneravelNaFase3)
                {
                    BecomeVulnerable();
                    RestoreHealth(); // Restaura a vida de Uldrich após todos os espíritos serem destruídos
                    if (_controller != null)
                    {
                        _controller.GravityActive(true);
                    }
                    _vulneravelNaFase3 = true;
                    MoveAwayFromPlayer();
                }
                else
                {
                    FlyToPoint(new Vector2(-38, -4));
                }
                
                break;
        }
    }

    private void RestoreHealth()
    {
        if (_health != null && !_vidaRestaurada)
        {
            Debug.Log("Restaurando saúde de Uldrich...");

            // Define uma quantidade de saúde a ser restaurada
            float healthToRestore = 1f; // Por exemplo, restaura 5 pontos de saúde

            // Verifica se a saúde não ultrapassa o máximo
            float newHealth = Mathf.Min(_health.CurrentHealth + healthToRestore, _health.MaximumHealth);

            // Atualiza a saúde
            _health.SetHealth(newHealth, gameObject);

            // Remove qualquer invulnerabilidade existente
            _health.Invulnerable = false;
            _health.TemporarilyInvulnerable = false;
            _health.PostDamageInvulnerable = false;
            _health.ImmuneToDamage = false;

            // Garante que o estado do Character é alterado de 'Dead' para 'Normal'
            if (_character != null)
            {
                _character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Normal);
            }

            // Reativa colisões e física
            if (_controller != null)
            {
                _controller.CollisionsOn();
                _controller.GravityActive(true);
                _controller.ResetParameters();
            }

            if (_collider2D != null)
            {
                _collider2D.enabled = true;
            }

            _vidaRestaurada = true; // Evita restaurações repetidas desnecessárias
            Debug.Log($"Vida de Uldrich restaurada: {_health.CurrentHealth} / {_health.MaximumHealth}");
        }
    }


    public void SetFlyingAnimation(bool isFlying)
    {
        if (_animator != null)
        {
            _animator.SetBool("IsFlying", isFlying);
        }
    }

    public void SetDeathAnimation(bool Alive)
    {
        if (_animator != null)
        {
            _animator.SetBool("Alive", Alive);
        }
    }


    /// <summary>
    /// Faz com que Uldrich voe até um ponto específico.
    /// </summary>
    /// <param name="targetPosition">A posição alvo onde Uldrich deve voar.</param>
    public void FlyToPoint(Vector2 targetPosition)
    {
        SetFlyingAnimation(true); // Ativa a animação de voar
        StartCoroutine(FlyToPointCoroutine(targetPosition));
    }

    private IEnumerator FlyToPointCoroutine(Vector2 targetPosition)
    {
        // Define o estado de movimento para Voando (ou um estado apropriado)
        _character.MovementState.ChangeState(CharacterStates.MovementStates.Flying);

        float distance = Vector2.Distance(transform.position, targetPosition);

        while (distance > 0.1f) // Tolerância para evitar paradas prematuras
        {
            // Recalcula a distância a cada frame
            distance = Vector2.Distance(transform.position, targetPosition);

            // Move Uldrich na direção do alvo
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
            Vector2 velocity = direction * FlySpeed;

            // Aplica a força usando o CorgiController
            _controller.SetHorizontalForce(velocity.x);
            _controller.SetVerticalForce(velocity.y);

            // Aguarda um frame antes de continuar
            yield return null;
        }

        // Define o estado de movimento como Idle
        _character.MovementState.ChangeState(CharacterStates.MovementStates.Idle);
        SetFlyingAnimation(false);

        // Para quando atingir a posição
        _controller.SetHorizontalForce(0);
        _controller.SetVerticalForce(0);
    }

    private float _currentDirection = 1f; // 1 para direita, -1 para esquerda

    private void MoveAwayFromPlayer()
    {
        if (_horizontalMovement != null)
        {
            // Comprimento do Raycast para detectar a parede
            float raycastDistance = 1.5f;

            // Direção atual do movimento (direita ou esquerda)
            Vector2 direction = _currentDirection > 0 ? Vector2.right : Vector2.left;

            // Raycast na direção atual para detectar obstáculos (como paredes)
            RaycastHit2D hitInDirection = Physics2D.Raycast(transform.position, direction, raycastDistance, LayerMask.GetMask("Platforms"));

            if (hitInDirection.collider != null && (hitInDirection.collider.CompareTag("Wall") || hitInDirection.collider.CompareTag("Ground")))
            {
                // Se bater na parede, inverte a direção
                _currentDirection = -_currentDirection;
                Debug.Log("Parede detectada! Mudando de direção.");
            }

            // Ajusta o movimento horizontal com base na direção atual
            _horizontalMovement.SetHorizontalMove(_currentDirection);

            // Configura o estado de movimento para "Walking"
            _character.MovementState.ChangeState(CharacterStates.MovementStates.Walking);
        }
    }




    public void BecomeVulnerable()
    {
        Debug.Log("Uldrich se tornou vulnerável.");
        _isInvulnerable = false;

        RemoveBarrier();
    }

    private void BecomeInvulnerable()
    {
        Debug.Log("Uldrich se tornou invulnerável.");
        _isInvulnerable = true;
        ApplyBarrier();

        // Reviver o Potentializer se ele estiver disponível
        if (potentializer != null && _phaseManager.CurrentPhase < 2)
        {
            potentializer.Revive();
        }

        // Uldrich voa novamente
        _character.MovementState.ChangeState(CharacterStates.MovementStates.Jumping);
    }



    private void SubscribeToSpiritsHealth()
    {
        if (EspiritosContainer != null)
        {
            Health[] spiritsHealth = EspiritosContainer.GetComponentsInChildren<Health>(true);
            foreach (Health spiritHealth in spiritsHealth)
            {
                spiritHealth.OnDeath -= OnSpiritDeath; // Evita múltiplas inscrições
                spiritHealth.OnDeath += OnSpiritDeath;
            }
        }
        else
        {
            Debug.LogWarning("EspiritosContainer não foi atribuído no inspetor!");
        }
    }

    // Defina as coordenadas de teletransporte
    [SerializeField] private Vector3 teleportDestination;

    public void OnSpiritDeath()
    {
        _spiritsDestroyed++;
        Debug.Log($"Espírito destruído. Total destruído: {_spiritsDestroyed}");

        if (_spiritsDestroyed >= 2 && !_vulneravelNaFase3)
        {
            // Se todos os espíritos foram destruídos, Uldrich deve tornar-se vulnerável e restaurar sua vida

            // Encontra o player na cena usando a tag "Player"
            GameObject playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null)
            {
                Transform playerTransform = playerObject.transform;
                playerTransform.position = teleportDestination;
                Debug.Log($"Player teleportado para {teleportDestination}.");
            }
            else
            {
                Debug.LogWarning("Player não encontrado na cena!");
            }

            BecomeVulnerable();
            RestoreHealth();
            _vulneravelNaFase3 = true;
        }
    }
    
    private void ActivateEspiritos()
    {
        if (EspiritosContainer != null && !EspiritosContainer.activeSelf)
        {
            EspiritosContainer.SetActive(true);
            _espiritosAtivados = true;
            Debug.Log("Espíritos ativados.");
            SubscribeToSpiritsHealth();
        }
        else
        {
            Debug.LogWarning("EspiritosContainer não está atribuído ou já está ativo.");
        }
    }



    public void OnPotentializerDestroyed()
    {
        Debug.Log("OnPotentializerDestroyed chamado no UldrichController.");
        if (_phaseManager.CurrentPhase == 1 && !_potentializerDestroyed)
        {
            _potentializerDestroyed = true;
            // Uldrich torna-se vulnerável
            BecomeVulnerable();
            CastExtincaoDaAlma();
        }
    }

    public void OnPotentializerRevived()
    {
        Debug.Log("OnPotentializerRevived chamado no UldrichController.");
        if (_phaseManager.CurrentPhase == 1 && _potentializerDestroyed)
        {
            _potentializerDestroyed = false;
            // Uldrich torna-se invulnerável novamente
            BecomeInvulnerable();
        }
    }

    private void ApplyBarrier()
    {
        if (BarrierVisual != null)
        {
            BarrierVisual.SetActive(true);
            Debug.Log("Barreira de fato ativada.");
        }
        // Torna Uldrich invulnerável
        if (_health != null)
        {
            _health.Invulnerable = true;
        }
    }

    private void RemoveBarrier()
    {
        if (BarrierVisual != null)
        {
            BarrierVisual.SetActive(false);
        }
        // Torna Uldrich vulnerável
        if (_health != null)
        {
            _health.Invulnerable = false;
        }
    }

    private void CastExtincaoDaAlma()
    {
        // Uldrich lança Extinção da Alma
        if (_attackManager != null)
        {   
            Debug.Log("Force cast extincao");
            _attackManager.ForceCastExtincaoDaAlma();
        }
    }

    public void DesativarPortais()
    {
        if (Portais == null || Portais.Count == 0)
        {
            Debug.LogWarning("Nenhum portal foi atribuído no inspetor!");
            return;
        }

        foreach (GameObject portal in Portais)
        {
            if (portal != null && portal.activeSelf)
            {
                portal.SetActive(false);
                Debug.Log("Portal desativado: " + portal.name);
            }
        }
    }

        public void AtivarPortais()
    {
        if (Portais == null || Portais.Count == 0)
        {
            Debug.LogWarning("Nenhum portal foi atribuído no inspetor!");
            return;
        }

        foreach (GameObject portal in Portais)
        {
            if (portal != null)
            {
                portal.SetActive(true);
                Debug.Log("Portal ativado: " + portal.name);
            }
        }
    }


    public void PlayerEnteredSpiritualWorld()
    {
        _playerInSpiritualWorld = true;
    }

    public void PlayerExitedSpiritualWorld()
    {
        _playerInSpiritualWorld = false;
    }



    private void OnDeath()
    {
        if (_phaseManager.CurrentPhase == 2)
        {
            // Transição para a Fase 3
            _phaseManager.SetPhase(3);
            // Restaura a vida de Uldrich para 1
            if (_health != null)
            {
                _health.SetHealth(10, gameObject);
            }
            Debug.Log("Uldrich Became Invulnerable");
            BecomeInvulnerable();

            DesativarPortais();
        }
        else if (_phaseManager.CurrentPhase == 3)
        {
            // Uldrich foi derrotado
            SetDeathAnimation(true);
            Debug.Log("Uldrich is defeated");
            // Implementar animações ou eventos de morte
        }
    }
}
