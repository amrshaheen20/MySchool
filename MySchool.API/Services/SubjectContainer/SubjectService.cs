using AutoMapper;
using MySchool.API.Common;
using MySchool.API.Interfaces;
using MySchool.API.Models.DbSet.SubjectEntities;
using MySchool.API.Models.Dtos;
using MySchool.API.Services.SubjectContainer.Injector;
using System.Net;

namespace MySchool.API.Services.SubjectContainer
{
    public class SubjectService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        SubjectInjector subjectInjector
        ) : IServiceInjector
    {
        private IGenericRepository<Subject> GetRepository()
        {
            return unitOfWork.GetRepository<Subject>().AddInjector(subjectInjector);
        }


        public async Task<IBaseResponse<SubjectResponseDto>> CreateSubjectAsync(SubjectRequestDto subject)
        {
            var Repository = unitOfWork.GetRepository<Subject>();
            var Entity = mapper.Map<Subject>(subject);

            await Repository.AddAsync(Entity);
            await unitOfWork.SaveAsync();

            return new BaseResponse<SubjectResponseDto>()
                .SetStatus(HttpStatusCode.Created)
                .SetData(mapper.Map<SubjectResponseDto>(Entity));
        }


        public async Task<IBaseResponse<SubjectResponseDto>> GetSubjectByIdAsync(int SubjectId)
        {
            var Entity = await GetRepository().GetByIdAsync<SubjectResponseDto>(SubjectId);
            if (Entity == null)
            {
                return new BaseResponse<SubjectResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Subject not found.");
            }
            return new BaseResponse<SubjectResponseDto>()
                   .SetStatus(HttpStatusCode.OK)
                   .SetData(Entity);
        }

        public IBaseResponse<PaginateBlock<SubjectResponseDto>> GetAllSubjects(PaginationFilter<SubjectResponseDto> filter)
        {
            return new BaseResponse<PaginateBlock<SubjectResponseDto>>()
                .SetData(GetRepository().Filter(filter));
        }


        public async Task<IBaseResponse<SubjectResponseDto>> DeleteSubjectByIdAsync(int SubjectId)
        {
            var Repository = unitOfWork.GetRepository<Subject>();
            var Entity = await Repository.GetByIdAsync(SubjectId);

            if (Entity == null)
            {
                return new BaseResponse<SubjectResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Subject not found.");
            }


            Repository.Delete(Entity);

            await unitOfWork.SaveAsync();
            return new BaseResponse<SubjectResponseDto>()
                .SetData(mapper.Map<SubjectResponseDto>(Entity));
        }

        public async Task<IBaseResponse<SubjectResponseDto>> UpdateSubjectAsync(int SubjectId, SubjectRequestDto updatedSubject)
        {
            var Repository = unitOfWork.GetRepository<Subject>();
            var Entity = await Repository.GetByIdAsync(SubjectId);

            if (Entity == null)
            {
                return new BaseResponse<SubjectResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Subject not found.");
            }

            mapper.Map(updatedSubject, Entity);
            Repository.Update(Entity);
            await unitOfWork.SaveAsync();

            return new BaseResponse<SubjectResponseDto>()
                .SetData(mapper.Map<SubjectResponseDto>(Entity));
        }

    }
}
