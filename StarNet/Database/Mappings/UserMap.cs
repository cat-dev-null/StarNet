using System;
using FluentNHibernate.Mapping;

namespace StarNet.Database.Mappings
{
    public class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            Id(m => m.Id);
            Map(m => m.AccountName);
            Map(m => m.Password); // Plain text, don't bother me about it
            HasMany(m => m.Characters);
        }
    }
}