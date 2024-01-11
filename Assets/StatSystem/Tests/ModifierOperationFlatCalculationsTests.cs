using NUnit.Framework;

namespace StatSystem.Tests
{
public class ModifierOperationFlatCalculationsTests
{
    private const int BASE_VALUE = 20;
    private const int TEN = 10;
    private const int THIRTY = 30;
    private Stat _testStat;
    private Modifier _flatModifier10;
    private Modifier _flatModifier30;
    private Modifier _flatModifierMinus30;
    
    [SetUp]
    public void SetUp()
    {
        _testStat = new Stat(BASE_VALUE);
        _flatModifier10  = new Modifier(TEN, ModifierType.Flat);
        _flatModifier30 = new Modifier(THIRTY, ModifierType.Flat);
        _flatModifierMinus30 = new Modifier(-THIRTY, ModifierType.Flat);
    }

    [Test]
    public void Adding10_ReturnsBaseValuePlus10()
    {
        _testStat.AddModifier(_flatModifier10);
        
        Assert.AreEqual(_testStat.Value, _testStat.BaseValue + 10);
    }

    [Test]
    public void Adding10and30_returnsBaseValuePlus40()
    {
        _testStat.AddModifier(_flatModifier10);
        _testStat.AddModifier(_flatModifier30);
        
        Assert.AreEqual(_testStat.Value, _testStat.BaseValue + 40);
    }

    [Test]
    public void AddingMinus30_ReturnsBaseValueMinus30()
    {
        _testStat.AddModifier(_flatModifierMinus30);
        
        Assert.AreEqual(_testStat.Value, _testStat.BaseValue - 30);
    }

    [Test]
    public void Adding10AndMinus30AndMinus30_ReturnsBaseValueMinus50()
    {
        _testStat.AddModifier(_flatModifier10);
        _testStat.AddModifier(_flatModifierMinus30);
        _testStat.AddModifier(_flatModifierMinus30);
        
        Assert.AreEqual(_testStat.Value, _testStat.BaseValue - 50);
    }

    [Test]
    public void Adding10Removing10_ReturnsBaseValue()
    {
        _testStat.AddModifier(_flatModifier10);
        _testStat.TryRemoveModifier(_flatModifier10);
        
        Assert.AreEqual(_testStat.Value, _testStat.BaseValue);
    }

    [Test]
    public void AddingTwoTimes10AndRemoving10_ReturnsBaseValuePlus10()
    {
        _testStat.AddModifier(_flatModifier10);
        _testStat.AddModifier(_flatModifier10);
        _testStat.TryRemoveModifier(_flatModifier10);

        Assert.AreEqual(_testStat.Value, _testStat.BaseValue + 10);
    }

    [Test]
    public void AddingTwoTimes10AndRemovingThreeTimes10_ReturnsBaseValue()
    {
        _testStat.AddModifier(_flatModifier10);
        _testStat.AddModifier(_flatModifier10);
        _testStat.TryRemoveModifier(_flatModifier10);
        _testStat.TryRemoveModifier(_flatModifier10);
        _testStat.TryRemoveModifier(_flatModifier10);

        Assert.AreEqual(_testStat.Value, _testStat.BaseValue);
    }
}
}
