using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Attacks/EspiritosCristalizados")]
public class AttackEspiritoCristalizado : AttackPattern
{
    [Header("Prefabs e Efeitos")]
    public GameObject CristalPrefab;             // Prefab do cristal espiritual
    public GameObject SummonEffectPrefab;        // Prefab do efeito visual de invocação (opcional)
    public GameObject PortalPrefab;              // Prefab do portal para o mundo espiritual

    [Header("Parâmetros do Ataque")]
    public float CristalDuration = 10f;          // Tempo que o cristal permanecerá no chão
    public float CristalSpawnHeight = 5f;        // Altura de spawn do cristal em relação a Uldric
    public float AttackCooldown = 25f;           // Tempo de espera antes de poder realizar o próximo ataque

    private Transform _uldricTransform;          // Referência ao transform de Uldric
    private Transform _playerTransform;           // Referência ao transform do jogador

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

        // Aguarda um breve momento antes de lançar o cristal
        yield return new WaitForSeconds(1f);

        // Calcular a posição de spawn do cristal (centro da plataforma)
        Vector3 spawnPosition = new Vector3(0f, 0f, 0f); // Ajuste conforme a posição central da plataforma

        // Instanciar o cristal espiritual
        GameObject cristal = Instantiate(CristalPrefab, spawnPosition + Vector3.up * CristalSpawnHeight, Quaternion.identity);
        CristalEspiritualBehavior cristalBehavior = cristal.GetComponent<CristalEspiritualBehavior>();
        if (cristalBehavior != null)
        {
            cristalBehavior.Initialize(CristalDuration, PortalPrefab);
        }

        // Aguarda o cooldown antes de permitir o próximo ataque
        yield return new WaitForSeconds(AttackCooldown);
    }
}
