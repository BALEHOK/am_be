namespace AppFramework.Auth.Data.Models
{
    public enum TokenType : short
    {
        AuthorizationCode = (short) 1,
        TokenHandle = (short) 2,
        RefreshToken = (short) 3
    }
}