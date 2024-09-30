using UnityEngine;
using System.Collections.Generic;
using MoreMountains.CorgiEngine;

public class UldrichAttackManager : MonoBehaviour
{
    [Header("Habilidades")]
    public TempestadeEspiritual TempestadeEspiritual;
    public Ceifar Ceifar;
    public FuriaDoFogoFatuo FuriaDoFogoFatuo;
    public ImpactoEspiritual ImpactoEspiritual;
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
        UpdateAvailableAbilities();
    }

    void Update()
    {
        if (Time.time >= _lastAttackTime + _attackCooldown)
        {
            PerformRandomAttack();
            _lastAttackTime = Time.time;
        }

        // Uso de habilidades especiais em momentos específicos
        if (_phaseManager != null)
        {
            if (_phaseManager.CurrentPhase == 2 && !_aniquilarUsed)
            {
                UseAniquilar();
            }

            if (_phaseManager.CurrentPhase == 3)
            {
                UseExtincaoDaAlma();
            }
        }
    }

    public void UseAbility(IUldrichAbility ability)
    {
        if (ability != null && ability.AbilityPermitted && ability.CooldownReady)
        {
            ability.ActivateAbility();
        }
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
                break;
        }
    }

    public void PerformRandomAttack()
    {
        if (_availableAbilities.Count > 0)
        {
            int index = Random.Range(0, _availableAbilities.Count);
            UseAbility(_availableAbilities[index]);
        }
    }

    public void UseAniquilar()
    {
        if (Aniquilar != null && Aniquilar.AbilityPermitted && Aniquilar.CooldownReady && !_aniquilarUsed)
        {
            Aniquilar.ActivateAbility();
            _aniquilarUsed = true;
        }
    }

    public void UseExtincaoDaAlma()
    {
        if (ExtincaoDaAlma != null && ExtincaoDaAlma.AbilityPermitted && ExtincaoDaAlma.CooldownReady)
        {
            ExtincaoDaAlma.ActivateAbility();
        }
    }
}
