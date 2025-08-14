using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ApprovalSystem.Models;

namespace ApprovalSystem.Dtos
{
    public class AddRoleDto
    {
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public string ConcurrencyStamp { get; set; }
    }

    public class RoleListDto
    {
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ApprovalStatus ApprovalStatus { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EntityStatus Status { get; set; }

        public static RoleListDto FromRole(Role role)
        {
            return new RoleListDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                ApprovalStatus = role.ApprovalStatus,
                Status = role.Status
            };
        }
    }
}
