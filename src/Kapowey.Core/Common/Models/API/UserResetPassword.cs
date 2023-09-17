namespace Kapowey.Core.Common.Models.API
{
    [Serializable]
    public sealed class UserResetPassword
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string Token { get; set; }

        public string ResetUrl { get; set; }
    }
}
