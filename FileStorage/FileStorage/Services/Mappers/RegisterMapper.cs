using FileStorage.Models.Db;
using FileStorage.Models.Outcoming;
using Mapster;

namespace FileStorage.Services.Mappers
{
    public class RegisterMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // Users
            config.NewConfig<User, UserInfoDto>()
                .Map(dto => dto.PrimaryEmail, res => res.PrimaryEmail == null ? "" : res.PrimaryEmail.Name)
                .RequireDestinationMemberSource(true);
        }
    }
}
