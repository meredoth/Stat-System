using UnityEngine;

namespace StatSystem.Example
{
public class ModifierOperationsBaseAbsolute : ModifierOperationsBase
{
   public ModifierOperationsBaseAbsolute(int capacity) : base(capacity) { }
   public ModifierOperationsBaseAbsolute() { }

   public override float CalculateModifiersValue(float baseValue, float currentValue)
   {
      var biggestModifer = 0f;

      for (var i = 0; i < Modifiers.Count; i++)
         biggestModifer = Mathf.Max(biggestModifer, Modifiers[i]);

      var modifierValue = biggestModifer == 0f ? 0f : baseValue * (1 - biggestModifer) - currentValue;

      return modifierValue;
   }
}
}