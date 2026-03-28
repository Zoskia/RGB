namespace RedGreenBlue.Helpers;

public static class ColorHelper
{
    public static bool IsRed(string hex) => DominantChannel(hex) == 'R';
    public static bool IsGreen(string hex) => DominantChannel(hex) == 'G';
    public static bool IsBlue(string hex) => DominantChannel(hex) == 'B';

    private static char DominantChannel(string hex)
    {
        var (r, g, b) = HexStringToRgb(hex);
        if (r > g && r > b) return 'R';
        if (g > r && g > b) return 'G';
        if (b > r && b > g) return 'B';

        throw new InvalidOperationException("Color channels are tied. A unique dominant channel is required.");
    }

    private static (byte R, byte G, byte B) HexStringToRgb(string hexCode)
    {
        var normalizedHex = NormalizeHex(hexCode);
        string hex = normalizedHex.Substring(1);

        byte r = Convert.ToByte(hex.Substring(0, 2), 16);
        byte g = Convert.ToByte(hex.Substring(2, 2), 16);
        byte b = Convert.ToByte(hex.Substring(4, 2), 16);

        return (r, g, b);
    }

    private static string NormalizeHex(string hexCode)
    {
        if (string.IsNullOrWhiteSpace(hexCode))
        {
            throw new InvalidOperationException("HEX color is required.");
        }

        var value = hexCode.Trim();
        if (!value.StartsWith('#'))
        {
            throw new InvalidOperationException("Invalid HEX color format.");
        }

        var hex = value.Substring(1);
        if (!AreHexDigits(hex))
        {
            throw new InvalidOperationException("Invalid HEX color format.");
        }

        return hex.Length switch
        {
            3 => $"#{hex[0]}{hex[0]}{hex[1]}{hex[1]}{hex[2]}{hex[2]}",
            6 => value,
            8 => $"#{hex.Substring(0, 6)}",
            _ => throw new InvalidOperationException("Invalid HEX color format.")
        };
    }

    private static bool AreHexDigits(string value)
    {
        foreach (var c in value)
        {
            if (!Uri.IsHexDigit(c))
            {
                return false;
            }
        }

        return true;
    }

}
