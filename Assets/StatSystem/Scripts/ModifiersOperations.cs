using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace StatSystem
{
public abstract class ModifiersOperations : IModifiersOperations
{
   protected readonly List<Modifier> Modifiers;

   protected ModifiersOperations() => Modifiers = new List<Modifier>();

   protected ModifiersOperations(int capacity) => Modifiers = new List<Modifier>(capacity);

   public virtual void AddModifier(Modifier modifier)
   {
      CheckListCapacity(Modifiers, modifier.Type);
      Modifiers.Add(modifier);
   }

   public virtual bool TryRemoveModifier(Modifier modifier) => Modifiers.Remove(modifier);
   public virtual List<Modifier> GetAllModifiers() => Modifiers;

   public abstract float CalculateModifiersValue(float baseValue, float currentValue);
   
   [Conditional("UNITY_EDITOR")]
   private static void CheckListCapacity(List<Modifier> modifiersList, ModifierType type)
   {
#if UNITY_EDITOR
      if(modifiersList.Count == modifiersList.Capacity)
         Debug.LogWarning($"Resize of {type} modifiers List! Consider initializing the list with higher capacity.");
#endif
   }
}
}