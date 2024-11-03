using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;
using static UnityEngine.EventSystems.EventTrigger;

namespace Searching
{
    public class Character : Identity
    {
        [Header("Character")]
        public int energy;
        public int AttackPoint;

        protected bool isAlive;
        protected bool isFreeze;
        private bool isMoving = false;

        // Start is called before the first frame update
        protected void GetRemainEnergy()
        {
            Debug.Log(Name + " : " + energy);
        }

        public virtual void Move(Vector2 direction)
        {
            if (isFreeze == true || isMoving)
            {
                GetComponent<SpriteRenderer>().color = Color.white;
                isFreeze = false;
                return;
            }
          
            float toX = positionX + direction.x;
            float toY = positionY + direction.y;
            Vector2 nextPosition = new Vector2(toX, toY);
            
           
            Node[] nodes = FindObjectsOfType<Node>();
            bool hasNodeAtNextPosition = false;
    
            foreach (Node node in nodes)
            {
                if (Vector2.Distance(node.transform.position, nextPosition) < 0.1f)
                {
                    hasNodeAtNextPosition = true;
                    break;
                }
            }
            
            if (!hasNodeAtNextPosition)
            {
                return;
            }


            float fromX = positionX;
            float fromY = positionY;

            /*if (HasPlacement(toX, toY))
            {
                if (IsPotion(toX, toY))
                {
                    mapGenerator.potions[toX, toY].Hit();
                    positionX = toX;
                    positionY = toY;
                }
                else if (IsPotionBonus(toX, toY))
                {
                    mapGenerator.potions[toX, toY].Hit();
                    positionX = toX;
                    positionY = toY;
                }
                else if (IsExit(toX, toY))
                {
                    mapGenerator.Exit.Hit();
                    positionX = toX;
                    positionY = toY;
                }
                else if (IsKey(toX, toY))
                {
                    mapGenerator.keys[toX, toY].Hit();
                    positionX = toX;
                    positionY = toY;
                }
                else if (IsEnemy(toX, toY))
                {
                    mapGenerator.enemies[toX, toY].Hit();
                }
            }
            else
            {
                positionX = toX;
                positionY = toY;
                TakeDamage(1);
            }*/
            positionX = toX;
            positionY = toY;
            TakeDamage(1);
            
            Vector3 targetPosition = new Vector3(positionX, positionY, 0);
            StartCoroutine(MoveSmoothly(targetPosition));

            if (this is OOPPlayer)
            {
                if (fromX != positionX || fromY != positionY)
                {
                    /*mapGenerator.mapdata[fromX, fromY] = mapGenerator.empty;
                    mapGenerator.mapdata[positionX, positionY] = mapGenerator.playerBlock;*/
                    mapGenerator.MoveEnemies();
                }
            }

        }
        
        public IEnumerator MoveSmoothly(Vector3 targetPosition)
        {
            isMoving = true;
            Vector3 startPosition = transform.position;
            float elapsedTime = 0f;
            float duration = 0.5f;

            while (elapsedTime < duration)
            {
                transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
    
            transform.position = targetPosition;
            isMoving = false;
        }
        // hasPlacement คืนค่า true ถ้ามีการวางอะไรไว้บน map ที่ตำแหน่ง x,y
        /*public bool HasPlacement(int x, int y)
        {
            int mapData = mapGenerator.GetMapData(x, y);
            return mapData != mapGenerator.empty;
        }
        public bool IsPotion(int x, int y)
        {
            int mapData = mapGenerator.GetMapData(x, y);
            return mapData == mapGenerator.potion;
        }
        public bool IsPotionBonus(int x, int y)
        {
            int mapData = mapGenerator.GetMapData(x, y);
            return mapData == mapGenerator.potion;
        }
        public bool IsKey(int x, int y)
        {
            int mapData = mapGenerator.GetMapData(x, y);
            return mapData == mapGenerator.key;
        }
        public bool IsEnemy(int x, int y)
        {
            int mapData = mapGenerator.GetMapData(x, y);
            return mapData == mapGenerator.enemy;
        }
        public bool IsExit(int x, int y)
        {
            int mapData = mapGenerator.GetMapData(x, y);
            return mapData == mapGenerator.exit;
        }
        */

        public virtual void TakeDamage(int Damage)
        {
            energy -= Damage;
            Debug.Log(Name + " Current Energy : " + energy);
            CheckDead();
        }
        public virtual void TakeDamage(int Damage, bool freeze)
        {
            energy -= Damage;
            isFreeze = freeze;
            GetComponent<SpriteRenderer>().color = Color.blue;
            Debug.Log(Name + " Current Energy : " + energy);
            Debug.Log("you is Freeze");
            CheckDead();
        }


        public void Heal(int healPoint)
        {
            // energy += healPoint;
            // Debug.Log("Current Energy : " + energy);
            // เราสามารถเรียกใช้ฟังก์ชัน Heal โดยกำหนดให้ Bonuse = false ได้ เพื่อที่จะให้ logic ในการ heal อยู่ที่ฟังก์ชัน Heal อันเดียวและไม่ต้องเขียนซ้ำ
            Heal(healPoint, false);
        }

        public void Heal(int healPoint, bool Bonuse)
        {
            energy += healPoint * (Bonuse ? 2 : 1);
            Debug.Log("Current Energy : " + energy);
        }

        protected virtual void CheckDead()
        {
            if (energy <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}