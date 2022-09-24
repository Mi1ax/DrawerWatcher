using System.Numerics;
using System.Runtime.InteropServices;
using ImGuiNET;
using Raylib_CsLo;

namespace CouscousEngine.CCGui;

using static RlGl;

public static class rlImGui
{
    internal static IntPtr ImGuiContext = IntPtr.Zero;

    private static ImGuiMouseCursor CurrentMouseCursor = ImGuiMouseCursor.COUNT;
    private static Dictionary<ImGuiMouseCursor, MouseCursor>? MouseCursorMap;
    private static KeyboardKey[]? KeyEnumMap;

    private static Texture FontTexture;

    public static void Setup(bool darkTheme = true)
    {
        MouseCursorMap = new Dictionary<ImGuiMouseCursor, MouseCursor>();
        KeyEnumMap = Enum.GetValues(typeof(KeyboardKey)) as KeyboardKey[];

        FontTexture.id = 0;

        BeginInitImGui();

        if (darkTheme)
            ImGuiNET.ImGui.StyleColorsDark();
        else
            ImGuiNET.ImGui.StyleColorsLight();

        EndInitImGui();
    }

    public static void BeginInitImGui()
    {
        ImGuiContext = ImGuiNET.ImGui.CreateContext();
    }

    private static void SetupMouseCursors()
    {
        if (MouseCursorMap == null) return;
        MouseCursorMap.Clear();
        MouseCursorMap[ImGuiMouseCursor.Arrow] = MouseCursor.MOUSE_CURSOR_ARROW;
        MouseCursorMap[ImGuiMouseCursor.TextInput] = MouseCursor.MOUSE_CURSOR_IBEAM;
        MouseCursorMap[ImGuiMouseCursor.Hand] = MouseCursor.MOUSE_CURSOR_POINTING_HAND;
        MouseCursorMap[ImGuiMouseCursor.ResizeAll] = MouseCursor.MOUSE_CURSOR_RESIZE_ALL;
        MouseCursorMap[ImGuiMouseCursor.ResizeEW] = MouseCursor.MOUSE_CURSOR_RESIZE_EW;
        MouseCursorMap[ImGuiMouseCursor.ResizeNESW] = MouseCursor.MOUSE_CURSOR_RESIZE_NESW;
        MouseCursorMap[ImGuiMouseCursor.ResizeNS] = MouseCursor.MOUSE_CURSOR_RESIZE_NS;
        MouseCursorMap[ImGuiMouseCursor.ResizeNWSE] = MouseCursor.MOUSE_CURSOR_RESIZE_NWSE;
        MouseCursorMap[ImGuiMouseCursor.NotAllowed] = MouseCursor.MOUSE_CURSOR_NOT_ALLOWED;
    }

    public static unsafe void ReloadFonts()
    {
        ImGuiNET.ImGui.SetCurrentContext(ImGuiContext);
        var io = ImGuiNET.ImGui.GetIO();

        io.Fonts.GetTexDataAsRGBA32(out byte* pixels, out var width, out var height, out _);

        var image = new Image
        {
            data = pixels,
            width = width,
            height = height,
            mipmaps = 1,
            format = (int) PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8A8,
        };

        FontTexture = Raylib.LoadTextureFromImage(image);

        io.Fonts.SetTexID(new IntPtr(FontTexture.id));
    }

    public static void EndInitImGui()
    {
        SetupMouseCursors();

        ImGuiNET.ImGui.SetCurrentContext(ImGuiContext);
        _ = ImGuiNET.ImGui.GetIO().Fonts;
        ImGuiNET.ImGui.GetIO().Fonts.AddFontDefault();

        var io = ImGuiNET.ImGui.GetIO();
        io.KeyMap[(int)ImGuiKey.Tab] = (int)KeyboardKey.KEY_TAB;
        io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)KeyboardKey.KEY_LEFT;
        io.KeyMap[(int)ImGuiKey.RightArrow] = (int)KeyboardKey.KEY_RIGHT;
        io.KeyMap[(int)ImGuiKey.UpArrow] = (int)KeyboardKey.KEY_UP;
        io.KeyMap[(int)ImGuiKey.DownArrow] = (int)KeyboardKey.KEY_DOWN;
        io.KeyMap[(int)ImGuiKey.PageUp] = (int)KeyboardKey.KEY_PAGE_UP;
        io.KeyMap[(int)ImGuiKey.PageDown] = (int)KeyboardKey.KEY_PAGE_DOWN;
        io.KeyMap[(int)ImGuiKey.Home] = (int)KeyboardKey.KEY_HOME;
        io.KeyMap[(int)ImGuiKey.End] = (int)KeyboardKey.KEY_END;
        io.KeyMap[(int)ImGuiKey.Delete] = (int)KeyboardKey.KEY_DELETE;
        io.KeyMap[(int)ImGuiKey.Backspace] = (int)KeyboardKey.KEY_BACKSPACE;
        io.KeyMap[(int)ImGuiKey.Enter] = (int)KeyboardKey.KEY_ENTER;
        io.KeyMap[(int)ImGuiKey.Escape] = (int)KeyboardKey.KEY_ESCAPE;
        io.KeyMap[(int)ImGuiKey.Space] = (int)KeyboardKey.KEY_SPACE;
        io.KeyMap[(int)ImGuiKey.A] = (int)KeyboardKey.KEY_A;
        io.KeyMap[(int)ImGuiKey.C] = (int)KeyboardKey.KEY_C;
        io.KeyMap[(int)ImGuiKey.V] = (int)KeyboardKey.KEY_V;
        io.KeyMap[(int)ImGuiKey.X] = (int)KeyboardKey.KEY_X;
        io.KeyMap[(int)ImGuiKey.Y] = (int)KeyboardKey.KEY_Y;
        io.KeyMap[(int)ImGuiKey.Z] = (int)KeyboardKey.KEY_Z;

        ReloadFonts();
    }

    private static void NewFrame()
    {
        var io = ImGuiNET.ImGui.GetIO();

        if (Raylib.IsWindowFullscreen())
        {
            var monitor = Raylib.GetCurrentMonitor();
            io.DisplaySize = new Vector2(Raylib.GetMonitorWidth(monitor), Raylib.GetMonitorHeight(monitor));
        }
        else
        {
            io.DisplaySize = new Vector2(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
        }
            
        io.DisplayFramebufferScale = new Vector2(1, 1);
        io.DeltaTime = Raylib.GetFrameTime();

        io.KeyCtrl = Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_CONTROL) || Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL);
        io.KeyShift = Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_SHIFT) || Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT);
        io.KeyAlt = Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_ALT) || Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_ALT);
        io.KeySuper = Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_SUPER) || Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_SUPER);

        if (io.WantSetMousePos)
        {
            Raylib.SetMousePosition((int)io.MousePos.X, (int)io.MousePos.Y);
        }
        else
        {
            io.MousePos = Raylib.GetMousePosition();
        }

        io.MouseDown[0] = Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT);
        io.MouseDown[1] = Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_RIGHT);
        io.MouseDown[2] = Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_MIDDLE);

        if (Raylib.GetMouseWheelMove() > 0)
            io.MouseWheel += 1;
        else if (Raylib.GetMouseWheelMove() < 0)
            io.MouseWheel -= 1;

        if ((io.ConfigFlags & ImGuiConfigFlags.NoMouseCursorChange) != 0) return;
        
        var imgui_cursor = ImGuiNET.ImGui.GetMouseCursor();
        if (imgui_cursor == CurrentMouseCursor && !io.MouseDrawCursor) return;
        CurrentMouseCursor = imgui_cursor;
        if (io.MouseDrawCursor || imgui_cursor == ImGuiMouseCursor.None)
        {
            Raylib.HideCursor();
        }
        else
        {
            Raylib.ShowCursor();

            if ((io.ConfigFlags & ImGuiConfigFlags.NoMouseCursorChange) != 0 || MouseCursorMap == null) 
                return;
            Raylib.SetMouseCursor(!MouseCursorMap.ContainsKey(imgui_cursor)
                ? MouseCursor.MOUSE_CURSOR_DEFAULT
                : MouseCursorMap[imgui_cursor]);
        }
    }


    private static void FrameEvents()
    {
        var io = ImGuiNET.ImGui.GetIO();

        if (KeyEnumMap != null)
            foreach (var key in KeyEnumMap)
            {
                io.KeysDown[(int) key] = Raylib.IsKeyDown(key);
            }

        var pressed = (uint)Raylib.GetCharPressed();
        while (pressed != 0)
        {
            io.AddInputCharacter(pressed);
            pressed = (uint)Raylib.GetCharPressed();
        }
    }

    public static void Begin()
    {
        ImGuiNET.ImGui.SetCurrentContext(ImGuiContext);

        NewFrame();
        FrameEvents();
        ImGuiNET.ImGui.NewFrame();
    }

    private static void EnableScissor(float x, float y, float width, float height)
    {
        rlEnableScissorTest();
        rlScissor((int)x, Raylib.GetScreenHeight() - (int)(y + height), (int)width, (int)height);
    }

    private static void TriangleVert(ImDrawVertPtr idx_vert)
    {
        var c = BitConverter.GetBytes(idx_vert.col);

        rlColor4ub(c[0], c[1], c[2], c[3]);

        rlTexCoord2f(idx_vert.uv.X, idx_vert.uv.Y);
        rlVertex2f(idx_vert.pos.X, idx_vert.pos.Y);
    }

    private static void RenderTriangles(uint count, uint indexStart, ImVector<ushort> indexBuffer, ImPtrVector<ImDrawVertPtr> vertBuffer, IntPtr texturePtr)
    {
        if (count < 3)
            return;

        uint textureId = 0;
        if (texturePtr != IntPtr.Zero)
            textureId = (uint)texturePtr.ToInt32();

        rlBegin(RL_TRIANGLES);
        rlSetTexture(textureId);

        for (int i = 0; i <= (count - 3); i += 3)
        {
            if (rlCheckRenderBatchLimit(3))
            {
                rlBegin(RL_TRIANGLES);
                rlSetTexture(textureId);
            }

            var indexA = indexBuffer[(int)indexStart + i];
            var indexB = indexBuffer[(int)indexStart + i + 1];
            var indexC = indexBuffer[(int)indexStart + i + 2];

            var vertexA = vertBuffer[indexA];
            var vertexB = vertBuffer[indexB];
            var vertexC = vertBuffer[indexC];

            TriangleVert(vertexA);
            TriangleVert(vertexB);
            TriangleVert(vertexC);
        }
        rlEnd();
    }

    private delegate void Callback(ImDrawListPtr list, ImDrawCmdPtr cmd);

    private static void RenderData()
    {
        rlDrawRenderBatchActive();
        rlDisableBackfaceCulling();

        var data = ImGuiNET.ImGui.GetDrawData();

        for (var l = 0; l < data.CmdListsCount; l++)
        {
            var commandList = data.CmdListsRange[l];

            for (var cmdIndex = 0; cmdIndex < commandList.CmdBuffer.Size; cmdIndex++)
            {
                var cmd = commandList.CmdBuffer[cmdIndex];

                EnableScissor(cmd.ClipRect.X - data.DisplayPos.X, cmd.ClipRect.Y - data.DisplayPos.Y, cmd.ClipRect.Z - (cmd.ClipRect.X - data.DisplayPos.X), cmd.ClipRect.W - (cmd.ClipRect.Y - data.DisplayPos.Y));
                if (cmd.UserCallback != IntPtr.Zero)
                {
                    var cb = Marshal.GetDelegateForFunctionPointer<Callback>(cmd.UserCallback);
                    cb(commandList, cmd);
                    continue;
                }

                RenderTriangles(cmd.ElemCount, cmd.IdxOffset, commandList.IdxBuffer, commandList.VtxBuffer, cmd.TextureId);

                rlDrawRenderBatchActive();
            }
        }
        rlSetTexture(0);
        rlDisableScissorTest();
        rlEnableBackfaceCulling();
    }

    public static void End()
    {
        ImGuiNET.ImGui.SetCurrentContext(ImGuiContext);
        ImGuiNET.ImGui.Render();
        RenderData();
    }

    public static void Shutdown()
    {
        Raylib.UnloadTexture(FontTexture);
    }

    public static void Image(Texture image)
    {
        ImGuiNET.ImGui.Image(new IntPtr(image.id), new Vector2(image.width, image.height));
    }

    public static void ImageSize(Texture image, int width, int height)
    {
        ImGuiNET.ImGui.Image(new IntPtr(image.id), new Vector2(width, height));
    }

    public static void ImageSize(Texture image, Vector2 size)
    {
        ImGuiNET.ImGui.Image(new IntPtr(image.id), size);
    }

    public static void ImageRect(Texture image, int destWidth, int destHeight, Rectangle sourceRect)
    {
        var uv0 = new Vector2();
        var uv1 = new Vector2();

        if (sourceRect.width < 0)
        {
            uv0.X = -(sourceRect.x / image.width);
            uv1.X = uv0.X - Math.Abs(sourceRect.width) / image.width;
        }
        else
        {
            uv0.X = sourceRect.x / image.width;
            uv1.X = uv0.X + sourceRect.width / image.width;
        }

        if (sourceRect.height < 0)
        {
            uv0.Y = -(sourceRect.y / image.height);
            uv1.Y = uv0.Y - Math.Abs(sourceRect.height) / image.height;
        }
        else
        {
            uv0.Y = sourceRect.y / image.height;
            uv1.Y = uv0.Y + sourceRect.height / image.height;
        }

        ImGuiNET.ImGui.Image(new IntPtr(image.id), new Vector2(destWidth, destHeight), uv0, uv1);
    }
}