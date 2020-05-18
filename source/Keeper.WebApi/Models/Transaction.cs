using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace Keeper.WebApi.Models
{
    public class Transaction
    {
        [Required]
        [Key]
        public int Id { get; set; }
        [Required]
        public string Data { get; set; }
        [Required]
        [JsonIgnore]
        public IdentityUser Profile { get; set; }
        [Required]
        [JsonIgnore]
        public string ProfileId { get; set; }
    }
}