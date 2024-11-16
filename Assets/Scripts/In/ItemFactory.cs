using UnityEngine;
using Inventory.Model;

public static class ItemFactory
{
    public static ItemSO CreateRandomItem(Itemtype itemType, ItemSpriteCollection spriteCollection)
    {
        ItemSO randomItem = ScriptableObject.CreateInstance<ItemSO>();
        randomItem.itemType = itemType;

        switch (itemType)
        {
            case Itemtype.Potion:
                randomItem.Name = "Potion";
                randomItem.Description = $"Restores {Random.Range(10, 51)} health or mana.";
                randomItem.ItemImage = GetRandomSprite(spriteCollection.potionSprites);
                break;

            case Itemtype.Weapon:
                randomItem.Name = "Weapon";
                randomItem.Description = $"Grants {Random.Range(5, 21)} bonus attack.";
                randomItem.ItemImage = GetRandomSprite(spriteCollection.weaponSprites);
                break;

            case Itemtype.Armor:
                randomItem.Name = "Armor";
                randomItem.Description = $"Grants {Random.Range(5, 21)} bonus health.";
                randomItem.ItemImage = GetRandomSprite(spriteCollection.armorSprites);
                break;

            case Itemtype.Accessory:
                randomItem.Name = "Accessory";
                randomItem.Description = $"Grants {Random.Range(5, 21)} bonus evasion stats.";
                randomItem.ItemImage = GetRandomSprite(spriteCollection.accessorySprites);
                break;
        }

        return randomItem;
    }

    private static Sprite GetRandomSprite(Sprite[] sprites)
    {
        return sprites[Random.Range(0, sprites.Length)];
    }
}
