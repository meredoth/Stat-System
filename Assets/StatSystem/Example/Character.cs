using UnityEngine;
using static StatSystem.ModifierType;

namespace StatSystem.Example
{
public class Character : MonoBehaviour
{
    [SerializeField] private float strength;
    [SerializeField] private float dexterity;
    
    private Stat _strength;
    private Stat _dexterity;

    private readonly BootsOfSpeed _bootsOfSpeed = new();
    private readonly GlovesOfStrength _glovesOfStrength = new();
    private readonly EnchantedArmor _enchantedArmor = new();
    private readonly AmazingSword _amazingSword = new();
    private readonly WitchKillerAxe _witchKillerAxe = new();
    private ModifierType _baseAbsoluteReduction;

    private class BootsOfSpeed
    {
        public readonly Modifier DexterityModFlat;
        public readonly Modifier DexterityModAdditive;
        public readonly Modifier StrengthModAdditive;
        public readonly Modifier DexterityModMulti;

        public BootsOfSpeed()
        {
            DexterityModFlat = new Modifier(10f, Flat, this);
            DexterityModAdditive = new Modifier(0.2f, Additive, this);
            StrengthModAdditive = new Modifier(-0.1f, Additive, this);
            DexterityModMulti = new Modifier(0.1f, Multiplicative, this);
        }
    }

    private class GlovesOfStrength
    {
        public readonly Modifier StrengthModAdditive;

        public GlovesOfStrength()
            => StrengthModAdditive = new Modifier(0.3f, Additive, this);
    }

    private class EnchantedArmor
    {
        public readonly Modifier StrengthModMulti;
        public readonly Modifier DexterityModMulti;

        public EnchantedArmor()
        {
            StrengthModMulti = new Modifier(0.1f, Multiplicative, this);
            DexterityModMulti = new Modifier(0.3f, Multiplicative, this);
        }
    }

    private class AmazingSword
    {
        public readonly Modifier StrengthModMulti;
        public readonly Modifier StrengthModFlat;

        public AmazingSword()
        {
            StrengthModMulti = new Modifier(0.2f, Multiplicative, this);
            StrengthModFlat = new Modifier(20f, Flat, this);
        }
    }

    private class WitchKillerAxe
    {
        public readonly Modifier StrengthModMulti;
        public readonly Modifier DexterityModMulti;

        public WitchKillerAxe()
        {
            StrengthModMulti = new Modifier(-0.1f,Multiplicative, this);
            DexterityModMulti = new Modifier(0.5f, Multiplicative, this);
        }
    }

    private void Awake()
    {
        _baseAbsoluteReduction = Stat.NewModifierType(400, () => new ModifierOperationsBaseAbsoluteReduction());
    }
    
    void Start()
    {
        _strength = new Stat(strength);
        _dexterity = new Stat(dexterity);
        
        Modifier strengthCurse = new Modifier(0.2f, _baseAbsoluteReduction);
        Modifier witchCurse = new Modifier(0.4f, _baseAbsoluteReduction);

        Debug.Log($"Initial Strength: {_strength.Value} and Dexterity: {_dexterity.Value}");

        // strength = 100 - 0.1*100 = 90, dexterity = (50 + 10 + 0.2 * 50) * 1.1 = 77
        Debug.Log("Equipping boots of speed (-10% strength additive, 10 flat dexterity, 20% additive dexterity, 10% multiplicative dexterity)");
        _strength.AddModifier(_bootsOfSpeed.StrengthModAdditive);
        _dexterity.AddModifier(_bootsOfSpeed.DexterityModFlat);
        _dexterity.AddModifier(_bootsOfSpeed.DexterityModAdditive);
        _dexterity.AddModifier(_bootsOfSpeed.DexterityModMulti);
        Debug.Log($"Stats are now, Strength {_strength.Value} and Dexterity {_dexterity.Value}");

        // strength = 100 - 0.1 * 100 + 0.3 * 100 = 120
        Debug.Log("Equipping Gloves Of Strength (30% strength additive)");
        _strength.AddModifier(_glovesOfStrength.StrengthModAdditive);
        Debug.Log($"Stats are now, Strength {_strength.Value} and Dexterity {_dexterity.Value}");
        
        // strength = (100 - 0.1 * 100 + 0.3 * 100) * 1.1 = 132, dexterity = (50 + 10 + 0.2 * 50) * 1.1 * 1.3 = 100.1
        Debug.Log("Equipping Enchanted Armor (10% strength multiplicative, 30% dexterity multiplicative)");
        _strength.AddModifier(_enchantedArmor.StrengthModMulti);
        _dexterity.AddModifier(_enchantedArmor.DexterityModMulti);
        Debug.Log($"Stats are now, Strength {_strength.Value} and Dexterity {_dexterity.Value}");
        
        // strength = (100 + 0.3 * 100) * 1.1 = 143,
        // dexterity = 50 * 1.3 = 65
        Debug.Log("Removing boots of speed (-10% strength additive, 10 flat dexterity, 20% additive dexterity, 10% multiplicative dexterity)");
        _strength.TryRemoveAllModifiersOf(_bootsOfSpeed);
        _dexterity.TryRemoveAllModifiersOf(_bootsOfSpeed);
        Debug.Log($"Stats are now, Strength {_strength.Value} and Dexterity {_dexterity.Value}");

        // strength = (100 + 0.3 * 100 - 0.1 * 100) * 1.1 = 132,
        // dexterity = (50 + 10 + 0.2 * 50) * 1.1 * 1.3 = 100.1
        Debug.Log("Equipping boots of Speed again (-10% strength additive, 10 flat dexterity, 20% additive dexterity, 10% multiplicative dexterity)");
        _strength.AddModifier(_bootsOfSpeed.StrengthModAdditive);
        _dexterity.AddModifier(_bootsOfSpeed.DexterityModFlat);
        _dexterity.AddModifier(_bootsOfSpeed.DexterityModAdditive);
        _dexterity.AddModifier(_bootsOfSpeed.DexterityModMulti);
        Debug.Log($"Stats are now, Strength {_strength.Value} and Dexterity {_dexterity.Value}");
        
        // strength = (100 + 20 + 0.3 * 100 - 0.1 * 100) * 1.1 * 1.2 = 184.8
        Debug.Log("Equipping Amazing Sword (20 flat strength, 20% multiplicative strength)");
        _strength.AddModifier(_amazingSword.StrengthModFlat);
        _strength.AddModifier(_amazingSword.StrengthModMulti);
        Debug.Log($"Stats are now, Strength {_strength.Value} and Dexterity {_dexterity.Value}");
        
        // strength suppressed((100 + 20 + 0.3 * 100 - 0.1 * 100) * 1.1 * 1.2 = 184.8) = base value (100) * 0.8 = 80
        Debug.Log("The player drinks a curse potion! (20% strength absolute reduction and suppresses all modifiers)");
        _strength.AddModifier(strengthCurse);
        Debug.Log($"Stats are now, Strength {_strength.Value} and Dexterity {_dexterity.Value}");
        
        // strength suppressed((100 + 0.3 * 100 - 0.1 * 100) * 1.1 = 132) = base value (100) * 0.8 = 80
        Debug.Log("A witch appears and makes the player drop the Amazing Sword (20 flat strength, 20% multiplicative strength)");
        _strength.TryRemoveAllModifiersOf(_amazingSword);
        Debug.Log($"Stats are now, Strength {_strength.Value} and Dexterity {_dexterity.Value}");
        
        // strength suppressed((100 + 0.3 * 100 - 0.1 * 100) * 1.1 = 132) = base value (100) * 0.6 = 60
        Debug.Log("The witch casts a witch curse! (40% strength absolute reduction and suppresses all modifiers)");
        _strength.AddModifier(witchCurse);
        Debug.Log($"Stats are now, Strength {_strength.Value} and Dexterity {_dexterity.Value}");
        
        // strength suppressed((100 + 0.3 * 100 - 0.1 * 100) * 1.1 * 0.9 = 118.8) = base value (100) * 0.6 = 60,
        // dexterity = (50 + 10 + 0.2 * 50) * 1.1 * 1.3 * 1.5 = 150.15
        Debug.Log("The player equips the WitchKillerAxe (-10% strength multiplicative, 50% dexterity multiplicative)");
        _strength.AddModifier(_witchKillerAxe.StrengthModMulti);
        _dexterity.AddModifier(_witchKillerAxe.DexterityModMulti);
        Debug.Log($"Stats are now, Strength {_strength.Value} and Dexterity {_dexterity.Value}");
        
        // strength suppressed((100 + 0.3 * 100 - 0.1 * 100) * 1.1 * 0.9 = 118.8) = base value (100) * 0.6 = 60,
        Debug.Log("The curse potion effect wears off! (20% strength absolute reduction and suppresses all modifiers)");
        _strength.TryRemoveModifier(strengthCurse);
        Debug.Log($"Stats are now, Strength {_strength.Value} and Dexterity {_dexterity.Value}");
        
        // strength = (100 + 0.3 * 100 - 0.1 * 100) * 1.1 * 0.9 = 118.8
        Debug.Log("The witch curse wears off! (40% strength absolute reduction and suppresses all modifiers)");
        _strength.TryRemoveModifier(witchCurse);
        Debug.Log($"Stats are now, Strength {_strength.Value} and Dexterity {_dexterity.Value}");
        
        // strength = (100 + 0.3 * 100 - 0.1 * 100) * 1.1 = 132,
        // dexterity = (50 + 10 + 0.2 * 50) * 1.1 * 1.3 = 100.1
        Debug.Log("The player kills the witch, removes the WitchKillerAxe (-10% strength multiplicative, 50% dexterity multiplicative)");
        _strength.TryRemoveAllModifiersOf(_witchKillerAxe);
        _dexterity.TryRemoveAllModifiersOf(_witchKillerAxe);
        Debug.Log($"Stats are now, Strength {_strength.Value} and Dexterity {_dexterity.Value}");
        
        // strength = (100 + 20 + 0.3 * 100 - 0.1 * 100) * 1.1 * 1.2 = 184.8
        Debug.Log("The player equips his Amazing Sword again (20 flat strength, 20% multiplicative strength)");
        _strength.AddModifier(_amazingSword.StrengthModFlat);
        _strength.AddModifier(_amazingSword.StrengthModMulti);
        Debug.Log($"Stats are now, Strength {_strength.Value} and Dexterity {_dexterity.Value}");
    }
}
}
