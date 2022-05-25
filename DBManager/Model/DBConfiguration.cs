using System.Configuration;

namespace DBManager
{
    public class DBConfiguration
    {
        public string ConnectionString { get; private set; }
        public string TableName { get; private set; }
        public string LoginColumnName { get; private set; }
        public string PasswordColumnName { get; private set; }
        public string RegisterDateColumnName { get; private set; } 
        public string AgeColumnName { get; private set; }

        public DBConfiguration()
        {
            ConnectionString = ConfigurationManager.ConnectionStrings["DBManager"].ConnectionString ?? string.Empty;
            TableName = ConfigurationManager.AppSettings["TableName"] ?? string.Empty;
            LoginColumnName = ConfigurationManager.AppSettings["LoginColumnName"] ?? string.Empty;
            PasswordColumnName = ConfigurationManager.AppSettings["PasswordColumnName"] ?? string.Empty;
            RegisterDateColumnName = ConfigurationManager.AppSettings["RegisterDateColumnName"] ?? string.Empty;
            AgeColumnName = ConfigurationManager.AppSettings["AgeColumnName"] ?? string.Empty;
        }

        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(ConnectionString) ||
                string.IsNullOrWhiteSpace(TableName) ||
                string.IsNullOrWhiteSpace(LoginColumnName) ||
                string.IsNullOrWhiteSpace(PasswordColumnName) ||
                string.IsNullOrWhiteSpace(RegisterDateColumnName) ||
                string.IsNullOrWhiteSpace(AgeColumnName))
            {
                return false;
            }

            return true;
        }
    }
}
