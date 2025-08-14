using atena;
using AtenaAI.EventHandlers;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;
using Markdig;
using Markdown.Avalonia;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using TheArtOfDev.HtmlRenderer.Avalonia;
using AtenaAI.ViewModels;

namespace AtenaAI.Views
{
    public partial class ChatWindow : Window
    {

        private atenaNest.DataType? prevDataType = null;
        private List<string> textBuffer;
        private ChatViewModel chatViewModel;
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
            textBuffer = new List<string>();

            UIEventRouter.instance.UI_OnChatTextRecieved += OnRecvChat;

            Config.ConfigData configData = Config.Instance.Data;

            this.Width = configData.chatWindowWidth;
            this.Height = configData.chatWindowHeight;

            int rel_x = configData.chatWindowPosX + configData.mainWindowWidth;
            int rel_y = configData.chatWindowPosY + (configData.mainWindowPosY + configData.mainWindowHeight);
            this.Position = new Avalonia.PixelPoint(rel_x, configData.chatWindowPosY);

            Chat_Container.Width = this.Width;
            Chat_Container.Height = this.Height;
            Chat_Container.Background = Brush.Parse(configData.chatWindowBackgroundColor);

            chatViewModel = new ChatViewModel();
            this.DataContext = chatViewModel;
        }

        private string removeEmojis(string input)
        {
            // Regex pattern matches most emojis and symbols in Unicode ranges
            string emojiPattern = @"[\u2700-\u27BF]|" +        // Dingbats
                                  @"[\uE000-\uF8FF]|" +        // Private Use Area
                                  @"[\uD83C-\uDBFF\uDC00-\uDFFF]+";  // Surrogates (emojis)

            return Regex.Replace(input, emojiPattern, "");
        }

        private void addToMarkdown()
        {
            if (textBuffer.Count <= 10) return;

            StringBuilder dataStream = new StringBuilder();
            string? prevData = "";

            foreach(string s in textBuffer) dataStream.Append(s);

            string data = dataStream.ToString();
            
            try
            {
                prevData = MarkDown_Content.Markdown;

                //MarkDown_Content.Markdown += data;
                chatViewModel.MarkDownText += data;
                Chat_scrollView.ScrollToEnd();
                
                textBuffer.Clear();

                prevData = MarkDown_Content.Markdown;
            }
            catch (System.InvalidCastException e)
            {
                Log.Err("Caught an Exception {1} with data {0}", e, data);
                MarkDown_Content.Markdown = prevData;
            }
        }

        private void bufferAiText(string data)
        {
            textBuffer.Add(data);
        }

        public void OnRecvChat(string data, atenaNest.DataType type)
        {
            if (type == atenaNest.DataType.Event) return;

            if(type == atenaNest.DataType.AiGenText)
            {
                bufferAiText(removeEmojis(data));
                addToMarkdown();
                return;
            }

            //// if the texting comming is of ai and we already have ai text then append the new text
            //if (prevTextBlock != null && prevDataType == atenaNest.DataType.AiGenText && type == atenaNest.DataType.AiGenText)
            //{
            //    prevTextBlock.FontStyle = FontStyle.Italic;
            //    prevTextBlock.Text += data;

            //    return;
            //}

            string prefixUser = "[Atena] : ";
            IBrush textColor = Brush.Parse(Config.Instance.Data.atenaTextColor);

            if (type == atenaNest.DataType.NormalText)
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

            prevDataType = type;

            Log.Info("Added text block of type {0}", prevDataType);
        }
    }
}
