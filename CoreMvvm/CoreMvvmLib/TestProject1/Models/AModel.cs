using TestProject1.Interface;

namespace TestProject1.Model
{

    public class AModel : IAModel
    {
        IBModel bModel;
        #region Constructor
        public AModel(IBModel bModel)
        {
            this.bModel = bModel;
            WeakReferenceMessenger.Default.Register<IBModel, string>(bModel, OnReceive);            
        }

        private void OnReceive(IBModel model, string arg2)
        {
            if(model != null)
            {
                var test = arg2;
                if(test != null)
                {

                }
            }
        }
        #endregion

        #region Property
        public string Test { get; } = "Test1";
        #endregion
    }
}
