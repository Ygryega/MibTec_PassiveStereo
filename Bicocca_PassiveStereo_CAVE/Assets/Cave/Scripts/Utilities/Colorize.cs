using UnityEngine;

public class Colorize
{
    // Classic Color Palette
    public static Colorize Red = new Colorize(Color.red);
    public static Colorize Yellow = new Colorize(Color.yellow);
    public static Colorize Green = new Colorize(Color.green);
    public static Colorize Blue = new Colorize(Color.blue);
    public static Colorize Cyan = new Colorize(Color.cyan);
    public static Colorize Magenta = new Colorize(Color.magenta);
    public static Colorize White = new Colorize(Color.white);

    public static Colorize Lime = new Colorize("#00ff00ff");
    public static Colorize Orange = new Colorize("#ffa500ff");

    // Specific Category
    public static Colorize Server = Colorize.Lime;
    public static Colorize Client = Colorize.White;
    public static Colorize Display = Colorize.Yellow;


    private readonly string _prefix;
    private const string _suffix = "</color>";

    private Colorize(Color color)
    {
        this._prefix = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>";
    }

    private Colorize(string hexColor)
    {
        this._prefix = $"<color={hexColor}>";
    }

    public static string operator %(string text, Colorize color)
    {
#if UNITY_EDITOR
        return color._prefix + text + _suffix;
#else
        return text;
#endif
    }
}
