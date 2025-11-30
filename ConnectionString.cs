namespace Carpool_DB_Proj
{
    public class ConnectionString
    {
        private static string cs = "Server=localhost\\SQLEXPRESS;Database=my_sql_project;Trusted_Connection=True;TrustServerCertificate=True;";
        public static string dbcs { get => cs; }
    }
}
 