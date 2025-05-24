using System.ComponentModel.DataAnnotations;

namespace CreepyDonut.DTO
{
    public class PaymentRequest
    {
        [Required]
        public int GrossAmount { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }
    }
}
