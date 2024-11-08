using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Searching
{
    public class OOPPlayer : Character
    {
        public Inventory inventory;
        
        public void Start()
        {
            PrintInfo();
            GetRemainEnergy();
            mapScript = FindObjectOfType<OOPMapGenerator>();
            if (mapScript == null)
            {
                Debug.LogError("OOPMapGenerator not found in the scene!");
            }
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                Move(Vector2.up);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                Move(Vector2.down);
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                Move(Vector2.left);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                Move(Vector2.right);
            }
        }

        public void Attack(OOPEnemy _enemy)
        {
            _enemy.TakeDamage(damage);
        }

        protected override void CheckDead()
        {
            base.CheckDead();
            if (maxHealth <= 0)
            {
                Debug.Log("Player is Dead");
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