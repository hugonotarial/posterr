using System;
using System.Data;

namespace Infra.Data.Interface
{
    public interface IDbContext : IDisposable
    {
        IDbConnection Connection { get; }
        IDbTransaction Transaction { get; set; }
    }
}
