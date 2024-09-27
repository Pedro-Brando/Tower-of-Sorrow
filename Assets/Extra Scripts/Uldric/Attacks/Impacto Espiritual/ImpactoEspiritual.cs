using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.CorgiEngine;

[AddComponentMenu("Corgi Engine/Character/Abilities/ImpactoEspiritual")]
public class ImpactoEspiritual : CharacterAbility
{
    [Header("Configurações da Habilidade")]

    [Tooltip("Prefab do Meteoro a ser spawnado")]
    public GameObject MeteoroPrefab;

    [Tooltip("Prefab da Onda de Choque a ser spawnada")]
    public GameObject OndaDeChoquePrefab;

    [Tooltip("Referência ao PoolManager para meteoros")]
    public MMSimpleObjectPooler meteoroPooler;

    [Tooltip("Referência ao PoolManager para ondas de choque")]
    public MMSimpleObjectPooler ondaDeChoquePooler;

    [Tooltip("Número de ondas para cada lado (esquerda e direita)")]
    public int NumeroDeOndas = 3;

    [Tooltip("Espaçamento vertical entre as plataformas")]
    public float EspacamentoVerticalPlataformas = 1.5f;

    [Tooltip("Dano causado ao jogador")]
    public float Damage = 1f;

    [Tooltip("Tempo de espera antes de permitir outro ataque")]
    public float Cooldown = 10f;

    [Tooltip("Delay entre spawn de cada meteoro (se necessário)")]
    public float SpawnDelay = 0.0f; // Ajuste conforme a necessidade

    private bool canAttack = true;

    /// <summary>
    /// Inicialização da habilidade
    /// </summary>
    protected override void Initialization()
    {
        base.Initialization();

        if (MeteoroPrefab == null)
            Debug.LogError("MeteoroPrefab não está atribuído!");

        if (OndaDeChoquePrefab == null)
            Debug.LogError("OndaDeChoquePrefab não está atribuído!");

        if (meteoroPooler == null)
            Debug.LogError("meteoroPooler não está atribuído!");

        if (ondaDeChoquePooler == null)
            Debug.LogError("ondaDeChoquePooler não está atribuído!");
    }

    /// <summary>
    /// Método público para ativar a habilidade Impacto Espiritual
    /// </summary>
    public void AtivarHabilidade()
    {
        if (canAttack)
        {
            StartCoroutine(ImpactoEspiritualRoutine());
        }
    }

    /// <summary>
    /// Coroutine que gerencia a execução da habilidade
    /// </summary>
    /// <returns></returns>
    private IEnumerator ImpactoEspiritualRoutine()
    {
        canAttack = false;

        // Instancia o meteoro no topo central
        Vector3 spawnPosition = new Vector3(0, 10, 0); // Ajuste conforme a posição da sua arena
        GameObject meteoro = meteoroPooler.GetPooledGameObject();
        if (meteoro != null)
        {
            meteoro.transform.position = spawnPosition;
            meteoro.transform.rotation = Quaternion.identity;
            meteoro.SetActive(true);
            Debug.Log("Meteoro spawnado.");
        }
        else
        {
            Debug.LogWarning("Nenhum Meteoro disponível no pool!");
        }

        // Espera o tempo de execução da queda do meteoro
        yield return new WaitForSeconds(1f); // 1 segundo para a queda

        // A geração das ondas é gerenciada pelo script do Meteoro ao colidir

        // Inicia o cooldown antes de permitir outro ataque
        yield return new WaitForSeconds(Cooldown);
        canAttack = true;
    }

    /// <summary>
    /// Método chamado pelo meteoro ao colidir com o chão para gerar ondas de choque
    /// </summary>
    /// <param name="position">Posição da colisão</param>
    public void GerarOndaDeChoque(Vector3 position)
    {
        StartCoroutine(GerarOndasRoutine(position));
    }

    /// <summary>
    /// Coroutine para gerar ondas de choque nos lados esquerdo e direito
    /// </summary>
    /// <param name="position">Posição da colisão do meteoro</param>
    /// <returns></returns>
    private IEnumerator GerarOndasRoutine(Vector3 position)
    {
        for (int i = 0; i < NumeroDeOndas; i++)
        {
            // Calcular a posição das ondas em cada camada vertical
            float yOffset = i * EspacamentoVerticalPlataformas;

            // Esquerda
            Vector3 posEsquerda = new Vector3(position.x - 5f, position.y - yOffset, position.z); // Ajuste a distância conforme necessário
            GameObject ondaEsquerda = ondaDeChoquePooler.GetPooledGameObject();
            if (ondaEsquerda != null)
            {
                ondaEsquerda.transform.position = posEsquerda;
                ondaEsquerda.transform.rotation = Quaternion.Euler(0, 0, 0); // Direção para a direita
                ondaEsquerda.SetActive(true);
                Debug.Log($"Onda de choque esquerda spawnada em {posEsquerda}.");
            }
            else
            {
                Debug.LogWarning("Nenhuma Onda de Choque esquerda disponível no pool!");
            }

            // Direita
            Vector3 posDireita = new Vector3(position.x + 5f, position.y - yOffset, position.z); // Ajuste a distância conforme necessário
            GameObject ondaDireita = ondaDeChoquePooler.GetPooledGameObject();
            if (ondaDireita != null)
            {
                ondaDireita.transform.position = posDireita;
                ondaDireita.transform.rotation = Quaternion.Euler(0, 0, 180); // Direção para a esquerda
                ondaDireita.SetActive(true);
                Debug.Log($"Onda de choque direita spawnada em {posDireita}.");
            }
            else
            {
                Debug.LogWarning("Nenhuma Onda de Choque direita disponível no pool!");
            }

            // Espera um pequeno delay antes de spawnar a próxima camada de ondas
            yield return new WaitForSeconds(0.2f); // Ajuste conforme necessário
        }

        // Após a geração das ondas, ativa as plataformas nos cantos
        AtivarPlataformasNosCantores();

        // Opcional: Incrementar a intensidade para futuras ativações
        // Exemplo:
        // NumeroDeOndas += 1;
        // EspacamentoVerticalPlataformas += 0.5f;
    }

    /// <summary>
    /// Método para ativar as plataformas nos cantos após a passagem das ondas
    /// </summary>
    private void AtivarPlataformasNosCantores()
    {
        // Encontrar todas as plataformas marcadas com as tags específicas
        GameObject[] plataformasEsquerda = GameObject.FindGameObjectsWithTag("PlatformsEsquerda");
        GameObject[] plataformasDireita = GameObject.FindGameObjectsWithTag("PlatformsDireita");

        foreach (GameObject plataforma in plataformasEsquerda)
        {
            if (plataforma != null)
            {
                plataforma.SetActive(true);
                Debug.Log($"Plataforma esquerda {plataforma.name} ativada.");
            }
        }

        foreach (GameObject plataforma in plataformasDireita)
        {
            if (plataforma != null)
            {
                plataforma.SetActive(true);
                Debug.Log($"Plataforma direita {plataforma.name} ativada.");
            }
        }
    }
}
