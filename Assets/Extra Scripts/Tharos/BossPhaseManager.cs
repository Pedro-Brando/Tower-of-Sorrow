using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPhaseManager : MonoBehaviour
{
    // Referências aos objetos da armadura, cabeça e alma
    public GameObject armorObject;  // Armadura/Capacete
    public GameObject headObject;   // Cabeça
    public GameObject soulObject;   // Alma

    void Start()
    {
        // Garantimos que a cabeça e a alma começam desativadas
        if (headObject != null)
        {
            headObject.SetActive(false);
        }
        
        if (soulObject != null)
        {
            soulObject.SetActive(false);
        }
    }

    void Update()
    {
        // Se a armadura foi desativada (ou destruída)
        if (armorObject != null && !armorObject.activeInHierarchy)
        {
            // Ativamos a cabeça, se ela ainda estiver desativada
            if (headObject != null && !headObject.activeInHierarchy)
            {
                headObject.SetActive(true);
                Debug.Log("A armadura foi destruída. A cabeça está agora ativada.");
            }
        }

        // Se a cabeça foi desativada
        if (headObject != null && !headObject.activeInHierarchy)
        {
            // Ativamos a alma, se ela ainda estiver desativada
            if (soulObject != null && !soulObject.activeInHierarchy)
            {
                soulObject.SetActive(true);
                Debug.Log("A cabeça foi destruída. A alma está agora ativada.");
            }
        }
    }
}
