using System.ComponentModel.DataAnnotations;
namespace MockingBird.Models
{
    public class User
    {
        [Key]
        public int ID { get; set; }
        [StringLength(30, MinimumLength = 3)]
        [Required]
        public string Name { get; set; }
        [StringLength(15, MinimumLength = 3)]
        [Required]
        public string UserName { get; set; }
        [StringLength(30, MinimumLength = 6)]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string? ImagePath { get; set; }
    }
}
