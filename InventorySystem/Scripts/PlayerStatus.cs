using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public int Health = 50;
    public int Mana = 50;
    public int Strength = 10;
    public int Agility = 10;
    public int Intelligence = 10;
    public int Attack = 10;
    public int Defense = 10;
    public int Block = 10;
    public int MaxHealth = 100;
    public int MaxMana = 100;
    public int Stamina = 50;
    public int MaxStamina = 100;
    public int Speed = 10;
    public int Dexterity = 10;
    public int Luck = 10;

    public void AddStat(StatType statType, int value)
    {
        switch (statType)
        {
            case StatType.Strength:
                Strength += value;
                break;
            case StatType.Agility:
                Agility += value;
                break;
            case StatType.Intelligence:
                Intelligence += value;
                break;
            case StatType.Attack:
                Attack += value;
                break;
            case StatType.Defense:
                Defense += value;
                break;
            case StatType.Block:
                Block += value;
                break;
            case StatType.Health:
                Health = Mathf.Min(Health + value, MaxHealth);
                break;
            case StatType.MaxHealth:
                MaxHealth += value;
                break;
            case StatType.Mana:
                Mana = Mathf.Min(Mana + value, MaxMana);
                break;
            case StatType.MaxMana:
                MaxMana += value;
                break;
            case StatType.Stamina:
                Stamina = Mathf.Min(Stamina + value, MaxStamina);
                break;
            case StatType.MaxStamina:
                MaxStamina += value;
                break;
            case StatType.Speed:
                Speed += value;
                break;
            case StatType.Dexterity:
                Dexterity += value;
                break;
            case StatType.Luck:
                Luck += value;
                break;
                // Add other stat cases as needed
        }
    }

    public int GetStat(StatType statType)
    {
        switch (statType)
        {
            case StatType.Strength:
                return Strength;
            case StatType.Agility:
                return Agility;
            case StatType.Intelligence:
                return Intelligence;
            case StatType.Attack:
                return Attack;
            case StatType.Defense:
                return Defense;
            case StatType.Block:
                return Block;
            case StatType.Health:
                return Health;
            case StatType.MaxHealth:
                return MaxHealth;
            case StatType.Mana:
                return Mana;
            case StatType.Stamina:
                return Stamina;
            case StatType.MaxMana:
                return MaxMana;
            case StatType.MaxStamina:
                return MaxStamina;
            case StatType.Speed:
                return Speed;
            case StatType.Dexterity:
                return Dexterity;
            case StatType.Luck:
                return Luck;
            // Add other stat cases as needed
            default:
                return 0;
        }
    }
}
