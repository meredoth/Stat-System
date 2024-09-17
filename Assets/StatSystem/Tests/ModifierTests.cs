using NUnit.Framework;

namespace StatSystem.Tests
{
[TestFixture]
public class ModifierTests
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
   public void OperatorEquals_EqualityForDefaultModifer_ReturnsTrue() => 
      Assert.IsTrue(_defaultModifier == _defaultValuesModifier);
}
}