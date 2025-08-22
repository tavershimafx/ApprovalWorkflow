using ApprovalSystem.Data;
using ApprovalSystem.Dtos;
using ApprovalSystem.Helpers;
using ApprovalSystem.Interfaces;
using ApprovalSystem.Models;
using ApprovalSystem.Types;

namespace ApprovalSystem.Services
{
    public class ApprovalSetup : IApprovalSetup
    {
        private readonly IRepository<long, ApprovalType> _typeRepository;
        private readonly IRepository<long, ApprovalStep> _stepRepository;

        public ApprovalSetup(IRepository<long, ApprovalType> typeRepository,
            IRepository<long, ApprovalStep> stepRepository) 
        {
            _typeRepository = typeRepository;
            _stepRepository = stepRepository;
        }

        public TaskResult CreateApprovalType(ApprovalTypeModel model)
        {
            ApprovalType approvalType = new ()
            {
                Name = model.Name,
                Description = model.Description,
                FullImplementingInterface = model.FullImplementingInterface,
                Status = EntityStatus.Active
            };

            var inter = Type.GetType(approvalType.FullImplementingInterface);
            if (inter == null ||(inter.IsInterface && !inter.IsAssignableFrom(typeof(IApprovalStandard))))
            {
                return TaskResult.Fail("Cannot create the approval type because the provided interface " +
                    $"does not implement the {nameof(IApprovalStandard)} interface.");
            }
            
            if(_typeRepository.FirstOrDefault(n => n.FullImplementingInterface.ToUpper() == approvalType.FullImplementingInterface.ToUpper()) != null)
            {
                return TaskResult.Fail("Cannot create the approval type because an approval type with the same interface already exists.");
            }

            _typeRepository.Insert(approvalType);
            _typeRepository.SaveChanges();
            return TaskResult.Ok("Approval type created successfully.");
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="stepIds"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public TaskResult DeleteSteps(long typeId, IEnumerable<long> stepIds)
        {
            var tp = _typeRepository.FirstOrDefault(n => n.Id == typeId);
            if (tp != null)
            {
                var typeSteps = _stepRepository.AsUnFilteredQueryable(n => n.ApprovalTypeId == tp.Id);
                var toDelete = typeSteps.Where(n => stepIds.Any(k => k == n.Id));

                _stepRepository.DeleteRange(toDelete);
                _stepRepository.SaveChanges();
                return TaskResult.Ok("Approval steps have been deleted.");
            }

            return TaskResult<ApprovalTypeDetails>.Fail("Invalid approval id. Not found.");
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>
        public TaskResult<ApprovalTypeDetails> GetApprovalType(long typeId)
        {
            var tp = _typeRepository.FirstOrDefault(n => n.Id == typeId);
            if (tp != null) 
            {
                var details = new ApprovalTypeDetails
                {
                    Description = tp.Description,
                    Id = tp.Id,
                    Name = tp.Name,
                    Status = tp.Status,
                    Steps = _stepRepository.AsUnFilteredQueryable(s => s.ApprovalTypeId == tp.Id).Select(n => ApprovalStepDto.FromStep(n))
                };

                return TaskResult<ApprovalTypeDetails>.Ok(details);
            }

            return TaskResult<ApprovalTypeDetails>.Fail("Invalid approval id. Not found.");
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="implementingInterface"></param>
        /// <returns></returns>
        public TaskResult<ApprovalTypeDetails> GetApprovalType<T>(T implementingInterface) where T : IApprovalStandard
        {
            var tp = _typeRepository.FirstOrDefault(n => n.FullImplementingInterface == implementingInterface.GetType().FullName);
            if (tp != null)
            {
                var details = new ApprovalTypeDetails
                {
                    Description = tp.Description,
                    Id = tp.Id,
                    Name = tp.Name,
                    Status = tp.Status,
                    Steps = _stepRepository.AsUnFilteredQueryable(s => s.ApprovalTypeId == tp.Id).Select(n => ApprovalStepDto.FromStep(n))
                };

                return TaskResult<ApprovalTypeDetails>.Ok(details);
            }

            return TaskResult<ApprovalTypeDetails>.Fail("Invalid approval id. Not found.");
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public TaskResult<IEnumerable<ApprovalTypeItem>> GetApprovalTypes()
        {
            var types = _typeRepository.AsUnFilteredQueryable().
                        Select(x => ApprovalTypeItem.FromApprovalType(x, 
                        (byte)_stepRepository.AsQueryable(x => x.ApprovalTypeId == x.Id).Count()))
                        .AsEnumerable();

            return TaskResult<IEnumerable<ApprovalTypeItem>>.Ok(types);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="steps"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public TaskResult UpdateSteps(long typeId, IEnumerable<ApprovalStepDto> steps)
        {
            var tp = _typeRepository.FirstOrDefault(n => n.Id == typeId);
            if (tp != null)
            {
                var typeSteps = _stepRepository.AsUnFilteredQueryable(n => n.ApprovalTypeId == tp.Id);
                
                var deletedSteps = typeSteps.Where(n => !steps.Any(k => k.Id == n.Id));
                _stepRepository.DeleteRange(deletedSteps);

                var newSteps = steps.Where(n => n.Id == default).Select(k => { 
                    var b = new ApprovalStep()
                    {
                        Id = k.Id,
                        ApprovalTypeId = typeId,
                        Name = k.Name,
                        NumberOfUsers = k.NumberOfUsers,
                        Order = k.Order,
                        RoleId = k.RoleId,
                        Rule = k.Rule
                    };
                    b.SetData(TypeConstants.StepUserIdentifier, k.XUserIds);
                    return b;
                });

                foreach (var step in steps.Where(a => a.Id != default))
                {
                    var old = typeSteps.FirstOrDefault(k => k.Id == step.Id);
                    if (old != null)
                    {
                        old.Order = step.Order;
                        old.RoleId = step.RoleId;
                        old.Rule = step.Rule;
                        old.NumberOfUsers = step.NumberOfUsers;
                        old.Name = step.Name;
                        continue;
                    }

                    throw new InvalidOperationException("Cannot update the step presented because it does not previously exit in the store");
                }

                _stepRepository.InsertRange(newSteps);
                _stepRepository.SaveChanges();
                return TaskResult.Ok("Approval steps have been updated.");
            }

            return TaskResult.Fail("Invalid approval id. Not found.");
        }
    }
}
