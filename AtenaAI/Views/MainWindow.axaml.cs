using atena;
using AtenaAI.EventHandlers;
using AtenaAI.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AtenaAI.Views;

// main ui class
// if you wanna make change to the ui based on any event
// first make a method in this file then add that to the appropriate event in Atena.Event
public partial class MainWindow : Window
{
    private Window? _chatWindow;
    private bool isChatWindowOpen = false;
    private MainViewModel mainWindowViewModel;
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
        //UIEventRouter.instance.UI_OnServiceStopped += RemoveActiveServiceNameFromUI;

        Config.ConfigData configData = Config.Instance.Data;

        this.Width = configData.mainWindowWidth;
        this.Height = configData.mainWindowHeight;

        this.Position = new Avalonia.PixelPoint(configData.mainWindowPosX, configData.mainWindowPosY);

        mainWindowViewModel = new MainViewModel();
        this.DataContext = mainWindowViewModel;

        if (_chatWindow == null)
        {
            _chatWindow = new ChatWindow();
            showChatWindow();
        }
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

    private Border AddNotifications(string content)
    {

        //    <Border Width = "200"
        //        Height="50"
        //        Background="Black"
        //        CornerRadius="20">
        //    <StackPanel Orientation = "Horizontal" VerticalAlignment="Center">
        //        <TextBlock Padding = "10"
        //                   VerticalAlignment="Center"
        //                   Foreground="White"
        //                   Text="Recording Audio"/>
        //        <Border Width = "15"
        //                Height="15"
        //                Background="Red"
        //                CornerRadius="50"/>
        //    </StackPanel>
        //</Border>

        Config.ConfigData confData = Config.Instance.Data;

        // notification border
        Border mainBorder = new Border();
        mainBorder.Height = confData.notificationBorderHeight;
        mainBorder.Width = confData.notificationBorderWidth;
        mainBorder.Background = Brush.Parse(confData.notificationBorderBackground);
        mainBorder.CornerRadius = Avalonia.CornerRadius.Parse(confData.notificatonBorderCornerRadius.ToString());

        // stack panel inside the border
        StackPanel stackPanel = new StackPanel();
        stackPanel.Orientation = Enum.Parse<Orientation>(confData.notificationStackPanelOrientation);
        stackPanel.VerticalAlignment = Enum.Parse<VerticalAlignment>(confData.notificationStackPanelVertialAlignment);

        // text block
        TextBlock textBlock = new TextBlock();
        textBlock.Padding = new Thickness(confData.notificationTextBlockPadding);
        textBlock.VerticalAlignment = Enum.Parse<VerticalAlignment>(confData.notificationTextBlockVerticialAlignment);
        textBlock.Foreground = Brush.Parse(confData.notificationTextBlockForeground);
        textBlock.Text = content;

        // small border (dot)
        Border smallBorder = new Border();
        smallBorder.Width = confData.notificationTextBlockBorderWidth;
        smallBorder.Height = confData.notificationTextBlockBorderHeight;
        smallBorder.Background = Brush.Parse(confData.notificationTextBlockBorderBackground);
        smallBorder.CornerRadius = Avalonia.CornerRadius.Parse(confData.notificationTextBlockBorderCornerRadius.ToString());

        // add elements
        stackPanel.Children.Add(textBlock);
        stackPanel.Children.Add(smallBorder);
        mainBorder.Child = stackPanel;

        return mainBorder;
    }
    private void AddNewActiveServiceNameToUI(string serviceName)
    {

        if (Body_Layout_Container_Scr_Content == null)
        {
            Log.Err("Body_Layout_Container_Scr_Content is NULL bro");
            return;
        }

        Log.Info("Started service {0}", serviceName);

        Border notification = AddNotifications(serviceName);

        Body_Layout_Container_Scr_Content.Children.Add(notification);
    }

    private void showChatWindow()
    {
        if (!isChatWindowOpen)
        {
            _chatWindow?.Show();
            mainWindowViewModel.ChatShowHideButtonContent = "Hide chat";
        }
        else
        {
            _chatWindow?.Hide();
            mainWindowViewModel.ChatShowHideButtonContent = "Show chat";
        }
        
        isChatWindowOpen = !isChatWindowOpen;
    }

    public void OnShowChatWindowClick(object? sender, RoutedEventArgs args)
    {
        //Services.Instance?.DispatchService(new atena.ServiceType.ListenDeskAudio());

        showChatWindow();
        //MakeWindowClickThrough(_chatWindow);
    }

    //public void OnStopAudioClick(object? sender, RoutedEventArgs args)
    //{
    //    Services.Instance?.DispatchService(new atena.ServiceType.StopListenDesktop());

    //    if(isChatWindowOpen)
    //    {
    //        _chatWindow?.Hide();
    //        isChatWindowOpen = false;
    //    }
    //}
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
