using System;
using System.Collections.Generic;

namespace StatSystem.ModifierOperations
{
public static class ModifierOperations
{
    public static void ApplyTo(this Modifier modifier,Stat stat)
    {
        if (stat == null)
            throw new ArgumentNullException(nameof(stat));
        
        stat.AddModifier(modifier);
    }

    public static void ApplyTo(this Modifier modifier, IEnumerable<Stat> stats)
    {
        if (stats == null)
            throw new ArgumentNullException(nameof(stats));

        foreach (var stat in stats)
        {
            modifier.ApplyTo(stat);
        }
    }
}
}
