using System;
using System.Collections.Generic;

namespace StatSystem
{
public sealed class Stat
{
   private const int DEFAULT_LIST_CAPACITY = 4;
   private const int DEFAULT_DIGIT_ACCURACY = 2;

   public float BaseValue {
      get => _baseValue;
      set
      {
         _baseValue = value;
         _currentValue = CalculateModifiedValue(_digitAccuracy);
         OnValueChanged();
      }
   }

   public IReadOnlyList<Modifier> Modifiers
   {
      get
      {
         List<Modifier> modifiersOperationsList = new();
         foreach (var modifierType in _modifiersOperations.Keys)
         {
            modifiersOperationsList.AddRange(_modifiersOperations[modifierType].GetAllModifiers());
         }

         return modifiersOperationsList;
      }
   }

   public event Action ValueChanged;
   public event Action ModifiersChanged;
   
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
   
   private const int MAXIMUM_ROUND_DIGITS = 8;

   private readonly Dictionary<ModifierType, IModifiersOperations> _modifiersOperations = new();
   
   private float _baseValue;
   private float _currentValue;
   private bool _isDirty;
   private readonly int _digitAccuracy;
   
   public Stat(float baseValue, int digitAccuracy, int modsMaxCapacity)
   {
      _baseValue = baseValue;
      _currentValue = baseValue;
      _digitAccuracy = digitAccuracy;

      InitializeModifierOperations(modsMaxCapacity);
   }
   public Stat(float baseValue) : this(baseValue, DEFAULT_DIGIT_ACCURACY, DEFAULT_LIST_CAPACITY) { }
   public Stat(float baseValue, int digitAccuracy) : this(baseValue, digitAccuracy, DEFAULT_LIST_CAPACITY) { }

   public void AddModifier(Modifier modifier)
   {
      IsDirty = true;
      _modifiersOperations[modifier.Type].AddModifier(modifier);
   }

   public bool TryRemoveModifier(Modifier modifier) => IsDirty = _modifiersOperations[modifier.Type].TryRemoveModifier(modifier);

   public bool TryRemoveAllModifiersOf(object source)
   {
      bool isRemoved = false;
      
      foreach (var modifierType in _modifiersOperations)
         isRemoved = isRemoved || TryRemoveAllModifiersOfSourceFromList(source, modifierType.Value.GetAllModifiers());

      return isRemoved;
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

   private void InitializeModifierOperations(int capacity)
   {
      var modifierOperations = ModifierOperationsFactory.GetModifierOperations(capacity);
      
      foreach (var operationType in modifierOperations.Keys)
         _modifiersOperations.Add(operationType, modifierOperations[operationType]());
   }
   
   private void OnValueChanged() => ValueChanged?.Invoke();
   private void OnModifiersChanged() => ModifiersChanged?.Invoke();
   
   private float CalculateModifiedValue(int digitAccuracy)
   {
      digitAccuracy = Math.Clamp(digitAccuracy, 0, MAXIMUM_ROUND_DIGITS);

      float finalValue = _baseValue;

      foreach (var modifiersOperation in _modifiersOperations.Values)
      {
         finalValue += modifiersOperation.CalculateModifiersValue(_baseValue, finalValue);
      }

      IsDirty = false;

      return (float)Math.Round(finalValue, digitAccuracy);
   }
}
}
