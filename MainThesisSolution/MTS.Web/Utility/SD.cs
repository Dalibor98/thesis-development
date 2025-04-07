namespace MTS.Web.Utility
{
    public class SD
    {
        public static string CurriculumAPIBase { get; set; }
        public static string AuthAPIBase { get; set; }
        public static string UserAPIBase { get; set; }

        //super hero rules

        public const string RoleLeader = "PROFESSOR";
        public const string RoleSidekick = "STUDENT";

        public const string TokenCookie = "JWTToken";
        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE
        }
        public enum IdType
        {
            STUDENT = 0,
            PROFESSOR = 1
        }

        public const string Status_Pending = "Pending";
        public const string Status_Approved = "Approved";
        public const string Status_ReadyForPickup = "ReadyForPickup";
        public const string Status_Completed = "Completed";
        public const string Status_Refunded = "Refunded";
        public const string Status_Cancelled = "Cancelled";

        public enum ContentType
        {
            Json,
            MultipartFormData,
        }
    }
}
