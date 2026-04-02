using FluentAssertions;
using RedGreenBlue.Helpers;

namespace RedGreenBlue.UnitTests.Helpers;

public class ColorHelperTests
{
    [Fact]
    public void IsRed_ShouldReturnTrue_ForRedDominantHex()
    {
        var result = ColorHelper.IsRed("#ff3300");

        result.Should().BeTrue();
    }

    [Fact]
    public void IsGreen_ShouldSupportShortHexFormat()
    {
        var result = ColorHelper.IsGreen("#0f0");

        result.Should().BeTrue();
    }

    [Fact]
    public void IsBlue_ShouldSupportHexWithAlpha()
    {
        var result = ColorHelper.IsBlue("#1122ccff");

        result.Should().BeTrue();
    }

    [Fact]
    public void IsRed_ShouldReturnFalse_WhenAnotherChannelIsDominant()
    {
        var result = ColorHelper.IsRed("#0000ff");

        result.Should().BeFalse();
    }

    [Fact]
    public void IsBlue_ShouldThrow_WhenChannelsAreTied()
    {
        var act = () => ColorHelper.IsBlue("#808080");

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Color channels are tied*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("ff0000")]
    [InlineData("#zz0000")]
    [InlineData("#12345")]
    public void IsGreen_ShouldThrow_ForInvalidHex(string? hex)
    {
        var act = () => ColorHelper.IsGreen(hex!);

        act.Should().Throw<InvalidOperationException>();
    }
}
