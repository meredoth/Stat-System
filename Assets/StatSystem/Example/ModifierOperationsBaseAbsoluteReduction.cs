using UnityEngine;

namespace StatSystem.Example
{
public class ModifierOperationsBaseAbsoluteReduction : ModifierOperationsBase
{
   public ModifierOperationsBaseAbsoluteReduction(int capacity) : base(capacity) { }
   public ModifierOperationsBaseAbsoluteReduction() { }

   public override float CalculateModifiersValue(float baseValue, float currentValue)
   {
      var biggestModifier = 0f;

      for (var i = 0; i < Modifiers.Count; i++)
         biggestModifier = Mathf.Max(biggestModifier, Modifiers[i]);

      var modifierValue = biggestModifier == 0f ? 0f : baseValue * (1 - biggestModifier) - currentValue;

      return modifierValue;
   }
}
}