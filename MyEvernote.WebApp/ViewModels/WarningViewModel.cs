﻿using MyEvernote.Entities.Messages;

namespace MyEvernote.WebApp.ViewModels
{
    public class WarningViewModel : NotifyViewModelBase<string>
    {
        public WarningViewModel()
        {
            Title = "Uyarı!";
        }
    }
}