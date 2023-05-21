using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GetronicsOTPServices.Contexts
{
    public class OTPCode
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [StringLength(30)]
        [Required]
        public string Email { get; set; }

        [StringLength(6)]
        [Required]
        public string Code { get; set; }

        public int AttemptFailCount { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}
