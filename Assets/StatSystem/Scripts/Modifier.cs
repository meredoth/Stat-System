using System;
using System.Globalization;

namespace StatSystem
{
/// <summary>Represents a modifier that can be applied to a stat, with a value, type, and an optional source.</summary>
public readonly struct Modifier : IEquatable<Modifier>
{
   private readonly ModifierType? _type;

   /// <summary>Gets the type of the modifier. Defaults to <see cref="ModifierType.Flat"/> if not set.</summary>
   public ModifierType Type => _type ?? ModifierType.Flat;

   /// <summary>Gets the source of the modifier, which is the object that caused the modifier to be applied.</summary>
   public object Source { get; }

   /// <summary>Gets the value of the modifier.</summary>
   public float Value { get; }

   /// <summary>Initializes a new instance of the <see cref="Modifier"/> struct with the specified value, type, and source.</summary>
   /// <param name="value">The value of the modifier.</param>
   /// <param name="modifierType">The type of the modifier.</param>
   /// <param name="source">The source object that caused the modifier, or <c>null</c> if not applicable.</param>
   public Modifier(float value, ModifierType modifierType, object source = null)
   {
      Value = value;
      _type = modifierType;
      Source = source;
   }

   /// <summary>Determines whether the current modifier is equal to another modifier.</summary>
   /// <param name="other">The other modifier to compare with this modifier.</param>
   /// <returns><c>true</c> if the modifiers are equal; otherwise, <c>false</c>.</returns>
   public bool Equals(Modifier other) =>
      Type == other.Type && Equals(Source, other.Source) && Value.Equals(other.Value);

   /// <summary>Determines whether the current modifier object is equal to another modifier object.</summary>
   /// <param name="obj">The modifier object to compare with the current modifier object.</param>
   /// <returns><c>true</c> if the modifier objects are equal; otherwise, <c>false</c>.</returns>
   public override bool Equals(object obj) => obj is Modifier modifier && Equals(modifier);

   /// <summary>Returns a hash code for the current modifier.</summary>
   /// <returns>A hash code for the current modifier.</returns>
   public override int GetHashCode() => HashCode.Combine(Type, Source, Value);

   /// <summary>Determines whether two modifiers are equal.</summary>
   /// <param name="left">The first modifier to compare.</param>
   /// <param name="right">The second modifier to compare.</param>
   /// <returns><c>true</c> if the modifiers are equal; otherwise, <c>false</c>.</returns>
   public static bool operator ==(Modifier left, Modifier right) => left.Equals(right);

   /// <summary>Determines whether two modifiers are not equal.</summary>
   /// <param name="left">The first modifier to compare.</param>
   /// <param name="right">The second modifier to compare.</param>
   /// <returns><c>true</c> if the modifiers are not equal; otherwise, <c>false</c>.</returns>
   public static bool operator !=(Modifier left, Modifier right) => !left.Equals(right);

   /// <summary>Returns a string that represents the current modifier. Logs its value, type and source object.</summary>
   /// <returns>A string that represents the current modifier.</returns>
   public override string ToString() => 
      $"Value:{Value.ToString(CultureInfo.InvariantCulture)} Type:{Type} Source object: {Source ?? "None"}";

   /// <summary>Converts a <see cref="Modifier"/> to a <see cref="float"/> representing its value.</summary>
   /// <param name="modifier">The modifier to convert.</param>
   public static implicit operator float(Modifier modifier) => modifier.Value;

   /// <summary>Deconstructs the modifier into its component parts.</summary>
   /// <param name="value">The value of the modifier.</param>
   /// <param name="type">The type of the modifier.</param>
   /// <param name="source">The source object of the modifier.</param>
   public void Deconstruct(out float value, out ModifierType type, out object source)
   {
      value = Value;
      type = Type;
      source = Source;
   }
}
}
