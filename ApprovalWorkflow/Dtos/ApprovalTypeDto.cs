using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ApprovalSystem.Helpers;
using ApprovalSystem.Interfaces;
using ApprovalSystem.Models;

namespace ApprovalSystem.Dtos
{
    public class ApprovalTypeModel
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public string FullImplementingInterface { get; set; }
    }
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

    public class ApprovalRequestItem
    {
        public long Id { get; set; }

        public ApprovalType ApprovalType { get; set; }

        public byte CurrentStep { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RequestAction RequestAction { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ApprovalItemStatus RequestStatus { get; set; }
    }
    public class ApprovalRequestDetails
    {
        public long Id { get; set; }

        public string ApprovalType { get; set; }

        public byte CurrentStep { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ApprovalStepRule StepRule { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RequestAction RequestAction { get; set; }

        public object PreviousState { get; set; }

        public object NewState { get; set; }

        public IEnumerable<ApprovalHistoryItem> History { get; set; }

    }
    public class ApprovalHistoryItem
    {
        public byte Step { get; set; }

        public string Comments { get; set; }

        public string ApprovingUser { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ApprovalItemStatus Status { get; set; }

        public DateTimeOffset DateRequested { get; set; }

        public DateTimeOffset? DatePerformed { get; set; }
    }
}
