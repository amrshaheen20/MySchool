using AutoMapper;
using AutoMapper.QueryableExtensions;
using MySchool.API.Common;
using MySchool.API.Extensions;
using MySchool.API.Interfaces;
using MySchool.API.Models.DbSet;
using MySchool.API.Models.DbSet.ExamEntities;
using MySchool.API.Models.Dtos;
using MySchool.API.Services.AssignmentContainer.Injector;
using MySchool.API.Services.Common;
using MySchool.API.Services.EnrollmentContainer.Injector;
using System.Net;

namespace MySchool.API.Services.AssignmentContainer
{
    public class AssignmentService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        AssignmentInjector assignmentInjector,
        FileStorageService fileStorage,
        IHttpContextAccessor contextAccessor,
        EnrollmentInjector enrollmentInjector,
        SubmissionInjector submissionInjector
        ) : IServiceInjector
    {

        private IGenericRepository<Assignment> GetRepository()
        {
            return unitOfWork.GetRepository<Assignment>().AddInjector(assignmentInjector);
        }

        private IGenericRepository<AssignmentSubmission> GetSubmissionRepository()
        {
            return unitOfWork.GetRepository<AssignmentSubmission>().AddInjector(submissionInjector);
        }

        public async Task<IBaseResponse<AssignmentResponseDto>> CreateAssignment(AssignmentRequestDto requestDto)
        {
            //Does this teacher give this class?
            var RepositoryClassRoom = unitOfWork.GetRepository<ClassRoom>();
            var injector = new CommandsInjector<ClassRoom>();
            injector.Where(x => x.Id == requestDto.ClassId && x.Timetables.Any(s => s.TeacherId == contextAccessor.GetUserId()));

            var classRoomEntity = await RepositoryClassRoom.GetByAsync(injector);
            if (classRoomEntity == null)
            {
                return new BaseResponse<AssignmentResponseDto>()
                    .SetStatus(HttpStatusCode.Forbidden)
                    .SetMessage("You are not allowed to create assignment for this class");
            }

            var Repository = GetRepository();
            var assignmentEntity = mapper.Map<Assignment>(requestDto);
            assignmentEntity.CreatedById = contextAccessor.GetUserId();
            assignmentEntity.FilePath = fileStorage.SaveFile(requestDto.Attachment!);

            await Repository.AddAsync(assignmentEntity);
            await unitOfWork.SaveAsync();

            return (await GetAssignmentByIdAsync(assignmentEntity.Id))
                .SetStatus(HttpStatusCode.Created);
        }

        public async Task<IBaseResponse<AssignmentResponseDto>> GetAssignmentByIdAsync(int assignmentId)
        {
            var assignmentEntity = await GetRepository().GetByIdAsync<AssignmentResponseDto>(assignmentId);
            if (assignmentEntity == null)
            {
                return new BaseResponse<AssignmentResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Assignment not found");
            }

            return new BaseResponse<AssignmentResponseDto>()
                .SetStatus(HttpStatusCode.OK)
                .SetData(assignmentEntity);
        }

        public IBaseResponse<PaginateBlock<AssignmentResponseDto>> GetAllAssignments(PaginationFilter<AssignmentResponseDto> filter)
        {
            return new BaseResponse<PaginateBlock<AssignmentResponseDto>>()
                .SetStatus(HttpStatusCode.OK)
                .SetData(GetRepository().Filter(filter));
        }

        public async Task<IBaseResponse<object>> UpdateAssignment(int assignmentId, AssignmentRequestDto requestDto)
        {
            var assignmentEntity = await GetRepository().GetByIdAsync(assignmentId);
            if (assignmentEntity == null)
            {
                return new BaseResponse()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Assignment not found.");
            }
            mapper.Map(requestDto, assignmentEntity);
            assignmentEntity.FilePath = requestDto.Attachment != null ? fileStorage.SaveFile(requestDto.Attachment!) : assignmentEntity.FilePath;
            await unitOfWork.SaveAsync();

            return new BaseResponse()
                           .SetStatus(HttpStatusCode.NoContent)
                           .SetMessage("Assignment updated successfully.");
        }

        public async Task<IBaseResponse<object>> DeleteAssignment(int assignmentId)
        {
            var Repository = GetRepository();
            var assignmentEntity = await Repository.GetByIdAsync(assignmentId);
            if (assignmentEntity == null)
            {
                return new BaseResponse()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Assignment not found");
            }
            Repository.Delete(assignmentEntity);
            fileStorage.DeleteFile(assignmentEntity.FilePath);
            await unitOfWork.SaveAsync();
            return new BaseResponse()
                .SetStatus(HttpStatusCode.NoContent)
                .SetMessage("Assignment deleted successfully.");
        }

        //send submissions
        public async Task<IBaseResponse<AssignmentSubmissionResponseDto>> SendSubmissions(int assignmentId, AssignmentSubmissionRequestDto requestDto)
        {
            var Repository = GetRepository();
            var assignmentEntity = await Repository.GetByIdAsync(assignmentId);
            if (assignmentEntity == null)
            {
                return new BaseResponse<AssignmentSubmissionResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Assignment not found");
            }

            if (assignmentEntity.Deadline < DateTime.UtcNow)
            {
                return new BaseResponse<AssignmentSubmissionResponseDto>()
                    .SetStatus(HttpStatusCode.BadRequest)
                    .SetMessage("Assignment deadline has passed");
            }
            var submissionRepo = GetSubmissionRepository();

            var Entry = new AssignmentSubmission()
            {
                AssignmentId = assignmentId,
                StudentId = contextAccessor.GetUserId(),
                FilePath = fileStorage.SaveFile(requestDto.Attachment!)
            };


            var ExistsEntity = await submissionRepo.GetByAsync(new CommandsInjector<AssignmentSubmission>().Where(x => x.StudentId == contextAccessor.GetUserId() && x.AssignmentId == assignmentId));
            if (ExistsEntity != null)
            {
                fileStorage.DeleteFile(ExistsEntity.FilePath);
                Entry.FilePath = fileStorage.SaveFile(requestDto.Attachment!);
                await unitOfWork.SaveAsync();

                return (await GetSubmissionsByIdAsync(assignmentId, ExistsEntity.Id))
                    .SetStatus(HttpStatusCode.OK);
            }


            await submissionRepo.AddAsync(Entry);
            await unitOfWork.SaveAsync();
            return (await GetSubmissionsByIdAsync(assignmentId, Entry.Id))
                       .SetStatus(HttpStatusCode.Created);
        }

        public async Task<IBaseResponse<AssignmentSubmissionResponseDto>> GetSubmissionsByIdAsync(int assignmentId, int submissionsId)
        {
            var Repository = GetSubmissionRepository();
            var injector = new CommandsInjector<AssignmentSubmission>();
            injector.AddCommand(x => x.Where(x => x.AssignmentId == assignmentId && x.Id == submissionsId));

            var assignmentEntity = await Repository.GetByAsync<AssignmentSubmissionResponseDto>(injector);
            if (assignmentEntity == null)
            {
                return new BaseResponse<AssignmentSubmissionResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Submissions not found");
            }
            return new BaseResponse<AssignmentSubmissionResponseDto>()
                .SetStatus(HttpStatusCode.OK)
                .SetData(assignmentEntity);
        }

        public IBaseResponse<PaginateBlock<AssignmentSubmissionResponseDto>> GetAllSubmissionss(int assignmentId, PaginationFilter<AssignmentSubmissionResponseDto> filter)
        {
            var assignmentEntity = GetRepository().GetByIdAsync(assignmentId);
            if (assignmentEntity == null)
            {
                return new BaseResponse<PaginateBlock<AssignmentSubmissionResponseDto>>()
                    .SetStatus(HttpStatusCode.Forbidden)
                    .SetMessage("You are not allowed to see this assignment's submissions");
            }

            var Repository = GetSubmissionRepository();
            var injector = new CommandsInjector<AssignmentSubmission>();
            injector.AddCommand(x => x.Where(x => x.AssignmentId == assignmentId));

            return new BaseResponse<PaginateBlock<AssignmentSubmissionResponseDto>>()
                .SetStatus(HttpStatusCode.OK)
                .SetData(Repository.FilterBy(filter, injector));
        }

        public async Task<IBaseResponse<PaginateBlock<AssignmentSubmissionWithMissingResponseDto>>> GetAllSubmissionsWithMissingAsync
            (
            int assignmentId,
            PaginationFilter<AssignmentSubmissionWithMissingResponseDto> filter)
        {
            var assignmentEntity = await GetRepository().GetByIdAsync<AssignmentResponseDto>(assignmentId);
            if (assignmentEntity == null)
            {
                return new BaseResponse<PaginateBlock<AssignmentSubmissionWithMissingResponseDto>>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Assignment not found");
            }



            var students = unitOfWork.GetRepository<Enrollment>().AddInjector(enrollmentInjector).GetAllBy(
                new CommandsInjector<Enrollment>()
                    .Where(x => x.ClassRoom.Assignments.Any(a => a.Id == assignmentId))
            );

            var submissions = GetSubmissionRepository().GetAllBy(
                new CommandsInjector<AssignmentSubmission>()
                    .Where(x => x.AssignmentId == assignmentId)
            ).ProjectTo<AssignmentSubmissionResponseDto>(mapper.ConfigurationProvider);

            var resultList = students
                 .Select(x => new AssignmentSubmissionWithMissingResponseDto
                 {
                     Student = mapper.Map<AccountResponseDto>(x.Student),
                     Assignment = assignmentEntity,
                     Submission = submissions.FirstOrDefault(s => s.Student.Id == x.Student.Id && s.Assignment.Id == assignmentId),
                 });


            return new BaseResponse<PaginateBlock<AssignmentSubmissionWithMissingResponseDto>>()
                .SetStatus(HttpStatusCode.OK)
                .SetData(filter.Apply(resultList));
        }


        public IBaseResponse<PaginateBlock<AssignmentSubmissionWithMissingResponseDto>> GetAllStudentSubmissionsWithMissing
           (
           int studentId,
           PaginationFilter<AssignmentSubmissionWithMissingResponseDto> filter)
        {
            var assigments = GetRepository().GetAllBy(
                new CommandsInjector<Assignment>()
                    .Where(x => x.ClassRoom.Enrollments.Any(t => t.StudentId == studentId)))
                    .ProjectTo<AssignmentResponseDto>(mapper.ConfigurationProvider);

            var student = unitOfWork.GetRepository<Enrollment>().GetAll()
                .Select(x => x.Student)
                .ProjectTo<AccountResponseDto>(mapper.ConfigurationProvider)
                .FirstOrDefault(t => t.Id == studentId);
           
            if (student == null)
            {
                return new BaseResponse<PaginateBlock<AssignmentSubmissionWithMissingResponseDto>>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Student not found");
            }

            if (contextAccessor.GetUserRole() == Enums.eRole.Student && contextAccessor.GetUserId() != studentId)
            {
                return new BaseResponse<PaginateBlock<AssignmentSubmissionWithMissingResponseDto>>()
                    .SetStatus(HttpStatusCode.Forbidden)
                    .SetMessage("You are not allowed to see this student's submissions");
            }



            if (!assigments.Any())
            {
                return new BaseResponse<PaginateBlock<AssignmentSubmissionWithMissingResponseDto>>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Assignment not found");
            }

            var submissions = GetSubmissionRepository()
                .GetAll()
                .ProjectTo<AssignmentSubmissionResponseDto>(mapper.ConfigurationProvider);

            var resultList = assigments.Select(x => new AssignmentSubmissionWithMissingResponseDto
            {
                Student = student,
                Assignment = x,
                Submission = submissions.FirstOrDefault(s => s.Student.Id == studentId && s.Assignment.Id == x.Id),
            });


            return new BaseResponse<PaginateBlock<AssignmentSubmissionWithMissingResponseDto>>()
                .SetStatus(HttpStatusCode.OK)
                .SetData(filter.Apply(resultList));
        }

        public async Task<IBaseResponse<object>> DeleteSubmissions(int assignmentId, int submissionsId)
        {
            var submissionRepo = GetSubmissionRepository();
            var injector = new CommandsInjector<AssignmentSubmission>();
            injector.Where(x => x.AssignmentId == assignmentId && x.Id == submissionsId);


            var assignmentEntity = await submissionRepo.GetByAsync(injector);
            if (assignmentEntity == null)
            {
                return new BaseResponse()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Assignment or submissions not found");
            }

            submissionRepo.Delete(assignmentEntity);
            await unitOfWork.SaveAsync();
            return new BaseResponse()
                .SetStatus(HttpStatusCode.NoContent)
                .SetMessage("Submissions deleted successfully.");
        }
    }

}
