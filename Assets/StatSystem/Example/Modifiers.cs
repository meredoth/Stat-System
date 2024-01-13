namespace StatSystem.Example
{
public static class Modifiers
{
   public static readonly ModifierType BaseAbsolute = Stat.AddNewModifier(400, () => new ModifierOperationsBaseAbsoluteReduction());
}
}