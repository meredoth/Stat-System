using UnityEngine;

namespace StatSystem.Example
{
public sealed class MultiplicativeModifiersOperations : ModifiersOperationsBase
{
    internal MultiplicativeModifiersOperations(int capacity) : base(capacity) { }
   
    public override float CalculateModifiersValue(float baseValue, float currentValue)
    {
        float calculatedValue = currentValue;

        for (var i = 0; i < Modifiers.Count; i++)
            calculatedValue += calculatedValue *  Modifiers[i];

        return calculatedValue - currentValue;
    }
}

public class Character : MonoBehaviour
{
    [SerializeField] private Stat strength;
    [SerializeField] private Stat dexterity;
    
    void Start()
    {
        ModifierOperationsCollection.AddModifierOperation(ModifierType.BaseReduction, 
            () => new ModifierOperationsBaseAbsolute(4));
        
        strength = new Stat(100);
        dexterity = new Stat(50);
        
        Modifier mod1 = new Modifier(20, ModifierType.Flat, this);
        Modifier mod2 = new Modifier(0.1f, ModifierType.Additive, this);
        Modifier mod3 = new Modifier(0.2f, ModifierType.Multiplicative);
        Modifier baseReduction = new Modifier(0.4f, ModifierType.BaseReduction);
        
        
        strength.AddModifier(mod2);
        strength.AddModifier(mod1);
        strength.AddModifier(mod1);
        strength.AddModifier(mod3);
        strength.AddModifier(mod3); // 216
        strength.AddModifier(mod1); // 244.8
        //strength.AddModifier(mod1); 
        //strength.AddModifier(mod1); // 302.4
        strength.AddModifier(baseReduction); // 60
        

        Debug.Log($"Strength value: {strength.Value}");
        //strength.TryRemoveModifier(mod1);
        //strength.TryRemoveModifier(mod3); //228
        strength.TryRemoveModifier(baseReduction);
        Debug.Log($"Strength value: {strength.Value}");
        strength.TryRemoveAllModifiersOf(this); // 144

        dexterity.AddModifier(mod1);
        dexterity.AddModifier(mod3);
        dexterity.AddModifier(mod2);
        dexterity.AddModifier(mod1); // 114
        
        
        Debug.Log($"Strength final value: {strength.Value}");
        Debug.Log($"Dexterity final value: {dexterity.Value}");
    }
}
}
