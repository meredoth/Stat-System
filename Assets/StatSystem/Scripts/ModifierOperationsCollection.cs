using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace StatSystem
{
public static class ModifierOperationsCollection
{
   private static readonly Dictionary<ModifierType, Func<IModifiersOperations>> ModifierOperationsDict;
   
   [SuppressMessage("NDepend", "ND1901:AvoidNonReadOnlyStaticFields", Justification="AddModifierOperation cannot run after GetModifierOperations")]
   private static bool _IsInitialized;
   
   static ModifierOperationsCollection() => ModifierOperationsDict = new Dictionary<ModifierType, Func<IModifiersOperations>>();

   private sealed class FlatModifiersOperations : ModifiersOperationsBase
   {
      internal FlatModifiersOperations(int capacity) : base(capacity) { }

      public override float CalculateModifiersValue(float baseValue, float currentValue)
      {
         float flatModifiersSum = 0f;
      
         for (var i = 0; i < Modifiers.Count; i++)
            flatModifiersSum += Modifiers[i];

         return flatModifiersSum;
      }
   }
   
   private sealed class AdditiveModifiersOperations : ModifiersOperationsBase
   {
      internal AdditiveModifiersOperations(int capacity) : base(capacity) { }

      public override float CalculateModifiersValue(float baseValue, float currentValue)
      {
         float additiveModifiersSum = 0f;
      
         for (var i = 0; i < Modifiers.Count; i++)
            additiveModifiersSum += Modifiers[i];

         return baseValue * additiveModifiersSum;
      }
   }
   
   private sealed class MultiplicativeModifiersOperations : ModifiersOperationsBase
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

   public static void AddModifierOperation(ModifierType modifierType, Func<IModifiersOperations> modifierOperationsDelegate)
   {
      if (_IsInitialized)
         throw new InvalidOperationException(
            "Add any modifier operations before any initialization of the Stat class!");

      if (modifierType is ModifierType.Flat or ModifierType.Additive or ModifierType.Multiplicative)
         Debug.LogWarning("modifier operations for types flat, additive and multiplicative cannot be changed! Default operations for these types will be used.");
      
      ModifierOperationsDict[modifierType] = modifierOperationsDelegate;
   }

   internal static Dictionary<ModifierType, Func<IModifiersOperations>> GetModifierOperations(int capacity)
   {
      ModifierOperationsDict[ModifierType.Flat] = () => new FlatModifiersOperations(capacity);
      ModifierOperationsDict[ModifierType.Additive] = () => new AdditiveModifiersOperations(capacity);
      ModifierOperationsDict[ModifierType.Multiplicative] = () => new MultiplicativeModifiersOperations(capacity);

      _IsInitialized = true;
      
      return ModifierOperationsDict;
   }
}
}