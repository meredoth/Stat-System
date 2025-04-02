using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace StatSystem.Tests
{
[TestFixture]
internal class StatTests
{
    private Stat _testStat;
    private Modifier _modifierFlat;
    private Modifier _modifierAdditive;
    private Modifier _modifierMultiplicative;
    private Modifier _modifierCustom;
    private ModifierType _customModifierType;

    [OneTimeSetUp]
    public void SetupOnce()
    {
        Stat.Init();
        _customModifierType = Stat.NewModifierType(1000, () => new StubModifiersOperations());
    }
    
    [SetUp]
    public void Setup()
    {
        _testStat = new(100);
        _modifierFlat = new Modifier(10, ModifierType.Flat);
        _modifierAdditive = new Modifier(0.1f, ModifierType.Additive);
        _modifierMultiplicative = new Modifier(0.2f, ModifierType.Multiplicative);
        _modifierCustom = new Modifier(10f, _customModifierType);
    }

    [Test]
    public void CanAddNewModifierType_AfterNewModifierTypeHasBeenAdded_ReturnsFalse()
    {
        Assert.False(Stat.CanAddNewModifierType);
    }
    
    [Test]
    public void CreateDefaultModifier_AddModifier_DoesNotThrow()
    {
        Stat testStat = new Stat(10);
        Modifier defaultModifier = default;
        
        Assert.DoesNotThrow( () => testStat.AddModifier(defaultModifier));
    }

    [Test]
    public void CreateDefaultModifier_DefaultModifierEqualsToFlatWithZeroValue()
    {
        Modifier flatZeroValue = new Modifier(0f, ModifierType.Flat);
        Modifier defaultModifier = default;
        
        Assert.IsTrue(flatZeroValue == defaultModifier);
    }

    [Test]
    public void AddModifier_AddOneModifier_NumberOfStatsModifiersIncreasedByOne()
    {
        Modifier modifier = new Modifier(10, ModifierType.Flat);
        
        var numberOfModifiersBefore = _testStat.GetModifiers().Count;
        _testStat.AddModifier(modifier);
        var numberOfModifiersAfter = _testStat.GetModifiers().Count;
        
        Assert.AreEqual(numberOfModifiersAfter, numberOfModifiersBefore + 1);
    }

    [Test]
    public void AddModifiers_AddArrayOfModifiers_ContainEachModifierReturnsTrue()
    {
        Modifier[] modifiersArray = { _modifierFlat, _modifierAdditive, _modifierMultiplicative, _modifierCustom };
        
        var numberOfModifiersBefore = _testStat.GetModifiers().Count;
        _testStat.AddModifiers(modifiersArray.AsSpan());
        var numberOfModifiersAfter = _testStat.GetModifiers().Count;
        
        Assert.AreEqual(numberOfModifiersAfter, numberOfModifiersBefore + 4);
        Assert.True(_testStat.ContainsModifier(_modifierFlat));
        Assert.True(_testStat.ContainsModifier(_modifierAdditive));
        Assert.True(_testStat.ContainsModifier(_modifierMultiplicative));
        Assert.True(_testStat.ContainsModifier(_modifierCustom));
    }
    
    [Test]
    public void AddModifiers_AddListOfModifiers_ContainEachModifierReturnsTrue()
    {
        List<Modifier> modifiersList = new (){ _modifierFlat, _modifierAdditive, _modifierMultiplicative, _modifierCustom };
        
        var numberOfModifiersBefore = _testStat.GetModifiers().Count;
        _testStat.AddModifiers(modifiersList);
        var numberOfModifiersAfter = _testStat.GetModifiers().Count;
        
        Assert.AreEqual(numberOfModifiersAfter, numberOfModifiersBefore + 4);
        Assert.True(_testStat.ContainsModifier(_modifierFlat));
        Assert.True(_testStat.ContainsModifier(_modifierAdditive));
        Assert.True(_testStat.ContainsModifier(_modifierMultiplicative));
        Assert.True(_testStat.ContainsModifier(_modifierCustom));
    }
    
    [Test]
    public void AddModifiers_AddParamArrayOfModifiers_ContainEachModifierReturnsTrue()
    {
        var numberOfModifiersBefore = _testStat.GetModifiers().Count;
        _testStat.AddModifiers(_modifierFlat, _modifierAdditive, _modifierMultiplicative, _modifierCustom);
        var numberOfModifiersAfter = _testStat.GetModifiers().Count;
        
        Assert.AreEqual(numberOfModifiersAfter, numberOfModifiersBefore + 4);
        Assert.True(_testStat.ContainsModifier(_modifierFlat));
        Assert.True(_testStat.ContainsModifier(_modifierAdditive));
        Assert.True(_testStat.ContainsModifier(_modifierMultiplicative));
        Assert.True(_testStat.ContainsModifier(_modifierCustom));
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
    public void GetModifier_InvalidModifierType_ThrowsOutOfRangeException() 
        => Assert.Throws(typeof(ArgumentOutOfRangeException), 
                         () => { _testStat.GetModifiers((ModifierType)(-666)); });

    [Test]
    public void GetModifier_CustomModifierType_ReturnsCustomModifier()
    {
        _testStat.AddModifier(_modifierCustom);
        var sut =_testStat.GetModifiers(_customModifierType);
        
        Assert.IsNotEmpty(sut);
        Assert.AreEqual(sut[0],_modifierCustom);
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
        // ReSharper disable once UnusedVariable
        Stat testStat = new Stat(100);
        
        Assert.Throws(typeof(InvalidOperationException),
            delegate { Stat.NewModifierType(400, () => new StubModifiersOperations()); });
    }

    [Test]
    public void ContainsModifier_ModifierExists_ReturnsTrue()
    {
        var modifierWithObject = new Modifier(10f, _customModifierType, this);
        
        _testStat.AddModifier(modifierWithObject);
        _testStat.AddModifier(_modifierFlat);
        
        Assert.True(_testStat.ContainsModifier(_modifierFlat));
        Assert.True(_testStat.ContainsModifier(modifierWithObject));
    }

    [Test]
    public void ContainsModifier_ModifierDoesNotExist_ReturnsFalse()
    {
        var modifierWithObject = new Modifier(10f, _customModifierType, this);
        var modifierWithNewObject = new Modifier(10f, _customModifierType, new object());
        
        _testStat.AddModifier(modifierWithNewObject);
        
        Assert.False(_testStat.ContainsModifier(_modifierFlat));
        Assert.False(_testStat.ContainsModifier(modifierWithObject));
    }

    [Test]
    public void GetModifiers_ListIsEmpty_ReturnsEmptyCollection()
    {
        var modifiers = _testStat.GetModifiers();
        
        Assert.NotNull(modifiers);
        Assert.IsEmpty(modifiers);
    }
    
    private class StubModifiersOperations : ModifierOperationsBase
    {
        public override float CalculateModifiersValue(float baseValue, float currentValue) => 0;
    }
}
}
