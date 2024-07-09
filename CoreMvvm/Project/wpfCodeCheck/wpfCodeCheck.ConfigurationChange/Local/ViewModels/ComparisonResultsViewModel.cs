using CoreMvvmLib.Core.Attributes;
using CoreMvvmLib.Core.Components;
using wpfCodeCheck.Shared.Local.Services;

namespace wpfCodeCheck.ConfigurationChange.Local.ViewModels
{
    public partial class ComparisonResultsViewModel : ViewModelBase
    {
        private readonly IBaseService _baseService;

        public ComparisonResultsViewModel(IBaseService baseService)
        {
            _baseService = baseService;
        }
        [RelayCommand]
        private void Export()
        {

        }
    }
}
