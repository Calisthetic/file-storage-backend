using FileStorage.Feedback.Models.Db;
using FileStorage.Feedback.Models.Outcoming;
using Mapster;

namespace FileStorage.Feedback.Services;

public class RegisterMapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Bugs
        config.NewConfig<Bug, BugInfoDto>()
            .Map(d => d.AppPart, r => r.AppPart.Name)
            .Map(d => d.BugStatus, r => r.BugStatus.Name)
            .RequireDestinationMemberSource(true);

        // Questions
        config.NewConfig<Question, QuestionInfoDto>()
            .RequireDestinationMemberSource(true);
    }
}

