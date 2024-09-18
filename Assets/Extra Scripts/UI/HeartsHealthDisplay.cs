using UnityEngine;
using UnityEngine.UI;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using System.Collections;

public class HeartsHealthDisplay : MonoBehaviour, MMEventListener<CorgiEngineEvent>
{
    [Header("Configurações dos Corações")]
    [Tooltip("Array de componentes Image que representam os corações de saúde.")]
    public Image[] hearts;

    [Tooltip("Sprite para coração cheio.")]
    public Sprite fullHeart;

    [Tooltip("Sprite para meio coração.")]
    public Sprite halfHeart;

    [Tooltip("Sprite para coração vazio.")]
    public Sprite emptyHeart;

    private Health playerHealth;
    private int maxHearts;
    private int healthPerHeart;

    void Start()
    {
        // Inicialmente, não fazemos nada aqui. A inicialização será feita quando o evento LevelStart for recebido.
        Debug.Log("HeartsHealthDisplay: Aguardando evento LevelStart para inicialização.");
    }

    void OnEnable()
    {
        // Iniciar a escuta de eventos
        this.MMEventStartListening<CorgiEngineEvent>();
    }

    void OnDisable()
    {
        // Parar de escutar eventos para evitar vazamentos de memória
        this.MMEventStopListening<CorgiEngineEvent>();
    }

    /// <summary>
    /// Método chamado quando um evento da CorgiEngine é disparado.
    /// </summary>
    /// <param name="engineEvent">Evento disparado.</param>
    public void OnMMEvent(CorgiEngineEvent engineEvent)
    {
        if (engineEvent.EventType == CorgiEngineEventTypes.LevelStart)
        {
            Debug.Log("HeartsHealthDisplay: Evento LevelStart recebido.");
            InitializeHealthDisplay(engineEvent.OriginCharacter);
        }
    }

    /// <summary>
    /// Inicializa a exibição dos corações de saúde com base no jogador fornecido.
    /// </summary>
    /// <param name="player">Jogador para o qual exibir a saúde.</param>
    private void InitializeHealthDisplay(Character player)
    {
        if (player == null)
        {
            Debug.LogError("HeartsHealthDisplay: O jogador fornecido é null.");
            return;
        }

        playerHealth = player.GetComponent<Health>();
        if (playerHealth == null)
        {
            Debug.LogError("HeartsHealthDisplay: O jogador não possui o componente Health.");
            return;
        }

        // Inicializa as configurações dos corações
        maxHearts = hearts.Length;
        healthPerHeart = Mathf.CeilToInt(playerHealth.MaximumHealth / (float)maxHearts);
        Debug.Log($"HeartsHealthDisplay: maxHearts = {maxHearts}, healthPerHeart = {healthPerHeart}");

        // Atualiza a exibição inicial dos corações
        UpdateHeartsDisplay();

        // Inscreve-se nos eventos de mudança de saúde
        playerHealth.OnHit += OnHealthChanged;
        playerHealth.OnDeath += OnHealthChanged;
        playerHealth.OnRevive += OnHealthChanged;

        Debug.Log("HeartsHealthDisplay: Inscrito nos eventos de Health.");
    }

    void OnDestroy()
    {
        // Desinscreve-se dos eventos para evitar vazamentos de memória
        if (playerHealth != null)
        {
            playerHealth.OnHit -= OnHealthChanged;
            playerHealth.OnDeath -= OnHealthChanged;
            playerHealth.OnRevive -= OnHealthChanged;
            Debug.Log("HeartsHealthDisplay: Desinscrito dos eventos de Health.");
        }
    }

    /// <summary>
    /// Método chamado quando a saúde do jogador muda.
    /// </summary>
    void OnHealthChanged()
    {
        Debug.Log("HeartsHealthDisplay: Saúde do jogador mudou.");
        UpdateHeartsDisplay();
    }

    /// <summary>
    /// Atualiza a exibição dos corações de saúde no HUD.
    /// </summary>
    void UpdateHeartsDisplay()
    {
        if (playerHealth == null)
        {
            Debug.LogWarning("HeartsHealthDisplay: playerHealth é null. Abortando UpdateHeartsDisplay.");
            return;
        }

        float currentHealth = playerHealth.CurrentHealth;
        Debug.Log($"HeartsHealthDisplay: currentHealth = {currentHealth}");

        for (int i = 0; i < hearts.Length; i++)
        {
            float heartThreshold = (i + 1) * healthPerHeart;
            float previousHeartThreshold = i * healthPerHeart;

            if (currentHealth >= heartThreshold)
            {
                hearts[i].sprite = fullHeart;
                Debug.Log($"HeartsHealthDisplay: Coração {i + 1} cheio.");
            }
            else if (currentHealth >= previousHeartThreshold + (healthPerHeart / 2f))
            {
                hearts[i].sprite = halfHeart;
                Debug.Log($"HeartsHealthDisplay: Coração {i + 1} meio cheio.");
            }
            else
            {
                hearts[i].sprite = emptyHeart;
                Debug.Log($"HeartsHealthDisplay: Coração {i + 1} vazio.");
            }
        }
    }
}
