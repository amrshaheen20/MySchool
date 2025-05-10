using AutoMapper;
using MySchool.API.Common;
using MySchool.API.Extensions;
using MySchool.API.Interfaces;
using MySchool.API.Models.DbSet;
using MySchool.API.Models.Dtos;
using MySchool.API.Services.AnnouncementContainer.Injector;
using System.Net;

namespace MySchool.API.Services.AnnouncementContainer
{
    public class AnnouncementService(IUnitOfWork unitOfWork,
                                     IMapper mapper,
                                     IHttpContextAccessor contextAccessor,
                                     AnnouncementInjector announcementInjector) : IServiceInjector
    {
        private IGenericRepository<Announcement> GetRepository()
        {
            return unitOfWork.GetRepository<Announcement>().AddInjector(announcementInjector);
        }

        public async Task<IBaseResponse<AnnouncementResponseDto>> CreateAnnouncementAsync(AnnouncementRequestDto requestDto)
        {
            var Repository = GetRepository();
            var Entity = mapper.Map<Announcement>(requestDto);

            Entity.CreatedById = contextAccessor.GetUserId();
            await Repository.AddAsync(Entity);
            await unitOfWork.SaveAsync();

            return new BaseResponse<AnnouncementResponseDto>()
                .SetStatus(HttpStatusCode.Created)
                .SetData(mapper.Map<AnnouncementResponseDto>(Entity));
        }
        public async Task<IBaseResponse<AnnouncementResponseDto>> GetAnnouncementByIdAsync(int AnnouncementId)
        {
            var Entity = await GetRepository().GetByIdAsync<AnnouncementResponseDto>(AnnouncementId);

            if (Entity == null)
            {
                return new BaseResponse<AnnouncementResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Announcement not found");
            }

            return new BaseResponse<AnnouncementResponseDto>()
                .SetStatus(HttpStatusCode.OK)
                .SetData(Entity);
        }

        public IBaseResponse<PaginateBlock<AnnouncementResponseDto>> GetAllAnnouncementAsync(PaginationFilter<AnnouncementResponseDto> filter)
        {
            return new BaseResponse<PaginateBlock<AnnouncementResponseDto>>()
                .SetStatus(HttpStatusCode.OK)
                .SetData(GetRepository().Filter(filter));
        }

        public async Task<IBaseResponse<object>> DeleteAnnouncementAsync(int AnnouncementId)
        {
            var Repository = GetRepository();
            var Entity = await Repository.GetByIdAsync(AnnouncementId);



            if (Entity == null)
            {
                return new BaseResponse()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Announcement not found");
            }

            Repository.Delete(Entity);
            await unitOfWork.SaveAsync();

            return new BaseResponse()
                .SetStatus(HttpStatusCode.OK)
                .SetMessage("Announcement deleted successfully");
        }



    }
}
