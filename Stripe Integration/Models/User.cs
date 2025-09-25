using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stripe_Integration.Models
{
    public class User
    {
        [Key, MaxLength(150)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string B2CSubID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public virtual ICollection<UserPlan> Plans { get; set; }
    }
}
