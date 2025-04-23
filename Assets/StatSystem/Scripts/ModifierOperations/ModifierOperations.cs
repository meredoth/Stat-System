using System;
using System.Collections.Generic;

namespace StatSystem.ModifierOperations
{
public static class ModifierOperations
{
    /// <summary>Adds this modifier to a stat.</summary>
    /// <param name="stat">The stat to add this modifier.</param>
    public static void ApplyTo(this Modifier modifier,Stat stat)
    {
        if (stat == null)
            throw new ArgumentNullException(nameof(stat));
        
        stat.AddModifier(modifier);
    }

    /// <summary>Adds this modifier to multiple stats.</summary>
    /// <param name="stats">A collection of stats to add this modifier.</param>
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
