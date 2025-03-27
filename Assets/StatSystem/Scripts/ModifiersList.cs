using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace StatSystem
{
public class ModifiersList : ReadOnlyCollection<Modifier>
{
   internal ModifiersList(IList<Modifier> list) : base(list) { }
}
}
