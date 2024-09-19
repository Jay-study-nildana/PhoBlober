namespace AzureBlogWebAppDemo.Utility
{
    public class SD
    {
        //public static string AuthAPIBase { get; set; }
        public static string PhotoAPIBase { get; set; }
        //public const string RoleAdmin = "ADMIN";
        //public const string RoleCustomer = "CUSTOMER";

        //super hero rules

        //public const string RoleLeader = "LEADER";
        //public const string RoleSidekick = "SIDEKICK";

        public const string TokenCookie = "JWTToken";
        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE
        }

        public enum ContentType
        {
            Json,
            MultipartFormData,
        }
    }
}
