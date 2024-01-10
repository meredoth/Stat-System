using UnityEngine;
namespace StatSystem.Example
{
public class Character : MonoBehaviour
{
    [SerializeField] private Stat strength;
    [SerializeField] private Stat dexterity;
    private const ModifierType BASE_ABSOLUTE = (ModifierType)400;
    
    void Start()
    {
        
        ModifierOperationsCollection.AddModifierOperation(BASE_ABSOLUTE,
            () => new ModifierOperationsBaseAbsolute());
        
        strength = new Stat(100);
        dexterity = new Stat(50);
        
        Modifier mod1 = new Modifier(20, ModifierType.Flat, this);
        Modifier mod2 = new Modifier(0.1f, ModifierType.Additive, this);
        Modifier mod3 = new Modifier(0.2f, ModifierType.Multiplicative);
        Modifier baseReduction = new Modifier(0.4f, BASE_ABSOLUTE);
        
        
        strength.AddModifier(mod2);
        strength.AddModifier(mod1);
        strength.AddModifier(mod1);
        strength.AddModifier(mod3);
        strength.AddModifier(mod3); // 216
        strength.AddModifier(mod1); // 244.8
        strength.AddModifier(mod1); 
        strength.AddModifier(mod1); // 302.4
        strength.AddModifier(baseReduction); // 60
        

        Debug.Log($"Strength value: {strength.Value}");
        strength.TryRemoveModifier(mod1);
        strength.TryRemoveModifier(mod3); //228
        var foo = strength.TryRemoveModifier(baseReduction);
        Debug.Log($"{foo} Strength value: {strength.Value}");
        strength.TryRemoveAllModifiersOf(this); // 120

        dexterity.AddModifier(mod1);
        dexterity.AddModifier(mod3);
        dexterity.AddModifier(mod2);
        dexterity.AddModifier(mod1); // 114
        
        
        Debug.Log($"Strength final value: {strength.Value}");
        Debug.Log($"Dexterity final value: {dexterity.Value}");
    }
}
}
