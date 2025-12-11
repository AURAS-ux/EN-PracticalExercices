using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingList.Domain.Entities
{
    public class ReadingStats
    {
        public int TotalBooks { get; set; }
        public int FinishedBooks { get; set; }
        public int UnfinishedBooks { get; set; }
        public float AverageRating { get; set; }
        public int TotalPages { get; set; }
        public List<AuthorStatistic> TopAuthors { get; set; } = new();

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"Total Books: {TotalBooks}");
            sb.AppendLine($"Finished: {FinishedBooks}");
            sb.AppendLine($"Unfinished: {UnfinishedBooks}");
            sb.AppendLine($"Average Rating: {AverageRating:F2}");
            sb.AppendLine($"Total Pages: {TotalPages}");
            sb.AppendLine("Top 5 Authors:");
            foreach (var author in TopAuthors)
            {
                sb.AppendLine($"  - {author.AuthorName}: {author.BookCount} book(s)");
            }
            return sb.ToString();
        }
    }
    public class AuthorStatistic
    {
        public required string AuthorName { get; set; }
        public int BookCount { get; set; }
    }
}
