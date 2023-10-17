using System;
using System.Collections.Generic;

namespace FileStorage.Feedback.Models.Db;

public partial class Bug
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Text { get; set; } = null!;

    public int BugStatusId { get; set; }

    public int AppPartId { get; set; }

    public virtual AppPart AppPart { get; set; } = null!;

    public virtual BugStatus BugStatus { get; set; } = null!;
}
