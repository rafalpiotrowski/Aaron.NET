using FluentAssertions;

namespace Aaron.Contracts.Tests;

public class DecimalValueTests
{
    [Fact]
    public void DecimalZero_ToDecimalValue_Success()
    {
        decimal price = decimal.Zero;
        var priceDc = new DecimalValue()
        {
            Units = 0,
            Nanos = 0
        };
        DecimalValue result = price;
        result.Should().Be(priceDc);
    }

    [Fact]
    public void Decimal_ToDecimalValue_Success()
    {
        decimal price = 154.65M;
        var priceDc = new DecimalValue()
        {
            Units = 154,
            Nanos = 650000000
        };
        DecimalValue result = price;
        result.Should().Be(priceDc);
    }

    [Fact]
    public void LongCastToDecimal_ToDecimalValue_Success()
    {
        decimal price = (decimal)154L;
        var priceDc = new DecimalValue()
        {
            Units = 154,
            Nanos = 0
        };
        DecimalValue result = price;
        result.Should().Be(priceDc);
    }

    [Fact]
    public void DecimalValue_ToDecimal_Success()
    {
        var dc = new DecimalValue()
        {
            Units = 154,
            Nanos = 650000000
        };
        decimal result = dc;
        result.Should().Be(154.65M);
    }

    [Fact]
    public void DecimalValueWithoutNanos_ToDecimal_Success()
    {
        var dc = new DecimalValue()
        {
            Units = 154,
            Nanos = 0
        };
        decimal result = dc;
        result.Should().Be(154M);
    }

    [Fact]
    public void DecimalValueWithoutUnits_ToDecimal_Success()
    {
        var dc = new DecimalValue()
        {
            Units = 0,
            Nanos = 650000000
        };
        decimal result = dc;
        result.Should().Be(0.65M);
    }

    [Fact]
    public void DecimalValueZero_ToDecimal_Success()
    {
        var dc = new DecimalValue()
        {
            Units = 0,
            Nanos = 0
        };
        decimal result = dc;
        result.Should().Be(decimal.Zero);
    }
}