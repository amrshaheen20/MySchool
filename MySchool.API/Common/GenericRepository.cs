using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MySchool.API.Interfaces;
using MySchool.API.Models.DbSet;
using MySchool.API.Models.Dtos;
using System.Linq.Expressions;

namespace MySchool.API.Common
{
    public class GenericRepository<TEntity>(DataBaseContext context, IMapper mapper) : IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly DataBaseContext db = context;
        private readonly IMapper mapper = mapper;
        private readonly DbSet<TEntity> dbSet = context.Set<TEntity>();
        private CommandsInjector<TEntity> commands { get; set; } = new CommandsInjector<TEntity>();

        public IGenericRepository<TEntity> AddInjector(CommandsInjector<TEntity> injector)
        {
            commands = injector;
            return this;
        }

        public IGenericRepository<TEntity> AddInjector<T>() where T : CommandsInjector<TEntity>, new()
        {
            commands = new T();
            return this;
        }


        public IGenericRepository<TEntity> AddCommand(Expression<Func<IQueryable<TEntity>, IQueryable<TEntity>>> Command)
        {
            commands.AddCommand(Command);
            return this;
        }


        #region GET
        public async Task<TEntity?> GetByAsync(CommandsInjector<TEntity> injector, CancellationToken cancellationToken = default)
            => await injector.ApplyCommand(commands.ApplyCommand(dbSet)).FirstOrDefaultAsync(cancellationToken);

        public async Task<TBaseResponseDto?> GetByAsync<TBaseResponseDto>(CommandsInjector<TEntity> injector, CancellationToken cancellationToken = default) where TBaseResponseDto : BaseResponseDto
            => await injector.ApplyCommand(commands.ApplyCommand(dbSet)).ProjectTo<TBaseResponseDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);



        public async Task<TEntity?> GetByIdAsync(int id)
            => await commands.ApplyCommand(dbSet).FirstOrDefaultAsync(e => e.Id == id);

        public async Task<TBaseResponseDto?> GetByIdAsync<TBaseResponseDto>(int id) where TBaseResponseDto : BaseResponseDto
            => await commands.ApplyCommand(dbSet).ProjectTo<TBaseResponseDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync(e => e.Id == id);

        public IQueryable<TEntity> GetAll()
            => commands.ApplyCommand(dbSet);

        public IQueryable<TEntity> GetAllBy(CommandsInjector<TEntity> injector)
            => injector.ApplyCommand(commands.ApplyCommand(dbSet));

        public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
            => await commands.ApplyCommand(dbSet).ToListAsync(cancellationToken);

        public async Task<IEnumerable<TEntity>> GetAllByAsync(CommandsInjector<TEntity> injector, CancellationToken cancellationToken = default)
            => await injector.ApplyCommand(commands.ApplyCommand(dbSet)).ToListAsync(cancellationToken);

        public PaginateBlock<T> Filter<T>(PaginationFilter<T> filter)
         => filter.Apply(mapper.ProjectTo<T>(commands.ApplyCommand(dbSet)));



        public PaginateBlock<T> FilterBy<T>(PaginationFilter<T> filter, CommandsInjector<TEntity> injector)
         => filter.Apply(mapper.ProjectTo<T>(injector.ApplyCommand(commands.ApplyCommand(dbSet))));


        public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
            => dbSet.AnyAsync(predicate, cancellationToken);

        #endregion
        #region ADD
        public void Add(TEntity entity)
            => dbSet.Add(entity);

        public async ValueTask<EntityEntry<TEntity>> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
            => await dbSet.AddAsync(entity, cancellationToken);

        #endregion
        #region UPDATE
        public void Update(TEntity entity)
            => dbSet.Update(entity);

        public void UpdateRange(params TEntity[] entity)
            => dbSet.UpdateRange(entity);

        #endregion
        #region REMOVE
        public void Delete(TEntity entity)
            => dbSet.Remove(entity);

        public void DeleteRange(params TEntity[] entity)
            => dbSet.RemoveRange(entity);

        public async void DeleteAsync(int id, CancellationToken cancellationToken = default)
            => await dbSet.Where(x => x.Id == id).ExecuteDeleteAsync(cancellationToken);

        #endregion



    }
}
