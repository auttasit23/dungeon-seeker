using System.Collections.Generic;
using Inventory.Model;
using UnityEngine;

public class ItemSpriteRandomizer : MonoBehaviour
{
    [SerializeField]
    private List<Sprite> weaponSprites; // เก็บ Sprite สำหรับ Weapon
    [SerializeField]
    private List<Sprite> armorSprites;  // เก็บ Sprite สำหรับ Armor
    [SerializeField]
    private List<Sprite> accessorySprites; // เก็บ Sprite สำหรับ Accessory

    public void AssignRandomSprite(ItemSO item)
    {
        Sprite randomSprite = null;
        switch (item.itemType)
        {
            case Itemtype.Weapon:
                randomSprite = GetRandomSprite(weaponSprites);
                break;
            case Itemtype.Armor:
                randomSprite = GetRandomSprite(armorSprites);
                break;
            case Itemtype.Accessory:
                randomSprite = GetRandomSprite(accessorySprites);
                break;
            default:
                Debug.LogWarning("Unknown item type. Cannot assign sprite.");
                return;
        }
        
        if (randomSprite != null)
        {
            item.ItemImage = randomSprite;
            Debug.Log($"Assigned Sprite: {randomSprite.name} to Item: {item.itemName}");
        }
    }

    private Sprite GetRandomSprite(List<Sprite> sprites)
    {
        if (sprites == null || sprites.Count == 0)
        {
            Debug.LogWarning("Sprite list is empty or null.");
            return null;
        }

        // สุ่ม Sprite จาก List
        return sprites[UnityEngine.Random.Range(0, sprites.Count)];
    }
}