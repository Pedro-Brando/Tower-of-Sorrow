using UnityEngine;
using UnityEngine.UI;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using System.Collections;

public class HeartsHealthDisplay : MonoBehaviour, MMEventListener<CorgiEngineEvent>
{
    [Header("Configurações dos Corações")]
    [Tooltip("Prefab para o coração de saúde.")]
    public GameObject heartPrefab;

    [Tooltip("Transform que será o pai dos corações de saúde.")]
    public Transform heartsContainer;

    [Tooltip("Sprite para coração cheio.")]
    public Sprite fullHeart;

    [Tooltip("Sprite para meio coração.")]
    public Sprite halfHeart;

    [Tooltip("Sprite para coração vazio.")]
    public Sprite emptyHeart;

    private Health playerHealth;
    private int maxHearts;
    private float healthPerHeart;
    private Image[] hearts;

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
        healthPerHeart = 1f; // Cada coração representa 1 ponto de vida
        maxHearts = Mathf.CeilToInt(playerHealth.MaximumHealth / healthPerHeart);
        Debug.Log($"HeartsHealthDisplay: maxHearts = {maxHearts}, healthPerHeart = {healthPerHeart}");

        // Limpa corações existentes
        foreach (Transform child in heartsContainer)
        {
            Destroy(child.gameObject);
        }

        // Cria novos corações
        hearts = new Image[maxHearts];
        for (int i = 0; i < maxHearts; i++)
        {
            GameObject heartGO = Instantiate(heartPrefab, heartsContainer);
            Image heartImage = heartGO.GetComponent<Image>();
            hearts[i] = heartImage;
        }

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
            float heartPosition = (i + 1) * healthPerHeart;

            if (currentHealth >= heartPosition)
            {
                hearts[i].sprite = fullHeart;
                Debug.Log($"HeartsHealthDisplay: Coração {i + 1} cheio.");
            }
            else if (currentHealth > heartPosition - (healthPerHeart / 2f))
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
