using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeLightBehavior : MonoBehaviour
{
    public Transform sprite;
    public float speed, minSize, maxSize;
    private float activeSize;
    public int direction = 1;

    // Start is called before the first frame update
    void Start()
    {
        activeSize = maxSize;
    }

    // Update is called once per frame
    void Update()
    {

        sprite.localScale = Vector3.MoveTowards(sprite.localScale, Vector3.one * activeSize, speed * Time.deltaTime);

        if(sprite.localScale.x == activeSize)
        {
            if(activeSize == maxSize)
            {
                activeSize = minSize;
            }
            else
            {
                activeSize = maxSize;
            }
        }
    }
}
