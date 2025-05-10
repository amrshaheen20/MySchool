using Microsoft.EntityFrameworkCore.ChangeTracking;
using MySchool.API.Common;
using MySchool.API.Models.DbSet;
using MySchool.API.Models.Dtos;
using System.Linq.Expressions;

namespace MySchool.API.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        IGenericRepository<TEntity> AddCommand(Expression<Func<IQueryable<TEntity>, IQueryable<TEntity>>> Command);
        IGenericRepository<TEntity> AddInjector(CommandsInjector<TEntity> injector);
        IGenericRepository<TEntity> AddInjector<T>() where T : CommandsInjector<TEntity>, new();
        IQueryable<TEntity> GetAll();
        IQueryable<TEntity> GetAllBy(CommandsInjector<TEntity> injector);
        PaginateBlock<T> Filter<T>(PaginationFilter<T> filter);
        PaginateBlock<T> FilterBy<T>(PaginationFilter<T> filter, CommandsInjector<TEntity> injector);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> GetAllByAsync(CommandsInjector<TEntity> injector, CancellationToken cancellationToken = default);
        Task<TBaseResponseDto?> GetByAsync<TBaseResponseDto>(CommandsInjector<TEntity> injector, CancellationToken cancellationToken = default) where TBaseResponseDto : BaseResponseDto;
        Task<TBaseResponseDto?> GetByIdAsync<TBaseResponseDto>(int id) where TBaseResponseDto : BaseResponseDto;
        Task<TEntity?> GetByAsync(CommandsInjector<TEntity> injector, CancellationToken cancellationToken = default);
        Task<TEntity?> GetByIdAsync(int id);
        ValueTask<EntityEntry<TEntity>> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        void Add(TEntity entity);
        void Delete(TEntity entity);
        void DeleteAsync(int id, CancellationToken cancellationToken = default);
        void DeleteRange(params TEntity[] entity);
        void Update(TEntity entity);
        void UpdateRange(params TEntity[] entity);
    }
}