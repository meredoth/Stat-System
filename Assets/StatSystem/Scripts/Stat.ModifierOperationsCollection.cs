using System;
using System.Collections.Generic;
using UnityEngine;
using static StatSystem.ModifierType;

namespace StatSystem
{
public sealed partial class Stat
{
   /// <summary>Represents a collection of modifier operations.</summary>
   private sealed class ModifierOperationsGroups
   {
      private readonly Dictionary<ModifierType, Func<IModifiersOperations>> _modifierOperationsDict = new();

      /// <summary>Gets a value indicating whether the collection has been returned.</summary>
      internal bool HasCollectionBeenReturned { get; private set; }

      
      /// <summary>Adds a new modifier operation to the collection.</summary>
      /// <param name="order">The order of the modifier type.</param>
      /// <param name="modifierOperationsDelegate">The delegate that defines the operations for the modifier type.</param>
      /// <returns>The newly created <see cref="ModifierType"/>.</returns>
      /// <exception cref="InvalidOperationException">Thrown if attempting to change the collection after it has been returned.</exception>
      internal ModifierType AddModifierOperation(int order, Func<IModifiersOperations> modifierOperationsDelegate)
      {
         if (HasCollectionBeenReturned)
            throw new InvalidOperationException("Cannot change collection after it has been returned");
         
         var modifierType = (ModifierType)order;

         if (modifierType is Flat or Additive or Multiplicative)
            Debug.LogWarning("modifier operations for types flat, additive and multiplicative cannot be changed! Default operations for these types will be used.");

         _modifierOperationsDict[modifierType] = modifierOperationsDelegate;

         return modifierType;
      }

      /// <summary>Gets the modifier operations collection and sets default operations for flat, additive and multiplicative operation types.</summary>
      /// <param name="capacity">The expected capacity of each modifier operations collection.</param>
      /// <returns>A read-only dictionary of modifier operations.</returns>
      internal IReadOnlyDictionary<ModifierType, Func<IModifiersOperations>> GetModifierOperations(int capacity)
      {
         _modifierOperationsDict[Flat] = () => new FlatModifierOperations(capacity);
         _modifierOperationsDict[Additive] = () => new AdditiveModifierOperations(capacity);
         _modifierOperationsDict[Multiplicative] = () => new MultiplicativeModifierOperations(capacity);

         HasCollectionBeenReturned = true;
         
         return _modifierOperationsDict;
      }

      private sealed class FlatModifierOperations : ModifierOperationsBase
      {
         internal FlatModifierOperations(int capacity) : base(capacity) { }

         /// <summary>Calculates the value of flat modifiers.</summary>
         /// <param name="baseValue">The base value of the stat.</param>
         /// <param name="currentValue">The current value of the stat.</param>
         /// <returns>The calculated value of the flat modifiers.</returns>
         public override float CalculateModifiersValue(float baseValue, float currentValue)
         {
            float flatModifiersSum = 0f;

            foreach (var t in Modifiers)
               flatModifiersSum += t;

            return flatModifiersSum;
         }
      }

      private sealed class AdditiveModifierOperations : ModifierOperationsBase
      {
         internal AdditiveModifierOperations(int capacity) : base(capacity) { }

         /// <summary>Calculates the value of additive modifiers.</summary>
         /// <param name="baseValue">The base value of the stat.</param>
         /// <param name="currentValue">The current value of the stat.</param>
         /// <returns>The calculated value of the additive modifiers.</returns>
         public override float CalculateModifiersValue(float baseValue, float currentValue)
         {
            float additiveModifiersSum = 0f;

            foreach (var t in Modifiers)
               additiveModifiersSum += t;

            return baseValue * additiveModifiersSum;
         }
      }

      private sealed class MultiplicativeModifierOperations : ModifierOperationsBase
      {
         internal MultiplicativeModifierOperations(int capacity) : base(capacity) { }

         /// <summary>Calculates the value of multiplicative modifiers.</summary>
         /// <param name="baseValue">The base value of the stat.</param>
         /// <param name="currentValue">The current value of the stat.</param>
         /// <returns>The calculated value of the multiplicative modifiers.</returns>
         public override float CalculateModifiersValue(float baseValue, float currentValue)
         {
            float calculatedValue = currentValue;

            foreach (var t in Modifiers)
               calculatedValue += calculatedValue * t;

            return calculatedValue - currentValue;
         }
      }
   }
}
}