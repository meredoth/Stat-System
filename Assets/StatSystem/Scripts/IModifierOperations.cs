using System.Collections.Generic;

namespace StatSystem
{
public interface IModifiersOperations
{
   void AddModifier(Modifier modifier);
   bool TryRemoveModifier(Modifier modifier);
   List<Modifier> GetAllModifiers();
   float CalculateModifiersValue(float baseValue, float currentValue);
}
}