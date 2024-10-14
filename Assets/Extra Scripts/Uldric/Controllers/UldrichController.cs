using UnityEngine;
using System.Collections;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;

public class UldrichController : MonoBehaviour
{
    [Header("Uldrich Settings")]

    public GameObject BarrierVisual; // Representação visual da barreira
    public float VulnerableDuration = 7f; // Duração em que Uldrich permanece vulnerável após cair

    [Header("Spiritual World Settings")]
    public float MaxTimeInSpiritualWorld = 10f; // Tempo máximo que o jogador pode ficar no mundo espiritual antes de Uldrich lançar Extinção da Alma

    private UldrichAttackManager _attackManager;
    private UldrichPhaseManager _phaseManager;
    private Character _character; // Referência ao script Character de Uldrich
    private CharacterHorizontalMovement _horizontalMovement; // Referência à habilidade de movimento horizontal
    private Health _health;
    private GameObject _player; // Referência ao jogador

    private bool _isInvulnerable = true;
    private bool _isVulnerableTimerActive = false;
    private bool _playerInSpiritualWorld = false;
    private float _spiritualWorldTimer = 0f;
    private bool _potentializerDestroyed = false;
    private int _spiritsDestroyed = 0;
    private bool _isVulnerableInSpiritualWorld = false;
    private bool _isVulnerableInMaterialWorld = false;
    public float FlySpeed = 5f; // Velocidade de voo ajustável
    private CorgiController _controller; // Referência ao CorgiController

    void Start()
    {
        _attackManager = GetComponent<UldrichAttackManager>();
        _phaseManager = GetComponent<UldrichPhaseManager>();
        _character = GetComponent<Character>();
        _health = GetComponent<Health>();

        // Instancia a barreira uma vez na inicialização, caso necessário
        if (BarrierVisual != null)
        {
            Instantiate(BarrierVisual, transform.position, Quaternion.identity, transform); // Instancia no Uldrich
            
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

        // Encontrar o jogador
        _player = GameObject.FindGameObjectWithTag("Player");
        if (_player == null)
        {
            Debug.LogError("Player not found in the scene!");
        }

        _controller = GetComponent<CorgiController>(); // Inicializa o CorgiController
        if (_controller == null)
        {
            Debug.LogError("CorgiController não encontrado no Uldrich!");
        }

        // Iniciar na Fase 1
        _phaseManager.SetPhase(1);
        ApplyBarrier();
        Debug.Log("Barreira aplicada.");
    }

    void Update()
    {
        // Gerenciar movimentação baseada na fase
        HandleMovement();

        // Atualizar status de invulnerabilidade
        if (_isVulnerableTimerActive && !_isInvulnerable)
        {
            VulnerableDuration -= Time.deltaTime;
            if (VulnerableDuration <= 0)
            {
                Debug.Log("Uldrich Became Invulnerable");
                BecomeInvulnerable();
            }
        }

        // Gerenciar o tempo do jogador no mundo espiritual
        if (_playerInSpiritualWorld)
        {
            _spiritualWorldTimer += Time.deltaTime;
            if (_spiritualWorldTimer >= MaxTimeInSpiritualWorld)
            {
                // O jogador ficou muito tempo no mundo espiritual, Uldrich lança Extinção da Alma
                CastExtincaoDaAlma();
                _spiritualWorldTimer = 0f; // Reseta o temporizador
            }
        }
        else
        {
            _spiritualWorldTimer = 0f; // Reseta o temporizador se o jogador não estiver no mundo espiritual
        }
    }

    private void HandleMovement()
    {
        switch (_phaseManager.CurrentPhase)
        {
            case 1:
                // Uldrich está voando e estacionário
                FlyToPoint(new Vector2(-38, -4)); // Substitua pela posição desejada
                break;

            case 2:
                // Uldrich está no chão e se afasta do jogador
                MoveAwayFromPlayer();
                break;

            case 3:
                // Uldrich é invulnerável e pode ter um comportamento diferente
                if (_spiritsDestroyed < 2)
                {
                    // Continua atacando o jogador
                    MoveAwayFromPlayer();
                }
                else
                {
                    // Após os dois espíritos serem destruídos, Uldrich fica vulnerável no mundo espiritual
                    _character.MovementState.ChangeState(CharacterStates.MovementStates.Idle);
                }
                break;
        }
    }

    /// <summary>
    /// Faz com que Uldrich voe até um ponto específico.
    /// </summary>
    /// <param name="targetPosition">A posição alvo onde Uldrich deve voar.</param>
    public void FlyToPoint(Vector2 targetPosition)
    {
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

        // Para quando atingir a posição
        _controller.SetHorizontalForce(0);
        _controller.SetVerticalForce(0);

        // Define o estado de movimento como Idle
        _character.MovementState.ChangeState(CharacterStates.MovementStates.Idle);
    }

    private void MoveAwayFromPlayer()
    {
        if (_player != null && _horizontalMovement != null)
        {
            // Determina a direção oposta ao jogador
            Vector2 direction = transform.position.x > _player.transform.position.x ? Vector2.right : Vector2.left;
            _horizontalMovement.SetHorizontalMove(direction.x);

            // Configura o estado de movimento
            _character.MovementState.ChangeState(CharacterStates.MovementStates.Walking);
        }
    }

    public void BecomeVulnerable(bool inMaterialWorld = true, bool inSpiritualWorld = false)
    {
        _isInvulnerable = false;
        _isVulnerableTimerActive = true;
        _isVulnerableInMaterialWorld = inMaterialWorld;
        _isVulnerableInSpiritualWorld = inSpiritualWorld;

        RemoveBarrier();

        if (inMaterialWorld)
        {
            // Uldrich cai no chão e se torna vulnerável
            _character.MovementState.ChangeState(CharacterStates.MovementStates.Falling);
            VulnerableDuration = 7f; // Reseta o temporizador
        }
    }

    private void BecomeInvulnerable()
    {
        _isInvulnerable = true;
        _isVulnerableTimerActive = false;
        VulnerableDuration = 7f; // Reseta o temporizador
        _isVulnerableInMaterialWorld = false;
        _isVulnerableInSpiritualWorld = false;
        ApplyBarrier();

        // Uldrich voa novamente
        _character.MovementState.ChangeState(CharacterStates.MovementStates.Jumping);
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
            _health.DamageDisabled();
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
            _health.DamageEnabled();
        }
    }

    private void CastExtincaoDaAlma()
    {
        // Uldrich lança Extinção da Alma
        if (_attackManager != null)
        {
            _attackManager.UseExtincaoDaAlma();
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

    public void OnPotentializerDestroyed()
    {
        if (_phaseManager.CurrentPhase == 1 && !_potentializerDestroyed)
        {
            _potentializerDestroyed = true;
            // Uldrich torna-se vulnerável
            BecomeVulnerable();
        }
    }

    public void OnSpiritDestroyed()
    {
        _spiritsDestroyed++;
        if (_spiritsDestroyed >= 2)
        {
            // Ambos os espíritos foram destruídos
            // Uldrich torna-se vulnerável no mundo espiritual
            BecomeVulnerable(inMaterialWorld: false, inSpiritualWorld: true);
        }
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
                _health.SetHealth(1, gameObject);
            }
            Debug.Log("Uldrich Became Invulnerable");
            BecomeInvulnerable();
        }
        else if (_phaseManager.CurrentPhase == 3)
        {
            // Uldrich foi derrotado
            Debug.Log("Uldrich is defeated");
            // Implementar animações ou eventos de morte
        }
    }
}
