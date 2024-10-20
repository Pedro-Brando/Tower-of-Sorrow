using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhispsBehaviour : MonoBehaviour
{
    public float rotationSpeed = 100f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Armazenar a rotação global (world rotation) de cada filho antes de rotacionar o pai
        Quaternion[] worldRotations = new Quaternion[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            worldRotations[i] = transform.GetChild(i).rotation;
        }

        // Rotaciona o objeto pai ao redor do eixo Z
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        // Após a rotação, restaura a rotação original de cada filho para que sua orientação no mundo não mude
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).rotation = worldRotations[i];
        }
    }
}
