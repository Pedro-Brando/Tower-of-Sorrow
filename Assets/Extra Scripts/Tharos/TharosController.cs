using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TharosController : MonoBehaviour
{
    [Header("Configurações do Ataque")]
    public GameObject warningPrefab; // Prefab do aviso de ataque
    public float desvio_warningY = 1;
    public GameObject attackPrefab;  // Prefab do ataque real
    public float intervaloAtaque = 5f; // Intervalo entre ataques
    public float tempoAntesDeAtacar = 2f; // Tempo de preparação antes do ataque

    [Header("Configurações do Dano")]
    public GameObject instigator; // Referência ao boss ou ao objeto que causou a explosão
    public float flickerDuration = 0.5f; // Duração do efeito de piscada
    public float invincibilityDuration = 1f; // Duração da invulnerabilidade após o dano

    private List<Transform> plataformasAlvo = new List<Transform>();
    private bool podeAtacar = true;

    void Start()
    {
        // Verificar se os prefabs estão atribuídos
        if (warningPrefab == null)
        {
            Debug.LogError("warningPrefab não está atribuído no Inspector.");
        }

        if (attackPrefab == null)
        {
            Debug.LogError("attackPrefab não está atribuído no Inspector.");
        }

        // Encontrar todas as plataformas com a tag "PlataformaBoss"
        GameObject[] plataformas = GameObject.FindGameObjectsWithTag("PlataformaBoss");
        foreach (GameObject plataforma in plataformas)
        {
            plataformasAlvo.Add(plataforma.transform);
        }

        if (plataformasAlvo.Count == 0)
        {
            Debug.LogWarning("Nenhuma plataforma alvo encontrada com a tag 'PlataformaBoss'.");
        }

        // Iniciar a rotina de ataques
        StartCoroutine(RotinaDeAtaque());
    }

    IEnumerator RotinaDeAtaque()
    {
        while (podeAtacar)
        {
            yield return StartCoroutine(AtacarPlataformas());
            yield return new WaitForSeconds(intervaloAtaque);
        }
    }

    IEnumerator AtacarPlataformas()
    {
        if (plataformasAlvo.Count == 0)
        {
            Debug.LogWarning("Nenhuma plataforma alvo para atacar.");
            yield break; // Sai da coroutine se não houver plataformas
        }

        // Escolher uma plataforma aleatória
        int indiceAleatorio = Random.Range(0, plataformasAlvo.Count);
        Transform plataformaSelecionada = plataformasAlvo[indiceAleatorio];

        // Definir a posição de instânciação acima da plataforma
        Vector3 posicaoAtaque = plataformaSelecionada.position + Vector3.up * 5f; // Ajuste a altura conforme necessário

        // Instanciar o aviso de ataque
        if (warningPrefab != null)
        {
            Instantiate(warningPrefab, plataformaSelecionada.position + new Vector3(0f, desvio_warningY, 0f), Quaternion.identity);
            Debug.Log($"Aviso de ataque instanciado na plataforma: {plataformaSelecionada.name}");
        }
        else
        {
            Debug.LogError("warningPrefab não está atribuído no Inspector.");
        }

        // Tempo de preparação (pode adicionar animações ou efeitos aqui)
        yield return new WaitForSeconds(tempoAntesDeAtacar);

        // Instanciar o ataque real
        if (attackPrefab != null)
        {
            GameObject ataque = Instantiate(attackPrefab, posicaoAtaque, Quaternion.identity);
            Debug.Log($"Ataque instanciado na plataforma: {plataformaSelecionada.name}");

            // Opcional: Configurar o ataque para saber qual plataforma está atacando
            Explosao explosao = ataque.GetComponent<Explosao>();
            if (explosao != null)
            {
                explosao.instigator = instigator;
                // Configure outros parâmetros se necessário
            }
            else
            {
                Debug.LogError("Explosao script não encontrado no attackPrefab.");
            }
        }
        else
        {
            Debug.LogError("attackPrefab não está atribuído no Inspector.");
        }

        // Opcional: Adicionar efeitos visuais ou sonoros
    }

    // Método para parar os ataques, caso necessário
    public void PararAtaques()
    {
        podeAtacar = false;
        StopAllCoroutines();
        Debug.Log("Ataques do boss foram parados.");
    }
}
