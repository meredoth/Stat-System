using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace StatSystem
{
[SuppressMessage("NDepend", "ND1903:StructuresShouldBeImmutable", Justification="const is immutable, false positive")]
public readonly struct Modifier : IEquatable<Modifier>
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

   public bool Equals(Modifier other) => 
      _type == other._type && Equals(Source, other.Source) && Value.Equals(other.Value);
   
   public override bool Equals(object obj) => obj is Modifier modifier && Equals(modifier);

   public override int GetHashCode() => HashCode.Combine(_type, Source, Value);

   public static bool operator ==(Modifier first, Modifier second) => 
      first.Type == second.Type && 
      Math.Abs(first.Value - second.Value) < float.Epsilon && 
      ReferenceEquals(first.Source, second.Source);
   
   public static bool operator !=(Modifier first, Modifier second) => !(first == second);
   
   public override string ToString() => $"Value:{Value.ToString(CultureInfo.InvariantCulture)} Type:{Type} Source object: {Source ?? "None"}";

   public static implicit operator float(Modifier modifier) => modifier.Value;
}
}
