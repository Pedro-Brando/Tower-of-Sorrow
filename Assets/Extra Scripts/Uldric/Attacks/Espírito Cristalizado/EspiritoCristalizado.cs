using UnityEngine;
using System.Collections;
using MoreMountains.CorgiEngine;

public class EspiritoCristalizado : CharacterAbility
{
    [Header("Configurações da Habilidade")]

    [Tooltip("Prefab do cristal")]
    public GameObject CristalPrefab;

    [Tooltip("Área onde os cristais podem aparecer (BoxCollider2D)")]
    public BoxCollider2D SpawnArea;

    [Tooltip("Número de cristais a serem gerados por ativação")]
    public int NumberOfCrystals = 3;

    [Tooltip("Tempo entre o spawn de cada cristal")]
    public float SpawnDelay = 1f;

    [Tooltip("Tempo que o cristal fica flutuando antes de cair")]
    public float FloatTime = 2f;

    [Tooltip("Prefab do portal a ser instanciado quando o cristal for destruído")]
    public GameObject PortalPrefab;

    /// <summary>
    /// Método público para ativar a habilidade Espírito Cristalizado
    /// </summary>
    public void ActivateAbility()
    {
        Debug.Log("EspiritoCristalizado: ActivateAbility() chamado.");
        StartCoroutine(SpawnCrystals());
    }

    private IEnumerator SpawnCrystals()
    {
        Debug.Log($"EspiritoCristalizado: Iniciando SpawnCrystals(). Número de cristais: {NumberOfCrystals}");

        for (int i = 0; i < NumberOfCrystals; i++)
        {
            SpawnCristal();
            yield return new WaitForSeconds(SpawnDelay);
        }

        Debug.Log("EspiritoCristalizado: SpawnCrystals() concluído.");
    }

    private void SpawnCristal()
    {
        // Seleciona uma posição aleatória dentro da área de spawn
        Vector2 spawnPosition = GetRandomPositionInArea();
        Debug.Log($"EspiritoCristalizado: Spawning cristal na posição {spawnPosition}");

        // Instancia o cristal
        GameObject cristal = Instantiate(CristalPrefab, spawnPosition, Quaternion.identity);
        Debug.Log("EspiritoCristalizado: Cristal instanciado.");

        // Configura o controlador do cristal
        CristalController cristalController = cristal.GetComponent<CristalController>();
        if (cristalController != null)
        {
            cristalController.FloatTime = FloatTime;
            cristalController.PortalPrefab = PortalPrefab;
            Debug.Log("EspiritoCristalizado: Parâmetros do cristal configurados.");
        }
        else
        {
            Debug.LogError("EspiritoCristalizado: O prefab do cristal não possui o script CristalController!");
        }
    }

    private Vector2 GetRandomPositionInArea()
    {
        if (SpawnArea == null)
        {
            Debug.LogError("EspiritoCristalizado: SpawnArea não está atribuída!");
            return Vector2.zero;
        }

        Bounds bounds = SpawnArea.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = bounds.max.y; // Topo da área

        return new Vector2(x, y);
    }

    protected virtual void OnDrawGizmosSelected()
    {
        if (SpawnArea != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(SpawnArea.bounds.center, SpawnArea.bounds.size);
        }
    }
}
