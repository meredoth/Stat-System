namespace StatSystem
{
/// <summary>Defines the operations that can be performed on a collection of modifiers of the same type.</summary>
public interface IModifiersOperations
{
   /// <summary>Adds a modifier to the collection.</summary>
   /// <param name="modifier">The modifier to add.</param>
   void AddModifier(Modifier modifier);
   
   /// <summary>Attempts to remove a specific modifier from the collection.</summary>
   /// <param name="modifier">The modifier to remove.</param>
   /// <returns><c>true</c> if the modifier was successfully removed; otherwise, <c>false</c>.</returns>
   bool TryRemoveModifier(Modifier modifier);
   
   /// <summary>Retrieves all modifiers in the collection.</summary>
   /// <returns>A read only collection of all the modifiers.</returns>
   ModifiersCollection GetAllModifiers();
   
   /// <summary>Calculates the total value of the modifiers applied to a base value.</summary>
   /// <param name="baseValue">The base value of the stat before any modifiers are applied.</param>
   /// <param name="currentValue">The current value of the stat after previous modifiers have been applied.</param>
   /// <returns>The calculated value of the stat after all modifiers have been applied.</returns>
   float CalculateModifiersValue(float baseValue, float currentValue);

   /// <summary>Clears all modifiers from the collection.</summary>
   void Clear();
}
}