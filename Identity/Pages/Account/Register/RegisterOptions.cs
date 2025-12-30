namespace Identity.Pages.Account.Register
{
    public static class RegisterOptions
    {
        public static readonly string InvalidPasswordErrorMessage = "You password needs to have 8 or more characters, containing at least a special character and an uppercase.";
        public static readonly string InvalidUsernameErrorMessage = "The user already exists.";
        public static readonly string InvalidEmailErrorMessage = "The provided email already exists.";
    }
}
