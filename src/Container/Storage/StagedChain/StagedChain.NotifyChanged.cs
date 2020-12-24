using System;

namespace Unity.Storage
{
    #region Change Event Handler

    /// <summary>
    /// Represents the method that will handle a changed event
    /// </summary>
    /// <remarks>In normal circumstances the monitoring subscriber does not care what has 
    /// changed. Details of the change are not important, just the fact that change has happened</remarks>
    /// <param name="chain">The chain that has been changed</param>
    /// <param name="type"><see cref="Type"/> this chain is responsible for</param>
    public delegate void StagedChainChagedHandler(object sender, Type type);

    #endregion


    public interface INotifyChainChanged
    {
        event StagedChainChagedHandler ChainChanged;
    }


    public partial class StagedChain<TStageEnum, TStrategyType> : INotifyChainChanged
    {
        #region Change Event

        public event StagedChainChagedHandler? ChainChanged;

        #endregion
    }
}
