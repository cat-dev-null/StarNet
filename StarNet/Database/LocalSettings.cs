using System;

namespace StarNet.Database
{
    public class LocalSettings
    {
        public virtual int Id { get; protected set; } // Note: There will only ever be one of these per db
        public virtual Guid UUID { get; protected set; }
        public virtual int Version { get; set; }
        public virtual string SharedDBConnectionString { get; set; }

        public LocalSettings()
        {
        }

        public LocalSettings(int version)
        {
            Version = version;
            UUID = Guid.NewGuid();
        }
    }
}

