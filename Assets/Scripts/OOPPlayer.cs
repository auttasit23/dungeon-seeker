using Inventory.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

namespace Searching
{
    public class OOPPlayer : Character
    {
        public enum PlayerState
        {
            Idle,
            Walk,
            Attack,
        }
        private PlayerState currentState = PlayerState.Idle;
        public Inventory inventory;
        private UIInventoryItem itemSlot;
        public float moveCooldown = 0.5f;
        private float moveCooldownTimer = 0f;
        public float maxHealth;
        private bool isFacingRight = true;
        public int statPoint;
        public GameObject DeadMenu;

        [SerializeField] private TextMeshProUGUI statText;
        [SerializeField] private TextMeshProUGUI hpText;
        [SerializeField] private TextMeshProUGUI atkText;
        [SerializeField] private TextMeshProUGUI hitText;
        [SerializeField] private TextMeshProUGUI evaText;
        public void Start()
        {
            GetRemainEnergy();
            maxHealth = health;
            mapScript = FindObjectOfType<OOPMapGenerator>();
            animator = gameObject.GetComponent<Animator>();
            itemSlot = FindObjectOfType<UIInventoryItem>(); 
            if (mapScript == null)
            {
                Debug.LogError("OOPMapGenerator not found in the scene!");
            }
            
            if (statText == null)
            {
                Debug.LogError("StatText is not assigned in UIManager!");
            }
        }

        public void Update()
        {
            UpdateStatText();
            PlayerUpdateState();
            if (moveCooldownTimer > 0)
            {
                moveCooldownTimer -= Time.deltaTime;
            }
            
            float horizontalInput = Input.GetAxis("Horizontal");
            if (horizontalInput != 0)
            {
                Flip(horizontalInput);
            }

            if (moveCooldownTimer <= 0)
            {
                if (Input.GetKeyDown(KeyCode.W) && !mapScript.selectingTreasure)
                {
                    Move(Vector2.up);
                    moveCooldownTimer = moveCooldown;
                }
                else if (Input.GetKeyDown(KeyCode.S) && !mapScript.selectingTreasure)
                {
                    Move(Vector2.down);
                    moveCooldownTimer = moveCooldown;
                }
                else if (Input.GetKeyDown(KeyCode.A) && !mapScript.selectingTreasure)
                {
                    Move(Vector2.left);
                    moveCooldownTimer = moveCooldown;
                }
                else if (Input.GetKeyDown(KeyCode.D) && !mapScript.selectingTreasure)
                {
                    Move(Vector2.right);
                    moveCooldownTimer = moveCooldown;
                }
            }
        }

        

        public void Flip(float direction)
        {
            if (direction > 0 && !isFacingRight)
            {
                FlipCharacter();
            }
            else if (direction < 0 && isFacingRight)
            {
                FlipCharacter();
            }
        }

        private void FlipCharacter()
        {
            isFacingRight = !isFacingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
        
        public void PlayerUpdateState()
        {
            if (currentState == PlayerState.Idle)
            {
                animator.SetBool("idle", true);
                animator.SetBool("walk", false);
                animator.SetBool("attack", false);
            }
            if (currentState == PlayerState.Walk)
            {
                animator.SetBool("idle", false);
                animator.SetBool("walk", true);
                animator.SetBool("attack", false);
            }
            if (currentState == PlayerState.Attack)
            {
                animator.SetBool("idle", false);
                animator.SetBool("walk", false);
                animator.SetBool("attack", true);
            }
        }

        public IEnumerator PlayerMoveAnimator()
        {
            currentState = PlayerState.Walk;
            yield return new WaitForSeconds(0.3f);
            currentState = PlayerState.Idle;
        }
        
        public IEnumerator PlayerAttackAnimator()
        {
            currentState = PlayerState.Attack;
            yield return new WaitForSeconds(0.3f);
            currentState = PlayerState.Idle;
        }

        public void Attack(OOPEnemy _enemy)
        {
            _enemy.TakeDamage(damage);
        }

        protected override void CheckDead()
        {
            base.CheckDead();
            if (health <= 0)
            {
                Debug.Log("Player is Dead");
                DeadMenu.SetActive(true);
            }
        }
        
        public void AddStat(string statName, int value)
        {
            switch (statName.ToLower())
            {
                case "damage":
                    damage += value;
                    break;
                case "hitchance":
                    hitchance += value;
                    break;
                case "maxhealth": 
                    maxHealth += value;
                    break;
                case "evasion":
                    evasion += value;
                    break;
                default:
                    Debug.LogWarning("Stat name not recognized: " + statName);
                    break;
            }
        }

        public void IncreaseStatPoints(int point)
        {
            statPoint += point;
            Debug.Log("increase status "+statPoint);
        }
        

        private void UpdateStatText()
        {
            if (statText != null)
            {
                statText.text = $"Status point: " + statPoint;
            }
            else
            {
                Debug.LogError("Stat Text is not assigned in the inspector!");
            }
            if(atkText != null)
            {
                atkText.text = "ATK " + damage;
            }
            else
            {
                Debug.LogError("Stat Text is not assigned in the inspector!");
            }
            if(hpText != null)
            {
                hpText.text = "MAXHP " + maxHealth;
            }
            else
            {
                Debug.LogError("Stat Text is not assigned in the inspector!");
            }
            if(hitText != null)
            {
                hitText.text = "HIT " + hitchance;
            }
            else
            {
                Debug.LogError("Stat Text is not assigned in the inspector!");
            }
            if(evaText != null)
            {
                evaText.text = "EVA " + evasion;
            }
            else
            {
                Debug.LogError("Stat Text is not assigned in the inspector!");
            }
        }

        public void MaxHpUpgrade()
        {
            if (statPoint > 0 && maxHealth <= 75)
            {
                AddStat("maxhealth", 1);
                statPoint -= 1;
                Debug.Log("Current Max HP: " + maxHealth);
            }
            else
            {
                Debug.Log("Not enough stat");
            }
        }
        public void AttackUpgrade()
        {
            if (statPoint > 0)
            {
                AddStat("damage", 1);
                statPoint -= 1;
                Debug.Log("Current damage: " + damage);
            }
            else
            {
                Debug.Log("Not enough stat");
            }
        }

        public void HitChanceUpgrade()
        {
            if (statPoint > 0)
            {
                AddStat("hitchance", 1);
                statPoint -= 1;
                Debug.Log("Current hit chance: " + hitchance);
            }
            else
            {
                Debug.Log("Not enough stat");
            }
        }

        public void EvasionUpgrade()
        {
            if (statPoint > 0)
            {
                AddStat("evasion", 1);
                statPoint -= 1;
                Debug.Log("Current hit chance: " + evasion);
            }
            else
            {
                Debug.Log("Not enough stat");
            }
        }


        /*public void UseFireStorm()
        {
            if (inventory.numberOfItem("FireStorm") > 0)
            {
                inventory.UseItem("FireStorm");
                OOPEnemy[] enemies = SortEnemiesByRemainningEnergy2();
                int count = 3;
                if (count > enemies.Length)
                {
                    count = enemies.Length;
                }
                for (int i = 0; i < count; i++)
                {
                    enemies[i].TakeDamage(10);
                }
            }
            else
            {
                Debug.Log("No FireStorm in inventory");
            }
        }*/

        /*public OOPEnemy[] SortEnemiesByRemainningEnergy1()
        {
            // do selection sort of enemy's energy
            var enemies = mapGenerator.GetEnemies();
            for (int i = 0; i < enemies.Length - 1; i++)
            {
                int minIndex = i;
                for (int j = i + 1; j < enemies.Length; j++)
                {
                    if (enemies[j].energy < enemies[minIndex].energy)
                    {
                        minIndex = j;
                    }
                }
                var temp = enemies[i];
                enemies[i] = enemies[minIndex];
                enemies[minIndex] = temp;
            }
            return enemies;
        }

        public OOPEnemy[] SortEnemiesByRemainningEnergy2()
        {
            var enemies = mapGenerator.GetEnemies();
            Array.Sort(enemies, (a, b) => a.energy.CompareTo(b.energy));
            return enemies;
        }
        */

    }

}