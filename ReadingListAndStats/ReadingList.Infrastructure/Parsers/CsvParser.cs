using ReadingList.Application.Interfaces;
using ReadingList.src.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ReadingList.Infrastructure.Parsers
{
    public class CsvParser : IParser<Book>
    {
        public Result<List<Book>> Parse(string[] lines,bool hasHeader)
        {
            var books = new List<Book>();
            int startLine = hasHeader ? 1 : 0;
            Result<List<Book>> result;
            StringBuilder errorMessages = new StringBuilder();
            for (int i = startLine; i < lines.Length; i++)
            {
                var line = lines[i];
                var fields = line.Split(',');
                if (fields.Length != 8)
                {
                    errorMessages.AppendLine($"Skipping line {i + 1}: Expected 8 fields but got {fields.Length}.\n");
                    continue;
                }
                try
                {
                    var book = new Book
                    {
                        Id = int.Parse(fields[0].Trim()),
                        Title = SanitizeField(fields[1]),
                        Author = fields[2].Trim(),
                        Year = int.Parse(fields[3].Trim()),
                        Pages = int.Parse(fields[4].Trim()),
                        Genre = fields[5].Trim(),
                        Finished = ParseFinishedField(fields[6]),
                        Rating = float.Parse(fields[7].Trim())
                    };
                    books.Add(book);
                }
                catch (Exception ex)
                {
                    errorMessages.AppendLine($"Skipping line {i + 1}: Error parsing fields. {ex.Message}\n");
                    continue;
                }
            }
            if (errorMessages.Length > 0)
            {
                result = Result<List<Book>>.Fail(errorMessages.ToString());
            }
            else
            {
                result = Result<List<Book>>.Ok(books);
            }
            return result;
        }

        private static string SanitizeField(string field)
        {
            if (string.IsNullOrEmpty(field))
            {
                return string.Empty;
            }
            return field.Trim().Replace("\"", "").Replace("\'", "");
        } //TODO move as extention method

        private static bool ParseFinishedField(string field)
        {
            var sanitized = field.Trim().ToLower();
            return sanitized == "yes" || sanitized == "true" || sanitized == "1" || sanitized == "y";
        }
    }
}
