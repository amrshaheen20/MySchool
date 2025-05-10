using AutoMapper;
using MySchool.API.Common;
using MySchool.API.Extensions;
using MySchool.API.Interfaces;
using MySchool.API.Models.DbSet.ClassRoomEntities;
using MySchool.API.Models.Dtos;
using MySchool.API.Services.AttendanceContainer.Injector;
using MySchool.API.Services.EnrollmentContainer.Injector;
using System.Net;

namespace MySchool.API.Services.AttendanceContainer
{
    public class AttendanceService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        AttendanceInjector attendanceInjector,
        IHttpContextAccessor contextAccessor,
        EnrollmentInjector enrollmentInjector

        ) : IServiceInjector
    {
        private IGenericRepository<Attendance> GetRepository()
        {
            return unitOfWork.GetRepository<Attendance>().AddInjector(attendanceInjector);
        }

        public async Task<IBaseResponse<object>> CreateAttendanceAsync(AttendanceRequestDto requestDto)
        {
            /*
            # Case Scenarios
            - Add new attendance
            - If student already has attendance for the same date update the existing one
            */

            var Repository = GetRepository();

            var filter = new CommandsInjector<Attendance>();
            filter.Where(e => e.StudentId == requestDto.StudentId && e.Date == requestDto.Date);
            var existingAttendance = await Repository.GetByAsync(filter);
            if (existingAttendance != null)
            {
                mapper.Map(requestDto, existingAttendance);
                Repository.Update(existingAttendance);
                await unitOfWork.SaveAsync();
                return new BaseResponse()
                    .SetStatus(HttpStatusCode.OK)
                    .SetMessage("Attendance updated successfully");
            }

            var Entity = mapper.Map<Attendance>(requestDto);
            Entity.CreatedById = contextAccessor.GetUserId();


            await Repository.AddAsync(Entity);
            await unitOfWork.SaveAsync();

            return new BaseResponse()
                .SetStatus(HttpStatusCode.Created)
                .SetMessage("Attendance added successfully");
        }

        public async Task<IBaseResponse<AttendanceResponseDto>> GetAttendanceByIdAsync(int attendanceId)
        {
            var Repository = GetRepository();
            var Entity = await Repository.GetByIdAsync<AttendanceResponseDto>(attendanceId);

            if (Entity == null)
            {
                return new BaseResponse<AttendanceResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Attendance not found");
            }

            return new BaseResponse<AttendanceResponseDto>()
                .SetStatus(HttpStatusCode.OK)
                .SetData(Entity);
        }

        public IBaseResponse<PaginateBlock<AttendanceResponseDto>> GetAllAttendances(PaginationFilter<AttendanceResponseDto> filter)
        {
            /*
            # Case Scenarios
            - Get all attendance records even if no attendance records exist for some students
            */


            return new BaseResponse<PaginateBlock<AttendanceResponseDto>>()
                .SetStatus(HttpStatusCode.OK)
                .SetData(GetRepository().Filter(filter));
        }


        public async Task<IBaseResponse<AttendanceResponseDto>> UpdateAttendanceAsync(int attendanceId, AttendanceRequestDto requestDto)
        {
            var Repository = GetRepository();
            var Entity = await Repository.GetByIdAsync(attendanceId);
            if (Entity == null)
            {
                return new BaseResponse<AttendanceResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Attendance not found");
            }

            mapper.Map(requestDto, Entity);
            Repository.Update(Entity);
            await unitOfWork.SaveAsync();

            return new BaseResponse<AttendanceResponseDto>()
                .SetStatus(HttpStatusCode.OK)
                .SetMessage("Attendance updated successfully");
        }

        public async Task<IBaseResponse<AttendanceResponseDto>> DeleteAttendanceAsync(int attendanceId)
        {
            var Repository = GetRepository();
            var Entity = await Repository.GetByIdAsync(attendanceId);



            if (Entity == null)
            {
                return new BaseResponse<AttendanceResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Attendance not found");
            }

            Repository.Delete(Entity);
            await unitOfWork.SaveAsync();

            return new BaseResponse<AttendanceResponseDto>()
                .SetStatus(HttpStatusCode.OK)
                .SetMessage("Attendance deleted successfully");
        }

        //Class
        public IBaseResponse<PaginateBlock<ClassAttendanceResponseDto>> GetClassAttendanceById(ClassAttendanceRequestDto request, PaginationFilter<ClassAttendanceResponseDto> filter)
        {
            enrollmentInjector.IncludeSTudent();
            //get all students in the class
            var injector = new CommandsInjector<Enrollment>();
            injector.Where(x => x.ClassRoomId == request.ClassId);
            var students = mapper.ProjectTo<EnrollmentResponseDto>(unitOfWork.GetRepository<Enrollment>().AddInjector(enrollmentInjector).GetAllBy(injector));
            if (students == null)
            {
                return new BaseResponse<PaginateBlock<ClassAttendanceResponseDto>>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Class not found");
            }

            //get all class attendance
            var attendanceInjector = new CommandsInjector<Attendance>();
            attendanceInjector.Where(x => x.ClassRoomId == request.ClassId);
            var attendances = mapper.ProjectTo<AttendanceResponseDto>(GetRepository().GetAllBy(attendanceInjector));


            var classAttendances = students.Select(enrollment => new ClassAttendanceResponseDto
            {
                Student = enrollment.Student,
                Attendance = attendances.FirstOrDefault(a => a.StudentId == enrollment.Student.Id && a.Date == request.Date)
            });

            return new BaseResponse<PaginateBlock<ClassAttendanceResponseDto>>()
                .SetStatus(HttpStatusCode.OK)
                .SetData(filter.Apply(classAttendances));
        }
    }
}
