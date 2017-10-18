namespace Rpg.Item
{
    public enum ItemType
    {
        Consumable = 0,
        MeleeWeapon = 1,
        Shooter = 2,
        Ammo = 3,
        DefenseHead = 4,
        DefenseShield = 5,
        DefenseHand = 6,
        DefensePants = 7,
        DefenseShoulders = 8,
        DefenseChest = 9,
        Accessory = 10
    }

    public enum ItemAttributes
    {
        Health = 0,
        Stamina = 1,
        Damage = 2,
        StaminaCost = 3,
        DefenseRate = 4,
        DefenseRange = 5,
        AmmoCount = 6,
        MaxHealth = 7,
        MaxStamina = 8
    }
}
