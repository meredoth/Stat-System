using UnityEngine;

namespace StatSystem.Example
{
public sealed class ModifierOperationsBaseAbsoluteReduction : ModifierOperationsBase
{
   public ModifierOperationsBaseAbsoluteReduction(int capacity) : base(capacity) { }
   public ModifierOperationsBaseAbsoluteReduction() { }

   public override float CalculateModifiersValue(float baseValue, float currentValue)
   {
      var biggestModifier = 0f;

      foreach (var t in Modifiers)
         biggestModifier = Mathf.Max(biggestModifier, t);

      var modifierValue = biggestModifier == 0f ? 0f : baseValue * (1 - biggestModifier) - currentValue;

      return modifierValue;
   }
}
}