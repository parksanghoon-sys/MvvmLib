using CoreMvvmLib.Core.Attributes;
using CoreMvvmLib.Core.Components;
using CoreMvvmLib.Core.Messenger;
using CoreMvvmLib.WPF.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wpfCodeCheck.Shared.Local.Models;

namespace wpfCodeCheck.Shared.Local.ViewModels
{
    public partial class CircularProgressBarViewModel : ViewModelBase
    {
        [Property]
        private double _progress;
        public CircularProgressBarViewModel()
        {
            WeakReferenceMessenger.Default.Register<CircularProgressBarViewModel, double>(this, OnReceiveProgress);
        }

        private void OnReceiveProgress(CircularProgressBarViewModel model, double arg2)
        {
            Progress = arg2; 
        }
    }
}
