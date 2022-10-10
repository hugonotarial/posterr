using Dapper;
using Domain.Model.Base;
using Infra.Data.Interface;
using Infra.Data.Interface.Repository.Base;
using System.Threading.Tasks;

namespace Infra.Data.Repository.Base
{
    public class BaseRepository<T> : IBaseRepository<T> where T : ModelBase
    {
        protected readonly IDbContext _dbContext;

        public BaseRepository(IDbContext context)
        {
            _dbContext = context;
        }

        protected virtual string AddScript { get; set; }
        protected virtual string DeleteScript { get; set; }

        public async Task<T> AddAsync(T model)
        {
            var id = await _dbContext.Connection.QuerySingleAsync<int>(AddScript, model, _dbContext.Transaction);
            model.Id = id;
            return model;
        }

        public Task DeleteAync(int id)
        {
            return _dbContext.Connection.ExecuteAsync(DeleteScript, new { Id = id }, _dbContext.Transaction);
        }
    }
}
