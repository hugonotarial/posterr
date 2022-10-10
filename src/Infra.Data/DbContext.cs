using Domain.Setting.Interface;
using Infra.Data.Interface;
using System.Data;
using System.Data.SqlClient;

namespace Infra.Data
{
    public class DbContext : IDbContext
    {
        private readonly IProjectSettings _settings;

        public IDbConnection Connection { get; }
        public IDbTransaction Transaction { get; set; }

        public DbContext(IProjectSettings settings)
        {
            _settings = settings;
            Connection = new SqlConnection(_settings.DbConnectionString);
            Connection.Open();
        }

        public void Dispose() => Connection?.Dispose();
    }
}
