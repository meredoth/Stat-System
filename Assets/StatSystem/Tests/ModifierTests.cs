using System.Collections.Generic;
using NUnit.Framework;
using StatSystem.ModifierOperations;

namespace StatSystem.Tests
{
[TestFixture]
internal class ModifierTests
{
   private Modifier _firstModifier;
   private Modifier _secondModifier;
   private Modifier _defaultModifier;
   private Modifier _defaultValuesModifier;
   
   [SetUp]
   public void Setup()
   {
      _firstModifier = new Modifier(10, ModifierType.Additive, this);
      _secondModifier = new Modifier(10, ModifierType.Additive, this);
      _defaultModifier = default;
      _defaultValuesModifier = new Modifier(0, ModifierType.Flat);
   }

   [Test]
   public void Equals_EqualityForSameModifier_ReturnsTrue() => 
      Assert.IsTrue(_firstModifier.Equals(_secondModifier));

   [Test]
   public void Equals_EqualityForDifferentModifiers_ReturnsFalse() => 
      Assert.IsFalse(_firstModifier.Equals(_defaultValuesModifier));

   [Test]
   public void Equals_EqualityForDefaultModifer_ReturnsTrue() => 
      Assert.IsTrue(_defaultModifier.Equals(_defaultValuesModifier));

   [Test]
   public void OperatorEquals_EqualityForSameModifier_ReturnsTrue() => 
      Assert.IsTrue(_firstModifier == _secondModifier);

   [Test]
   public void OperatorEquals_EqualityForDifferentModifiers_ReturnsFalse() => 
      Assert.IsFalse(_firstModifier == _defaultValuesModifier);

   [Test]
   public void OperatorEquals_EqualityForDefaultModifier_ReturnsTrue() => 
      Assert.IsTrue(_defaultModifier == _defaultValuesModifier);

   [Test]
   public void ApplyTo_AfterAppliedToStat_StatHasModifier()
   {
      Stat stat = new(100);
      Modifier sut = new(10, ModifierType.Flat);
      
      sut.ApplyTo(stat);
      
      Assert.IsTrue(stat.ContainsModifier(sut));
   }
   
   [Test]
   public void ApplyTo_AfterAppliedToStats_AllStatsHaveModifier()
   {
      Stat stat = new(100);
      Stat stat2 = new(200);
      Stat stat3 = new(300);
      Stat[] stats = { stat, stat2, stat3};
      Modifier sut = new(10, ModifierType.Flat);
      
      sut.ApplyTo(stats);
      
      Assert.IsTrue(stat.ContainsModifier(sut));
      Assert.IsTrue(stat2.ContainsModifier(sut));
      Assert.IsTrue(stat3.ContainsModifier(sut));
   }
}
}