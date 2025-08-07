using atena;
using AtenaAI.EventHandlers;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;

namespace AtenaAI.Views;

// main ui class
// if you wanna make change to the ui based on any event
// first make a method in this file then add that to the appropriate event in Atena.Event
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

#if DEBUG
        Banner_Text.Text = "DEBUG BUILD!!!!";
#else
        MainLayout.Children.Remove(Banner);
#endif
        if(UIEventRouter.instance == null)
        {
            Log.Err("UIEventRouter.instance does not exists");
            return;
        }

        UIEventRouter.instance.UI_OnServiceStarted += AddNewActiveServiceNameToUI;
        UIEventRouter.instance.UI_OnServiceStopped += RemoveActiveServiceNameFromUI;

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

    public void OnPingClick(object? sender, RoutedEventArgs args)
    {
        Services.Instance?.DispatchService(new atena.ServiceType.Ping());
    }
}
