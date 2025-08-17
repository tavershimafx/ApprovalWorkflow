using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ApprovalSystem.Helpers;
using ApprovalSystem.Interfaces;
using ApprovalSystem.Models;

namespace ApprovalSystem.Dtos
{
    public class ApprovalTypeItem
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public byte NumberOfSteps { get; set; }

        public static ApprovalTypeItem FromApprovalType(ApprovalType apr, byte stepCount)
        {
            return new ApprovalTypeItem
            {
                Id = apr.Id,
                Name = apr.Name,
                Description = apr.Description,
                NumberOfSteps = stepCount
            };
        }
    }

    public class ApprovalTypeDetails
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EntityStatus Status { get; set; }

        public IEnumerable<ApprovalStepDto> Steps { get; set; }
    }
    public class ApprovalStepDto
    {
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public byte Order { get; set; }

        [Required]
        public long RoleId { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ApprovalStepRule Rule { get; set; }

        public byte NumberOfUsers { get; set; }

        public IEnumerable<long> XUserIds { get; set; }

        public static ApprovalStepDto FromStep(ApprovalStep step) 
        {
            return new ApprovalStepDto
            {
                Id = step.Id,
                Name = step.Name,
                Order = step.Order,
                RoleId = step.RoleId,
                Rule = step.Rule,
                NumberOfUsers = step.NumberOfUsers,
                XUserIds = step.GetData<IEnumerable<long>>(TypeConstants.StepUserIdentifier)
            };
        }
    }
}
