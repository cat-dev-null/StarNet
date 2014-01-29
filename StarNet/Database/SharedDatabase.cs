using System;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Cfg;
using NHibernate;
using NHibernate.Cfg;
using System.IO;
using NHibernate.Tool.hbm2ddl;
using StarNet.Database.Mappings;

namespace StarNet.Database
{
    public class SharedDatabase
    {
        public ISessionFactory SessionFactory { get; set; }

        public SharedDatabase(string connectionString)
        {
            var config = PostgreSQLConfiguration.Standard.ConnectionString("");
            SessionFactory = Fluently.Configure()
                .Database(config)
                    .Mappings(m => m.FluentMappings.Add<UserMap>())
                    .Mappings(m => m.FluentMappings.Add<CharacterMap>())
                    .ExposeConfiguration(BuildSchema)
                    .BuildSessionFactory();
        }

        private void BuildSchema(Configuration config)
        {
            new SchemaUpdate(config).Execute(false, true);
        }
    }
}