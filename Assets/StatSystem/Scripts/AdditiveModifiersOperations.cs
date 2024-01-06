namespace StatSystem
{
public sealed class AdditiveModifiersOperations : ModifiersOperations
{
   public AdditiveModifiersOperations(int capacity) : base(capacity) { }

   public override float CalculateModifiersValue(float baseValue, float currentValue)
   {
      float additiveModifiersSum = 0f;
      
      for (var i = 0; i < Modifiers.Count; i++)
         additiveModifiersSum += Modifiers[i];

      return baseValue * additiveModifiersSum;
   }
}
}
