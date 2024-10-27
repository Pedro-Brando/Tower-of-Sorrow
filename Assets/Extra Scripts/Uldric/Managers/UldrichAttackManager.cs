using UnityEngine;
using System.Collections.Generic;
using MoreMountains.CorgiEngine;
using Unity.VisualScripting;
using System.Collections;

public class UldrichAttackManager : MonoBehaviour
{
    [Header("Habilidades")]
    public TempestadeEspiritual TempestadeEspiritual;
    public Ceifar Ceifar;
    public FuriaDoFogoFatuo FuriaDoFogoFatuo;
    public ImpactoEspiritual ImpactoEspiritual;
    public TempestadeDeFogoFatuo TempestadeDeFogoFatuo;     
    public ExtincaoDaAlma Aniquilar; // Habilidade especial
    public OndaDeChamas OndaDeChamas;
    public ConsumirVida ConsumirVida;
    public EspiritoCristalizado EspiritoCristalizado;
    public ExtincaoDaAlma ExtincaoDaAlma; // Habilidade especial

    private UldrichPhaseManager _phaseManager;
    private UldrichController _controller;

    private List<IUldrichAbility> _availableAbilities = new List<IUldrichAbility>();
    private bool _aniquilarUsed = false;

    private float _attackCooldown = 3f;
    private float _lastAttackTime = -Mathf.Infinity;

    private bool _abilityInExecution = false;

    void Awake()
    {
        _phaseManager = GetComponent<UldrichPhaseManager>();
        _controller = GetComponent<UldrichController>();

        if (_phaseManager == null)
        {
            Debug.LogError("UldrichPhaseManager not found on Uldrich!");
        }
    }

    void Start()
    {
        StartCoroutine(WaitBeforeStartingAttacks());
        UpdateAvailableAbilities();
    }

    void Update()
    {
        if (!_abilityInExecution && Time.time >= _lastAttackTime + _attackCooldown)
        {
            PerformRandomAttack();
            _lastAttackTime = Time.time;
        }

        // Uso de habilidades especiais em momentos específicos
        if (_phaseManager != null)
        {
            if (_phaseManager.CurrentPhase == 2 && !_aniquilarUsed && !_abilityInExecution)
            {
                UseAniquilar();
            }

            if (_phaseManager.CurrentPhase == 3 && !_abilityInExecution)
            {
                UseExtincaoDaAlma();
            }
        }
    }

    public void UseAbility(IUldrichAbility ability)
    {
        if (ability != null && ability.AbilityPermitted && ability.CooldownReady && !_abilityInExecution)
        {
            ability.ActivateAbility();
            _abilityInExecution = true;

            // Inscrevendo-se no evento OnAbilityCompleted para saber quando a habilidade termina
            ability.OnAbilityCompleted += OnAbilityCompleted;
        }
    }

    private IEnumerator WaitBeforeStartingAttacks()
    {
        // Aguarda 5 segundos antes de iniciar os ataques
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
                _availableAbilities.Add(TempestadeEspiritual);
                _availableAbilities.Add(Ceifar);
                _availableAbilities.Add(FuriaDoFogoFatuo);
                _availableAbilities.Add(ImpactoEspiritual);
                _availableAbilities.Add(TempestadeDeFogoFatuo);
                break;

            case 2:
                _availableAbilities.Add(TempestadeEspiritual);
                _availableAbilities.Add(Ceifar);
                _availableAbilities.Add(FuriaDoFogoFatuo);
                _availableAbilities.Add(ImpactoEspiritual);
                _availableAbilities.Add(OndaDeChamas);
                break;

            case 3:
                _availableAbilities.Add(TempestadeEspiritual);
                _availableAbilities.Add(Ceifar);
                _availableAbilities.Add(FuriaDoFogoFatuo);
                _availableAbilities.Add(ImpactoEspiritual);
                _availableAbilities.Add(OndaDeChamas);
                _availableAbilities.Add(ConsumirVida);
                _availableAbilities.Add(EspiritoCristalizado);
                _availableAbilities.Add(TempestadeDeFogoFatuo);
                break;
        }
    }

    public void PerformRandomAttack()
    {
        if (_availableAbilities.Count > 0 && !_abilityInExecution)
        {
            int index = Random.Range(0, _availableAbilities.Count);
            UseAbility(_availableAbilities[index]);
        }
    }

    public void UseAniquilar()
    {
        if (Aniquilar != null && Aniquilar.AbilityPermitted && Aniquilar.CooldownReady && !_aniquilarUsed && !_abilityInExecution)
        {
            UseAbility(Aniquilar);
            _aniquilarUsed = true;
        }
    }

    public void UseExtincaoDaAlma()
    {
        if (ExtincaoDaAlma != null && ExtincaoDaAlma.AbilityPermitted && ExtincaoDaAlma.CooldownReady && !_abilityInExecution)
        {
            UseAbility(ExtincaoDaAlma);
        }
    }

    // Callback que será chamado quando uma habilidade for concluída
    private void OnAbilityCompleted()
    {
        _abilityInExecution = false;

        // Desinscrever de todas as habilidades (assumindo que pode ser qualquer habilidade que terminou)
        if (TempestadeEspiritual != null) TempestadeEspiritual.OnAbilityCompleted -= OnAbilityCompleted;
        if (Ceifar != null) Ceifar.OnAbilityCompleted -= OnAbilityCompleted;
        if (FuriaDoFogoFatuo != null) FuriaDoFogoFatuo.OnAbilityCompleted -= OnAbilityCompleted;
        if (ImpactoEspiritual != null) ImpactoEspiritual.OnAbilityCompleted -= OnAbilityCompleted;
        if (OndaDeChamas != null) OndaDeChamas.OnAbilityCompleted -= OnAbilityCompleted;
        if (ConsumirVida != null) ConsumirVida.OnAbilityCompleted -= OnAbilityCompleted;
        if (EspiritoCristalizado != null) EspiritoCristalizado.OnAbilityCompleted -= OnAbilityCompleted;
        if (ExtincaoDaAlma != null) ExtincaoDaAlma.OnAbilityCompleted -= OnAbilityCompleted;
        if (Aniquilar != null) Aniquilar.OnAbilityCompleted -= OnAbilityCompleted;
        if (TempestadeDeFogoFatuo != null) TempestadeDeFogoFatuo.OnAbilityCompleted -= OnAbilityCompleted;

        Debug.Log("Habilidade concluída. Pronto para a próxima ação.");
    }
}
