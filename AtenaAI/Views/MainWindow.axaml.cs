using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Diagnostics;

namespace AtenaAI.Views;

// main ui class
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

#if DEBUG
        DebugBanner.Text = "DEBUG BUILD!!!!";
#endif
    }
}
