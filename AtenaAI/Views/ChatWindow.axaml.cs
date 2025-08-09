using System;
using System.Runtime.InteropServices;
using atena;
using AtenaAI.EventHandlers;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;

namespace AtenaAI.Views
{
    public partial class ChatWindow : Window
    {
        public ChatWindow()
        {
            InitializeComponent();

            
            if(!Design.IsDesignMode)
            {
                Init();
            }
        }

        private void Init()
        {
            UIEventRouter.instance.UI_OnChatTextRecieved += OnRecvChat;

            Config.ConfigData configData = Config.Instance.Data;

            this.Width = configData.chatWindowWidth;
            this.Height = configData.chatWindowHeight;

            this.Position = new Avalonia.PixelPoint(configData.chatWindowPosX, configData.chatWindowPosY);
        }
         
        public void OnRecvChat(string data, atenaNest.DataType type)
        {
            string prefixUser = "[Atena] : ";
            IBrush textColor = Brush.Parse(Config.Instance.Data.atenaTextColor);

            if(type == atenaNest.DataType.NormalText)
            {
                prefixUser = "[User]  : ";
                textColor = Brush.Parse(Config.Instance.Data.userTextColor);
            }

            TextBlock textBlock = new TextBlock();
            textBlock.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
            textBlock.Text = prefixUser + data;
            textBlock.Foreground = textColor;
            textBlock.TextWrapping = TextWrapping.Wrap;
            
            Chat_Content.Children.Add(textBlock);
            Chat_scrollView.ScrollToEnd();
        }
    }
}
