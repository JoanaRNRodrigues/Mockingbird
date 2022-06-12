using System.ComponentModel.DataAnnotations;

namespace MockingBird.Models
{
    public class UsersViewModel
    {
       
        public int ID { get; set; }
        public string Name { get; set; }
        [StringLength(15, MinimumLength = 3)]
        [Required]
        public string UserName { get; set; }
        [StringLength(30, MinimumLength = 6)]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string Email { get; set; }       
        public string ImagePath { get; set; }
    }
}
