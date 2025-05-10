using MySchool.API.Models.DbSet;

namespace MySchool.API.Interfaces
{
    public interface IUnitOfWork : IAsyncDisposable, IDisposable
    {
        IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity;
        int Save();
        Task<int> SaveAsync(CancellationToken cancellationToken = default);
    }
}