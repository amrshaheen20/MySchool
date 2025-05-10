using AutoMapper;
using MySchool.API.Common;
using MySchool.API.Interfaces;
using MySchool.API.Models.DbSet.ClassRoomEntities;
using MySchool.API.Models.Dtos;
using MySchool.API.Services.TimeTableContainer.Injector;
using System.Net;

namespace MySchool.API.Services.TimeTableContainer
{
    public class TimeTableService(IUnitOfWork unitOfWork, IMapper mapper, TimeTableInjector timeTableInjector) : IServiceInjector
    {

        private IGenericRepository<Timetable> GetRepository()
        {
            return unitOfWork.GetRepository<Timetable>().AddInjector(timeTableInjector);
        }

        public async Task<IBaseResponse<TimeTableResponseDto>> CreateTimetableAsync(TimeTableRequestDto Timetable)
        {
            var repository = GetRepository();
            var TimetableEntity = mapper.Map<Timetable>(Timetable);

            await repository.AddAsync(TimetableEntity);
            await unitOfWork.SaveAsync();


            return (await GetTimeTableByIdAsync(TimetableEntity.Id))
                .SetStatus(HttpStatusCode.Created);
        }

        public async Task<IBaseResponse<TimeTableResponseDto>> GetTimeTableByIdAsync(int TimetableId)
        {
            var repository = GetRepository();
            var entity = await repository.GetByIdAsync<TimeTableResponseDto>(TimetableId);
            if (entity == null)
            {
                return new BaseResponse<TimeTableResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Timetable not found.");
            }
            return new BaseResponse<TimeTableResponseDto>()
                   .SetStatus(HttpStatusCode.OK)
                   .SetData(entity);
        }

        public IBaseResponse<PaginateBlock<TimeTableResponseDto>> GetAllTimetables(PaginationFilter<TimeTableResponseDto> filter)
        {
            return new BaseResponse<PaginateBlock<TimeTableResponseDto>>()
                .SetData(GetRepository().Filter(filter));
        }

        public async Task<IBaseResponse<object>> UpdateTimeTableAsync(int TimetableId, TimeTableRequestDto Timetable)
        {
            var repository = GetRepository();
            var entity = await repository.GetByIdAsync(TimetableId);
            if (entity == null)
            {
                return new BaseResponse<object>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Timetable not found.");
            }

            mapper.Map(Timetable, entity);

            await unitOfWork.SaveAsync();
            return new BaseResponse<object>()
                .SetStatus(HttpStatusCode.OK)
                .SetMessage("Timetable updated successfully.");
        }

        public async Task<IBaseResponse<ClassResponseDto>> DeleteTimeTableByIdAsync(int TimetableId)
        {
            var repository = GetRepository();
            var entity = await repository.GetByIdAsync(TimetableId);
            if (entity == null)
            {
                return new BaseResponse<ClassResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Timetable not found.");
            }

            repository.Delete(entity);
            await unitOfWork.SaveAsync();
            return new BaseResponse<ClassResponseDto>()
                .SetStatus(HttpStatusCode.OK)
                .SetMessage("Timetable removed successfully.");
        }
    }
}
