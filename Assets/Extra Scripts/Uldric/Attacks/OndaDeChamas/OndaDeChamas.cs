using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
    [AddComponentMenu("Corgi Engine/Character/Abilities/Onda De Chamas")]
    public class OndaDeChamas : CharacterAbility
    {
        [Header("Configurações da Onda de Chamas")]

        [Tooltip("Prefab da bola de fogo")]
        public GameObject FireballPrefab;

        [Tooltip("Pooler para as bolas de fogo")]
        public MMSimpleObjectPooler FireballPooler; // Adicionado

        [Tooltip("Número de paredes por uso da habilidade")]
        public int NumberOfWalls = 5;

        [Tooltip("Delay entre as paredes")]
        public float WallDelay = 1f;

        [Tooltip("Velocidade das bolas de fogo")]
        public float FireballSpeed = 5f;

        [Tooltip("Dano de cada bola de fogo")]
        public int FireballDamage = 1;

        [Tooltip("Altura (número de linhas) da parede")]
        public int WallHeight = 5;

        [Tooltip("Espaçamento vertical entre as bolas de fogo")]
        public float VerticalSpacing = 1f;

        [Tooltip("Ponto de spawn das paredes")]
        public Transform WallSpawnPoint;

        [Tooltip("Número de vezes que Uldrich pode usar essa habilidade")]
        public int NumberOfUses = 3;

        private int _usesRemaining;
        private bool _abilityInProgress = false;

        private List<int[]> _patterns;

        protected override void Initialization()
        {
            base.Initialization();
            _usesRemaining = NumberOfUses;

            // Inicializa os padrões
            _patterns = new List<int[]>
            {
                // Padrão 1: Ficar parado para desviar (abertura na linha inferior)
                new int[] { 0, 1, 1, 1, 1 },
                // Padrão 2: Um pulo para desviar (abertura no meio)
                new int[] { 1, 1, 0, 1, 1 },
                // Padrão 3: Pulo duplo para desviar (abertura na linha superior)
                new int[] { 1, 1, 1, 1, 0 }
            };

            // Verifica se o Pooler está atribuído
            if (FireballPooler == null)
            {
                Debug.LogError("FireballPooler não está atribuído no OndaDeChamas!");
            }
        }

        public void ActivateAbility()
        {
            if (!_abilityInProgress && _usesRemaining > 0)
            {
                StartCoroutine(OndaDeChamasRoutine());
            }
        }

        private IEnumerator OndaDeChamasRoutine()
        {
            _abilityInProgress = true;
            _usesRemaining--;

            for (int i = 0; i < NumberOfWalls; i++)
            {
                SpawnWall();
                yield return new WaitForSeconds(WallDelay);
            }

            _abilityInProgress = false;
        }

        private void SpawnWall()
        {
            // Seleciona um padrão aleatório
            int patternIndex = Random.Range(0, _patterns.Count);
            int[] pattern = _patterns[patternIndex];

            Vector3 spawnPosition = WallSpawnPoint.position;

            for (int i = 0; i < WallHeight; i++)
            {
                if (pattern[i] == 1)
                {
                    Vector3 fireballPosition = spawnPosition + new Vector3(0, i * VerticalSpacing, 0);
                    GameObject fireball = FireballPooler.GetPooledGameObject();
                    if (fireball != null)
                    {
                        fireball.transform.position = fireballPosition;
                        fireball.transform.rotation = Quaternion.identity;
                        fireball.SetActive(true);

                        // Inicializa a bola de fogo
                        Fireball fireballScript = fireball.GetComponent<Fireball>();
                        if (fireballScript != null)
                        {
                            fireballScript.Speed = FireballSpeed;
                            fireballScript.Damage = FireballDamage;
                            Vector2 direction = Vector2.left; // Supondo que Uldrich esteja à direita do jogador
                            fireballScript.Initialize(direction);
                        }
                        else
                        {
                            Debug.LogWarning("O prefab da bola de fogo não possui o script Fireball.");
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Nenhuma bola de fogo disponível no pool!");
                    }
                }
            }
        }
    }
}
