namespace StatSystem.ModifierOperations
{
public static class ModifierOperations
{
    public static void ApplyTo(this Modifier modifier, Stat stat) => stat.AddModifier(modifier);
}
}
