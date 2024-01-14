using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace StatSystem.Tests
{
public class StatTests
{
    private Stat _testStat;
    private Modifier _modifierFlat;
    private Modifier _modifierAdditive;
    private Modifier _modifierMultiplicative;

    [SetUp]
    public void Setup()
    {
        _testStat = new(100);
        _modifierFlat = new Modifier(10, ModifierType.Flat);
        _modifierAdditive = new Modifier(0.1f, ModifierType.Additive);
        _modifierMultiplicative = new Modifier(0.2f, ModifierType.Multiplicative);
    }
    
    [Test]
    public void AddModifier_AddOneModifier_NumberOfStatsModifiersIncreasedByOne()
    {
        Stat testStat = new(10);
        Modifier modifier = new Modifier(10, ModifierType.Flat);
        
        var numberOfModifiersBefore = testStat.GetModifiers().Count;
        testStat.AddModifier(modifier);
        var numberOfModifiersAfter = testStat.GetModifiers().Count;
        
        Assert.AreEqual(numberOfModifiersAfter, numberOfModifiersBefore + 1);
    }

    [TestCase(ModifierType.Flat)]
    [TestCase(ModifierType.Additive)]
    [TestCase(ModifierType.Multiplicative)]
    public void GetModifier_ParameterOfModifierType_ListOfThatTypeOfModifiers(ModifierType modifierType)
    {
        _testStat.AddModifier(_modifierFlat);
        _testStat.AddModifier(_modifierAdditive);
        _testStat.AddModifier(_modifierMultiplicative);
        _testStat.AddModifier(_modifierFlat);
        _testStat.AddModifier(_modifierAdditive);
        _testStat.AddModifier(_modifierMultiplicative);

        var modifiers = _testStat.GetModifiers(modifierType);

        foreach (var modifier in modifiers)
        {
            Assert.AreEqual(modifier.Type, modifierType);
        }
    }

    [TestCase(ModifierType.Flat, 2)]
    [TestCase(ModifierType.Additive, 3)]
    [TestCase(ModifierType.Multiplicative, 4)]
    public void GetModifier_AddMultipleModifiers_ListCountIsNumberOfModifiers(ModifierType modifierType, int count)
    {
        _testStat.AddModifier(_modifierFlat);
        _testStat.AddModifier(_modifierAdditive);
        _testStat.AddModifier(_modifierMultiplicative);
        _testStat.AddModifier(_modifierFlat);
        _testStat.AddModifier(_modifierAdditive);
        _testStat.AddModifier(_modifierMultiplicative);
        _testStat.AddModifier(_modifierAdditive);
        _testStat.AddModifier(_modifierMultiplicative);
        _testStat.AddModifier(_modifierMultiplicative);

        var modifiers = _testStat.GetModifiers(modifierType);
        
        Assert.AreEqual(modifiers.Count, count);
    }

    [Test]
    public void TryRemoveModifier_RemoveOneModifier_ListOfGetModifiersDecreasedExactlyByOne()
    {
        _testStat.AddModifier(_modifierFlat);
        _testStat.AddModifier(_modifierAdditive);
        _testStat.AddModifier(_modifierMultiplicative);
        _testStat.AddModifier(_modifierFlat);
        _testStat.AddModifier(_modifierAdditive);
        _testStat.AddModifier(_modifierMultiplicative);
        _testStat.AddModifier(_modifierAdditive);
        _testStat.AddModifier(_modifierMultiplicative);
        _testStat.AddModifier(_modifierMultiplicative);
        var modifiersCountBefore = _testStat.GetModifiers().Count;

        _testStat.TryRemoveModifier(_modifierFlat);
        var modifiersCountAfter = _testStat.GetModifiers().Count;
        
        Assert.AreEqual(modifiersCountBefore, modifiersCountAfter + 1);
    }

    [Test]
    public void TryRemoveModifier_ModifierExists_ReturnsTrue()
    {
        _testStat.AddModifier(_modifierFlat);

        bool isRemoved = _testStat.TryRemoveModifier(_modifierFlat);
        
        Assert.IsTrue(isRemoved);
    }

    [Test]
    public void TryRemoveModifier_NoModifiersExist_ReturnsFalse()
    {
        bool isRemoved = _testStat.TryRemoveModifier(_modifierFlat);
        Assert.IsFalse(isRemoved);
    }
    
    [Test]
    public void TryRemoveModifier_SpecificModifierDoesNotExist_ReturnsFalse()
    {
        _testStat.AddModifier(_modifierAdditive);
        _testStat.AddModifier(_modifierAdditive);
        
        bool isRemoved = _testStat.TryRemoveModifier(_modifierFlat);
        Assert.IsFalse(isRemoved);
    }

    [Test]
    public void TryRemoveAllModifiersOf_HasSourceModifiersRemoved_ReturnsTrue()
    {
        object obj = new();
        Modifier objFlatModifier = new Modifier(10, ModifierType.Flat, obj);
        Modifier objAdditiveModifier = new Modifier(10, ModifierType.Additive, obj);
        
        _testStat.AddModifier(_modifierAdditive);
        _testStat.AddModifier(_modifierAdditive);
        _testStat.AddModifier(objFlatModifier);
        _testStat.AddModifier(objAdditiveModifier);

        var areRemoved = _testStat.TryRemoveAllModifiersOf(obj);
        
        Assert.IsTrue(areRemoved);
    }

    [Test]
    public void TryRemoveAllModifiersOf_HasNotSourceModifiers_ReturnsFalse()
    {
        object obj = new();
        Modifier objFlatModifier = new Modifier(10, ModifierType.Flat, obj);
        Modifier objAdditiveModifier = new Modifier(10, ModifierType.Additive, obj);
        
        _testStat.AddModifier(_modifierAdditive);
        _testStat.AddModifier(objFlatModifier);
        _testStat.AddModifier(objAdditiveModifier);
        _testStat.AddModifier(_modifierAdditive);
        _testStat.TryRemoveModifier(objFlatModifier);
        _testStat.TryRemoveModifier(objAdditiveModifier);
        
        var areRemoved = _testStat.TryRemoveAllModifiersOf(obj);
        
        Assert.IsFalse(areRemoved);
    }
    
    [Test]
    public void AddNewModifier_AddModifierAfterInitialization_ThrowsInvalidOperationException()
    {
        Stat testStat = new Stat(100);
        
        Assert.Throws(typeof(InvalidOperationException),
            delegate { Stat.NewModifierType(400, () => new StubModifiersOperations()); });
    }
    
    private class StubModifiersOperations : IModifiersOperations
    {
        public void AddModifier(Modifier modifier) => throw new NotImplementedException();
        public bool TryRemoveModifier(Modifier modifier) => throw new NotImplementedException();
        public List<Modifier> GetAllModifiers() => throw new NotImplementedException();
        public float CalculateModifiersValue(float baseValue, float currentValue) => throw new NotImplementedException();
    }
}
}
