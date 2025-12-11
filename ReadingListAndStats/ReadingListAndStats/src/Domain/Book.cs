using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingList.src.Domain
{
    public class Book
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Author { get; set; }
        public int Year { get; set; }
        public int Pages { get; set; }
        public required string Genre { get; set; }
        public bool Finished { get; set; }
        public float Rating { get; set; }

        public override string ToString()
        {
            string status = Finished ? "✓ Finished" : "⏳ In Progress";
            string stars = new string('★', (int)Math.Round(Rating)) + new string('☆', 5 - (int)Math.Round(Rating));

            var sb = new StringBuilder();
            sb.AppendLine($"┌─ Book #{Id} ─────────────────────────────────");
            sb.AppendLine($"│ 📖 {Title}");
            sb.AppendLine($"│ ✍️  Author: {Author}");
            sb.AppendLine($"│ 📅 Year: {Year}  │  📚 Genre: {Genre}  │  📄 Pages: {Pages:N0}");
            sb.AppendLine($"│ {status}");
            sb.AppendLine($"│ ⭐ Rating: {stars} ({Rating:F1}/5.0)");
            sb.AppendLine($"└────────────────────────────────────────────");

            return sb.ToString();
        }
    }
}
