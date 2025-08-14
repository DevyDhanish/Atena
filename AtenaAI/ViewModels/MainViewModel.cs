using AtenaAI.EventHandlers;
using Avalonia.Controls;
using Avalonia.Media;
using System.ComponentModel;

namespace AtenaAI.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private string _ChatShowHideButtonContent = "";

    public string ChatShowHideButtonContent
    {
        get { return _ChatShowHideButtonContent; }
        set
        {
            if(value != _ChatShowHideButtonContent)
            {
                _ChatShowHideButtonContent = value;
                OnPropertyChanged(nameof(ChatShowHideButtonContent));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
