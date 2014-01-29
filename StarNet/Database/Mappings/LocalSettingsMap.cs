using System;
using FluentNHibernate.Mapping;

namespace StarNet.Database.Mappings
{
    public class LocalSettingsMap : ClassMap<LocalSettings>
    {
        public LocalSettingsMap()
        {
            Id(m => m.Id);
            Map(m => m.UUID);
            Map(m => m.Version);
            Map(m => m.SharedDBConnectionString);
        }
    }
}