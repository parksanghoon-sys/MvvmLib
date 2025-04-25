namespace CoreMvvmLib.Core.Services.DialogService
{
    public interface IModalDialogViewModel
    {
        public bool DialogResult { get; }
        public string Title { get; set; }
    }
}
