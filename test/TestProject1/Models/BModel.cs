using TestProject1.Interface;

namespace TestProject1.Model
{
    public class BModel : IBModel
    {
        #region Constructor
        public BModel() {
            WeakReferenceMessenger.Default.Send("tes1231231231231t");
        }
        #endregion

        #region Property
        public string Test { get; } = "Test2";
        #endregion
    }
}
