using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MySchool.API.Common;
using MySchool.API.Interfaces;
using MySchool.API.Models.DbSet.ClassRoomEntities;
using MySchool.API.Models.Dtos;
using MySchool.API.Services.EnrollmentContainer.Injector;
using System.Net;

namespace MySchool.API.Services.EnrollmentContainer
{
    public class EnrollmentService(IUnitOfWork unitOfWork, IMapper mapper, EnrollmentInjector timeTableInjector) : IServiceInjector
    {

        private IGenericRepository<Enrollment> GetRepository()
        {
            return unitOfWork.GetRepository<Enrollment>().AddInjector(timeTableInjector);
        }

        public async Task<IBaseResponse<EnrollmentResponseDto>> EnrollStudentAsync(EnrollmentRequestDto request)
        {
            /*
            # Case Scenarios

            - If student already enrolled in the same class, return conflict
            - If enrolled in a different class, remove the old one and enroll in the new class
            */

            var enrollmentRepo = GetRepository();

            var filter = new CommandsInjector<Enrollment>();
            filter.Where(e => e.StudentId == request.StudentId);

            var existingEnrollments = await enrollmentRepo.GetAllBy(filter).ToListAsync();

            if (existingEnrollments.Any(e => e.ClassRoomId == request.ClassId))
            {
                return new BaseResponse<EnrollmentResponseDto>()
                    .SetStatus(HttpStatusCode.Conflict)
                    .SetMessage("Student is already enrolled in this class.");
            }

            var previousEnrollment = existingEnrollments.FirstOrDefault();
            if (previousEnrollment != null)
            {
                enrollmentRepo.Delete(previousEnrollment);
            }

            var newEnrollment = mapper.Map<Enrollment>(request);
            await enrollmentRepo.AddAsync(newEnrollment);
            await unitOfWork.SaveAsync();

            return (await GetEnrollmentByIdAsync(newEnrollment.Id))
                .SetStatus(HttpStatusCode.Created);
        }

        public async Task<IBaseResponse<EnrollmentResponseDto>> GetEnrollmentByIdAsync(int enrollmentId)
        {
            var repository = GetRepository();
            var enrollmentEntity = await repository.GetByIdAsync<EnrollmentResponseDto>(enrollmentId);
            if (enrollmentEntity == null)
            {
                return new BaseResponse<EnrollmentResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Enrollment not found.");
            }
            return new BaseResponse<EnrollmentResponseDto>()
                .SetStatus(HttpStatusCode.OK)
                .SetData(enrollmentEntity);
        }


        public IBaseResponse<PaginateBlock<EnrollmentResponseDto>> GetAllEnrollmentssAsync(PaginationFilter<EnrollmentResponseDto> filter)
        {
            return new BaseResponse<PaginateBlock<EnrollmentResponseDto>>()
                .SetData(GetRepository().Filter(filter));
        }

        public async Task<IBaseResponse<object>> RemoveEnrollmentAsync(int id)
        {
            var repository = GetRepository();
            var entity = await repository.GetByIdAsync(id);
            if (entity == null)
            {
                return new BaseResponse()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Enrollment not found.");
            }
            repository.Delete(entity);
            await unitOfWork.SaveAsync();
            return new BaseResponse()
                .SetStatus(HttpStatusCode.OK)
                .SetMessage("Enrollment deleted successfully.");
        }

    }
}
