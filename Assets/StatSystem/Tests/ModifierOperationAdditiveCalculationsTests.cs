using NUnit.Framework;

namespace StatSystem.Tests
{
public class ModifierOperationAdditiveCalculationsTests
{
   private const int BASE_VALUE = 20;
   private const float TEN_PERCENT = 0.1f;
   private const float THIRTY_PERCENT = 0.3f;
   private const float EPSILON = 0.0001f;

   private Stat _testStat;
   private Modifier _additiveModifier10;
   private Modifier _additiveModifier30;
   private Modifier _additiveModifierMinus30;
    
   [SetUp]
   public void SetUp()
   {
      _testStat = new Stat(BASE_VALUE);
      _additiveModifier10  = new Modifier(TEN_PERCENT, ModifierType.Additive);
      _additiveModifier30 = new Modifier(THIRTY_PERCENT, ModifierType.Additive);
      _additiveModifierMinus30 = new Modifier(-THIRTY_PERCENT, ModifierType.Additive);
   }
   
   [Test]
   public void Adding10_ReturnsBaseValuePlus10Percent()
   {
      _testStat.AddModifier(_additiveModifier10);
           
      Assert.AreEqual(_testStat.Value, _testStat.BaseValue * (1 + TEN_PERCENT), EPSILON);
   }
   
   [Test]
   public void Adding10and30_returnsBaseValuePlus40Percent()
   {
      _testStat.AddModifier(_additiveModifier10);
      _testStat.AddModifier(_additiveModifier30);
           
      Assert.AreEqual(_testStat.Value, _testStat.BaseValue  * (1 + 0.4f), EPSILON);
   }
   
   [Test]
   public void AddingMinus30_ReturnsBaseValueMinus30Percent()
   {
      _testStat.AddModifier(_additiveModifierMinus30);
           
      Assert.AreEqual(_testStat.Value, _testStat.BaseValue  * (1 - THIRTY_PERCENT), EPSILON);
   }
   
   [Test]
   public void Adding10AndMinus30AndMinus30_ReturnsBaseValueMinus50Percent()
   {
      _testStat.AddModifier(_additiveModifier10);
      _testStat.AddModifier(_additiveModifierMinus30);
      _testStat.AddModifier(_additiveModifierMinus30);
           
      Assert.AreEqual(_testStat.Value, _testStat.BaseValue  * (1 - 0.5f), EPSILON);
   }
   
   [Test]
   public void Adding10Removing10_ReturnsBaseValue()
   {
      _testStat.AddModifier(_additiveModifier10);
      _testStat.TryRemoveModifier(_additiveModifier10);
           
      Assert.AreEqual(_testStat.Value, _testStat.BaseValue, EPSILON);
   }
   
   [Test]
   public void AddingTwoTimes10AndRemoving10_ReturnsBaseValuePlus10Percent()
   {
      _testStat.AddModifier(_additiveModifier10);
      _testStat.AddModifier(_additiveModifier10);
      _testStat.TryRemoveModifier(_additiveModifier10);
   
      Assert.AreEqual(_testStat.Value, _testStat.BaseValue  * (1 + TEN_PERCENT), EPSILON);
   }
   
   [Test]
   public void AddingTwoTimes10AndRemovingThreeTimes10_ReturnsBaseValue()
   {
      _testStat.AddModifier(_additiveModifier10);
      _testStat.AddModifier(_additiveModifier10);
      _testStat.TryRemoveModifier(_additiveModifier10);
      _testStat.TryRemoveModifier(_additiveModifier10);
      _testStat.TryRemoveModifier(_additiveModifier10);
   
      Assert.AreEqual(_testStat.Value, _testStat.BaseValue, EPSILON);
   }
}
}