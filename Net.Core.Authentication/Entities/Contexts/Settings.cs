namespace Net.Core.Authentication.Entities.Contexts
{
    // <summary> Represents settings configured in appsettings.json file </summary>
    public class Settings
    {
        /// <summary> Option 1: move connection strings to custom strong typed Settings class </summary>
        public Connections ConnectionStrings { get; set; }

        /// <summary> Option 2: store DB settings any custom way you like - also to get via strong typed Settings class </summary>
        public Database DatabaseSettings { get; set; }
        public string Secret { get; set; }
        public double TokenExpired { get; set; }
    }

    //default connection
    public class Connections
    {
        public string DefaultConnection { get; set; }
    }

    //may u need another connection
    public class Database
    {
        public bool UseInMemory { get; set; }
        public string Host { get; set; }
        public string Name { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string BuildConnectionString() => $"Server={Host};Database={Name};User Id={User};Password={Password}";
    }
}
