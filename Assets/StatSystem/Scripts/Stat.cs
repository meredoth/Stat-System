using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace StatSystem
{
[Serializable]
public sealed class Stat
{
   private const int DEFAULT_LIST_CAPACITY = 4;
   private const int DEFAULT_DIGIT_ACCURACY = 2;
   internal const int MAXIMUM_ROUND_DIGITS = 8;

   [SerializeField] private float baseValue;
   
   private static readonly ModifierOperationsCollection _ModifierOperationsCollection = new();
   
   [SuppressMessage("NDepend", "ND1901:AvoidNonReadOnlyStaticFields", Justification="Needed for calling InitStatics")]
   [SuppressMessage("NDepend", "ND1211:DontAssignStaticFieldsFromInstanceMethods", Justification="Needed for instance checking")]
   private static bool _InstanceHasBeenCreated;

   private readonly int _digitAccuracy;
   private readonly List<Modifier> _modifiersList = new();
   private readonly SortedList<ModifierType, IModifiersOperations> _modifiersOperations = new();
   
   private float _currentValue;
   private bool _isDirty;

   [SuppressMessage("NDepend", "ND1701:PotentiallyDeadMethods", Justification="Needed for Unity's reset of static fields with disable domain reload feature")]
   [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
   private static void InitStatics() => _InstanceHasBeenCreated = false;

   public Stat(float baseValue, int digitAccuracy, int modsMaxCapacity)
   {
      this.baseValue = baseValue;
      _currentValue = baseValue;
      _digitAccuracy = digitAccuracy;
      _InstanceHasBeenCreated = true;
      
      InitializeModifierOperations(modsMaxCapacity);
   }

   public Stat(float baseValue) : this(baseValue, DEFAULT_DIGIT_ACCURACY, DEFAULT_LIST_CAPACITY) { }
   public Stat(float baseValue, int digitAccuracy) : this(baseValue, digitAccuracy, DEFAULT_LIST_CAPACITY) { }

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

   public float Value
   {
      get
      {
         if(IsDirty)
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
         if(_isDirty)
            OnModifiersChanged();
      }
   }

   public event Action ValueChanged;
   public event Action ModifiersChanged;

   public void AddModifier(Modifier modifier)
   {
      IsDirty = true;
      _modifiersOperations[modifier.Type].AddModifier(modifier);
   }

   public static ModifierType AddNewModifier(int order, Func<IModifiersOperations> modifierOperationsDelegate)
   {
      if (_InstanceHasBeenCreated)
         throw new InvalidOperationException("Add any modifier operations before any initialization of the Stat class!");

      return _ModifierOperationsCollection.AddModifierOperation(order, modifierOperationsDelegate);
   }

   public IReadOnlyList<Modifier> GetModifiers()
   {
      _modifiersList.Clear();
         
      foreach (var modifiersOperation in _modifiersOperations.Values)
         _modifiersList.AddRange(modifiersOperation.GetAllModifiers());

      return _modifiersList.AsReadOnly();
   }
   
   public IReadOnlyList<Modifier> GetModifiers(ModifierType modifierType) 
      => _modifiersOperations[modifierType].GetAllModifiers().AsReadOnly();

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

   public bool TryRemoveAllModifiersOf(object source)
   {
      bool isModifierRemoved = false;

      for (int i = 0; i < _modifiersOperations.Count; i++)
         isModifierRemoved = TryRemoveAllModifiersOfSourceFromList(source, 
            _modifiersOperations.Values[i].GetAllModifiers()) || isModifierRemoved;

      return isModifierRemoved;
   }
   
   private float CalculateModifiedValue(int digitAccuracy)
   {
      digitAccuracy = Math.Clamp(digitAccuracy, 0, MAXIMUM_ROUND_DIGITS);

      float finalValue = baseValue;

      for (int i = 0; i < _modifiersOperations.Count; i++)
         finalValue += _modifiersOperations.Values[i].CalculateModifiersValue(baseValue, finalValue);

      IsDirty = false;

      return (float)Math.Round(finalValue, digitAccuracy);
   }

   private void InitializeModifierOperations(int capacity)
   {
      var modifierOperations = _ModifierOperationsCollection.GetModifierOperations(capacity);
      
      foreach (var operationType in modifierOperations.Keys)
         _modifiersOperations[operationType] = modifierOperations[operationType]();
   }

   private bool TryRemoveAllModifiersOfSourceFromList(object source, List<Modifier> listOfModifiers)
   {
      bool isModifierRemoved = false;

      for (var i = listOfModifiers.Count - 1; i >= 0; i--)
      {
         if (ReferenceEquals(source, listOfModifiers[i].Source))
         {
            listOfModifiers.RemoveAt(i);
            IsDirty = true;
            isModifierRemoved = true;
         }
      }
      
      return isModifierRemoved;
   }

   private void OnValueChanged() => ValueChanged?.Invoke();
   private void OnModifiersChanged() => ModifiersChanged?.Invoke();

   private sealed class ModifierOperationsCollection
   {
      private readonly Dictionary<ModifierType, Func<IModifiersOperations>> _modifierOperationsDict = new();

      internal ModifierType AddModifierOperation(int order, Func<IModifiersOperations> modifierOperationsDelegate)
      {
         var modifierType = (ModifierType)order;

         if (modifierType is ModifierType.Flat or ModifierType.Additive or ModifierType.Multiplicative)
            Debug.LogWarning("modifier operations for types flat, additive and multiplicative cannot be changed! Default operations for these types will be used.");
      
         _modifierOperationsDict[modifierType] = modifierOperationsDelegate;

         return modifierType;
      }

      internal Dictionary<ModifierType, Func<IModifiersOperations>> GetModifierOperations(int capacity)
      {
         _modifierOperationsDict[ModifierType.Flat] = () => new FlatModifierOperations(capacity);
         _modifierOperationsDict[ModifierType.Additive] = () => new AdditiveModifierOperations(capacity);
         _modifierOperationsDict[ModifierType.Multiplicative] = () => new MultiplicativeModifierOperations(capacity);

         return _modifierOperationsDict;
      }

      private sealed class FlatModifierOperations : ModifierOperationsBase
      {
         internal FlatModifierOperations(int capacity) : base(capacity) { }

         public override float CalculateModifiersValue(float baseValue, float currentValue)
         {
            float flatModifiersSum = 0f;
      
            for (var i = 0; i < Modifiers.Count; i++)
               flatModifiersSum += Modifiers[i];

            return flatModifiersSum;
         }
      }

      private sealed class AdditiveModifierOperations : ModifierOperationsBase
      {
         internal AdditiveModifierOperations(int capacity) : base(capacity) { }

         public override float CalculateModifiersValue(float baseValue, float currentValue)
         {
            float additiveModifiersSum = 0f;
      
            for (var i = 0; i < Modifiers.Count; i++)
               additiveModifiersSum += Modifiers[i];

            return baseValue * additiveModifiersSum;
         }
      }

      private sealed class MultiplicativeModifierOperations : ModifierOperationsBase
      {
         internal MultiplicativeModifierOperations(int capacity) : base(capacity) { }

         public override float CalculateModifiersValue(float baseValue, float currentValue)
         {
            float calculatedValue = currentValue;

            for (var i = 0; i < Modifiers.Count; i++)
               calculatedValue += calculatedValue *  Modifiers[i];

            return calculatedValue - currentValue;
         }
      }
   }
}
}