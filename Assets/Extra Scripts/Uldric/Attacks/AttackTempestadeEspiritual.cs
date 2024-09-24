using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Attacks/TempestadeEspiritual")]
public class AttackTempestadeEspiritual : AttackPattern
{
    public GameObject ProjectilePrefab;        // Prefab do projétil que cairá do céu
    public GameObject IndicatorPrefab;         // Prefab do indicador visual (trilha fantasma)
    public float IndicatorDuration = 1f;       // Duração antes do projétil cair
    public int NumberOfProjectiles = 5;        // Número de projéteis que serão lançados
    public float DelayBetweenProjectiles = 0.5f; // Tempo entre cada projétil
    public float ProjectileSpawnHeight = 10f;  // Altura de onde o projétil será instanciado
    public Vector2 AttackAreaMin;              // Limites mínimos da área de ataque (x, y)
    public Vector2 AttackAreaMax;              // Limites máximos da área de ataque (x, y)

    private Transform _playerTransform;        // Referência ao transform do jogador

    public override IEnumerator Execute()
    {
        // Obter referência ao jogador
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _playerTransform = player.transform;
        }

        for (int i = 0; i < NumberOfProjectiles; i++)
        {
            // Determinar a posição alvo
            Vector3 targetPosition = GetRandomTargetPosition();

            // Instanciar o indicador visual (trilha fantasma) na posição alvo
            GameObject indicator = Instantiate(IndicatorPrefab, targetPosition, Quaternion.identity);

            // Esperar pela duração do indicador
            yield return new WaitForSeconds(IndicatorDuration);

            // Destruir o indicador
            Destroy(indicator);

            // Instanciar o projétil acima da posição alvo
            Vector3 spawnPosition = new Vector3(targetPosition.x, targetPosition.y + ProjectileSpawnHeight, targetPosition.z);
            Instantiate(ProjectilePrefab, spawnPosition, Quaternion.identity);

            // Pode adicionar efeitos sonoros aqui, se desejar

            // Esperar pelo tempo entre projéteis
            yield return new WaitForSeconds(DelayBetweenProjectiles);
        }

        // Esperar um curto período após o ataque
        yield return new WaitForSeconds(1f);
    }

    // Método para determinar posição do jogador
    private Vector3 GetRandomTargetPosition()
    {
        if (_playerTransform != null)
        {
            return new Vector3(_playerTransform.position.x, AttackAreaMin.y, 0f);
        }
        else
        {
            // Se não conseguir a referência ao jogador, retorna uma posição aleatória
            float x = Random.Range(AttackAreaMin.x, AttackAreaMax.x);
            float y = AttackAreaMin.y;
            return new Vector3(x, y, 0f);
        }
    }

}
