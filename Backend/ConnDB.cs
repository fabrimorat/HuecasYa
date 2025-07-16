namespace Backend
{
    public class ConnDB
    {
        private string connectionString = String.Empty;
        private string database = String.Empty; 

        public ConnDB()
        {
            var constructor = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            connectionString = constructor.GetSection("ConnectionStrings:MongoDB").Value;
            database = constructor.GetSection("ConnectionStrings:Database").Value;
        }

        public string GetConnectionString()
        {
            return connectionString;
        }

        public string GetDB()
        {
            return database;
        }
    }
}
