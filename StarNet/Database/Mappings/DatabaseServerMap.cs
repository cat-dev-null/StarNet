using System;
using FluentNHibernate.Mapping;

namespace StarNet.Database.Mappings
{
    public class DatabaseServerMap : ClassMap<DatabaseServer>
    {
        public DatabaseServerMap()
        {
            Id(m => m.Id);
            Map(m => m.UUID);
            Map(m => m.IPv4Address);
            Map(m => m.Port);
            Map(m => m.Password);
            HasMany(m => m.Coordinates).Inverse();
        }
    }
}

