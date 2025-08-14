using atena;
using AtenaAI.EventHandlers;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using AtenaAI.ViewModels;

namespace AtenaAI.Views;

// main ui class
// if you wanna make change to the ui based on any event
// first make a method in this file then add that to the appropriate event in Atena.Event
public partial class MainWindow : Window
{
    private Window? _chatWindow;
    private bool isChatWindowOpen = false;
    public MainWindow()
    {
        InitializeComponent();

        if(!Design.IsDesignMode)
        {
            Init();
        }
    }

    private void Init()
    {
#if DEBUG
        Banner_Text.Text = "Atena Alpha build";
#else
        MainLayout.Children.Remove(Banner);
#endif
        if (UIEventRouter.instance == null)
        {
            Log.Err("UIEventRouter.instance does not exists");
            return;
        }

        UIEventRouter.instance.UI_OnServiceStarted += AddNewActiveServiceNameToUI;
        UIEventRouter.instance.UI_OnServiceStopped += RemoveActiveServiceNameFromUI;

        Config.ConfigData configData = Config.Instance.Data;

        this.Width = configData.mainWindowWidth;
        this.Height = configData.mainWindowHeight;

        this.Position = new Avalonia.PixelPoint(configData.mainWindowPosX, configData.mainWindowPosY);
    }

    private void RemoveActiveServiceNameFromUI(string serviceName)
    {
        var toRemove = Body_Layout_Container_Scr_Content.Children
            .OfType<TextBlock>()
            .FirstOrDefault(tb => tb.Name == serviceName);

        if (toRemove == null)
        {
            Log.Err("No TextBlock with name {0}", serviceName);
            return;
        }

        Body_Layout_Container_Scr_Content.Children.Remove(toRemove);
    }
    private void AddNewActiveServiceNameToUI(string serviceName)
    {

        if (Body_Layout_Container_Scr_Content == null)
        {
            Log.Err("Body_Layout_Container_Scr_Content is NULL bro");
            return;
        }

        TextBlock serviceNameTextBlock = new TextBlock();
        serviceNameTextBlock.Text = Config.Instance.GetServiceFilePathFromName(serviceName);
        serviceNameTextBlock.FontSize = 15;
        serviceNameTextBlock.Name = serviceName;
        serviceNameTextBlock.Foreground = Brushes.White;

        Body_Layout_Container_Scr_Content.Children.Add(serviceNameTextBlock);
    }

    public void OnStartAudioClick(object? sender, RoutedEventArgs args)
    {
        Services.Instance?.DispatchService(new atena.ServiceType.ListenDeskAudio());

        if(_chatWindow == null)
        {
        }

        if(!isChatWindowOpen)
        {
            _chatWindow = new ChatWindow();
            _chatWindow.Show();
            isChatWindowOpen = true;
        }
        //MakeWindowClickThrough(_chatWindow);
    }

    public void OnStopAudioClick(object? sender, RoutedEventArgs args)
    {
        Services.Instance?.DispatchService(new atena.ServiceType.StopListenDesktop());

        if(isChatWindowOpen)
        {
            _chatWindow?.Close();
            isChatWindowOpen = false;
        }
    }
    public void MakeWindowClickThrough(Window window)
    {
        IntPtr hwnd = IntPtr.Zero;

        if (window.TryGetPlatformHandle() is { } platformHandle)
        {
            hwnd = platformHandle.Handle;
        }
        else
        {
            Log.Err("Failed to get platform handle for window.");
            return;
        }

        const int GWL_EXSTYLE = -20;
        const int WS_EX_TRANSPARENT = 0x00000020;
        const int WS_EX_LAYERED = 0x00080000;

        int exStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
        SetWindowLong(hwnd, GWL_EXSTYLE, exStyle | WS_EX_TRANSPARENT | WS_EX_LAYERED);
    }

    [DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
}
