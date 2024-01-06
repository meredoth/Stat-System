using UnityEngine;

namespace StatSystem.Example
{
public class Example : MonoBehaviour
{
    private Stat strength = new(100);
    private Stat dexterity = new(50);
    
    void Start()
    {
        Modifier mod1 = new Modifier(20, ModifierType.Flat);
        Modifier mod2 = new Modifier(0.1f, ModifierType.Additive);
        Modifier mod3 = new Modifier(0.2f, ModifierType.Multiplicative);
        
        strength.AddModifier(mod1);
        strength.AddModifier(mod1);
        strength.AddModifier(mod2);
        strength.AddModifier(mod3);
        strength.AddModifier(mod3); // 216
        strength.AddModifier(mod1); // 244.8

        strength.TryRemoveModifier(mod1);
        strength.TryRemoveModifier(mod3); //180
        
        dexterity.AddModifier(mod1);
        dexterity.AddModifier(mod3);
        dexterity.AddModifier(mod2);
        dexterity.AddModifier(mod1); // 114
        
        
        Debug.Log($"Strength final value: {strength.Value}");
        Debug.Log($"Dexterity final value: {dexterity.Value}");
    }
}
}
