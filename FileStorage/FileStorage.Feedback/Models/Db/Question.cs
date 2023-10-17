using System;
using System.Collections.Generic;

namespace FileStorage.Feedback.Models.Db;

public partial class Question
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Text { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string? Answer { get; set; }

    public DateTime? RespondedAt { get; set; }
}
