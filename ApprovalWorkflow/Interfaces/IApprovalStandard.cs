using ApprovalSystem.Types;

namespace ApprovalSystem.Interfaces
{
    public interface IApprovalStandard
    {
        /// <summary>
        /// Gets the entity which is being requested for approval. If the request was for modification
        /// <paramref name="includePreviousState"/> is set to <see cref="true"/> to inform clients to
        /// include the previous state of the entity before the change was requested.
        /// The type object which is returned in <see cref="IApprovingEntity.NewState"/> and
        /// <see cref="IApprovingEntity.PreviousState"/> should be a model which the client would
        /// expect when the approving officer makes modifications on the entity. This object is sent
        /// back to the client through the <see cref="IApprovalStandard.UpdateEntity(long, object)"/>
        /// method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityId"></param>
        /// <param name="includePreviousState"></param>
        /// <returns></returns>
        T GetApprovingEntity<T>(long entityId, bool includePreviousState = false) where T : class, IApprovingEntity;

        /// <summary>
        /// Updates an item which was sent for approval as requested by the workflow process
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="item"></param>
        /// <param name="userId">The user who wants to make the change</param>
        /// <returns></returns>
        TaskResult UpdateEntity(long entityId, object item, long userId);

        /// <summary>
        /// A callback that is fired when a requested item for approval has been approved
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="hashId"></param>
        void OnApproved(long entityId, string hashId);

        /// <summary>
        /// A callback that is fired when a requested item for approval has been rejected
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="hashId"></param>
        void OnRejected(long entityId, string hashId);

        /// <summary>
        /// A callback that is fired when a step in the workflow for the item requested for
        /// approval has changed to another
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="currentStep"></param>
        /// <param name="totalSteps"></param>
        void OnStepChanged(long entityId, byte currentStep, int totalSteps);
    }
}
