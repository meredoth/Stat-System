using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using UnityEngine;
// ReSharper disable IntroduceOptionalParameters.Global

[assembly:InternalsVisibleTo("Tests")]
namespace StatSystem
{
/// <summary>Represents a statistical value that can be modified by various types of modifiers.</summary>
[Serializable]
public sealed partial class Stat
{
   private const int DEFAULT_LIST_CAPACITY = 4;
   private const int DEFAULT_DIGIT_ACCURACY = 2;
   private const int MAXIMUM_ROUND_DIGITS = 6;

   [SerializeField] private float baseValue;

   [SuppressMessage("NDepend", "ND1902:AvoidStaticFieldsWithAMutableFieldType", Justification="Cannot mutate after Instantiation of Stat, will throw.")]
   [SuppressMessage("NDepend", "ND1901:AvoidNonReadOnlyStaticFields", Justification="Not readonly so that it can be called from Init() for reset.")]
   private static ModifierOperationsCollection _ModifierOperationsCollection = new();

   private readonly int _digitAccuracy;
   private readonly SortedList<ModifierType, IModifiersOperations> _modifiersOperations = new();

   private float _currentValue;
   private bool _isDirty;

   /// <summary>Initializes the static _ModifierOperationsCollection. Used for Unity's domain reload feature.</summary>
   [SuppressMessage("NDepend", "ND1701:PotentiallyDeadMethods", Justification="Needed for Unity's disable domain reload feature.")]
   [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
   internal static void Init() =>  _ModifierOperationsCollection = new ModifierOperationsCollection();
   
   /// <summary>
   /// Initializes a new instance of the <see cref="Stat"/> class with a base value, digit accuracy,
   /// and expected maximum capacity for each modifier type, to avoid extra array allocations from the list.
   /// </summary>
   /// <param name="baseValue">The base value of the stat.</param>
   /// <param name="digitAccuracy">The number of digits to round the calculated value to.</param>
   /// <param name="modsMaxCapacity">The maximum number of modifiers each modifier type is expected to hold.</param>
   public Stat(float baseValue, int digitAccuracy, int modsMaxCapacity)
   {
      this.baseValue = baseValue;
      _currentValue = baseValue;
      _digitAccuracy = digitAccuracy;

      var modifierOperations = _ModifierOperationsCollection.GetModifierOperations(modsMaxCapacity);
      
      foreach (var operationType in modifierOperations.Keys)
         _modifiersOperations[operationType] = modifierOperations[operationType]();
   }
   
   /// <summary>Initializes a new instance of the <see cref="Stat"/> class with a base value and default settings.</summary>
   /// <param name="baseValue">The base value of the stat.</param>
   public Stat(float baseValue) : this(baseValue, DEFAULT_DIGIT_ACCURACY, DEFAULT_LIST_CAPACITY) { }
   
   /// <summary>Initializes a new instance of the <see cref="Stat"/> class with a base value and specified digit accuracy.</summary>
   /// <param name="baseValue">The base value of the stat.</param>
   /// <param name="digitAccuracy">The number of digits to round the calculated value to.</param>
   public Stat(float baseValue, int digitAccuracy) : this(baseValue, digitAccuracy, DEFAULT_LIST_CAPACITY) { }

   /// <summary>Gets a value indicating whether a new modifier type can be added.</summary>
   public static bool CanAddNewModifierType => !_ModifierOperationsCollection.HasCollectionBeenReturned;
   
   /// <summary>Gets or sets the base value of the stat.</summary>
   public float BaseValue
   {
      get => baseValue;
      set
      {
         baseValue = value;
         _currentValue = CalculateModifiedValue(_digitAccuracy);
         OnValueChanged();
      }
   }

   /// <summary>Gets the current value of the stat, taking into account all applied modifiers.</summary>
   public float Value
   {
      get
      {
         if (IsDirty)
         {
            _currentValue = CalculateModifiedValue(_digitAccuracy);
            OnValueChanged();
         }

         return _currentValue;
      }
   }

   private bool IsDirty
   {
      get => _isDirty;
      set
      {
         _isDirty = value;
         if (_isDirty)
            OnModifiersChanged();
      }
   }

   /// <summary>Occurs when the stat's current or base value changes.</summary>
   public event Action ValueChanged;
   
   /// <summary>Occurs when the stat's modifiers change.</summary>
   public event Action ModifiersChanged;

   /// <summary>Adds a new modifier type with a specified order and operation.</summary>
   /// <param name="order">The calculation order of the modifier type.</param>
   /// <param name="modifierOperationsDelegate">The delegate that defines the operations for the modifier type.</param>
   /// <returns>The newly created <see cref="ModifierType"/>.</returns>
   /// <exception cref="InvalidOperationException">Thrown if attempting to add a modifier type after stat initialization.</exception>
   public static ModifierType NewModifierType(int order, Func<IModifiersOperations> modifierOperationsDelegate)
   {
      try
      {
         return _ModifierOperationsCollection.AddModifierOperation(order, modifierOperationsDelegate);
      }
      catch
      {
         throw new InvalidOperationException("Add any modifier operations before any initialization of the Stat class!");
      }
   }
   
   /// <summary>Adds a modifier to the stat.</summary>
   /// <param name="modifier">The modifier to add.</param>
   public void AddModifier(Modifier modifier)
   {
      IsDirty = true;
      _modifiersOperations[modifier.Type].AddModifier(modifier);
   }

   /// <summary>Adds multiple modifiers to the stat.</summary>
   /// <param name="modifiers">A read-only span of modifiers to add.</param>
   public void AddModifiers(ReadOnlySpan<Modifier> modifiers)
   {
      IsDirty = true;
      
      foreach (var modifier in modifiers)
         _modifiersOperations[modifier.Type].AddModifier(modifier);
   }
   
   /// <summary>Adds multiple modifiers to the stat.</summary>
   /// <param name="modifiers">A list of modifiers to add.</param>
   public void AddModifiers(IEnumerable<Modifier> modifiers)
   {
      IsDirty = true;
      
      foreach (var modifier in modifiers)
         _modifiersOperations[modifier.Type].AddModifier(modifier);
   }

   /// <summary>Gets all modifiers currently applied to the stat.</summary>
   /// <returns>A read-only list of all modifiers.</returns>
   public ModifiersList GetModifiers()
   {
      List<Modifier> modifiersList = new();

      foreach (var modifiersOperation in _modifiersOperations.Values)
         modifiersList.AddRange(modifiersOperation.GetAllModifiers());

      return new ModifiersList(modifiersList.AsReadOnly());
   }

   /// <summary>Gets all modifiers of a specific type currently applied to the stat.</summary>
   /// <param name="modifierType">The type of modifier to retrieve.</param>
   /// <returns>A read-only list of modifiers of the specified type.</returns>
   /// <exception cref="ArgumentOutOfRangeException">Thrown if the specified modifier type does not exist.</exception>
   public ModifiersList GetModifiers(ModifierType modifierType)
   {
      if(!_modifiersOperations.TryGetValue(modifierType, out _))
         throw new ArgumentOutOfRangeException(nameof(modifierType), $"ModifierType {modifierType} does NOT exist!");
      
      return new ModifiersList(_modifiersOperations[modifierType].GetAllModifiers().AsReadOnly());
   }

   /// <summary>Tries to remove a modifier from the stat.</summary>
   /// <param name="modifier">The modifier to remove.</param>
   /// <returns><c>true</c> if the modifier was removed; otherwise, <c>false</c>.</returns>
   public bool TryRemoveModifier(Modifier modifier)
   {
      var isModifierRemoved = false;

      if (_modifiersOperations[modifier.Type].TryRemoveModifier(modifier))
      {
         IsDirty = true;
         isModifierRemoved = true;
      }

      return isModifierRemoved;
   }

   /// <summary>Tries to remove all modifiers from a specific source.</summary>
   /// <param name="source">The source of the modifiers to remove.</param>
   /// <returns><c>true</c> if any modifiers were removed; otherwise, <c>false</c>.</returns>
   public bool TryRemoveAllModifiersOf(object source)
   {
      bool isModifierRemoved = false;

      for (int i = 0; i < _modifiersOperations.Count; i++)
      {
         if (TryRemoveAllModifiersOfSourceFromList(source,
                _modifiersOperations.Values[i].GetAllModifiers()))
         {
            isModifierRemoved = true;
            IsDirty = true;
         }
      }

      return isModifierRemoved;

      // local method, static guarantees that it won't be allocated to the heap
      // (It is never converted to delegate, no variable captures)
      static bool TryRemoveAllModifiersOfSourceFromList(object source, List<Modifier> listOfModifiers)
      {
         bool modifierHasBeenRemoved = false;

         for (var i = listOfModifiers.Count - 1; i >= 0; i--)
         {
            if (ReferenceEquals(source, listOfModifiers[i].Source))
            {
               listOfModifiers.RemoveAt(i);
               modifierHasBeenRemoved = true;
            }
         }

         return modifierHasBeenRemoved;
      }
   }

   /// <summary>Determines whether the stat contains a specific modifier.</summary>
   /// <param name="modifier">The modifier to check for.</param>
   /// <returns><c>true</c> if the stat contains the modifier; otherwise, <c>false</c>.</returns>
   public bool ContainsModifier(Modifier modifier) => 
      _modifiersOperations.ContainsKey(modifier.Type) && 
      _modifiersOperations[modifier.Type].GetAllModifiers().Contains(modifier);

   private float CalculateModifiedValue(int digitAccuracy)
   {
      digitAccuracy = Math.Clamp(digitAccuracy, 0, MAXIMUM_ROUND_DIGITS);

      float finalValue = baseValue;

      for (int i = 0; i < _modifiersOperations.Count; i++)
         finalValue += _modifiersOperations.Values[i].CalculateModifiersValue(baseValue, finalValue);

      IsDirty = false;

      return (float)Math.Round(finalValue, digitAccuracy);
   }

   private void OnValueChanged() => ValueChanged?.Invoke();
   private void OnModifiersChanged() => ModifiersChanged?.Invoke();
}
}