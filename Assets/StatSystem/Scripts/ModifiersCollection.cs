using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace StatSystem
{
public class ModifiersCollection : ReadOnlyCollection<Modifier>
{
   internal ModifiersCollection(List<Modifier> list) : base(list) { }

   internal IList<Modifier> GetModifiersList() => Items;
}
}
