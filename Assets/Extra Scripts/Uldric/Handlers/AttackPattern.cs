using UnityEngine;
using System.Collections;

public abstract class AttackPattern : ScriptableObject
{
    public string AttackName;

    // Método que será implementado por cada ataque específico
    public abstract IEnumerator Execute();
}
