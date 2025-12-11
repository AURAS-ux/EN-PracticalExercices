using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingList.src.Domain
{
    public class Stats
    {
        public int FinishedBooks { get; set; }
        public int TotalBooks { get; set; }
        public float AvgRating { get; set; }
        public List<(int, string)> PagesByGenre { get; set; } = [];
        public List<(string, int)> TopAutorsByBookCount { get; set; } = [];
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("═══════════════════════════════════════");
            sb.AppendLine("          READING STATISTICS           ");
            sb.AppendLine("═══════════════════════════════════════");
            sb.AppendLine();
            sb.AppendLine($"📚 Total Books:        {TotalBooks}");
            sb.AppendLine($"✓  Finished Books:     {FinishedBooks}");
            sb.AppendLine($"⭐ Average Rating:     {AvgRating:F2}/5.0");
            sb.AppendLine();

            if (PagesByGenre.Count > 0)
            {
                sb.AppendLine("───────────────────────────────────────");
                sb.AppendLine("   PAGES READ BY GENRE");
                sb.AppendLine("───────────────────────────────────────");
                foreach (var (pages, genre) in PagesByGenre)
                {
                    sb.AppendLine($"  📖 {genre,-25} {pages,6:N0} pages");
                }
                sb.AppendLine();
            }

            if (TopAutorsByBookCount.Count > 0)
            {
                sb.AppendLine("───────────────────────────────────────");
                sb.AppendLine("   TOP AUTHORS BY BOOK COUNT");
                sb.AppendLine("───────────────────────────────────────");
                for (int i = 0; i < TopAutorsByBookCount.Count; i++)
                {
                    var (author, count) = TopAutorsByBookCount[i];
                    sb.AppendLine($"  {i + 1}. {author,-30} ({count} book{(count != 1 ? "s" : "")})");
                }
            }

            sb.AppendLine("═══════════════════════════════════════");

            return sb.ToString();
        }
    }
}
