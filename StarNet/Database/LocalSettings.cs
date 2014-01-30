using System;
using Newtonsoft.Json;

namespace StarNet.Database
{
    public class LocalSettings
    {
        public string UUID_string
        {
            get
            {
                return UUID.ToString();
            }
            set
            {
                UUID = Guid.Parse(value);
            }
        }
        [JsonIgnore]
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

