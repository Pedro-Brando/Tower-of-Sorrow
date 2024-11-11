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

        [Tooltip("Duração da habilidade antes de terminar automaticamente")]
        public float AbilityDuration = 5f;

        [Tooltip("Duração do cooldown da habilidade")]
        public float CooldownDuration = 10f;

        [Tooltip("Posições onde Hod e suas cópias aparecerão")]
        public Transform[] Positions;

        [Tooltip("Feedback ao iniciar a habilidade Dispersar")]
        public MMF_Player DispersarFeedback;

        private float _lastActivationTime = -Mathf.Infinity;

        public new bool AbilityPermitted => base.AbilityPermitted;

        public bool CooldownReady => Time.time >= _lastActivationTime + CooldownDuration;

        public event System.Action OnAbilityCompleted;

        private HodController _hodController;
        private List<GameObject> _copies = new List<GameObject>();
        private GameObject _trueHodInstance;

        protected override void Initialization()
        {
            base.Initialization();
            _hodController = GetComponent<HodController>();
            if (_hodController == null)
            {
                Debug.LogError("HodController não encontrado no GameObject!");
            }
        }

        /// <summary>
        /// Método público para ativar a habilidade Dispersar
        /// </summary>
        public void ActivateAbility()
        {
            if (AbilityAuthorized && CooldownReady)
            {
                _lastActivationTime = Time.time;
                StartCoroutine(DispersarRoutine());
            }
        }

        /// <summary>
        /// Coroutine que gerencia a execução da habilidade Dispersar
        /// </summary>
        private IEnumerator DispersarRoutine()
        {
            // Início da habilidade
            Debug.Log("Hod começou a usar Dispersar.");

            // Feedback ao iniciar a habilidade
            if (DispersarFeedback != null)
            {
                DispersarFeedback.PlayFeedbacks();
            }

            // Fazer Hod desaparecer
            _hodController.SetVisible(false);

            // Criar cópias e posicioná-las
            CreateCopies();

            // Esperar a duração da habilidade ou até que seja finalizada
            float elapsedTime = 0f;
            while (elapsedTime < AbilityDuration)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Destruir cópias e fazer Hod reaparecer
            EndDispersar();
        }

        /// <summary>
        /// Método para finalizar a habilidade Dispersar antecipadamente
        /// </summary>
        public void EndDispersar()
        {
            // Parar quaisquer coroutines em execução
            StopAllCoroutines();

            // Destruir cópias
            DestroyCopies();

            // Reposicionar o HodController na posição da cópia verdadeira
            if (_trueHodInstance != null)
            {
                _hodController.transform.position = _trueHodInstance.transform.position;
                Destroy(_trueHodInstance);
                _trueHodInstance = null;
            }

            // Fazer Hod reaparecer
            _hodController.SetVisible(true);

            // Iniciar cooldown
            StartCoroutine(CooldownRoutine());

            // Notificar que a habilidade foi concluída
            OnAbilityCompleted?.Invoke();
        }

        /// <summary>
        /// Cria as cópias de Hod e posiciona-as nas posições definidas
        /// </summary>
        private void CreateCopies()
        {
            int trueHodIndex = Random.Range(0, Positions.Length);

            for (int i = 0; i < Positions.Length; i++)
            {
                // Criar cópia
                GameObject copy = Instantiate(HodCopyPrefab, Positions[i].position, Quaternion.identity);
                HodCopy hodCopyScript = copy.GetComponent<HodCopy>();

                if (hodCopyScript != null)
                {
                    if (i == trueHodIndex)
                    {
                        // Configurar esta cópia como o verdadeiro Hod
                        hodCopyScript.Initialize(true, _hodController);
                        _trueHodInstance = copy;
                    }
                    else
                    {
                        hodCopyScript.Initialize(false);
                    }
                }
                _copies.Add(copy);
            }
        }

        /// <summary>
        /// Destroi todas as cópias criadas durante a habilidade
        /// </summary>
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

        /// <summary>
        /// Inicia o cooldown da habilidade
        /// </summary>
        private IEnumerator CooldownRoutine()
        {
            yield return new WaitForSeconds(CooldownDuration);
        }
    }
}
