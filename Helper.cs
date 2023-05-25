using System.Configuration;

namespace LibraryManagementSystem
{
    public static class Helper
    {
        public static string ConnVal(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }
    }
}
