using AutoMapper;
using MySchool.API.Common;
using MySchool.API.Interfaces;
using MySchool.API.Models.DbSet;
using MySchool.API.Models.Dtos;
using MySchool.API.Services.ClassContainer.Injector;
using System.Net;

namespace MySchool.API.Services.ClassContainer
{
    public class ClassService(IUnitOfWork unitOfWork, IMapper mapper, ClassRoomInjector roomInjector) : IServiceInjector
    {
        private IGenericRepository<ClassRoom> GetRepository()
        {
            return unitOfWork.GetRepository<ClassRoom>().AddInjector(roomInjector);
        }
        public async Task<IBaseResponse<ClassResponseDto>> CreateClassAsync(ClassRequestDto Class)
        {
            var Repository = GetRepository();
            var Entity = mapper.Map<ClassRoom>(Class);

            await Repository.AddAsync(Entity);
            await unitOfWork.SaveAsync();

            var newEntity = await Repository.GetByIdAsync(Entity.Id);

            return new BaseResponse<ClassResponseDto>()
                .SetStatus(HttpStatusCode.Created)
                .SetData(mapper.Map<ClassResponseDto>(newEntity));
        }


        public async Task<IBaseResponse<ClassResponseDto>> GetClassByIdAsync(int ClassId)
        {
            var Entity = await GetRepository().GetByIdAsync<ClassResponseDto>(ClassId);

            if (Entity == null)
            {
                return new BaseResponse<ClassResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Class not found.");
            }

            return new BaseResponse<ClassResponseDto>()
                .SetData(Entity);
        }


        public IBaseResponse<PaginateBlock<ClassResponseDto>> GetAllClasses(PaginationFilter<ClassResponseDto> filter)
        {
            return new BaseResponse<PaginateBlock<ClassResponseDto>>()
                .SetData(GetRepository().Filter(filter));
        }



        public async Task<IBaseResponse<object>> UpdateClassAsync(int ClassId, ClassRequestDto updatedClass)
        {
            var Repository = GetRepository();
            var Entity = await Repository.GetByIdAsync(ClassId);

            if (Entity == null)
            {
                return new BaseResponse()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Class not found.");
            }

            mapper.Map(updatedClass, Entity);
            Repository.Update(Entity);
            await unitOfWork.SaveAsync();

            return new BaseResponse()
                .SetStatus(HttpStatusCode.NoContent)
                .SetMessage("Class updated successfully.");
        }

        public async Task<IBaseResponse<object>> DeleteClassByIdAsync(int ClassId)
        {
            var Repository = GetRepository();
            var Entity = await Repository.GetByIdAsync(ClassId);

            if (Entity == null)
            {
                return new BaseResponse()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Class not found.");
            }

            Repository.Delete(Entity);
            await unitOfWork.SaveAsync();
            return new BaseResponse()
                 .SetStatus(HttpStatusCode.NoContent)
                 .SetMessage("Class deleted successfully.");
        }

    }
}
