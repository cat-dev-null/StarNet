using System;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Cfg;
using NHibernate;
using NHibernate.Cfg;
using System.IO;
using NHibernate.Tool.hbm2ddl;
using StarNet.Database.Mappings;
using Npgsql;

namespace StarNet.Database
{
    public class SharedDatabase
    {
        public ISessionFactory SessionFactory { get; set; }

        public SharedDatabase(string connectionString)
        {
            NpgsqlCommand foo = null;
            var config = PostgreSQLConfiguration.Standard.ConnectionString(connectionString);
            SessionFactory = Fluently.Configure()
                .Database(config)
                .Mappings(m => m.FluentMappings.Add<UserMap>())
                .Mappings(m => m.FluentMappings.Add<CharacterMap>())
                .Mappings(m => m.FluentMappings.Add<DatabaseServerMap>())
                .Mappings(m => m.FluentMappings.Add<DatabaseCoordinatesMap>())
                .ExposeConfiguration(BuildSchema)
                .BuildSessionFactory();
        }

        private void BuildSchema(Configuration config)
        {
            new SchemaUpdate(config).Execute(false, true);
        }
    }
}