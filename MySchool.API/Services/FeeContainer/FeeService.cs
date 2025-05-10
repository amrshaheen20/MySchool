using AutoMapper;
using MySchool.API.Common;
using MySchool.API.Extensions;
using MySchool.API.Interfaces;
using MySchool.API.Models.DbSet;
using MySchool.API.Models.Dtos;
using MySchool.API.Services.FeeContainer.Injector;
using System.Net;

namespace MySchool.API.Services.FeeContainer
{
    public class FeeService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        FeeInjector feeInjector,
        IHttpContextAccessor contextAccessor
        ) : IServiceInjector
    {
        private IGenericRepository<Fee> GetRepository()
        {
            return unitOfWork.GetRepository<Fee>().AddInjector(feeInjector);
        }

        public async Task<IBaseResponse<FeeResponseDto>> CreateFeeAsync(FeeRequestDto requestDto)
        {
            var feeRepo = GetRepository();
            var Entity = mapper.Map<Fee>(requestDto);

            var user = contextAccessor.HttpContext!.GetCurrentUser();
            Entity.CreatedById = user.Id;
            Entity.CreatedBy = user;

            await feeRepo.AddAsync(Entity);
            await unitOfWork.SaveAsync();

            return new BaseResponse<FeeResponseDto>()
                .SetStatus(HttpStatusCode.Created)
                .SetData(mapper.Map<FeeResponseDto>(Entity));
        }

        public async Task<IBaseResponse<FeeResponseDto>> GetFeeByIdAsync(int feeId)
        {
            var feeEntity = await GetRepository().GetByIdAsync<FeeResponseDto>(feeId);
            if (feeEntity == null)
            {
                return new BaseResponse<FeeResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Fee not found.");
            }
            return new BaseResponse<FeeResponseDto>()
                .SetStatus(HttpStatusCode.OK)
                .SetData(feeEntity);
        }

        public IBaseResponse<PaginateBlock<FeeResponseDto>> GetAllFees(PaginationFilter<FeeResponseDto> filter)
        {
            return new BaseResponse<PaginateBlock<FeeResponseDto>>()
                .SetStatus(HttpStatusCode.OK)
                .SetData(GetRepository().Filter(filter));
        }

        public async Task<IBaseResponse<FeeResponseDto>> UpdateFeeAsync(int feeId, FeeRequestDto requestDto)
        {
            var feeRepo = GetRepository();
            var feeEntity = await feeRepo.GetByIdAsync(feeId);
            if (feeEntity == null)
            {
                return new BaseResponse<FeeResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Fee not found.");
            }
            mapper.Map(requestDto, feeEntity);
            feeRepo.Update(feeEntity);
            await unitOfWork.SaveAsync();
            return new BaseResponse<FeeResponseDto>()
                .SetStatus(HttpStatusCode.NoContent)
                .SetMessage("Fee updated successfully.");
        }

        public async Task<IBaseResponse<object>> DeleteFeeAsync(int feeId)
        {
            var feeRepo = GetRepository();
            var feeEntity = await feeRepo.GetByIdAsync(feeId);
            if (feeEntity == null)
            {
                return new BaseResponse()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Fee not found.");
            }

            feeRepo.Delete(feeEntity);
            await unitOfWork.SaveAsync();

            return new BaseResponse()
                .SetStatus(HttpStatusCode.NoContent)
                .SetMessage("Fee deleted successfully.");
        }
    }
}
