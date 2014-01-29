using System;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Cfg;
using NHibernate;
using NHibernate.Cfg;
using System.IO;
using NHibernate.Tool.hbm2ddl;

namespace StarNet.Database
{
    public class StarNetDatabase
    {
        public ISessionFactory SessionFactory { get; set; }

        public StarNetDatabase(string databaseFile)
        {
            var config = MonoSQLiteConfiguration.Standard.UsingFile(databaseFile);
            SessionFactory = Fluently.Configure()
                .Database(config)
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<StarNetDatabase>())
                .ExposeConfiguration(BuildSchema)
                .BuildSessionFactory();
        }

        private void BuildSchema(Configuration config)
        {
            new SchemaExport(config).Create(false, true);
        }
    }
}