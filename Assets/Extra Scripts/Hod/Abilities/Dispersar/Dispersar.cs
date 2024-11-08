using UnityEngine;
using System.Collections;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using System.Collections.Generic;
using MoreMountains.Feedbacks;

namespace MoreMountains.CorgiEngine
{
    [AddComponentMenu("Corgi Engine/Character/Abilities/Hod/Dispersar")]
    public class Dispersar : CharacterAbility, IHodAbility
    {
        [Header("Configurações do Dispersar")]
        public GameObject HodCopyPrefab;
        public float AbilityDuration = 5f;
        public float CooldownDuration = 10f;
        public Transform[] Positions; // Posições onde Hod e suas cópias aparecerão
        public MMF_Player DispersarFeedback;

        private bool _abilityPermitted = true;
        private bool _cooldownReady = true;
        public new bool AbilityPermitted => _abilityPermitted;
        public bool CooldownReady => _cooldownReady;
        public event System.Action OnAbilityCompleted;

        private HodController _hodController;
        private List<GameObject> _copies = new List<GameObject>();
        private GameObject _trueHodInstance;

        protected override void Initialization()
        {
            base.Initialization();
            _hodController = GetComponent<HodController>();
        }

        public void ActivateAbility()
        {
            if (!_abilityPermitted || !_cooldownReady)
                return;

            StartCoroutine(DispersarRoutine());
        }

        private IEnumerator DispersarRoutine()
        {
            _abilityPermitted = false;
            _cooldownReady = false;

            // Inicia o feedback de dispersão
            if (DispersarFeedback != null)
            {
                DispersarFeedback.PlayFeedbacks();
            }

            // Fazer Hod desaparecer
            _hodController.SetVisible(false);

            // Criar cópias e posicioná-las
            CreateCopies();

            // Esperar a duração da habilidade
            yield return new WaitForSeconds(AbilityDuration);

            // Destruir cópias e fazer Hod reaparecer
            DestroyCopies();
            _hodController.SetVisible(true);

            // Iniciar o cooldown
            StartCoroutine(CooldownRoutine());

            // Notificar que a habilidade foi concluída
            OnAbilityCompleted?.Invoke();
        }

        private void CreateCopies()
        {
            int trueHodIndex = Random.Range(0, Positions.Length);

            for (int i = 0; i < Positions.Length; i++)
            {
                if (i == trueHodIndex)
                {
                    // Posicionar o verdadeiro Hod
                    _hodController.transform.position = Positions[i].position;
                    _trueHodInstance = _hodController.gameObject;
                }
                else
                {
                    // Criar cópia
                    GameObject copy = Instantiate(HodCopyPrefab, Positions[i].position, Quaternion.identity);
                    _copies.Add(copy);
                }
            }
        }

        private void DestroyCopies()
        {
            foreach (var copy in _copies)
            {
                if (copy != null)
                {
                    Destroy(copy);
                }
            }
            _copies.Clear();
        }

        private IEnumerator CooldownRoutine()
        {
            yield return new WaitForSeconds(CooldownDuration);
            _cooldownReady = true;
        }
    }
}
