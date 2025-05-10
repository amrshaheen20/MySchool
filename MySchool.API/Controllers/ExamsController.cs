using Microsoft.AspNetCore.Mvc;
using MySchool.API.Common;
using MySchool.API.Models.Dtos.Request;
using MySchool.API.Models.Dtos.Response;
using MySchool.API.Services;

namespace MySchool.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamsController(ExamService examService) : BaseController
    {

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ExamResponseDto))]
        public async Task<IActionResult> CreateExam([FromBody] ExamRequestDto exam)
        {
            return BuildResponse(await examService.CreateExamAsync(exam));
        }

        [HttpGet("{exam_id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExamResponseDto))]
        public async Task<IActionResult> GetExam(int exam_id)
        {
            return BuildResponse(await examService.GetExamByIdAsync(exam_id));
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginateBlock<ExamResponseDto>))]
        public async Task<IActionResult> GetExams([FromQuery] PaginationFilter<ExamResponseDto> filter)
        {
            return BuildResponse(await examService.GetExamsAsync(filter));
        }

        [HttpDelete("{exam_id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExamResponseDto))]
        public async Task<IActionResult> DeleteExam(int exam_id)
        {
            return BuildResponse(await examService.DeleteExamAsync(exam_id));
        }

        [HttpPut("{exam_id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExamResponseDto))]
        public async Task<IActionResult> UpdateExam(int exam_id, ExamRequestDto updatedExam)
        {
            return BuildResponse(await examService.UpdateExamAsync(exam_id, updatedExam));
        }

        //[HttpPost("{exam_id}/questions")]
        //[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(QuestionResponseDto))]
        //public async Task<IActionResult> CreateQuestion(int exam_id, QuestionRequestDto question)
        //{
        //    return BuildResponse(await examService.CreateQuestionAsync(exam_id, question));
        //}

        //[HttpPut("{exam_id}/questions/{question_id}")]
        //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(QuestionResponseDto))]
        //public async Task<IActionResult> UpdateQuestion(int exam_id, int question_id, QuestionRequestDto question)
        //{
        //    return BuildResponse(await examService.UpdateQuestionAsync(exam_id, question_id, question));
        //}


        //[HttpDelete("{exam_id}/questions/{question_id}")]
        //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(QuestionResponseDto))]
        //public async Task<IActionResult> DeleteQuestion(int exam_id, int question_id)
        //{
        //    return BuildResponse(await examService.DeleteQuestionAsync(exam_id, question_id));
        //}


        [HttpPost("{exam_id}/questions/{question_id}/answers")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AnswerResponseDto))]
        public async Task<IActionResult> AddStudentAnswer(int exam_id, int question_id, AnswerRequestDto answer)
        {
            return BuildResponse(await examService.AddStudentAnswerAsync(exam_id, question_id, answer));
        }

        [HttpGet("{exam_id}/questions/{question_id}/answers")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AnswerResponseDto))]
        public async Task<IActionResult> GetStudentAnswers(int exam_id, int question_id)
        {
            return BuildResponse(await examService.GetStudentAnswersAsync(exam_id, question_id));
        }

        [HttpGet("{exam_id}/questions/{question_id}/answers/{answer_id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AnswerResponseDto))]
        public async Task<IActionResult> GetStudentAnswer(int exam_id, int question_id, int answer_id)
        {
            return BuildResponse(await examService.GetStudentAnswerAsync(exam_id, question_id, answer_id));
        }

        [HttpPut("{exam_id}/questions/{question_id}/answers/{answer_id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AnswerResponseDto))]
        public async Task<IActionResult> UpdateStudentAnswer(int exam_id, int question_id, int answer_id, AnswerRequestDto answer)
        {
            return BuildResponse(await examService.UpdateStudentAnswerAsync(exam_id, question_id, answer_id, answer));
        }








    }


}
