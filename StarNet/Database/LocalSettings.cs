using System;

namespace StarNet.Database
{
    public class LocalSettings
    {
        public Guid UUID { get; protected set; }
        public string ConnectionString { get; set; }

        public LocalSettings()
        {
        }

        public LocalSettings(string connectionString)
        {
            ConnectionString = connectionString;
            UUID = Guid.NewGuid();
        }
    }
}

