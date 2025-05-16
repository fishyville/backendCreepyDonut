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
}
