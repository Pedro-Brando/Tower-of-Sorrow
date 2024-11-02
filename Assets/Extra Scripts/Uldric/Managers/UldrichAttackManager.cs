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
    public Aniquilar Aniquilar; // Habilidade especial
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

    // Controle para habilidades programadas
    private float _impactoEspiritualNextTime = 0f;
    private float _impactoEspiritualInterval = 10f;
    private int _impactoEspiritualCastCount = 0; // Contador para quantas vezes Impacto Espiritual foi lançado
    private bool _extincaoDaAlmaUsed = false; // Controle se Extinção da Alma foi usado


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
            UpdateAvailableAbilities();
            PerformAttack();
            _lastAttackTime = Time.time;
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

            // Lógica adicional para Impacto Espiritual e Extinção da Alma
            if (ability == ImpactoEspiritual)
            {
                _impactoEspiritualCastCount++;
            }

            if (ability == ExtincaoDaAlma)
            {
                _extincaoDaAlmaUsed = true;
                _impactoEspiritualCastCount = 0; // Reinicia o contador de Impacto Espiritual
                _impactoEspiritualNextTime = Time.time + _impactoEspiritualInterval; // Reinicia o agendamento
            }
        }
    }

    private IEnumerator WaitBeforeStartingAttacks()
    {
        // Aguarda 5 segundos antes de iniciar os ataques
        yield return new WaitForSeconds(5f);
        _impactoEspiritualNextTime = Time.time + _impactoEspiritualInterval;
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
                _availableAbilities.Add(TempestadeDeFogoFatuo);

                // Impacto Espiritual deve ser castado no máximo três vezes, então adiciona se não chegou ao limite
                if (_impactoEspiritualCastCount < 3)
                {
                    _availableAbilities.Add(ImpactoEspiritual);
                }

                break;

            case 2:
                _availableAbilities.Add(TempestadeEspiritual);
                _availableAbilities.Add(Ceifar);
                _availableAbilities.Add(FuriaDoFogoFatuo);
                _availableAbilities.Add(OndaDeChamas);
                break;

            case 3:
                _availableAbilities.Add(TempestadeEspiritual);
                _availableAbilities.Add(Ceifar);
                _availableAbilities.Add(FuriaDoFogoFatuo);
                _availableAbilities.Add(OndaDeChamas);
                _availableAbilities.Add(ConsumirVida);
                break;
            case 4:
                break;
        }
    }

    private bool _espiritoCristalizadoUsed;

    public void PerformAttack()
    {
        // Verifica se é hora de executar ImpactoEspiritual na Fase 1
        if (_phaseManager.CurrentPhase == 1 && Time.time >= _impactoEspiritualNextTime && _impactoEspiritualCastCount < 3 && !_abilityInExecution)
        {
            UseAbility(ImpactoEspiritual);
            _impactoEspiritualNextTime = Time.time + _impactoEspiritualInterval; // Agendar o próximo uso
            return;
        }

        if (_impactoEspiritualCastCount == 3)
        {
            _controller.AtivarPortais();
        }

        // Habilidade Espirito Cristalizado na fase 3 após 30s, apenas uma vez
        if (_phaseManager.CurrentPhase == 3 && Time.time >= _phaseManager.PhaseStartTime + 30f && !_espiritoCristalizadoUsed)
        {
            UseAbility(EspiritoCristalizado);
            _espiritoCristalizadoUsed = true;
            return;
        }

        // Se não for o momento de usar ImpactoEspiritual, realiza um ataque aleatório
        if (_availableAbilities.Count > 0 && !_abilityInExecution)
        {
            PerformRandomAttack();
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



    public void UseExtincaoDaAlma()
    {
        if (ExtincaoDaAlma != null && ExtincaoDaAlma.AbilityPermitted && ExtincaoDaAlma.CooldownReady && !_abilityInExecution)
        {
            UseAbility(ExtincaoDaAlma);
        }

    }

    public void ForceCastExtincaoDaAlma()
    {
        if (ExtincaoDaAlma != null)
        {
            ExtincaoDaAlma.ActivateAbility();
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

    public void UnsubscribeAllAbilities()
    {
        // Desinscrever eventos de conclusão de todas as habilidades
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

        Debug.Log("Todos os eventos de habilidades foram desinscritos.");
    }

}
