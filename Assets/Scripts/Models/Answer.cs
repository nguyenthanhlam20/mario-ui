public class Answer
{
    public int AnswerId { get; set; }

    public string Content { get; set; } = null!;

    public int? QuestionId { get; set; }

    public Question Question { get; set; }
}