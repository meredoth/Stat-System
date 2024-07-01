using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace StatSystem
{
[SuppressMessage("NDepend", "ND1903:StructuresShouldBeImmutable", Justification="const is immutable, false positive")]
public readonly struct Modifier
{
   private readonly ModifierType? _type;
   public ModifierType Type => _type ?? ModifierType.Flat;
   public object Source { get; }
   public float Value { get; }

   public Modifier(float value, ModifierType modifierType, object source = null)
   {
      Value = value;
      _type = modifierType;
      Source = source;
   }

   public override bool Equals(object obj) => obj is Modifier modifier && this == modifier;

   public override int GetHashCode() => Tuple.Create(Type, Value).GetHashCode();

   public static bool operator ==(Modifier first, Modifier second) => first.Type == second.Type && 
         Math.Abs(first.Value - second.Value) < float.Epsilon && 
         ReferenceEquals(first.Source, second.Source);
   
   public static bool operator !=(Modifier first, Modifier second) => !(first == second);
   
   public override string ToString() => $"Value:{Value.ToString(CultureInfo.InvariantCulture)} Type:{Type}";

   public static implicit operator float(Modifier modifier) => modifier.Value;
}
}
