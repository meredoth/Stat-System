using NUnit.Framework;

namespace StatSystem.Tests
{
public class ModifierOperationMultiplicativeCalculationsTests
{
   private const int BASE_VALUE = 20;
   private const float TEN_PERCENT = 0.1f;
   private const float THIRTY_PERCENT = 0.3f;
   private const float EPSILON = 0.0001f;
   private Stat _testStat;
   private Modifier _multiplicativeModifier10;
   private Modifier _multiplicativeModifier30;
   private Modifier _multiplicativeModifierMinus30;
    
   [SetUp]
   public void SetUp()
   {
      _testStat = new Stat(BASE_VALUE);
      _multiplicativeModifier10  = new Modifier(TEN_PERCENT, ModifierType.Multiplicative);
      _multiplicativeModifier30 = new Modifier(THIRTY_PERCENT, ModifierType.Multiplicative);
      _multiplicativeModifierMinus30 = new Modifier(-THIRTY_PERCENT, ModifierType.Multiplicative);
   }
   
   [Test]
   public void Adding10_ReturnsBaseValuePlus10Percent()
   {
      _testStat.AddModifier(_multiplicativeModifier10);
           
      Assert.AreEqual(_testStat.Value, _testStat.BaseValue * (1 + TEN_PERCENT), EPSILON);
   }
   
   [Test]
   public void Adding10and30_returnsBaseValuePlusTimes10PlusTimes30Percent()
   {
      _testStat.AddModifier(_multiplicativeModifier10);
      _testStat.AddModifier(_multiplicativeModifier30);
           
      Assert.AreEqual(_testStat.Value, _testStat.BaseValue  * (1 + TEN_PERCENT)  * (1 + THIRTY_PERCENT), EPSILON);
   }
   
   [Test]
   public void AddingMinus30_ReturnsBaseValueMinus30Percent()
   {
      _testStat.AddModifier(_multiplicativeModifierMinus30);
           
      Assert.AreEqual(_testStat.Value, _testStat.BaseValue  * (1 - THIRTY_PERCENT), EPSILON);
   }
   
   [Test]
   public void Adding10AndMinus30AndMinus30_ReturnsBaseValueMultipliedBy10AndMinus30AndMinus30Percent()
   {
      _testStat.AddModifier(_multiplicativeModifier10);
      _testStat.AddModifier(_multiplicativeModifierMinus30);
      _testStat.AddModifier(_multiplicativeModifierMinus30);
           
      Assert.AreEqual(_testStat.Value, _testStat.BaseValue  * ( 1 + TEN_PERCENT)  * (1 - THIRTY_PERCENT) * (1 - THIRTY_PERCENT), EPSILON);
   }
   
   [Test]
   public void Adding10Removing10_ReturnsBaseValue()
   {
      _testStat.AddModifier(_multiplicativeModifier10);
      _testStat.TryRemoveModifier(_multiplicativeModifier10);
           
      Assert.AreEqual(_testStat.Value, _testStat.BaseValue, EPSILON);
   }
   
   [Test]
   public void AddingTwoTimes10AndRemoving10_ReturnsBaseValuePlus10Percent()
   {
      _testStat.AddModifier(_multiplicativeModifier10);
      _testStat.AddModifier(_multiplicativeModifier10);
      _testStat.TryRemoveModifier(_multiplicativeModifier10);
   
      Assert.AreEqual(_testStat.Value, _testStat.BaseValue * (1 + TEN_PERCENT), EPSILON);
   }
   
   [Test]
   public void AddingTwoTimes10AndRemovingThreeTimes10_ReturnsBaseValue()
   {
      _testStat.AddModifier(_multiplicativeModifier10);
      _testStat.AddModifier(_multiplicativeModifier10);
      _testStat.TryRemoveModifier(_multiplicativeModifier10);
      _testStat.TryRemoveModifier(_multiplicativeModifier10);
      _testStat.TryRemoveModifier(_multiplicativeModifier10);
   
      Assert.AreEqual(_testStat.Value, _testStat.BaseValue, EPSILON);
   }
}
}
