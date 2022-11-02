using System.Drawing;
using CouscousEngine.GUI;
using Drawer_Watcher.Managers;
using ImGuiNET;
using Color = CouscousEngine.Utils.Color;
using Rectangle = Raylib_CsLo.Rectangle;

namespace Drawer_Watcher.Screens;

public class ConnectionScreen : Screen
{
    private ConnectionInfo _connectionInfo = ConnectionInfo.Default;

    private readonly Rectangle _frame;

    private readonly Entry _ipAdress;
    private readonly Entry _password;
    private readonly Button _connect;

    public ConnectionScreen(string nickname)
    {
        // TODO: Temp positions/sizes
        _frame = new Rectangle(215, 85, 850, 550);
        
        _ipAdress = new Entry(new Rectangle(606, 306, 200, 45))
        {
            Placeholder = "127.0.0.1:34827",
            
            BordeThickness = 2f,
            BorderColor =  Color.BLACK,
            CornerRadius = 0.65f,
            
            Label = "IP Address",
            LabelFontSize = 32
        };
        
        _password = new Entry(new Rectangle(608, 368, 200, 45))
        {
            Placeholder = "********",
            
            BordeThickness = 2f,
            BorderColor = Color.BLACK,
            CornerRadius = 0.65f,
            
            Label = "Password",
            LabelFontSize = 32
        };
        
        _connect = new Button(new Rectangle(546, 437, 150, 45))
        {
            Text = "Connect",
            FontSize = 24,
            FontColor = Color.BLACK,

            CornerRadius = 0.65f,
            BorderThickness = 3f,
            BorderColor = Color.BLACK,

            Color = ColorTranslator.FromHtml("#FFBF00"),
            OnButtonClick = (sender, args) =>
            {
                if (_ipAdress.Text == "") _ipAdress.Text = "127.0.0.1:34827";
                var ipPort = _ipAdress.Text.Split(':');
                _connectionInfo.Ip = ipPort[0];
                _connectionInfo.Port = Convert.ToInt32(ipPort[1]);
                // TODO: Handle input error
                NetworkManager.ConnectToServer(_connectionInfo, nickname);
                ScreenManager.NavigateTo(new LobbyScreen());
            }
        };
    }
    
    public override void OnUpdate()
    {
        _rl.DrawRectangleRounded(_frame, 0.1f, 15, Color.WHITE);

        _ipAdress.OnUpdate();
        _password.OnUpdate();
        _connect.OnUpdate();
    }

    public override void OnImGuiUpdate()
    {
        
    }

    public override void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}