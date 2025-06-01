using AutoMapper;
using AutoMapper.QueryableExtensions;
using MySchool.API.Common;
using MySchool.API.Enums;
using MySchool.API.Interfaces;
using MySchool.API.Models.DbSet;
using MySchool.API.Models.Dtos;
using MySchool.API.Services.AccountContainer.Injector;
using MySchool.API.Services.GuardianContainer.Injector;
using System.Net;

namespace MySchool.API.Services.GuardianContainer
{
    public class GuardianService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        StudentGuardianInjector guardianInjector,
        AccountInjector accountInjector
        ) : IServiceInjector
    {
        private IGenericRepository<StudentGuardian> GetRepository()
        {
            return unitOfWork.GetRepository<StudentGuardian>().AddInjector(guardianInjector);
        }

        public async Task<IBaseResponse<object>> AddChildAsync(int guardianId, StudentGuardianRequestDto request)
        {
            var Repository = GetRepository();
            var Entity = mapper.Map<StudentGuardian>(request);
            Entity.GuardianId = guardianId;
            await Repository.AddAsync(Entity);
            await unitOfWork.SaveAsync();

            return new BaseResponse<object>()
                .SetStatus(HttpStatusCode.Created)
                .SetMessage("Child added to guardian");
        }


        public IBaseResponse<PaginateBlock<StudentGuardianResponseDto>> Getchildren(int GuardianId, PaginationFilter<StudentGuardianResponseDto> filter)
        {
            var Repository = GetRepository();
            Repository.AddCommand(q => q.Where(q => q.GuardianId == GuardianId));
            return new BaseResponse<PaginateBlock<StudentGuardianResponseDto>>()
                .SetStatus(HttpStatusCode.OK)
                .SetData(Repository.Filter(filter));
        }

        public IBaseResponse<PaginateBlock<StudentGuardianResponseDto>> GetAllchildren(PaginationFilter<StudentGuardianResponseDto> filter)
        {
            var Repository = GetRepository();
            return new BaseResponse<PaginateBlock<StudentGuardianResponseDto>>()
                .SetStatus(HttpStatusCode.OK)
                .SetData(Repository.Filter(filter));
        }

        public async Task<IBaseResponse<StudentGuardianResponseDto>> DeleteChildAsync(int guardianId, int studentId)
        {
            var Repository = GetRepository();
            var injector = new CommandsInjector<Models.DbSet.StudentGuardian>();
            injector.AddCommand(q => q.Where(q => q.GuardianId == guardianId && q.StudentId == studentId));
            var child = await Repository.GetByAsync(injector);
            if (child == null)
            {
                return new BaseResponse<StudentGuardianResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Can't find this child under this guardian");
            }

            Repository.Delete(child);
            await unitOfWork.SaveAsync();

            return new BaseResponse<StudentGuardianResponseDto>()
                .SetStatus(HttpStatusCode.NoContent)
                .SetMessage("child removed from guardian");
        }


        public IBaseResponse<PaginateBlock<GuardianResponseDto>> GetGuardians(PaginationFilter<GuardianResponseDto> filter)
        {
            var Guardians = unitOfWork.GetRepository<User>()
                .AddInjector(accountInjector.Where(q => q.Role == eRole.Guardian)).GetAll().ProjectTo<AccountResponseDto>(mapper.ConfigurationProvider);

            var AllStudents = GetRepository().GetAll();

            var Result = Guardians.Select(guardian => new GuardianResponseDto()
            {
                Guardian = guardian,
                TotalChildren = AllStudents.Where(q => q.GuardianId == guardian.Id).Count()
            });


            return new BaseResponse<PaginateBlock<GuardianResponseDto>>()
                .SetStatus(HttpStatusCode.OK)
                .SetData(filter.Apply(Result));
        }
    }
}
