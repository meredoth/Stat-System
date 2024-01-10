using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace StatSystem
{
public static class ModifierOperationsCollection
{
   [SuppressMessage("NDepend", "ND1901:AvoidNonReadOnlyStaticFields", Justification="Unity need this in an Init method for Domain Reload Disable")]
   private static Dictionary<ModifierType, Func<IModifiersOperations>> _ModifierOperationsDict;

   [SuppressMessage("NDepend", "ND1901:AvoidNonReadOnlyStaticFields", Justification="AddModifierOperation cannot run after GetModifierOperations")]
   private static bool _IsInitialized;

   static ModifierOperationsCollection() => Init();

   [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
   private static void Init() // Initializing static variables for Unity's Disable Domain Reload
   {
      _IsInitialized = false;
      _ModifierOperationsDict = new Dictionary<ModifierType, Func<IModifiersOperations>>();
   }
   
   public static void AddModifierOperation(ModifierType modifierType, Func<IModifiersOperations> modifierOperationsDelegate)
   {
      if (_IsInitialized)
         throw new InvalidOperationException(
            "Add any modifier operations before any initialization of the Stat class!");

      if (modifierType is ModifierType.Flat or ModifierType.Additive or ModifierType.Multiplicative)
         Debug.LogWarning("modifier operations for types flat, additive and multiplicative cannot be changed! Default operations for these types will be used.");
      
      _ModifierOperationsDict[modifierType] = modifierOperationsDelegate;
   }

   internal static Dictionary<ModifierType, Func<IModifiersOperations>> GetModifierOperations(int capacity)
   {
      _ModifierOperationsDict[ModifierType.Flat] = () => new FlatModifierOperations(capacity);
      _ModifierOperationsDict[ModifierType.Additive] = () => new AdditiveModifierOperations(capacity);
      _ModifierOperationsDict[ModifierType.Multiplicative] = () => new MultiplicativeModifierOperations(capacity);

      _IsInitialized = true;
      
      return _ModifierOperationsDict;
   }

   private sealed class FlatModifierOperations : ModifierOperationsBase
   {
      internal FlatModifierOperations(int capacity) : base(capacity) { }

      public override float CalculateModifiersValue(float baseValue, float currentValue)
      {
         float flatModifiersSum = 0f;
      
         for (var i = 0; i < Modifiers.Count; i++)
            flatModifiersSum += Modifiers[i];

         return flatModifiersSum;
      }
   }

   private sealed class AdditiveModifierOperations : ModifierOperationsBase
   {
      internal AdditiveModifierOperations(int capacity) : base(capacity) { }

      public override float CalculateModifiersValue(float baseValue, float currentValue)
      {
         float additiveModifiersSum = 0f;
      
         for (var i = 0; i < Modifiers.Count; i++)
            additiveModifiersSum += Modifiers[i];

         return baseValue * additiveModifiersSum;
      }
   }

   private sealed class MultiplicativeModifierOperations : ModifierOperationsBase
   {
      internal MultiplicativeModifierOperations(int capacity) : base(capacity) { }

      public override float CalculateModifiersValue(float baseValue, float currentValue)
      {
         float calculatedValue = currentValue;

         for (var i = 0; i < Modifiers.Count; i++)
            calculatedValue += calculatedValue *  Modifiers[i];

         return calculatedValue - currentValue;
      }
   }
}
}