using Raylib_CsLo;

namespace CouscousEngine.Core;

public static class AssetManager
{
    private static readonly Dictionary<string, Texture> _textures = new();
    private static readonly Dictionary<string, Font> _fonts = new();

    private static readonly Dictionary<int, Font> _defaultFonts = new();

    public static void SetDefaultFont(Font font, int fontSize)
    {
        _defaultFonts.Add(fontSize, font);
    }

    public static Font GetDefaultFont(int fontSize)
        => _defaultFonts[fontSize];

    public static Font? LoadFont(string name, string filePath, int fontSize = 24)
    {
        unsafe
        {
            fixed (int* codepoints = new int[512])
            {
                for (var i = 0; i < 95; i++) 
                    codepoints[i] = 32 + i;   // Basic ASCII characters
                for (var i = 0; i < 255; i++) 
                    codepoints[96 + i] = 0x400 + i;   // Cyrillic characters
                _fonts?.Add(name, _rl.LoadFontEx(filePath, fontSize, codepoints, 512));
            }
        }

        return _fonts?[name];
    }

    public static Font GetFont(string name)
        => _fonts[name];

    public static Texture LoadTexture(string name, string filePath)
    {
        _textures.Add(name, _rl.LoadTexture(filePath));
        return _textures[name];
    }

    public static Texture GetTexture(string name)
        => _textures[name];

    public static void Deinitialize()
    {
        foreach (var font in _fonts.Values)
            _rl.UnloadFont(font);
        foreach (var texture in _textures.Values)
            _rl.UnloadTexture(texture);
    }
}