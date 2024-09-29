using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
    [AddComponentMenu("Corgi Engine/Character/Abilities/Extincao Da Alma")]
    public class ExtincaoDaAlma : CharacterAbility
    {
        [Header("Configurações da Extinção da Alma")]

        [Tooltip("Tempo de carregamento antes de ativar a habilidade")]
        public float ChargeTime = 5f;

        [Tooltip("Área de efeito do golpe (GameObject com BoxCollider2D)")]
        public BoxCollider2D EffectArea;

        [Tooltip("Layers que serão afetadas pelo golpe")]
        public LayerMask TargetLayers;

        [Tooltip("Dano causado (use um valor alto para Hit Kill)")]
        public int Damage = 9999;

        [Tooltip("Lista de plataformas a serem destruídas no mundo material")]
        public List<GameObject> MaterialWorldPlatforms;

        [Header("Prefabs e Efeitos")]

        [Tooltip("Prefab do anúncio a ser instanciado durante o carregamento")]
        public GameObject AnnouncementPrefab;

        [Tooltip("Prefab do efeito de partículas na área de efeito")]
        public GameObject AreaEffectPrefab;

        private bool _canUseAbility = true;
        private GameObject _announcementInstance;
        private GameObject _areaEffectInstance;

        /// <summary>
        /// Método público para ativar a habilidade Extinção da Alma
        /// </summary>
        public void ActivateAbility()
        {
            if (_canUseAbility)
            {
                StartCoroutine(ExtincaoDaAlmaRoutine());
            }
        }

        /// <summary>
        /// Coroutine que gerencia a execução da Extinção da Alma
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator ExtincaoDaAlmaRoutine()
        {
            _canUseAbility = false;

            // Início do carregamento
            Debug.Log("Uldrich começou a carregar Extinção da Alma.");
            // TODO: Adicionar animação ou efeito de carregamento aqui

            // Instancia o anúncio
            if (AnnouncementPrefab != null)
            {
                _announcementInstance = Instantiate(AnnouncementPrefab, transform.position, Quaternion.identity);
                // Opcional: parent do anúncio ao personagem ou a outro objeto
                //_announcementInstance.transform.SetParent(transform);
            }

            // Aguarda o tempo de carregamento
            yield return new WaitForSeconds(ChargeTime);

            // Fim do carregamento
            Debug.Log("Extinção da Alma ativada!");

            // Destrói o anúncio
            if (_announcementInstance != null)
            {
                Destroy(_announcementInstance);
            }

            // Destrói as plataformas no mundo material
            DestroyMaterialWorldPlatforms();

            // Aplica dano às entidades na área de efeito
            ApplyDamageInArea();

            // Instancia o efeito de partículas na área de efeito
            if (AreaEffectPrefab != null && EffectArea != null)
            {
                _areaEffectInstance = Instantiate(AreaEffectPrefab, EffectArea.bounds.center, Quaternion.identity);
                // Opcional: ajustar o tamanho do efeito para cobrir a área
                // Exemplo: ajustar escala ou parâmetros do sistema de partículas
            }

            // Espera antes de permitir outro uso (se necessário)
            // yield return new WaitForSeconds(CooldownTime);

            _canUseAbility = true;
        }

        /// <summary>
        /// Destrói todas as plataformas listadas no mundo material
        /// </summary>
        protected virtual void DestroyMaterialWorldPlatforms()
        {
            foreach (GameObject platform in MaterialWorldPlatforms)
            {
                if (platform != null)
                {
                    Destroy(platform);
                    Debug.Log($"Plataforma {platform.name} destruída.");
                }
            }
        }

        /// <summary>
        /// Aplica dano às entidades na área de efeito
        /// </summary>
        protected virtual void ApplyDamageInArea()
        {
            if (EffectArea == null)
            {
                Debug.LogError("EffectArea não está atribuída!");
                return;
            }

            // Obtém todos os colliders na área de efeito que estão nas layers alvo
            Collider2D[] hits = Physics2D.OverlapBoxAll(
                EffectArea.bounds.center,
                EffectArea.bounds.size,
                0f,
                TargetLayers);

            foreach (Collider2D hit in hits)
            {
                Health healthComponent = hit.GetComponent<Health>();
                if (healthComponent != null)
                {
                    // Aplica dano letal
                    healthComponent.Damage(Damage, gameObject, 0f, 0f, Vector3.zero);
                    Debug.Log($"{hit.name} recebeu dano de Extinção da Alma.");
                }
            }
        }

        /// <summary>
        /// Visualiza a área de efeito no editor
        /// </summary>
        protected virtual void OnDrawGizmosSelected()
        {
            if (EffectArea != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(EffectArea.bounds.center, EffectArea.bounds.size);
            }
        }
    }
}
