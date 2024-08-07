﻿using TestProject1.Interface;

namespace TestProject1.Model
{
    public class CModel
    {
        #region Private Property
        private readonly AModel aModel = null;
        private readonly IBModel bModel = null;
        #endregion

        #region Constructor
        public CModel(AModel aModel,
                      IBModel bModel)
        {
            this.aModel = aModel;
            this.bModel = bModel;
        }
        #endregion


        #region Property
        public AModel AModel
        {
            get => aModel;
        }

        public IBModel BModel
        {
            get => bModel;
        }
        #endregion

    }
}
