using System;
using System.Collections.Generic;

namespace StatSystem
{
public static class ModifierOperationsFactory
{
   private static Dictionary<ModifierType, IModifiersOperations> _modifierOperations;

   public static Dictionary<ModifierType, Func<IModifiersOperations>> GetModifierOperations(int capacity)
   {
      // defines order of calculation
      return new Dictionary<ModifierType, Func<IModifiersOperations>>
      {
         { ModifierType.Flat , () => new FlatModifiersOperations(capacity)},
         { ModifierType.Additive , () => new AdditiveModifiersOperations(capacity)},
         { ModifierType.Multiplicative , () => new MultiplicativeModifiersOperations(capacity)}
      };
   }
}
}