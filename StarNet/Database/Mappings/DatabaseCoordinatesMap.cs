using System;
using FluentNHibernate.Mapping;

namespace StarNet.Database.Mappings
{
    public class DatabaseCoordinatesMap : ClassMap<DatabaseCoordinates>
    {
        public DatabaseCoordinatesMap()
        {
            Id(m => m.Id);
            Map(m => m.X);
            Map(m => m.Y);
            References(m => m.Server);
        }
    }
}

