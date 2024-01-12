using NUnit.Framework;

namespace StatSystem.Tests
{
public class CombinedOperationsTests
{
   private const int BASE_VALUE = 100;
   private Stat _testStat;
   private Modifier _flatModifier;
   private Modifier _additiveModifier;
   private Modifier _multiplicativeModifier;
   
   
   [SetUp]
   public void SetUp()
   {
      _testStat = new Stat(BASE_VALUE);
   }

   [TestCase(20,0.1f, 160f)]
   [TestCase(-10,0.1f, 100f )]
   [TestCase(20, -0.2f, 100f)]
   [TestCase(-10, -0.2f, 40f)]
   public void AdditiveModifiersCalculatedAfterFlatModifiers(float flatValue, float additiveValue, float result)
   {
      _flatModifier = new Modifier(flatValue, ModifierType.Flat);
      _additiveModifier = new Modifier(additiveValue, ModifierType.Additive);
      
      _testStat.AddModifier(_additiveModifier);
      _testStat.AddModifier(_flatModifier);
      _testStat.AddModifier(_additiveModifier);
      _testStat.AddModifier(_flatModifier);
      
      Assert.AreEqual(_testStat.Value, result);
   }

   [TestCase(0.2f,0.1f, 169.4f)]
   [TestCase(-0.1f,0.1f, 96.8f )]
   [TestCase(0.2f, -0.2f, 89.6f)]
   [TestCase(-0.1f, -0.2f, 51.2f)]
   public void MultiplicativeModifiersCalculatedAfterAdditiveModifiers(float additiveValue,
      float multiplicativeValue, float result)
   {
      _additiveModifier = new Modifier(additiveValue, ModifierType.Additive);
      _multiplicativeModifier = new Modifier(multiplicativeValue, ModifierType.Multiplicative);
      
      _testStat.AddModifier(_multiplicativeModifier);
      _testStat.AddModifier(_additiveModifier);
      _testStat.AddModifier(_multiplicativeModifier);
      _testStat.AddModifier(_additiveModifier);
      
      Assert.AreEqual(_testStat.Value, result);
   }

   [Test]
   public void RemovingModifiersThatAreNotAddedReturnsCorrectResult()
   {
      _additiveModifier = new Modifier(0.2f, ModifierType.Additive);
      _multiplicativeModifier = new Modifier(0.5f, ModifierType.Multiplicative);
      
      _testStat.AddModifier(_additiveModifier);
      _testStat.TryRemoveModifier(_multiplicativeModifier);
      _testStat.AddModifier(_additiveModifier);
      _testStat.TryRemoveModifier(_multiplicativeModifier);

      Assert.AreEqual(_testStat.Value, 140f);
   }
}
}
