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
    public class NodeDatabase
    {
        public ISessionFactory SessionFactory { get; set; }

        public NodeDatabase(string databaseFile)
        {
            var config = MonoSQLiteConfiguration.Standard.UsingFile(databaseFile);
            SessionFactory = Fluently.Configure()
                .Database(config)
                .Mappings(m => m.FluentMappings.Add<LocalSettingsMap>())
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