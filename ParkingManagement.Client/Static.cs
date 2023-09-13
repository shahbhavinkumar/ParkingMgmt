namespace ParkingManagement.Client
{
    public static class Spots
    {
        public static int TotalSpots = Int32.Parse(ConfigurationManager.AppSetting["Configs:totalSpot"]);
        public static int SpotsAvailable { get; set; } = Int32.Parse(ConfigurationManager.AppSetting["Configs:totalSpot"]);
    }

    public static class ConfigurationManager
    {
        public static IConfiguration AppSetting { get; }
        static ConfigurationManager()
        {
            AppSetting = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
        }
    }
}
