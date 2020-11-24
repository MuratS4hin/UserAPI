namespace UserApi.Settings
{
    public class AuthorizeSettings : IAuthorizeSettings
    {
        public string Key { set; get; }
        public string Issuer { set; get; }
        public string Audience { set; get; }
    }

    public interface IAuthorizeSettings
    {
        string Key { get; set; }
        string Issuer { get; set; }
        string Audience { get; set; }
    }
}