using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace StatSystem
{
[SuppressMessage("NDepend", "ND1903:StructuresShouldBeImmutable", Justification="const is immutable, false positive")]
public readonly struct Modifier
{
   private const float EPSILON = (Stat.MAXIMUM_ROUND_DIGITS + 1) ^ 10;
   public ModifierType Type { get; }
   public object Source { get; }

   private readonly float _value;

   public Modifier(float value, ModifierType modifierType, object source = null)
   {
      _value = value;
      Type = modifierType;
      Source = source;
   }

   public override bool Equals(Object obj) => obj is Modifier modifier && this == modifier;

   public override int GetHashCode() => Tuple.Create(Type, _value).GetHashCode();

   public static bool operator ==(Modifier first, Modifier second) => first.Type == second.Type && Math.Abs(first._value - second._value) < EPSILON;
   
   public static bool operator !=(Modifier first, Modifier second) => !(first == second);
   
   public override string ToString() => $"Value:{_value.ToString(CultureInfo.InvariantCulture)} Type:{Type}";

   public static implicit operator float(Modifier modifier) => modifier._value;
}
}
