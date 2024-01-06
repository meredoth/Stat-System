namespace StatSystem
{
public sealed class FlatModifiersOperations : ModifiersOperations
{
   public FlatModifiersOperations(int capacity) : base(capacity) { }

   public override float CalculateModifiersValue(float baseValue, float currentValue)
   {
      float flatModifiersSum = 0f;
      
      for (var i = 0; i < Modifiers.Count; i++)
         flatModifiersSum += Modifiers[i];

      return flatModifiersSum;
   }
}
}