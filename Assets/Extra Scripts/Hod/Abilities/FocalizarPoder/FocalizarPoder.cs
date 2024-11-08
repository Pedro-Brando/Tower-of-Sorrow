using UnityEngine;
using System.Collections;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using System;

namespace MoreMountains.CorgiEngine
{
    [AddComponentMenu("Corgi Engine/Character/Abilities/FocalizarPoder")]
    public class FocalizarPoder : CharacterAbility, IHodAbility
    {
        [Header("Configurações de Focalizar Poder")]

        [Tooltip("Prefab das cópias de Hod")]
        public GameObject CopyPrefab;

        [Tooltip("Prefab do feixe de energia")]
        public GameObject BeamPrefab;

        [Tooltip("Duração da canalização antes do feixe de energia")]
        public float ChannelingTime = 1.5f;

        [Tooltip("Duração do feixe de energia")]
        public float BeamDuration = 2f;

        [Tooltip("Distância vertical entre Hod e suas cópias")]
        public float VerticalSpacing = 2f;

        [Tooltip("Dano do feixe de Hod")]
        public int Damage = 20;

        [Tooltip("Tempo de cooldown da habilidade")]
        public float CooldownDuration = 5f; // Defina conforme necessário

        private GameObject[] _copies = new GameObject[2];
        private Vector3 _leftEdge;
        private Vector3 _rightEdge;

        private bool _abilityPermitted = true;
        private bool _cooldownReady = true;

        public new bool AbilityPermitted => _abilityPermitted;
        public bool CooldownReady => _cooldownReady;

        public event Action OnAbilityCompleted;

        protected override void Initialization()
        {
            base.Initialization();

            // Calcular a distância Z com base na posição do personagem
            float distance = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);

            // Encontrar bordas da tela (supondo que a câmera principal está centralizada)
            _leftEdge = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.5f, distance));
            _rightEdge = Camera.main.ViewportToWorldPoint(new Vector3(1, 0.5f, distance));
        }

        public void ActivateAbility()
        {
            if (!_abilityPermitted || !_cooldownReady)
                return;

            StartCoroutine(FocalizarPoderRoutine());
        }

        private IEnumerator FocalizarPoderRoutine()
        {
            _abilityPermitted = false;
            _cooldownReady = false;

            // Criar as cópias e posicioná-las
            CreateCopies();

            // Iniciar a canalização (pode ativar uma animação de carregamento aqui)
            yield return new WaitForSeconds(ChannelingTime);

            // Lançar feixes de energia
            LaunchBeams();

            // Manter os feixes por um tempo antes de destruí-los
            yield return new WaitForSeconds(BeamDuration);

            // Limpar feixes e cópias
            DestroyBeamsAndCopies();

            // Iniciar o cooldown
            StartCoroutine(CooldownRoutine());

            // Notificar que a habilidade foi concluída
            OnAbilityCompleted?.Invoke();
        }

        private IEnumerator CooldownRoutine()
        {
            yield return new WaitForSeconds(CooldownDuration);
            _cooldownReady = true;
        }

        private void CreateCopies()
        {
            for (int i = 0; i < _copies.Length; i++)
            {
                Vector3 copyPosition = transform.position + Vector3.up * (i == 0 ? VerticalSpacing : -VerticalSpacing);
                _copies[i] = Instantiate(CopyPrefab, copyPosition, Quaternion.identity);
            }
        }

        private void LaunchBeams()
        {
            // Cria o feixe de Hod com dano
            GameObject hodBeam = Instantiate(BeamPrefab, transform.position, Quaternion.identity);
            hodBeam.tag = "Beam"; // Assegura que a tag está correta
            hodBeam.transform.position = new Vector3(_leftEdge.x, transform.position.y, transform.position.z);
            var hodBeamCollider = hodBeam.AddComponent<BoxCollider2D>();
            hodBeamCollider.isTrigger = true;
            hodBeamCollider.size = new Vector2(Vector3.Distance(_leftEdge, _rightEdge), 1f);
            hodBeam.AddComponent<BeamDamage>().Damage = Damage;  // Adiciona um componente de dano ao feixe de Hod

            // Cria os feixes das cópias (sem dano)
            foreach (var copy in _copies)
            {
                if (copy != null)
                {
                    GameObject copyBeam = Instantiate(BeamPrefab, copy.transform.position, Quaternion.identity);
                    copyBeam.tag = "Beam"; // Assegura que a tag está correta
                    copyBeam.transform.position = new Vector3(_leftEdge.x, copy.transform.position.y, copy.transform.position.z);
                    var copyBeamCollider = copyBeam.AddComponent<BoxCollider2D>();
                    copyBeamCollider.isTrigger = true;
                    copyBeamCollider.size = new Vector2(Vector3.Distance(_leftEdge, _rightEdge), 1f);
                }
            }
        }

        private void DestroyBeamsAndCopies()
        {
            // Destruir feixes de energia
            foreach (var beam in GameObject.FindGameObjectsWithTag("Beam"))
            {
                Destroy(beam);
            }

            // Destruir cópias
            foreach (var copy in _copies)
            {
                if (copy != null)
                {
                    Destroy(copy);
                }
            }
        }

        /// <summary>
        /// Componente de dano para o feixe de Hod
        /// </summary>
        public class BeamDamage : MonoBehaviour
        {
            public int Damage;

            private void OnTriggerEnter2D(Collider2D other)
            {
                if (other.CompareTag("Player"))
                {
                    Health playerHealth = other.GetComponent<Health>();
                    if (playerHealth != null)
                    {
                        playerHealth.Damage(Damage, gameObject, 0f, 0f, Vector3.zero);
                    }
                }
            }
        }
    }
}
