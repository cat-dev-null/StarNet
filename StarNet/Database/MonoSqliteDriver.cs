using FluentNHibernate.Cfg.Db;
using NHibernate.Dialect;

namespace StarNet.Database
{
    public class MonoSqliteDriver : NHibernate.Driver.ReflectionBasedDriver
    {
        public MonoSqliteDriver() : base("Mono.Data.Sqlite", "Mono.Data.Sqlite", "Mono.Data.Sqlite.SqliteConnection", "Mono.Data.Sqlite.SqliteCommand")
        {
        }

        public override bool UseNamedPrefixInParameter
        {
            get
            {
                return true;
            }
        }

        public override bool UseNamedPrefixInSql
        {
            get
            {
                return true;
            }
        }

        public override string NamedPrefix
        {
            get
            {
                return "@";
            }
        }

        public override bool SupportsMultipleOpenReaders
        {
            get
            {
                return false;
            }
        }
    }

    public class MonoSQLiteConfiguration : PersistenceConfiguration<MonoSQLiteConfiguration>
    {
        public static MonoSQLiteConfiguration Standard
        {
            get { return new MonoSQLiteConfiguration(); }
        }

        public MonoSQLiteConfiguration()
        {
            Driver<MonoSqliteDriver>();
            Dialect<SQLiteDialect>();
            Raw("query.substitutions", "true=1;false=0");
        }

        public MonoSQLiteConfiguration InMemory()
        {
            Raw("connection.release_mode", "on_close");
            return ConnectionString(c => c.Is("Data Source=:memory:;Version=3;New=True;"));

        }

        public MonoSQLiteConfiguration UsingFile(string fileName)
        {
            return ConnectionString(c => c.Is(string.Format("Data Source={0};Version=3;New=True;", fileName)));
        }

        public MonoSQLiteConfiguration UsingFileWithPassword(string fileName, string password)
        {
            return ConnectionString(c => c.Is(string.Format("Data Source={0};Version=3;New=True;Password={1};", fileName, password)));
        }
    }
}