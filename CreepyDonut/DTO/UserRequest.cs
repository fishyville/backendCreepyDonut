namespace CreepyDonut.DTO
{
    public class LoginRequestUsername
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }

    public class LoginRequestEmail
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class UsersDTO
    {

        public required int UserId { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string PhoneNumber { get; set; }
        public required DateTime CreatedAt { get; set; }
    }

    public class RegisterDTO
    {

        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    namespace CreepyDonut.DTO
    {
        public class ResetPasswordDTO
        {
            public required string Email { get; set; }
            public required string NewPassword { get; set; }
            public required string ConfirmPassword { get; set; }
        }
    }

}
