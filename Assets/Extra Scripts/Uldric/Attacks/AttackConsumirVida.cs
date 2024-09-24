using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Attacks/ConsumirVida")]
public class AttackConsumirVida : AttackPattern
{
    [Header("Prefabs e Efeitos")]
    public GameObject MarkerPrefab;            // Prefab da marcação no mapa
    public GameObject SummonEffectPrefab;      // Prefab do efeito visual de invocação (opcional)

    [Header("Parâmetros do Ataque")]
    public List<Vector3> MarkerPositions = new List<Vector3>(); // Lista de posições para marcações
    public float ActivationDelay = 3f;         // Delay antes de ativar o dano nas áreas marcadas
    public float Damage = 30f;                  // Dano causado ao jogador ao estar na área marcada

    [Header("Cooldown")]
    public float AttackCooldown = 20f;          // Tempo de espera antes de poder realizar o próximo ataque

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

        // Opcional: Instanciar o efeito de invocação
        if (SummonEffectPrefab != null)
        {
            Instantiate(SummonEffectPrefab, _uldricTransform.position, Quaternion.identity);
        }

        // Aguarda um breve momento antes de marcar as posições
        yield return new WaitForSeconds(1f);

        // Instanciar as marcações no mapa
        foreach (Vector3 position in MarkerPositions)
        {
            Vector3 spawnPosition = position;
            GameObject marker = Instantiate(MarkerPrefab, spawnPosition, Quaternion.identity);
            ConsumirVidaMarker markerScript = marker.GetComponent<ConsumirVidaMarker>();
            if (markerScript != null)
            {
                markerScript.Initialize(ActivationDelay, Damage, _playerTransform);
            }

            // Aguarda o delay entre marcações, se necessário
            yield return null; // Pode ser ajustado conforme a necessidade
        }

        // Aguarda o cooldown antes de permitir o próximo ataque
        yield return new WaitForSeconds(AttackCooldown);
    }
}
