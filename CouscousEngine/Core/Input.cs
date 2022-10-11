using System.Numerics;

namespace CouscousEngine.Core;

using rl = Raylib_CsLo.Raylib;

public enum KeyboardKey 
{
    NULL = 0,
    BACK = 4,
    VOLUME_UP = 24, // 0x00000018
    VOLUME_DOWN = 25, // 0x00000019
    SPACE = 32, // 0x00000020
    APOSTROPHE = 39, // 0x00000027
    COMMA = 44, // 0x0000002C
    MINUS = 45, // 0x0000002D
    PERIOD = 46, // 0x0000002E
    SLASH = 47, // 0x0000002F
    ZERO = 48, // 0x00000030
    ONE = 49, // 0x00000031
    TWO = 50, // 0x00000032
    THREE = 51, // 0x00000033
    FOUR = 52, // 0x00000034
    FIVE = 53, // 0x00000035
    SIX = 54, // 0x00000036
    SEVEN = 55, // 0x00000037
    EIGHT = 56, // 0x00000038
    NINE = 57, // 0x00000039
    SEMICOLON = 59, // 0x0000003B
    EQUAL = 61, // 0x0000003D
    A = 65, // 0x00000041
    B = 66, // 0x00000042
    C = 67, // 0x00000043
    D = 68, // 0x00000044
    E = 69, // 0x00000045
    F = 70, // 0x00000046
    G = 71, // 0x00000047
    H = 72, // 0x00000048
    I = 73, // 0x00000049
    J = 74, // 0x0000004A
    K = 75, // 0x0000004B
    L = 76, // 0x0000004C
    M = 77, // 0x0000004D
    N = 78, // 0x0000004E
    O = 79, // 0x0000004F
    P = 80, // 0x00000050
    Q = 81, // 0x00000051
    MENU = 82, // 0x00000052
    R = 82, // 0x00000052
    S = 83, // 0x00000053
    T = 84, // 0x00000054
    U = 85, // 0x00000055
    V = 86, // 0x00000056
    W = 87, // 0x00000057
    X = 88, // 0x00000058
    Y = 89, // 0x00000059
    Z = 90, // 0x0000005A
    LEFT_BRACKET = 91, // 0x0000005B
    BACKSLASH = 92, // 0x0000005C
    RIGHT_BRACKET = 93, // 0x0000005D
    GRAVE = 96, // 0x00000060
    ESCAPE = 256, // 0x00000100
    ENTER = 257, // 0x00000101
    TAB = 258, // 0x00000102
    BACKSPACE = 259, // 0x00000103
    INSERT = 260, // 0x00000104
    DELETE = 261, // 0x00000105
    RIGHT = 262, // 0x00000106
    LEFT = 263, // 0x00000107
    DOWN = 264, // 0x00000108
    UP = 265, // 0x00000109
    PAGE_UP = 266, // 0x0000010A
    PAGE_DOWN = 267, // 0x0000010B
    HOME = 268, // 0x0000010C
    END = 269, // 0x0000010D
    CAPS_LOCK = 280, // 0x00000118
    SCROLL_LOCK = 281, // 0x00000119
    NUM_LOCK = 282, // 0x0000011A
    PRINT_SCREEN = 283, // 0x0000011B
    PAUSE = 284, // 0x0000011C
    F1 = 290, // 0x00000122
    F2 = 291, // 0x00000123
    F3 = 292, // 0x00000124
    F4 = 293, // 0x00000125
    F5 = 294, // 0x00000126
    F6 = 295, // 0x00000127
    F7 = 296, // 0x00000128
    F8 = 297, // 0x00000129
    F9 = 298, // 0x0000012A
    F10 = 299, // 0x0000012B
    F11 = 300, // 0x0000012C
    F12 = 301, // 0x0000012D
    KP_0 = 320, // 0x00000140
    KP_1 = 321, // 0x00000141
    KP_2 = 322, // 0x00000142
    KP_3 = 323, // 0x00000143
    KP_4 = 324, // 0x00000144
    KP_5 = 325, // 0x00000145
    KP_6 = 326, // 0x00000146
    KP_7 = 327, // 0x00000147
    KP_8 = 328, // 0x00000148
    KP_9 = 329, // 0x00000149
    KP_DECIMAL = 330, // 0x0000014A
    KP_DIVIDE = 331, // 0x0000014B
    KP_MULTIPLY = 332, // 0x0000014C
    KP_SUBTRACT = 333, // 0x0000014D
    KP_ADD = 334, // 0x0000014E
    KP_ENTER = 335, // 0x0000014F
    KP_EQUAL = 336, // 0x00000150
    LEFT_SHIFT = 340, // 0x00000154
    LEFT_CONTROL = 341, // 0x00000155
    LEFT_ALT = 342, // 0x00000156
    LEFT_SUPER = 343, // 0x00000157
    RIGHT_SHIFT = 344, // 0x00000158
    RIGHT_CONTROL = 345, // 0x00000159
    RIGHT_ALT = 346, // 0x0000015A
    RIGHT_SUPER = 347, // 0x0000015B
    KB_MENU = 348, // 0x0000015C
}

public enum MouseButton
{
    LEFT,
    RIGHT,
    MIDDLE,
    SIDE,
    EXTRA,
    FORWARD,
    BACK,
}

public static class Input
{
    private static bool _isDown;
    private static float _downTimer;
    
    #region Keyboard
    
    public static bool IsKeyDown(KeyboardKey key) => rl.IsKeyDown((int) key);

    public static float GetKeyDownTime(KeyboardKey key)
    {
        if (IsKeyPressed(key))
        {
            _isDown = true;
            _downTimer = 0;
        }

        if (_isDown) _downTimer += 1f * _rl.GetFrameTime();

        if (!IsKeyReleased(key)) return _downTimer;
        
        _isDown = false;
        _downTimer = 0;

        return _downTimer;
    }
    
    public static bool IsKeyPressed(KeyboardKey key) => rl.IsKeyPressed((int) key);
    public static bool IsKeyReleased(KeyboardKey key) => rl.IsKeyReleased((int) key);

    public static bool IsKeyPressedWithModifier(KeyboardKey modifier, KeyboardKey key)
        => IsKeyDown(modifier) && IsKeyDown(key);

    public static void GetAsciiKeyPressed(ref string text)
    {
        var key = _rl.GetCharPressed();

        while (key > 0)
        {
            if (key is >= 32 and <= 125 or >= 1040 and <= 1103)
                text += (char)key;
            
            key = _rl.GetCharPressed();
        }
    }

    #endregion
    
    #region Mouse
    
    public static Vector2 GetMousePosition() => rl.GetMousePosition();
    public static float GetMouseWheelMovement() => rl.GetMouseWheelMove();
    
    public static bool IsMouseButtonDown(MouseButton mouseButton) => rl.IsMouseButtonDown((int) mouseButton);
    public static bool IsMouseButtonPressed(MouseButton mouseButton) => rl.IsMouseButtonPressed((int) mouseButton);

    #endregion
}