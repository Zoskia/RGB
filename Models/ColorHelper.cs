namespace RedGreenBlue.Models;

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
        return 'B';
    }

    private static (byte R, byte G, byte B) HexStringToRgb(string hexCode)
    {
        string hex = hexCode.Substring(1);
        byte r = Convert.ToByte(hex.Substring(0, 2), 16);
        byte g = Convert.ToByte(hex.Substring(2, 2), 16);
        byte b = Convert.ToByte(hex.Substring(4, 2), 16);
        return (r, g, b);
    }

}