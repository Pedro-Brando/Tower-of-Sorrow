using UnityEngine;
using MoreMountains.CorgiEngine;

[AddComponentMenu("Corgi Engine/Character/Abilities/Footstep Audio")]
[RequireComponent(typeof(AudioSource))]
public class FootstepAudio : CharacterAbility
{
    [Header("Audio Settings")]
    [Tooltip("Array de clipes de passos")]
    public AudioClip[] footstepClips;

    [Tooltip("Intervalo entre os passos (em segundos)")]
    public float stepInterval = 0.5f;

    [Header("Animation Events")]
    [Tooltip("Ativa/desativa o uso de eventos de animação para passos.")]
    public bool UseAnimationEvents = false;

    private AudioSource _audioSource;
    private float _stepTimer;

    /// <summary>
    /// Inicializa os componentes necessários.
    /// </summary>
    protected override void Initialization()
    {
        base.Initialization();
        _audioSource = GetComponent<AudioSource>();

        if (_audioSource == null)
        {
            Debug.LogError("AudioSource componente não encontrado no personagem.");
        }

        if (footstepClips.Length == 0)
        {
            Debug.LogWarning("Nenhum clipe de passo atribuído ao FootstepAudio.");
        }
    }

    /// <summary>
    /// Processa a habilidade a cada frame, apenas se não estiver usando eventos de animação.
    /// </summary>
    public override void ProcessAbility()
    {
        base.ProcessAbility();
        if (!UseAnimationEvents)
        {
            HandleFootstep();
        }
    }

    /// <summary>
    /// Verifica o estado de solo e movimento para reproduzir sons de passo.
    /// </summary>
    protected virtual void HandleFootstep()
    {
        // Verifica se o personagem está no chão e se está se movendo horizontalmente
        if (_controller.State.IsGrounded && Mathf.Abs(_controller.Speed.x) > 0.1f)
        {
            _stepTimer += Time.deltaTime;

            if (_stepTimer >= stepInterval)
            {
                PlayFootstep();
                _stepTimer = 0f;
            }
        }
        else
        {
            _stepTimer = stepInterval; // Reseta o timer quando não está andando
        }
    }

    /// <summary>
    /// Reproduz um som de passo aleatório.
    /// </summary>
    public void PlayFootstep()
    {
        if (footstepClips.Length > 0 && _audioSource != null)
        {
            int index = Random.Range(0, footstepClips.Length);
            _audioSource.pitch = Random.Range(0.95f, 1.05f); // Variação sutil de pitch
            _audioSource.volume = Random.Range(0.8f, 1.0f); // Variação sutil de volume
            _audioSource.PlayOneShot(footstepClips[index]);
        }
    }
}
