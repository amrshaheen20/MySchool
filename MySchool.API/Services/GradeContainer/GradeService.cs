using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MySchool.API.Common;
using MySchool.API.Extensions;
using MySchool.API.Interfaces;
using MySchool.API.Models.DbSet;
using MySchool.API.Models.Dtos;
using MySchool.API.Services.GradeContainer.Injector;
using System.Net;

namespace MySchool.API.Services.GradeContainer
{
    public class GradeService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        GradeInjector gradeInjector,
        IHttpContextAccessor contextAccessor
    ) : IServiceInjector
    {
        private IGenericRepository<Grade> GetRepository()
        {
            return unitOfWork.GetRepository<Grade>().AddInjector(gradeInjector);
        }

        public async Task<IBaseResponse<GradeResponseDto>> CreateGradeAsync(GradeRequestDto requestDto)
        {
            var gradeRepo = GetRepository();
            var newGrade = mapper.Map<Grade>(requestDto);
            newGrade.CreatedById = contextAccessor.GetUserId();

            await gradeRepo.AddAsync(newGrade);
            await unitOfWork.SaveAsync();

            return new BaseResponse<GradeResponseDto>()
                .SetStatus(HttpStatusCode.Created)
                .SetData(mapper.Map<GradeResponseDto>(newGrade));
        }

        public async Task<IBaseResponse<GradeResponseDto>> GetGradeByIdAsync(int gradeId)
        {
            var gradeEntity = await GetRepository().GetByIdAsync<GradeResponseDto>(gradeId);
            if (gradeEntity == null)
            {
                return new BaseResponse<GradeResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Grade not found.");
            }

            return new BaseResponse<GradeResponseDto>()
                .SetStatus(HttpStatusCode.OK)
                .SetData(gradeEntity);
        }

        public IBaseResponse<PaginateBlock<GradeResponseDto>> GetAllGrades(PaginationFilter<GradeResponseDto> filter)
        {
            return new BaseResponse<PaginateBlock<GradeResponseDto>>()
                .SetStatus(HttpStatusCode.OK)
                .SetData(GetRepository().Filter(filter));
        }

        public async Task<IBaseResponse<object>> UpdateGradeAsync(int gradeId, GradeRequestDto requestDto)
        {
            var gradeRepo = GetRepository();
            var gradeEntity = await gradeRepo.GetByIdAsync(gradeId);
            if (gradeEntity == null)
            {
                return new BaseResponse()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Grade not found.");
            }

            mapper.Map(requestDto, gradeEntity);
            gradeRepo.Update(gradeEntity);
            await unitOfWork.SaveAsync();

            return new BaseResponse()
                .SetStatus(HttpStatusCode.OK)
                .SetMessage("Grade updated successfully.");
        }

        public async Task<IBaseResponse<object>> DeleteGradeAsync(int gradeId)
        {
            var gradeRepo = GetRepository();
            var gradeEntity = await gradeRepo.GetByIdAsync(gradeId);
            if (gradeEntity == null)
            {
                return new BaseResponse()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Grade not found.");
            }

            gradeRepo.Delete(gradeEntity);
            await unitOfWork.SaveAsync();

            return new BaseResponse()
                .SetStatus(HttpStatusCode.NoContent)
                .SetMessage("Grade deleted successfully.");
        }


        public async Task<IBaseResponse<object>> PublishGradesAsync()
        {
            var updatedCount = await GetRepository().GetAll()
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(g => g.IsPublished, true)
                );

            return new BaseResponse()
                .SetStatus(HttpStatusCode.OK)
                .SetMessage($"{updatedCount} grades published.");
        }


        public async Task<IBaseResponse<object>> UnpublishGradesAsync()
        {
            var updatedCount = await GetRepository().GetAll()
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(g => g.IsPublished, false)
                );

            return new BaseResponse()
                .SetStatus(HttpStatusCode.OK)
                .SetMessage($"{updatedCount} grades unpublished.");
        }


        public async Task<IBaseResponse<object>> DeleteAllGradesAsync()
        {
            var gradeRepo = GetRepository();

            var deletedCount = await gradeRepo.GetAll().ExecuteDeleteAsync();

            return new BaseResponse()
                 .SetStatus(HttpStatusCode.NoContent)
                 .SetMessage($"{deletedCount} grades deleted.");
        }
    }
}
