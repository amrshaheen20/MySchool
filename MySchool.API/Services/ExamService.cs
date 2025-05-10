using AutoMapper;
using MySchool.API.Commands;
using MySchool.API.Common;
using MySchool.API.Interfaces;
using MySchool.API.Models.DbSet.ExamEntities;
using MySchool.API.Models.Dtos.Request;
using MySchool.API.Models.Dtos.Response;
using System.Net;

namespace MySchool.API.Services
{
    public class ExamService(IUnitOfWork unitOfWork, IMapper mapper) : IServiceInjector
    {
        private IGenericRepository<Exam> GetRepository()
        {
            return unitOfWork.GetRepository<Exam>().AddInjector(new ExamInjector(null, null));
        }

        public async Task<BaseResponse<ExamResponseDto>> CreateExamAsync(ExamRequestDto exam)
        {
            var Repository = GetRepository();
            var Entity = mapper.Map<Exam>(exam);
            Entity.CreatedById = 20; // TODO: Get current user id


            await Repository.AddAsync(Entity);
            await unitOfWork.SaveAsync();

            var newEntity = await Repository.GetByIdAsync(Entity.Id);

            return new BaseResponse<ExamResponseDto>()
                .SetStatus(HttpStatusCode.Created)
                .SetData(mapper.Map<ExamResponseDto>(newEntity));
        }


        public async Task<BaseResponse<ExamResponseDto>> GetExamByIdAsync(int examId)
        {
            var Repository = GetRepository();
            var Entity = await Repository.GetByIdAsync(examId);

            var exam = mapper.Map<ExamResponseDto>(Entity);

            foreach (var option in exam.Questions.SelectMany(q => q.Options))
            {
                option.IsCorrect = null;
            }

            return new BaseResponse<ExamResponseDto>()
                .SetStatus(HttpStatusCode.OK)
                .SetData(exam);
        }


        public async Task<BaseResponse<PaginateBlock<ExamResponseDto>>> GetExamsAsync(PaginationFilter<ExamResponseDto> filter)
        {
            var Repository = GetRepository();
            var Entities = await Repository.GetAllAsync();

            var pages = Repository.Filter(filter);

            foreach (var option in pages.Data.SelectMany(p => p.Questions.SelectMany(q => q.Options)))
            {
                option.IsCorrect = null;
            }

            return new BaseResponse<PaginateBlock<ExamResponseDto>>()
                .SetStatus(HttpStatusCode.OK)
                .SetData(pages);
        }


        public async Task<BaseResponse<ExamResponseDto>> DeleteExamAsync(int examId)
        {
            var Repository = GetRepository();
            var Entity = await Repository.GetByIdAsync(examId);
            if (Entity == null)
            {
                return new BaseResponse<ExamResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Exam not found");
            }
            Repository.Delete(Entity);
            await unitOfWork.SaveAsync();
            return new BaseResponse<ExamResponseDto>()
                .SetStatus(HttpStatusCode.OK)
                .SetData(mapper.Map<ExamResponseDto>(Entity));
        }



        public async Task<BaseResponse<ExamResponseDto>> UpdateExamAsync(int examId, ExamRequestDto updatedExam)
        {
            var Repository = GetRepository();
            var Entity = await Repository.GetByIdAsync(examId);
            if (Entity == null)
            {
                return new BaseResponse<ExamResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Exam not found");
            }
            mapper.Map(updatedExam, Entity);
            Repository.Update(Entity);
            await unitOfWork.SaveAsync();
            return new BaseResponse<ExamResponseDto>()
                .SetStatus(HttpStatusCode.OK)
                .SetData(mapper.Map<ExamResponseDto>(Entity));
        }


        public async Task<BaseResponse<QuestionResponseDto>> CreateQuestionAsync(int examId, QuestionRequestDto question)
        {
            var Repository = GetRepository();
            var ExamEntity = await Repository.GetByIdAsync(examId);
            if (ExamEntity == null)
            {
                return new BaseResponse<QuestionResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Exam not found");
            }
            var QuestionEntity = mapper.Map<Question>(question);
            ExamEntity.Questions.Add(QuestionEntity);
            await unitOfWork.SaveAsync();
            var newEntity = await Repository.GetByIdAsync(QuestionEntity.Id);
            return new BaseResponse<QuestionResponseDto>()
                .SetStatus(HttpStatusCode.Created)
                .SetData(mapper.Map<QuestionResponseDto>(newEntity));
        }


        public async Task<BaseResponse<QuestionResponseDto>> UpdateQuestionAsync(int examId, int questionId, QuestionRequestDto question)
        {
            var Repository = GetRepository();
            var ExamEntity = await Repository.GetByIdAsync(examId);
            if (ExamEntity == null)
            {
                return new BaseResponse<QuestionResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Exam not found");
            }
            var QuestionEntity = ExamEntity.Questions.FirstOrDefault(x => x.Id == questionId);
            if (QuestionEntity == null)
            {
                return new BaseResponse<QuestionResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Question not found");
            }
            mapper.Map(question, QuestionEntity);
            Repository.Update(ExamEntity);
            await unitOfWork.SaveAsync();
            return new BaseResponse<QuestionResponseDto>()
                .SetStatus(HttpStatusCode.OK)
                .SetData(mapper.Map<QuestionResponseDto>(QuestionEntity));
        }

        public async Task<BaseResponse<QuestionResponseDto>> DeleteQuestionAsync(int examId, int questionId)
        {
            var Repository = GetRepository();
            var ExamEntity = await Repository.GetByIdAsync(examId);
            if (ExamEntity == null)
            {
                return new BaseResponse<QuestionResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Exam not found");
            }
            var QuestionEntity = ExamEntity.Questions.FirstOrDefault(x => x.Id == questionId);
            if (QuestionEntity == null)
            {
                return new BaseResponse<QuestionResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Question not found");
            }
            ExamEntity.Questions.Remove(QuestionEntity);
            await unitOfWork.SaveAsync();
            return new BaseResponse<QuestionResponseDto>()
                .SetStatus(HttpStatusCode.OK)
                .SetData(mapper.Map<QuestionResponseDto>(QuestionEntity));
        }

        public async Task<BaseResponse<AnswerResponseDto>> AddStudentAnswerAsync(int examId, int questionId, AnswerRequestDto answer)
        {
            var Repository = GetRepository();
            var ExamEntity = await Repository.GetByIdAsync(examId);
            if (ExamEntity == null)
            {
                return new BaseResponse<AnswerResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Exam not found");
            }
            var QuestionEntity = ExamEntity.Questions.FirstOrDefault(x => x.Id == questionId);
            if (QuestionEntity == null)
            {
                return new BaseResponse<AnswerResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Question not found");
            }
            var AnswerEntity = mapper.Map<StudentAnswer>(answer);
            QuestionEntity.Answers.Add(AnswerEntity);
            await unitOfWork.SaveAsync();
            var newEntity = await Repository.GetByIdAsync(AnswerEntity.Id);
            return new BaseResponse<AnswerResponseDto>()
                .SetStatus(HttpStatusCode.Created)
                .SetData(mapper.Map<AnswerResponseDto>(newEntity));
        }

        public async Task<BaseResponse<AnswerResponseDto>> GetStudentAnswersAsync(int examId, int questionId)
        {
            var Repository = GetRepository();
            var ExamEntity = await Repository.GetByIdAsync(examId);
            if (ExamEntity == null)
            {
                return new BaseResponse<AnswerResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Exam not found");
            }
            var QuestionEntity = ExamEntity.Questions.FirstOrDefault(x => x.Id == questionId);
            if (QuestionEntity == null)
            {
                return new BaseResponse<AnswerResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Question not found");
            }
            return new BaseResponse<AnswerResponseDto>()
                .SetStatus(HttpStatusCode.OK)
                .SetData(mapper.Map<AnswerResponseDto>(QuestionEntity.Answers));
        }

        public async Task<BaseResponse<AnswerResponseDto>> GetStudentAnswerAsync(int examId, int questionId, int answerId)
        {
            var Repository = GetRepository();
            var ExamEntity = await Repository.GetByIdAsync(examId);
            if (ExamEntity == null)
            {
                return new BaseResponse<AnswerResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Exam not found");
            }
            var QuestionEntity = ExamEntity.Questions.FirstOrDefault(x => x.Id == questionId);
            if (QuestionEntity == null)
            {
                return new BaseResponse<AnswerResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Question not found");
            }
            var AnswerEntity = QuestionEntity.Answers.FirstOrDefault(x => x.Id == answerId);
            if (AnswerEntity == null)
            {
                return new BaseResponse<AnswerResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Answer not found");
            }
            return new BaseResponse<AnswerResponseDto>()
                .SetStatus(HttpStatusCode.OK)
                .SetData(mapper.Map<AnswerResponseDto>(AnswerEntity));
        }

        public async Task<BaseResponse<AnswerResponseDto>> UpdateStudentAnswerAsync(int examId, int questionId, int answerId, AnswerRequestDto answer)
        {
            var Repository = GetRepository();
            var ExamEntity = await Repository.GetByIdAsync(examId);
            if (ExamEntity == null)
            {
                return new BaseResponse<AnswerResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Exam not found");
            }
            var QuestionEntity = ExamEntity.Questions.FirstOrDefault(x => x.Id == questionId);
            if (QuestionEntity == null)
            {
                return new BaseResponse<AnswerResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Question not found");
            }
            var AnswerEntity = QuestionEntity.Answers.FirstOrDefault(x => x.Id == answerId);
            if (AnswerEntity == null)
            {
                return new BaseResponse<AnswerResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Answer not found");
            }
            mapper.Map(answer, AnswerEntity);
            Repository.Update(ExamEntity);
            await unitOfWork.SaveAsync();
            return new BaseResponse<AnswerResponseDto>()
                .SetStatus(HttpStatusCode.OK)
                .SetData(mapper.Map<AnswerResponseDto>(AnswerEntity));
        }








    }
}
