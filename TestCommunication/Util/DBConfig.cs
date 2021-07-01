using System.Configuration;

namespace DCBProject.Util
{
    public class DBConfig
    {
        public static string connectionString = ConfigurationManager.AppSettings["HanZiDinner"];
        public static string posApi = ConfigurationManager.AppSettings["PosApi"];
    }
}
