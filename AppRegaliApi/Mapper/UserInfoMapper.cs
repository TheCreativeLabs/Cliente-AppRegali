namespace AppRegaliApi.Models
{
    using System;
    using System.Collections.Generic;
    using System.Security.Principal;

    public partial class UserInfoMapper
    {
        public UserInfoMapper()
        {
        }

        public static UserInfoDto UserInfoToUserInfoDto(UserInfo userInfo, String email)
        {
            UserInfoDto userInfoDto = new UserInfoDto();
            userInfoDto.Cognome = userInfo.Cognome;
            userInfoDto.Nome = userInfo.Nome;
            userInfoDto.DataDiNascita = userInfo.DataDiNascita;
            userInfoDto.FotoProfilo = userInfo.FotoProfilo;
            userInfoDto.IdAspNetUser = userInfo.IdAspNetUser;
            userInfoDto.Email = email;
            userInfoDto.PhotoUrl = userInfo.PhotoUrl;
            return userInfoDto;
        }

        public static List<UserInfoDto> UserInfoToUserInfoDtoList(List<UserInfo> listSource)
        {
            if (listSource == null || listSource.Count == 0)
            {
                return new List<UserInfoDto>();
            }
            List<UserInfoDto> listDto = new List<UserInfoDto>();
            listSource.ForEach(x => listDto.Add(UserInfoToUserInfoDto(x, null))); //FIXME non possiamo avere l'informazione della mail per ora
            return listDto;
        }
    }
}
