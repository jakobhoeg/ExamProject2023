using System.ComponentModel.DataAnnotations;

namespace BackendAPIMongo.Model
{
    public class UserRequest
    {
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
    }

    public class ChangeEmailRequest
    {
        public string CurrentEmail { get; set; }
        public string NewEmail { get; set; }
    }
}
