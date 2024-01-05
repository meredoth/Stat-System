using System.Globalization;

namespace StatSystem
{
public readonly struct Modifier
{
   public ModifierType Type { get; }
   public object Source { get; }

   private readonly float _value;

   public Modifier(float value, ModifierType modifierType, object source = null)
   {
      _value = value;
      Type = modifierType;
      Source = source;
   }
   
   public override string ToString() => $"Value:{_value.ToString(CultureInfo.InvariantCulture)} Type:{Type}";

   public static implicit operator float(Modifier modifier) => modifier._value;
}
}
