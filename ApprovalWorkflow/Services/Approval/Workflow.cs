using ApprovalSystem.Data;
using ApprovalSystem.Dtos;
using ApprovalSystem.Helpers;
using ApprovalSystem.Interfaces;
using ApprovalSystem.Models;
using ApprovalSystem.Types;
using Microsoft.EntityFrameworkCore;

namespace ApprovalSystem.Services
{
    public class Workflow : IWorkflow
    {
        private readonly IRepository<long, ApprovalRequest> _requestRepo;
        private readonly IRepository<long, ApprovalType> _typeRepo;
        private readonly IRepository<long, ApprovalStep> _stepRepo;
        private readonly IRepository<long, ApprovalItem> _itemRepo;
        private readonly IRepository<long, UserRole> _userRoleRepo;
        private readonly IRepository<string, ApprovalHash> _hashRepo;
        private readonly IServiceProvider _serviceProvider;

        public Workflow(IRepository<long, ApprovalRequest> requestRepo, IRepository<long, ApprovalType> typeRepo,
            IRepository<long, ApprovalStep> stepRepo, IRepository<long, ApprovalItem> itemRepo, IServiceProvider serviceProvider,
            IRepository<string, ApprovalHash> hashRepo, IRepository<long, UserRole> userRoleRepo)
        {
            _requestRepo = requestRepo;
            _typeRepo = typeRepo;
            _stepRepo = stepRepo;
            _itemRepo = itemRepo;
            _serviceProvider = serviceProvider;
            _hashRepo = hashRepo;
            _userRoleRepo = userRoleRepo;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="comment"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public TaskResult ApproveItem(long itemId, string comment, long userId)
        {
            var result = ExecuteApprovalAction(itemId, comment, userId, ApprovalItemStatus.Approved);
            if (result.Succeeded)
            {
                if (result.Value.IsLastItemForStep)
                {
                    // if its final step trigger final approval
                    if (result.Value.Item.ApprovalStepId == result.Value.TypeSteps.Last().Id)
                    {
                        var standard = GetStandardAndHash(result.Value.CurrentStep.ApprovalType.FullImplementingInterface, result.Value.Item.ApprovalRequest.EntityId);

                        // trigger last step approved, send hash to client
                        standard.Item1.OnApproved(result.Value.Item.ApprovalRequest.EntityId, standard.Item2);
                        return TaskResult.Ok("Approval has been confirmed for the current item and approval request completed.");
                    }

                    var nextStep = result.Value.TypeSteps.FirstOrDefault(k => k.Order == result.Value.CurrentStep.Order + 1);
                    var started = StartApprovalForStep(nextStep, result.Value.Item.ApprovalRequestId);
                    if (started.Succeeded)
                    {
                        var standard = GetStandard(result.Value.CurrentStep.ApprovalType.FullImplementingInterface);
                        standard.OnStepChanged(result.Value.Item.ApprovalRequest.EntityId, nextStep.Order, result.Value.TypeSteps.Count());
                        return TaskResult.Ok("Approval has been confirmed for the current item. Next step has automatically been triggered.");
                    }

                    return started;
                }

                return TaskResult.Ok("Approval has been confirmed for the current item.");
            }

            return result;
        }

        public TaskResult UpdateItem(long itemId, object newState, long userId)
        {
            var item = _itemRepo.AsQueryable(x => x.Id == itemId)
                .Include(n => n.ApprovalRequest).ThenInclude(n => n.ApprovalType)
                .Include(n => n.ApprovalStep)
                .FirstOrDefault();

            if (item == null)
            {
                throw new InvalidOperationException("Approval item not found");
            }

            var steps = _stepRepo.AsQueryable(n => n.ApprovalTypeId == item.ApprovalRequest.ApprovalTypeId)
                .Include(n => n.ApprovalType)
                .AsNoTracking()
                .OrderBy(k => k.Order);

            var prevStep = steps.FirstOrDefault(k => k.Order == item.ApprovalStep.Order - 1);
            if (prevStep == null)
            {
                return TaskResult.Fail("Cannot send back the current item to the previous step because it is from the first step");
            }

            var privilegeResult = HasPrivilegeForStep(item, item.ApprovalStep, userId);
            if (!privilegeResult.Succeeded)
            {
                return privilegeResult;
            }

            if (HasApprovalForStep(item.ApprovalStepId, item.ApprovalRequestId, userId))
            {
                return TaskResult.Fail("Cannot send back the requested item because you have already " +
                    "performed an action on this step.");
            }

            var standard = GetStandard(item.ApprovalRequest.ApprovalType.FullImplementingInterface);
            var updateResult = standard.UpdateEntity(item.ApprovalRequest.EntityId, newState, userId);
            if(updateResult.Succeeded)
            {
                return TaskResult.Ok("Request item updated successfully");
            }

            return updateResult;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public TaskResult CancelApproval(long requestId, long userId)
        {
            var request = _requestRepo.AsQueryable(x => x.Id == requestId)
                .Include(n => n.ApprovalType)
                .FirstOrDefault();
            if (request == null)
            {
                throw new InvalidOperationException("Approval request not found");
            }

            request.RequestStatus = ApprovalItemStatus.Terminated;

            var allPending = _itemRepo.AsQueryable(n => n.ApprovalRequestId == requestId && n.ApprovalStatus == ApprovalItemStatus.Pending).ToList();
            allPending.ForEach(x => {
                x.ApprovalStatus = ApprovalItemStatus.Terminated;
                x.AuthoringUserId = userId;
            });
            _itemRepo.UpdateRange(allPending);
            _itemRepo.SaveChanges();

            var standard = GetStandardAndHash(request.ApprovalType.FullImplementingInterface, request.EntityId);
            standard.Item1.OnRejected(request.EntityId, standard.Item2);

            return TaskResult.Ok("Approval process has been terminated for this item.");
        }

        public TaskResult<ApprovalRequestDetails> GetApprovalDetails(long requestId)
        {
            var request = _requestRepo.AsQueryable(x => x.Id == requestId)
               .Include(n => n.ApprovalType)
               .FirstOrDefault();

            if (request == null)
            {
                throw new InvalidOperationException("Approval request not found");
            }

            var standard = GetStandard(request.ApprovalType.FullImplementingInterface);
            bool shouldGetPrev = request.RequestAction != RequestAction.New;
            var itemData = standard.GetApprovingEntity<IApprovingEntity>(request.EntityId, shouldGetPrev);

            var curStep = _stepRepo.FirstOrDefault(x => x.ApprovalTypeId == request.ApprovalTypeId && x.Order == request.CurrentStep);
            var details = new ApprovalRequestDetails
            {
                Id = request.Id,
                RequestAction = request.RequestAction,
                CurrentStep = request.CurrentStep,
                ApprovalType = request.ApprovalType.Name,
                StepRule = curStep.Rule,
                NewState = itemData.NewState,
                PreviousState = itemData.PreviousState
            };

            details.History = _itemRepo.AsQueryable(n => n.ApprovalRequestId == request.Id)
                .OrderBy(k => k.ApprovalStep.Order)
                .Select(n => new ApprovalHistoryItem
                {
                    ApprovingUser = n.AuthoringUser.FullName,
                    Status = n.ApprovalStatus,
                    Comments = n.Comments,
                    DatePerformed = n.LastUpdated,
                    DateRequested = n.DateCreated,
                    Step = n.ApprovalStep.Order
                });

            return TaskResult<ApprovalRequestDetails>.Ok(details);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public TaskResult<IEnumerable<ApprovalRequestItem>> GetApprovals()
        {
            var requests = _requestRepo.AsQueryable()
                .Select(n => new ApprovalRequestItem
                {
                    Id = n.Id,
                    ApprovalType = n.ApprovalType,
                    RequestAction = n.RequestAction,
                    CurrentStep = n.CurrentStep,
                    RequestStatus = n.RequestStatus,
                });

            return TaskResult<IEnumerable<ApprovalRequestItem>>.Ok(requests);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="comment"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public TaskResult RejectItem(long itemId, string comment, long userId)
        {
            var result = ExecuteApprovalAction(itemId, comment, userId, ApprovalItemStatus.Rejected);
            if (!result.Succeeded)
            {
                return result;
            }

            // should rejecting one item reject the entire approval
            var standard = GetStandardAndHash(result.Value.CurrentStep.ApprovalType.FullImplementingInterface, result.Value.Item.ApprovalRequest.EntityId);
            standard.Item1.OnRejected(result.Value.Item.ApprovalRequest.EntityId, standard.Item2);

            return TaskResult.Ok("Approval has been rejected for the current item.");
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public TaskResult SendBackStep(long itemId, string comment, long userId)
        {
            var item = _itemRepo.AsQueryable(x => x.Id == itemId)
                .Include(n => n.ApprovalRequest)
                .Include(n => n.ApprovalStep)
                .FirstOrDefault();

            if (item == null)
            {
                throw new InvalidOperationException("Approval item not found");
            }

            var steps = _stepRepo.AsQueryable(n => n.ApprovalTypeId == item.ApprovalRequest.ApprovalTypeId)
                .Include(n => n.ApprovalType)
                .AsNoTracking()
                .OrderBy(k => k.Order);

            var prevStep = steps.FirstOrDefault(k => k.Order == item.ApprovalStep.Order - 1);
            if (prevStep == null)
            {
                return TaskResult.Fail("Cannot send back the current item to the previous step because it is from the first step");
            }

            var privilegeResult = HasPrivilegeForStep(item, item.ApprovalStep, userId);
            if (!privilegeResult.Succeeded)
            {
                return privilegeResult;
            }

            if (HasApprovalForStep(item.ApprovalStepId, item.ApprovalRequestId, userId))
            {
                return TaskResult.Fail("Cannot send back the requested item because you have already " +
                    "performed an action on this step.");
            }

            var allPending = _itemRepo.AsQueryable(n => n.ApprovalRequestId == item.ApprovalRequestId && n.ApprovalStatus == ApprovalItemStatus.Pending).ToList();
            allPending.ForEach(x => x.ApprovalStatus = ApprovalItemStatus.SentBack);
            _itemRepo.UpdateRange(allPending);
            _itemRepo.SaveChanges();

            var started = StartApprovalForStep(prevStep, item.ApprovalRequestId);
            if (started.Succeeded)
            {
                var standard = GetStandard(prevStep.ApprovalType.FullImplementingInterface);
                standard.OnStepChanged(item.ApprovalRequest.EntityId, prevStep.Order, steps.Count());
                return TaskResult.Ok("Approval has been confirmed for the current item. Next step has automatically been triggered.");
            }

            return started;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="implementingInterface"></param>
        /// <param name="entityId"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public TaskResult StartApproval<T>(T implementingInterface, long entityId, RequestAction action) where T : IApprovalStandard
        {
            var tp = _typeRepo.FirstOrDefault(n => n.FullImplementingInterface == implementingInterface.GetType().FullName);
            if (tp == null)
            {
                throw new ArgumentException($"Cannot find the approval type registered for the implementing interface. {typeof(T).FullName}. " +
                    $"Ensure this approval type is registered at application startup.");
            }

            var prevApprove = _requestRepo.FirstOrDefault(n => n.ApprovalTypeId == tp.Id && n.EntityId == entityId);
            if (prevApprove != null)
            {
                return TaskResult.Fail("Cannot start approval for this item because it has already undergone approval");
            }

            var step = _stepRepo.FirstOrDefault(n => n.ApprovalTypeId == tp.Id && n.Order == 1);
            if (step == null)
            {
                TaskResult.Fail("Cannot start an approval process for the requested entity because it has no registered steps");
            }

            var approval = new ApprovalRequest()
            {
                ApprovalTypeId = tp.Id,
                RequestAction = action,
                EntityId = entityId,
                Status = EntityStatus.Active,
                CurrentStep = step.Order,
            };

            _requestRepo.Insert(approval);
            _requestRepo.SaveChanges();

            var result =  StartApprovalForStep(step, approval.Id);
            if (result.Succeeded)
            {
                TaskResult.Ok("Approval process started for the requested entity");
            }

            return result;
        }

        /// <summary>
        /// Creates the interface and hashId which will be used to notify the approval process initiator of a new action
        /// on the approval request
        /// </summary>
        /// <param name="implInterface"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        private (IApprovalStandard, string) GetStandardAndHash(string implInterface, long entityId)
        {
            var apHash = new ApprovalHash(entityId);
            _hashRepo.Insert(apHash);
            _hashRepo.SaveChanges();

            return (GetStandard(implInterface), apHash.Id);
        }

        /// <summary>
        ///  Creates the interface for a process type which implements <see cref="IApprovalStandard"/>
        /// </summary>
        /// <param name="implInterface"></param>
        /// <returns></returns>
        private IApprovalStandard GetStandard(string implInterface)
        {
            Type tp = Type.GetType(implInterface);
            IApprovalStandard tpStand = _serviceProvider.GetRequiredService(tp) as IApprovalStandard;

            return tpStand;
        }

        /// <summary>
        /// Executes the said action on the requested item if the user passess all necessary checks
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="comment"></param>
        /// <param name="userId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private TaskResult<ApprovalStatic> ExecuteApprovalAction(long itemId, string comment, long userId, ApprovalItemStatus status)
        {
            var item = _itemRepo.AsQueryable(n => n.Id == itemId)
                .Include(n => n.ApprovalRequest)
                .FirstOrDefault();

            if (item == null)
            {
                throw new InvalidOperationException("Cannot find the requested item for approval");
            }

            // ensure a user cannot approve more than one item for a step
            if (HasApprovalForStep(item.ApprovalStepId, item.ApprovalRequestId, userId))
            {
                return TaskResult<ApprovalStatic>.Fail("Cannot approve the requested item because you have already " +
                    "performed an action on this step");
            }

            var steps = _stepRepo.AsQueryable(n => n.ApprovalTypeId == item.ApprovalRequest.ApprovalTypeId)
                .Include(n => n.ApprovalType)
                .AsNoTracking()
                .OrderBy(k => k.Order);

            if (!steps.Any())
            {
                throw new InvalidOperationException("Cannot proceed with the approval because the process currently " +
                    "has no steps attached to it");
            }

            var curStep = steps.FirstOrDefault(k => k.Id == item.ApprovalStepId);

            var privilegeResult = HasPrivilegeForStep(item, curStep, userId);
            if(!privilegeResult.Succeeded)
            {
                return TaskResult<ApprovalStatic>.Fail(privilegeResult.Errors);
            }

            item.Comments = comment;
            item.ApprovalStatus = status;
            item.AuthoringUserId = userId;
            item.ApprovalRequest.RequestStatus = status == ApprovalItemStatus.Approved ? ApprovalItemStatus.Pending : status;
            _itemRepo.SaveChanges();

            bool isLastItem = false;
            
            // If we have sent back items in the workflow, a step can have two set of items for the same rule.
            // In order to know the items which should currently work with, we select the items that
            // were created the same time with the item currently requested for action.
            // This time whould usually be less than 30 seconds
            var allStepItems = _itemRepo.AsQueryable(n => n.ApprovalRequestId == item.ApprovalRequestId 
            && n.ApprovalStepId == curStep.Id && n.DateCreated < item.DateCreated.AddSeconds(30));

            // this will not work if there's a sent back item in the workflow 
            if(allStepItems.All(n => n.ApprovalStatus == ApprovalItemStatus.Approved))
            {
                isLastItem = true;
            }

            var st = new ApprovalStatic(item, curStep, steps, isLastItem);
            return TaskResult<ApprovalStatic>.Ok(st);
        }

        /// <summary>
        /// Determines if a user has the privilege to execute an action on the item on the current step
        /// </summary>
        /// <param name="item"></param>
        /// <param name="curStep"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private TaskResult HasPrivilegeForStep(ApprovalItem item, ApprovalStep curStep, long userId)
        {
            var userInRole = _userRoleRepo.AsQueryable(n => n.RoleId == curStep.RoleId && n.UserId == userId).Any();
            var isNotForUser = curStep.Rule == ApprovalStepRule.SpecificUsers && item.AuthoringUserId != userId;
            if (!userInRole || isNotForUser)
            {
                TaskResult.Fail("Sorry you do not have the privilege to perform this action");
            }

            if (item.ApprovalRequest.RequestStatus == ApprovalItemStatus.Terminated || item.ApprovalStatus == ApprovalItemStatus.Terminated)
            {
                TaskResult.Fail("Cannot proceed with the current action because the approval process for this item has been terminated.");
            }

            // item was worked on by someone else, so it should be them who can maybe reapprove or reject
            if (item.ApprovalStatus != ApprovalItemStatus.Pending && item.AuthoringUserId != userId)
            {
                TaskResult.Fail("This item was previously executed by a different user. If you intend to " +
                    "change its state, ensure you are the same user who previously modified it.");
            }

            return TaskResult.Ok();
        }

        /// <summary>
        /// Starts approval for the specified step
        /// </summary>
        /// <param name="step"></param>
        /// <param name="approvalId"></param>
        /// <returns></returns>
        private TaskResult StartApprovalForStep(ApprovalStep step, long requestId)
        {
            var items = GetItemsForStep(step).ToList();
            items.ForEach(k => k.ApprovalRequestId = requestId);
            items.First().ApprovalRequest.CurrentStep = step.Order;

            _itemRepo.InsertRange(items);
            _itemRepo.SaveChanges();

            // send notifications to the concerned users
            return TaskResult.Ok();
        }

        /// <summary>
        /// Gets all the items which can be created for approval for the given step
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private IEnumerable<ApprovalItem> GetItemsForStep(ApprovalStep step)
        {
            return step.Rule switch
            {
                ApprovalStepRule.AllUsersFromRole => GetItemsForAllUsers(step),
                ApprovalStepRule.AnyUserFromRole => GetItemForAnyUserFromRole(step),
                ApprovalStepRule.AnyXUsersFromRole => GetItemsForXUsersFromRole(step),
                ApprovalStepRule.SpecificUsers => GetItemsForSpecificUsers(step),
                _ => throw new InvalidOperationException("Cannot find a matching rule for the specified step. Are you missing an implementation?"),
            };
        }

        /// <summary>
        /// Creates approval items for all users assigned to the role specified by the step at
        /// the given point in time
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        private IEnumerable<ApprovalItem> GetItemsForAllUsers(ApprovalStep step)
        {
            var usrRoleRepo = _serviceProvider.GetRequiredService<IRepository<long, UserRole>>();
            var items = usrRoleRepo.AsQueryable(n => n.RoleId == step.RoleId)
                .Select(n => new ApprovalItem
                {
                    ApprovalStatus = ApprovalItemStatus.Pending,
                    ApprovalStep = step,
                    Status = EntityStatus.Active,
                });

            return items;
        }

        /// <summary>
        /// Creates an approval item for the step such that any user from the given step role
        /// can approve
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        private static IEnumerable<ApprovalItem> GetItemForAnyUserFromRole(ApprovalStep step)
        {
            var item = new ApprovalItem
            {
                ApprovalStatus = ApprovalItemStatus.Pending,
                ApprovalStep = step,
                Status = EntityStatus.Active,
            };

            return [item];
        }

        /// <summary>
        /// Create x number of approval items for any x users from the role specified in the step
        /// to approve
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        private static IEnumerable<ApprovalItem> GetItemsForXUsersFromRole(ApprovalStep step)
        {
            IEnumerable<ApprovalItem> items = [];
            for(byte i = 0; i < step.NumberOfUsers; i++)
            {
                var item = new ApprovalItem
                {
                    ApprovalStatus = ApprovalItemStatus.Pending,
                    ApprovalStep = step,
                    Status = EntityStatus.Active,
                };

                items = [..items, item];
            }

            return items;
        }

        /// <summary>
        /// Creates approval items for the specified users in the step to approve
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        private IEnumerable<ApprovalItem> GetItemsForSpecificUsers(ApprovalStep step)
        {
            var usrRoleRepo = _serviceProvider.GetRequiredService<IRepository<long, UserRole>>();
            var stepUsers = step.GetData<IEnumerable<long>>(TypeConstants.StepUserIdentifier);
            var items = usrRoleRepo.AsQueryable(n => n.RoleId == step.RoleId)
                .Where(n => stepUsers.Any(k => k == n.UserId))
                .Select(n => new ApprovalItem
                {
                    ApprovalStatus = ApprovalItemStatus.Pending,
                    ApprovalStep = step,
                    Status = EntityStatus.Active,
                    AuthoringUserId = n.UserId
                });

            return items;
        }

        /// <summary>
        /// Determines if a user has already approved an item for this step
        /// </summary>
        /// <param name="stepId"></param>
        /// <param name="requestId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private bool HasApprovalForStep(long stepId, long requestId, long userId)
        {
            var items = _itemRepo.AsQueryable(n => n.ApprovalRequestId == requestId && n.ApprovalStepId == stepId);
            return items.Any(p => p.AuthoringUserId == userId && p.ApprovalStatus != ApprovalItemStatus.Pending);
        }

        private class ApprovalStatic
        {
            public ApprovalItem Item { get; set; }

            public ApprovalStep CurrentStep { get; set; }

            /// <summary>
            /// For last step to be true, it means all steps have been approved
            /// </summary>
            public bool IsLastItemForStep { get; set; }

            public IEnumerable<ApprovalStep> TypeSteps { get; set; }

            public ApprovalStatic(ApprovalItem item, ApprovalStep step, IEnumerable<ApprovalStep> typeSteps, bool isLastItem)
            {
                Item = item; 
                CurrentStep = step;
                TypeSteps = typeSteps;
                IsLastItemForStep = isLastItem;
            }
        }
    }
}
