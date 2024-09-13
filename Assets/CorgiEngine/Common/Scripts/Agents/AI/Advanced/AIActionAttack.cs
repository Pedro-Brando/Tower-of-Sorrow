using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.CorgiEngine;
using System.Collections;

/* Adicionado por Lucca */

public class AIActionAttack : AIAction
{
    private Animator _animator;
    private DamageOnTouch _damageOnTouch;
    private Collider2D _damageCollider;

    protected void Start()
    {
        _animator = GetComponent<Animator>();
        _damageOnTouch = GetComponent<DamageOnTouch>();  // Obtém o componente DamageOnTouch
        _damageCollider = GetComponent<Collider2D>(); // Obtém o collider de dano
        _damageCollider.enabled = false; // Desativa o collider inicialmente
    }

    public override void PerformAction()
    {
        // Disparar a animação de ataque pelo trigger "Attack"
        _animator.SetTrigger("Attack");

        // Ativar o collider de dano para causar dano durante o ataque
        _damageCollider.enabled = true;

        // Corrotina para desativar o collider após a animação de ataque
        StartCoroutine(DisableDamageCollider());
    }

    private IEnumerator DisableDamageCollider()
    {
        // Espera a duração da animação de ataque (você pode ajustar o tempo)
        yield return new WaitForSeconds(0.5f);

        // Desativar o collider após o ataque
        _damageCollider.enabled = false;
    }
}



