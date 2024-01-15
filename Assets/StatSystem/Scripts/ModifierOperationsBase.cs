using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace StatSystem
{
public abstract class ModifierOperationsBase : IModifiersOperations
{
   protected readonly List<Modifier> Modifiers;

   protected ModifierOperationsBase(int capacity) => Modifiers = new List<Modifier>(capacity);
   protected ModifierOperationsBase() => Modifiers = new List<Modifier>(4);

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
         Debug.LogWarning($"Resize of {type} modifiers List! Consider initializing the list with a higher capacity.");
#endif
   }
}
}