﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using MySchool.API.Common;
using MySchool.API.Extensions;
using MySchool.API.Interfaces;
using MySchool.API.Models.DbSet;
using MySchool.API.Models.Dtos;
using MySchool.API.Services.AccountContainer.Injector;
using MySchool.API.Services.Common;
using System.Net;

namespace MySchool.API.Services.AccountContainer
{
    public class AccountService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        JwtService jwtService,
        IHttpContextAccessor contextAccessor,
        AccountInjector accountInjector
        ) : IServiceInjector

    {
        private IGenericRepository<User> GetRepository()
        {
            return unitOfWork.GetRepository<User>().AddInjector(accountInjector);
        }

        public async Task<IBaseResponse<AccountAdminResponseDto>> CreateAccountAsync(AccountRequestDto account)
        {
            var userRepository = GetRepository();

            if (await userRepository.AnyAsync(x => x.UserName == account.UserName))
            {
                return new BaseResponse<AccountAdminResponseDto>()
                    .SetStatus(HttpStatusCode.Conflict)
                    .SetMessage("Username already exists.");
            }

            var userEntity = mapper.Map<User>(account);

            await userRepository.AddAsync(userEntity);
            await unitOfWork.SaveAsync();

            return new BaseResponse<AccountAdminResponseDto>()
                .SetStatus(HttpStatusCode.Created)
                .SetData(mapper.Map<AccountAdminResponseDto>(userEntity));
        }



        public async Task<IBaseResponse<AccountAdminResponseDto>> GetAccountByIdAsync(int accountId)
        {
            var userEntity = await GetRepository().GetByIdAsync<AccountAdminResponseDto>(accountId);

            if (userEntity == null)
            {
                return new BaseResponse<AccountAdminResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Account not found.");
            }

            return new BaseResponse<AccountAdminResponseDto>()
                .SetData(userEntity);
        }


        public IBaseResponse<PaginateBlock<AccountAdminResponseDto>> GetAllAccounts(PaginationFilter<AccountAdminResponseDto> filter)
        {
            return new BaseResponse<PaginateBlock<AccountAdminResponseDto>>()
                .SetData(GetRepository().Filter(filter));
        }

        public async Task<IBaseResponse<object>> UpdateAccountAsync(int accountId, AccountRequestDto updatedAccount)
        {
            var userRepository = GetRepository();
            var userEntity = await userRepository.GetByIdAsync(accountId);

            if (userEntity == null)
            {
                return new BaseResponse()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Account not found.");
            }

            mapper.Map(updatedAccount, userEntity);

            userRepository.Update(userEntity);
            await unitOfWork.SaveAsync();

            return new BaseResponse()
                .SetStatus(HttpStatusCode.NoContent)
                .SetMessage("Account updated successfully.");
        }

        public async Task<IBaseResponse<object>> DeleteAccountByIdAsync(int accountId)
        {
            var userRepository = GetRepository();
            var userEntity = await userRepository.GetByIdAsync(accountId);

            if (userEntity == null)
            {
                return new BaseResponse()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Account not found.");
            }

            userRepository.Delete(userEntity);

            await unitOfWork.SaveAsync();
            return new BaseResponse()
                .SetStatus(HttpStatusCode.NoContent)
                .SetMessage("Account deleted successfully.");
        }

        #region Auth

        //public async Task<IBaseResponse<bool>> IsUserOnlineAsync(int userId)
        //{
        //    var user = await unitOfWork.GetRepository<User>().AddInjector(pepoleInjector).GetByIdAsync(userId);
        //    if (user == null)
        //    {
        //        return new BaseResponse<bool>()
        //            .SetStatus(HttpStatusCode.NotFound)
        //            .SetMessage("User not found.");
        //    }

        //    var IsOnline = user.LastActiveTime <= DateTime.UtcNow.AddMinutes(10);

        //    return new BaseResponse<bool>()
        //        .SetStatus(HttpStatusCode.OK)
        //        .SetData(IsOnline);
        //}

        public IBaseResponse<AccountAdminResponseDto> GetAccountByAuthAsync()
        {
            return new BaseResponse<AccountAdminResponseDto>()
                .SetData(mapper.Map<AccountAdminResponseDto>(contextAccessor.GetCurrentUser()));
        }

        public async Task<IBaseResponse<LoginResponseDto>> LoginAsync(LoginRequestDto loginInfo)
        {
            var injector = new CommandsInjector<User>();
            injector.Where(x => x.UserName == loginInfo.UserName);

            var user = await GetRepository().GetByAsync(injector);

            if (user == null || !user.PasswordHash.VerifyPassword(loginInfo.Password))
            {
                return new BaseResponse<LoginResponseDto>()
                    .SetStatus(HttpStatusCode.Unauthorized)
                    .SetMessage("Invalid credentials");
            }

            if (!user.IsActive)
            {
                return new BaseResponse<LoginResponseDto>()
                    .SetStatus(HttpStatusCode.Unauthorized)
                    .SetMessage("Account is not active");
            }

            return await GenerateTokenAsync(user);
        }

        public async Task<IBaseResponse<object>> Logout()
        {
            var tokenJti = contextAccessor.GetTokenJti();

            await unitOfWork.GetRepository<UserSession>()
                .GetAll()
                .Where(x => x.TokenId == tokenJti)
                .ExecuteDeleteAsync();

            return new BaseResponse<object>()
                .SetStatus(HttpStatusCode.OK)
                .SetMessage("User is logged out.");
        }

        public async Task<IBaseResponse<LoginResponseDto>> ChangePassword(ChangePasswordRequestDto request)
        {
            var repository = GetRepository();

            var user = contextAccessor.GetCurrentUser();

            if (user == null || !user.PasswordHash.VerifyPassword(request.OldPassword))
            {
                return new BaseResponse<LoginResponseDto>()
                    .SetStatus(HttpStatusCode.BadRequest)
                    .SetMessage("Invalid old password.");
            }

            user.PasswordHash = request.NewPassword.HashPassword();
            user.MustChangePassword = false;
            repository.Update(user);
            await LogOutUserFromAllSessionsAsync(user.Id);
            await unitOfWork.SaveAsync();


            return await GenerateTokenAsync(user);
        }

        private async Task LogOutUserFromAllSessionsAsync(int userId)
        {
            await unitOfWork.GetRepository<UserSession>()
                .GetAll()
                .Where(x => x.UserId == userId)
                .ExecuteDeleteAsync();
        }

        private async Task<IBaseResponse<LoginResponseDto>> GenerateTokenAsync(User user)
        {
            var token = jwtService.GenerateJwtToken(user);
           
            StringValues userAgentValues;
            contextAccessor.HttpContext!.Request.Headers.TryGetValue(HeaderNames.UserAgent, out userAgentValues);
            var userSession = new UserSession
            {
                UserId = user.Id,
                TokenId = token.Token.Id,
                ExpirationTime = token.Token.ValidTo,
                UserAgent = userAgentValues.ToString()
            };

            await unitOfWork.GetRepository<UserSession>().AddAsync(userSession);
            await unitOfWork.SaveAsync();

            return new BaseResponse<LoginResponseDto>()
                .SetData(new LoginResponseDto
                {
                    Token = token.GenerateToken(),
                    Expiration = token.Token.ValidTo,
                    User = mapper.Map<AccountAdminResponseDto>(user)
                });
        }
        #endregion
    }
}
