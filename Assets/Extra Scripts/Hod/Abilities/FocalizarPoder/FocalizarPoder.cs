using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;

namespace MoreMountains.CorgiEngine
{
    [AddComponentMenu("Corgi Engine/Character/Abilities/Hod/FocalizarPoder")]
    public class FocalizarPoder : CharacterAbility, IHodAbility
    {
        #region Serialized Fields

        [Header("Configurações de Focalizar Poder")]

        [Tooltip("Prefab das cópias de Hod.")]
        [SerializeField] private GameObject copyPrefab;

        [Tooltip("Prefab do feixe de energia.")]
        [SerializeField] private GameObject beamPrefab;

        [Tooltip("Duração da canalização antes do feixe de energia (em segundos).")]
        [SerializeField] private float channelingTime = 1.5f;

        [Tooltip("Duração do feixe de energia (em segundos).")]
        [SerializeField] private float beamDuration = 2f;

        [Tooltip("Dano do feixe de Hod.")]
        [SerializeField] private int damage = 20;

        [Tooltip("Tempo de cooldown da habilidade (em segundos).")]
        [SerializeField] private float cooldownDuration = 5f;

        [Tooltip("Feedback visual e sonoro ao iniciar a habilidade FocalizarPoder.")]
        [SerializeField] private MMF_Player focalizarPoderFeedback;

        [Header("Configurações de Posição")]

        [Tooltip("GameObjects que definem as posições para teletransporte.")]
        [SerializeField] private Transform[] teleportPositions; // Deve conter pelo menos 3 posições

        [Tooltip("GameObject que define a borda esquerda da tela.")]
        [SerializeField] private Transform leftEdgeTransform;

        [Tooltip("GameObject que define a borda direita da tela.")]
        [SerializeField] private Transform rightEdgeTransform;

        #endregion

        #region Private Fields

        private float lastActivationTime = -Mathf.Infinity;

        public bool CooldownReady => Time.time >= lastActivationTime + cooldownDuration;

        public new bool AbilityPermitted => base.AbilityPermitted;

        public event Action OnAbilityCompleted;

        private HodController hodController;

        private HodCopy hodCopyScript;

        private Coroutine focalizarPoderRoutineCoroutine;

        private bool isAbilityActive = false;

        private GameObject[] copies = new GameObject[2];

        private Vector3 leftEdge;
        private Vector3 rightEdge;

        #endregion

        #region Initialization

        protected override void Initialization()
        {
            base.Initialization();

            // Referências necessárias
            hodController = GetComponent<HodController>();
            if (hodController == null)
            {
                Debug.LogError("HodController não encontrado no GameObject!");
            }

            if (copyPrefab == null)
            {
                Debug.LogError("CopyPrefab não atribuído na FocalizarPoder!");
            }

            if (beamPrefab == null)
            {
                Debug.LogError("BeamPrefab não atribuído na FocalizarPoder!");
            }

            if (teleportPositions == null || teleportPositions.Length < 3)
            {
                Debug.LogError("Deve haver pelo menos 3 teleportPositions definidas para FocalizarPoder!");
            }

            if (leftEdgeTransform == null || rightEdgeTransform == null)
            {
                Debug.LogError("LeftEdgeTransform e RightEdgeTransform devem ser definidos na FocalizarPoder!");
            }

            // Definir as bordas esquerda e direita com base nos GameObjects definidos
            leftEdge = leftEdgeTransform.position;
            rightEdge = rightEdgeTransform.position;
        }

        #endregion

        #region Ability Activation

        /// <summary>
        /// Método público para ativar a habilidade FocalizarPoder
        /// </summary>
        public void ActivateAbility()
        {
            if (AbilityAuthorized && CooldownReady && !isAbilityActive)
            {
                isAbilityActive = true;
                lastActivationTime = Time.time;
                focalizarPoderRoutineCoroutine = StartCoroutine(FocalizarPoderRoutine());
            }
        }

        /// <summary>
        /// Coroutine que gerencia a execução da habilidade FocalizarPoder
        /// </summary>
        private IEnumerator FocalizarPoderRoutine()
        {
            Debug.Log("Hod começou a usar FocalizarPoder.");

            // Play feedbacks
            focalizarPoderFeedback?.PlayFeedbacks();

            // Teletransportar Hod e criar as cópias nas posições aleatórias
            TeleportHodAndCreateCopies();
            hodController.Canalizar();
            
            // Iniciar a canalização
            yield return new WaitForSeconds(channelingTime);
            

            // Lançar feixes de energia
            LaunchBeams();

            // Manter os feixes por um tempo antes de destruí-los
            yield return new WaitForSeconds(beamDuration);

            // Limpar feixes e cópias
            DestroyBeamsAndCopies();

            // Iniciar o cooldown
            StartCoroutine(CooldownRoutine());

            // Finalizar a habilidade
            isAbilityActive = false;

            // Notificar que a habilidade foi concluída
            OnAbilityCompleted?.Invoke();
        }

        #endregion

        #region Ability Behavior

        /// <summary>
        /// Teletransporta Hod e cria as cópias nas posições aleatórias definidas
        /// </summary>
        private void TeleportHodAndCreateCopies()
        {
            // Verificar se há posições suficientes
            if (teleportPositions.Length < 3)
            {
                Debug.LogError("Não há posições suficientes para teletransporte!");
                return;
            }

            // Teletransportar Hod para a primeira posição definida
            hodController.transform.position = teleportPositions[0].position;

            // Criar e posicionar as cópias nas posições restantes
            for (int i = 0; i < copies.Length && i < teleportPositions.Length - 1; i++)
            {
                // Instanciar a cópia na posição correspondente
                copies[i] = Instantiate(copyPrefab, teleportPositions[i + 1].position, Quaternion.identity);
                
                hodCopyScript = copies[i].GetComponent<HodCopy>();
                if (hodCopyScript != null)
                {
                    // Inicializar a cópia como falsa
                    hodCopyScript.Initialize(false, hodController);
                    hodCopyScript.BecomeInvulnerable();
                    hodCopyScript.Canalizar();
                }
                else
                {
                    Debug.LogError("HodCopy script não encontrado no CopyPrefab!");
                }
            }
        }



        /// <summary>
        /// Lança os feixes de energia horizontalmente a partir do canto esquerdo até o direito da tela
        /// </summary>
        private void LaunchBeams()
        {
            // Lista para armazenar os feixes criados
            List<GameObject> beams = new List<GameObject>();

            // Lançar feixe da verdadeira Hod com dano
            GameObject hodBeam = Instantiate(beamPrefab, hodController.transform.position, Quaternion.identity);
            hodBeam.tag = "Beam";
            Beam hodBeamScript = hodBeam.GetComponent<Beam>();
            if (hodBeamScript != null)
            {
                hodBeamScript.SetDirection(Vector3.right); // Direção para a direita
                hodBeamScript.SetDamage(damage);
            }
            else
            {
                Debug.LogError("Beam script não encontrado no BeamPrefab!");
            }
            beams.Add(hodBeam);

            // Lançar feixes das cópias sem dano
            foreach (var copy in copies)
            {
                if (copy != null)
                {
                    GameObject copyBeam = Instantiate(beamPrefab, copy.transform.position, Quaternion.identity);
                    copyBeam.tag = "Beam";
                    Beam copyBeamScript = copyBeam.GetComponent<Beam>();
                    if (copyBeamScript != null)
                    {
                        copyBeamScript.SetDirection(Vector3.right);
                        copyBeamScript.SetDamage(0); // Feixes das cópias não causam dano
                        copyBeamScript.SetAsFalseBeam(); // Marca como feixe falso
                    }
                    else
                    {
                        Debug.LogError("Beam script não encontrado no BeamPrefab!");
                    }
                    beams.Add(copyBeam);
                }
            }

            // Opcional: Poderia armazenar a lista de feixes se precisar manipular posteriormente
        }

        /// <summary>
        /// Destrói todos os feixes e cópias criadas durante a habilidade
        /// </summary>
        private void DestroyBeamsAndCopies()
        {
            // Destruir feixes de energia
            foreach (var beam in GameObject.FindGameObjectsWithTag("Beam"))
            {
                Destroy(beam);
            }


            hodController.PararCanalizar();
            // Destruir cópias
            foreach (var copy in copies)
            {
                if (copy != null)
                {
                    Destroy(copy);
                }
            }
        }

        /// <summary>
        /// Coroutine para gerenciar o cooldown da habilidade
        /// </summary>
        private IEnumerator CooldownRoutine()
        {
            Debug.Log("Cooldown da habilidade FocalizarPoder iniciado.");
            yield return new WaitForSeconds(cooldownDuration);
            Debug.Log("Cooldown da habilidade FocalizarPoder concluído.");
        }

        #endregion

        #region Interface Implementation

        /// <summary>
        /// Método para lidar com mudanças de fase, se necessário
        /// </summary>
        /// <param name="currentPhase">A fase atual do jogo.</param>
        public void HandlePhase(int currentPhase)
        {
            // Implementar lógica específica para fases, se necessário
            // Por exemplo, ajustar número de cópias, posições, etc.
            // Este método pode ser expandido conforme as necessidades das fases
        }

        #endregion
    }
}
