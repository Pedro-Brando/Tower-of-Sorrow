using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedExplosion : MonoBehaviour
{
    public float explosionDelay = 30f; // Tempo de atraso para a explosão em segundos
    public MonoBehaviour explosaoScript; // Referência ao script de explosão
    public float disableDelay = 1f; // Tempo para desativar o objeto após a explosão

    void OnEnable()
    {
        // Inicia a contagem para a explosão
        Invoke("TriggerExplosion", explosionDelay);
    }

    void OnDisable()
    {
        // Cancela a explosão se o objeto for desativado antes do tempo
        CancelInvoke("TriggerExplosion");
    }

    void TriggerExplosion()
    {
        // Verifica se o script de explosão foi atribuído
        if (explosaoScript != null)
        {
            // Ativa o script de explosão
            explosaoScript.enabled = true;
            Debug.Log("Explosão ativada após 30 segundos!");

            // Aguarda 1 segundo para desativar o objeto após a explosão
            Invoke("DisableObject", disableDelay);
        }
    }

    void DisableObject()
    {
        // Desativa o objeto
        gameObject.SetActive(false);
        Debug.Log("Objeto desativado 1 segundo após a explosão.");
    }
}
