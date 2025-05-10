using AutoMapper;
using MySchool.API.Extensions;
using MySchool.API.Interfaces;
using MySchool.API.Models.DbSet;

namespace MySchool.API.Common
{
    [ServiceType(ServiceLifetimeType.Scoped, typeof(IUnitOfWork))]
    public class UnitOfWork(DataBaseContext context, IMapper mapper) : IUnitOfWork
    {
        private readonly DataBaseContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly Dictionary<Type, object> _repositories = new Dictionary<Type, object>();

        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity
        {
            if (_repositories.ContainsKey(typeof(TEntity)))
            {
                return (IGenericRepository<TEntity>)_repositories[typeof(TEntity)];
            }

            var repository = new GenericRepository<TEntity>(_context, _mapper);
            _repositories.Add(typeof(TEntity), repository);
            return repository;
        }

        public int Save() => _context.SaveChanges();
        public Task<int> SaveAsync(CancellationToken cancellationToken = default) => _context.SaveChangesAsync(cancellationToken);
        public void Dispose() => _context.Dispose();
        public ValueTask DisposeAsync() => _context.DisposeAsync();

    }
}
