﻿using CoreMvvmLib.Core.Attributes;
using CoreMvvmLib.Core.Commands;
using CoreMvvmLib.Core.Components;
using CoreMvvmLib.Core.Services.DialogService;
using System.Windows.Input;
using WpfTest1.Views;

namespace WpfTest1.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;
        public MainWindowViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;

            string fieldName = "ExampleField";
            fieldName = char.ToUpper(fieldName[0]) + fieldName.Substring(1);
        }

        [Property]
        private string _TestText = "tt";
        [Property]
        private bool _isChecked = false;
        [Property]
        private bool _TestA = false;
        //public ICommand TestCommand => new RelayCommand(new Action(() =>
        //{
        //    _dialogService.Show(this, "TestDialogView", 300, 200);            
        //}));
        [RelayCommand]
        private void Test()
        {
            _dialogService.Show(this, typeof(TestDialogView), 300, 200);
            IsChecked = !IsChecked;
        }
        public ICommand TestCommandAsync => new RelayCommandAsync(async () =>
        {
            IsChecked = !IsChecked;
        });
        public ICommand Loaded1Command => new RelayCommand(new Action(() =>
        {
            System.Diagnostics.Debug.WriteLine("View Loaded!!");
        }));      

    }
}
