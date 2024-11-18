using UnityEngine;
using System.Collections.Generic;
using MoreMountains.CorgiEngine;
using System.Collections;
using Unity.VisualScripting;

public class HodAbilityManager : MonoBehaviour
{
    [Header("Habilidades")]
    public Dispersar Dispersar;
    public Invisibilidade Invisibilidade;
    public Sobrecarga Sobrecarga;
    public Desmoronar Desmoronar;
    public SobrecargaFocalizada SobrecargaFocalizada;
    public FocalizarPoder FocalizarPoder;

    public HodPhaseManager _phaseManager;
    private HodController _controller;

    private List<IHodAbility> _availableAbilities = new List<IHodAbility>();
    private float _attackCooldown = 3f;
    private float _lastAttackTime = -Mathf.Infinity;
    private bool _abilityInExecution = false;

    void Start()
    {
        StartCoroutine(WaitBeforeStartingAttacks());
        UpdateAvailableAbilities();
    }

    void Update()
    {
        if (!_abilityInExecution && Time.time >= _lastAttackTime + _attackCooldown)
        {
            UpdateAvailableAbilities();
            PerformAttack();
            _lastAttackTime = Time.time;
        }
    }

    private IEnumerator WaitBeforeStartingAttacks()
    {
        // Aguarda alguns segundos antes de iniciar os ataques
        yield return new WaitForSeconds(5f);
    }

    public void UpdateAvailableAbilities()
    {
        _availableAbilities.Clear();

        if (_phaseManager == null)
        {
            Debug.LogError("_phaseManager is null in UpdateAvailableAbilities!");
            return;
        }

        switch (_phaseManager.CurrentPhase)
        {
            case 1:
                WaitBeforeStartingAttacks();
                _availableAbilities.Add(Dispersar);
                _availableAbilities.Add(Desmoronar);
                _availableAbilities.Add(Invisibilidade);
                _availableAbilities.Add(Sobrecarga);
                break;

            case 2:
                _availableAbilities.Add(Dispersar);
                _availableAbilities.Add(Desmoronar);
                _availableAbilities.Add(Invisibilidade);
                _availableAbilities.Add(Sobrecarga);
                _availableAbilities.Add(SobrecargaFocalizada);
                break;

            case 3:
                _availableAbilities.Add(Dispersar);
                _availableAbilities.Add(Desmoronar);
                _availableAbilities.Add(Invisibilidade);
                _availableAbilities.Add(Sobrecarga);
                _availableAbilities.Add(SobrecargaFocalizada);
                _availableAbilities.Add(FocalizarPoder);
                break;

            case 4:
                // Nenhuma habilidade na fase de derrota
                break;
        }
    }

    public void PerformAttack()
    {
        if (_availableAbilities.Count > 0 && !_abilityInExecution)
        {
            int index = Random.Range(0, _availableAbilities.Count);
            UseAbility(_availableAbilities[index]);
        }
    }

    public void UseAbility(IHodAbility ability)
    {
        if (ability != null && ability.AbilityPermitted && ability.CooldownReady && !_abilityInExecution)
        {
            ability.ActivateAbility();
            _abilityInExecution = true;

            // Inscrevendo-se no evento OnAbilityCompleted para saber quando a habilidade termina
            ability.OnAbilityCompleted += OnAbilityCompleted;
        }
    }

    // Callback que será chamado quando uma habilidade for concluída
    private void OnAbilityCompleted()
    {
        _abilityInExecution = false;

        // Desinscrever eventos de conclusão de todas as habilidades
        if (Dispersar != null) Dispersar.OnAbilityCompleted -= OnAbilityCompleted;
        if (Invisibilidade != null) Invisibilidade.OnAbilityCompleted -= OnAbilityCompleted;
        if (Sobrecarga != null) Sobrecarga.OnAbilityCompleted -= OnAbilityCompleted;
        if (Desmoronar != null) Desmoronar.OnAbilityCompleted -= OnAbilityCompleted;
        if (SobrecargaFocalizada != null) SobrecargaFocalizada.OnAbilityCompleted -= OnAbilityCompleted;

        Debug.Log("Habilidade concluída. Pronto para a próxima ação.");
    }
}
