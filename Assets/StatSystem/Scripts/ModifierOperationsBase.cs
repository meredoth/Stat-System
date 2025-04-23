using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace StatSystem
{
/// <summary>Provides a base implementation for handling operations on a list of a certain type of modifiers.</summary>
public abstract class ModifierOperationsBase : IModifiersOperations
{
   /// <summary>The list of modifiers managed by this class.</summary>
   protected readonly List<Modifier> Modifiers;

   /// <summary>Initializes a new instance of the <see cref="ModifierOperationsBase"/> class with an expected specified list capacity.</summary>
   /// <param name="capacity">The initial capacity of the modifiers list.</param>
   protected ModifierOperationsBase(int capacity) => Modifiers = new List<Modifier>(capacity);
   
   /// <summary>Initializes a new instance of the <see cref="ModifierOperationsBase"/> class with a default list capacity of 4.</summary>
   protected ModifierOperationsBase() => Modifiers = new List<Modifier>(4);

   /// <summary>Adds a modifier to the list.</summary>
   /// <param name="modifier">The modifier to add.</param>
   public void AddModifier(Modifier modifier)
   {
      CheckListCapacity(Modifiers, modifier.Type);
      Modifiers.Add(modifier);
   }

   /// <summary>Attempts to remove a specific modifier from the list.</summary>
   /// <param name="modifier">The modifier to remove.</param>
   /// <returns><c>true</c> if the modifier was successfully removed; otherwise, <c>false</c>.</returns>
   public bool TryRemoveModifier(Modifier modifier) => Modifiers.Remove(modifier);

   /// <summary>Retrieves all modifiers in the list.</summary>
   /// <returns>A read-only collection of all modifiers managed by this instance.</returns>
   public ModifiersCollection GetAllModifiers() => new(Modifiers);

   /// <summary>
   /// Calculates the total value of the modifiers applied to a base value.
   /// This method must be implemented by derived classes.
   /// </summary>
   /// <param name="baseValue">The base value of the stat before any modifiers are applied.</param>
   /// <param name="currentValue">The current value of the stat after previous modifiers have been applied.</param>
   /// <returns>The calculated value of the stat after all modifiers have been applied.</returns>
   public abstract float CalculateModifiersValue(float baseValue, float currentValue);

   /// <summary>Removes all modifiers from the list.</summary>
   public void Clear() => Modifiers.Clear();

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