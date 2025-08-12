using atena;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AtenaAI.ViewModels
{
	public class ChatViewModel : INotifyPropertyChanged
	{
		private string _defaultText = "### **listening**";
		public string MarkDownText
		{
			get
			{
				return _defaultText;
			}
			set 
			{
				if (_defaultText != value)
				{
					_defaultText = value;
					OnPropertyChanged(nameof(MarkDownText));
				}
			}
		}

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}