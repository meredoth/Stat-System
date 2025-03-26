using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;

namespace StatSystem
{
public class ModifiersList : ReadOnlyCollection<Modifier>
{
   internal ModifiersList([NotNull] IList<Modifier> list) : base(list) { }
}
}
