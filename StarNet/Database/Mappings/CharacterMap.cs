using System;
using FluentNHibernate.Mapping;

namespace StarNet.Database.Mappings
{
    public class CharacterMap : ClassMap<Character>
    {
        public CharacterMap()
        {
            Id(m => m.Id);
            Map(m => m.UUID);
            Map(m => m.Name);
            References(m => m.User);
        }
    }
}

