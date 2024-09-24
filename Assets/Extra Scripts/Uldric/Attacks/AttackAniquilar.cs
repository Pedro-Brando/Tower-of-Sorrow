using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Attacks/Aniquilar")]
public class AttackAniquilar : AttackPattern
{
    [Header("Prefabs e Efeitos")]
    public GameObject BeamPrefab;               // Prefab do feixe de Aniquilar
    public GameObject IndicatorPrefab;          // Prefab do indicador visual antes do ataque
    public GameObject SummonEffectPrefab;       // Prefab do efeito de invocação
    
    [Header("Parâmetros do Ataque")]
    public float IndicatorDuration = 1f;        // Duração do indicador visual
    public float BeamDuration = 2f;             // Duração do feixe
    public float BeamDamage = 100f;             // Dano causado pelo feixe
    public float BeamWidth = 0.5f;              // Largura do feixe
    public float BeamRange = 15f;               // Alcance do feixe
    public float AttackCooldown = 20f;          // Tempo de espera antes do próximo ataque

    private Transform _uldricTransform;         // Referência ao transform de Uldric
    private Transform _playerTransform;          // Referência ao transform do jogador

    public override IEnumerator Execute()
    {
        // Obter referência ao Uldric e ao jogador
        if (_uldricTransform == null)
        {
            _uldricTransform = this.transform.parent; // Assumindo que o ScriptableObject está dentro de um GameObject filho de Uldric
        }

        if (_playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                _playerTransform = player.transform;
            }
            else
            {
                Debug.LogWarning("Player não encontrado! Certifique-se de que o jogador tem a tag 'Player'.");
                yield break;
            }
        }

        // Instanciar o efeito de invocação
        if (SummonEffectPrefab != null)
        {
            Instantiate(SummonEffectPrefab, _uldricTransform.position, Quaternion.identity);
        }

        // Esperar um breve momento antes de começar a gerar o indicador
        yield return new WaitForSeconds(0.5f);

        // Instanciar o indicador visual
        if (IndicatorPrefab != null)
        {
            Instantiate(IndicatorPrefab, GetIndicatorPosition(), Quaternion.identity);
        }

        // Aguarda a duração do indicador
        yield return new WaitForSeconds(IndicatorDuration);

        // Instanciar e configurar o feixe de Aniquilar
        if (BeamPrefab != null)
        {
            GameObject beam = Instantiate(BeamPrefab, _uldricTransform.position, Quaternion.identity);
            BeamBehavior beamBehavior = beam.GetComponent<BeamBehavior>();
            if (beamBehavior != null)
            {
                Vector3 direction = (_playerTransform.position - _uldricTransform.position).normalized;
                beamBehavior.Initialize(direction, BeamRange, BeamWidth, BeamDamage);
            }

            // Reproduzir efeitos sonoros específicos do feixe, se necessário
            // Por exemplo:
            // AudioSource audioSource = beam.GetComponent<AudioSource>();
            // if (audioSource != null) audioSource.Play();
        }

        // Aguarda a duração do feixe
        yield return new WaitForSeconds(BeamDuration);

        // Destruir o feixe após a duração
        // (Isso pode ser feito no próprio BeamBehavior)
        // yield return new WaitForSeconds(BeamDuration);
        // Destroy(beam);
        
        // Aguarda o cooldown antes de permitir o próximo ataque
        yield return new WaitForSeconds(AttackCooldown);
    }

    // Método para determinar a posição do indicador (pode ser na frente de Uldric)
    private Vector3 GetIndicatorPosition()
    {
        Vector3 direction = (_playerTransform.position - _uldricTransform.position).normalized;
        Vector3 indicatorPosition = _uldricTransform.position + direction * 5f; // Ajuste a distância conforme necessário
        return indicatorPosition;
    }
}
