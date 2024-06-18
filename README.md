# Unity Stat System

[![License](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)
[![Made For Unity](https://img.shields.io/badge/Made%20for-Unity-blue)](https://unity3d.com)
[![Maintenance](https://img.shields.io/badge/Maintained%3F-yes-brightgreen.svg)](https://github.com/meredoth/Unity-Fluent-Debug/graphs/commit-activity)

This is a simple Stat system for the Unity game engine. The user can create stats that each stat can have any number of different types of modifiers.

These modifiers are calculated not in the order that they are added, but in a predetermined order. The system provides an easy way to add new types of modifiers, not only for having modifiers that are calculated in a desired order, but also to define the desired way these modifiers affect the stat they are added.

The three core types of modifiers (flat, additive and multiplicative) are already implemented.

See the Example folder, for a simple how to use example.

For the initial conception and the refactoring that led to the final code you can check my posts on my blog:

[A Stat System for Unity part 1](https://giannisakritidis.com/blog/Stat-System-Part1/)

[A Stat System for Unity part 2](https://giannisakritidis.com/blog/Stat-System-Part2/)

## Simple Usage

Create a Stat with:

```csharp
strength = new Stat(100);
```

Then you can create a new modifier with:

```csharp
StrengthModFlat = new Modifier(20f, ModifierType.Flat);
```

Adding a modifier:

```csharp
strength.AddModifier(StrengthModFlat);
```

Removing a modifier if it exists in the stat:

```csharp
strength.TryRemoveModifier(StrengthModFlat);
```

A modifier can be created with a reference to an object, this helps for removing all the modifiers that are part of a single object. For example a sword in a game may be represented with a class that has a number of modifiers that are added. We can then easily remove all those modifiers with the ```TryRemoveAllModifiersOf``` method:

```csharp
public class AmazingSword
{
    public readonly Modifier StrengthModMulti;
    public readonly Modifier StrengthModFlat;

    public AmazingSword()
    {
        StrengthModMulti = new Modifier(0.2f, ModifierType.Multiplicative, this);
        StrengthModFlat = new Modifier(20f, ModifierType.Flat, this);
    }
}
```

```csharp
private readonly AmazingSword _amazingSword = new();

strength.AddModifier(_amazingSword.StrengthModFlat);
strength.AddModifier(_amazingSword.StrengthModMulti);

// Removes both StrengthModMulti and StrengthModFlat modifiers of the _amazingSword instance
strength.TryRemoveAllModifiersOf(_amazingSword); 
```

### Example

Let's suppose that we have the following modifiers:

```csharp
DexterityModFlat = new Modifier(10f, ModifierType.Flat);
DexterityModAdditive = new Modifier(0.2f, ModifierType.Additive);
DexterityModMulti = new Modifier(0.1f, ModifierType.Multiplicative);
```

and the dexterity stat:

```csharp
dexterity = new Stat(50);
```

The order of addition of the modifiers to the stat doesn't affect the final calculation. Flat modifiers will always be calculated before the additive modifiers, that will always be calculated before the multiplicative modifiers. So the following two are equivalent and will always give the same result:

```csharp
dexterity.AddModifier(DexterityModFlat);
dexterity.AddModifier(DexterityModAdditive);
dexterity.AddModifier(DexterityModMulti);
```

```csharp
dexterity.AddModifier(DexterityModMulti);
dexterity.AddModifier(DexterityModAdditive);
dexterity.AddModifier(DexterityModFlat);
```

The result will be (50(dexterity base value) + 10 (flat modifier) + 0.2 * 50 (additive modifier)) * 1.1(multiplicative modifier) = 77.

The constant order of calculations of the type of modifiers, has the purpose of providing the same result that is independent of the order the player equipped his items or cast his spells on a player character or an NPC.

## Advanced Usage

The Stat system, allows the creation of new types of modifiers. The creation is simple with the only requirement that any new types of modifiers should be created **BEFORE** any instantiation of the Stat class.

To create a new type of modifier, inherit from the ```ModifierOperationsBase``` class and override the ```CalculateModifiersValue(float baseValue, float currentValue)``` method.

Then register the new type of modifier to the system by calling the ```Stat.NewModifierType``` static method, that accepts as a first parameter the order of calculation for the new type (Orders of calculation of the already defined types is 100 for Flat, 200 for Additive and 300 for Multiplicative) and as a second parameter a delegate to our new class.

### Example of adding new types of Stats

Let's suppose our new type of modifier will be called ```Base absolute reduction```. We want this modifier to suppress any existing modifier and make the stat that it is added to, reduced by a percentage of its starting base value. We also want only the biggest reduction of these modifiers to be applied to the stat, the others can be ignored.

For example, if we had a base strength stat with a starting value of 100, that had been modified with different modifiers to 200, after the application of the base absolute reduction type with a value of 0.2 (20%) the result should be 80. If we add a base absolute reduction after that with a value of 0.1 (10%) the result would still be 80 as only the biggest of these modifiers apply.

Even if we add a flat modifier of 20 after that, the value will continue to be 80. The moment we remove this base absolute reduction modifier though, all the suppressed modifiers will be applied normally, in our example the value of the strength stat after the removal will be 90 (the base absolute reduction of 10%) and after the removal of the 10% base absolute reduction, the current strength value will be calculated from start with all the previous modifiers that had made the final value 200 plus the 20 flat modifier that was added while the base absolute reductions were applied.

First, we create the ```ModifierOperationsBaseAbsoluteReduction``` class that inherits from the ```ModifierOperationsBase``` class, and we implement the ```CalculateModifiersValue``` method that is appropriate for our new type:

```csharp
public class ModifierOperationsBaseAbsoluteReduction : ModifierOperationsBase
{
   public ModifierOperationsBaseAbsoluteReduction(int capacity) : base(capacity) { }
   public ModifierOperationsBaseAbsoluteReduction() { }

   public override float CalculateModifiersValue(float baseValue, float currentValue)
   {
      var biggestModifier = 0f;

      for (var i = 0; i < Modifiers.Count; i++)
         biggestModifier = Mathf.Max(biggestModifier, Modifiers[i]);

      var modifierValue = biggestModifier == 0f ? 0f : baseValue * (1 - biggestModifier) - currentValue;

      return modifierValue;
   }
}
```

Here we find the biggest of the base absolute reduction modifiers, we multiply it with the base stat value, and we return this minus the current stat value. This effectively makes the calculation: ```currentValue = baseValue * (1 - biggestModifier) - currentValue```

Then, we want this modifier to be calculated *after* any other modifiers have been calculated, so that it can suppress them. We do that by calling:

```csharp
var  BaseAbsoluteReduction = Stat.NewModifierType(400, () => new ModifierOperationsBaseAbsoluteReduction());
```

400 is the order of calculation, which is higher than any of the existing modifiers.

Now, we can create these types of modifiers in different parts of our game, for example in a cursed item or as a spell effect:

```csharp
Modifier strengthCurse = new Modifier(0.2f, BaseAbsoluteReduction);
```

and add them whenever we want, for example whenever the cursed item is equipped, or a spell has been cast, to the appropriate stat:

```csharp
strength = new Stat(100);
strength.AddModifier(strengthCurse);
```

## Overloads of the Stat class constructor

The Stat class, has two optional parameters. It can be called like this:

```csharp
strength = new Stat(100, 2); 
```

To provide the digit accuracy desired for the float calculations, or it can be called like this:

```csharp
strength = new Stat(100, 2, 10);
```

Here, the third parameter, is the initial size of the maximum number of modifiers of each type that is expected this stat to have at any one point. This parameter helps with avoiding the garbage collector. As each type of modifier has a List with the applied modifiers for the stat, the third parameter effectively initializes those lists with a default capacity.

The initial default capacity of each list is 4. If at any point in time, a list resize is required, a warning will be showed in the Unity Console in the editor. This won't affect the functionality of the Stat system in any way, it is just a convenient way to avoid the garbage collector during the addition of new modifiers to any of our stats in the game.

## Getting the Modifiers of Each Stat

The modifiers that each Stat has, at any point in time can be seen by calling the ```GetModifiers()``` and ```GetModifiers(ModifierType modifierType)``` methods. Both of these methods return a ```IReadOnlyList<Modifier>``` with the current modifiers applied to the Stat.

## License

This project is licensed under the [Apache-2.0](LICENSE.md)
License - see the [LICENSE.md](LICENSE.md) file for
details.
