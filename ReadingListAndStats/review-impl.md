# Reading List & Stats - Implementation Review

## Findings
1. **Stats and query commands deviate from the spec**  
   Requirements 3 and 5 describe `by author <text>` as a case-insensitive contains search, `top rated n` as the highest-rated books regardless of status, and `stats` as total books, finished count, average rating, *pages by genre*, and the top three authors. `ListAndQueryService` instead uses strict equality for authors, filters `top N` down to finished books only, and collapses all per-genre data into a single page total while returning the top five authors (`ReadingList.Application/Services/ListAndQueryService.cs:25-107`). As written, the UI cannot satisfy the acceptance tests (for example, "Given imported books, when I run top rated 2") and the `stats` output is missing required sections.

2. **Imports abort on duplicates or malformed rows instead of skipping/logging them**  
   The CSV parser accumulates a single error string and returns `Result.Fail` whenever *any* row is malformed (`ReadingList.Infrastructure/Parsers/CsvParser.cs:14-58`), so even one bad record prevents every other valid row from loading. When the importer calls `AddMultipleAsync`, any duplicate IDs cause the entire batch to fail because `InMemoryRepository` aggregates errors and returns `Fail` instead of skipping duplicates and keeping the first occurrence (`ReadingList.Infrastructure/Persistance/InMemoryRepositroy.cs:25-58`). The CLI then only prints a generic success/error message and never reports the imported/duplicate/malformed counts that the requirements call out (`ReadingList.CLI/Menu/ImportMenu.cs:38-53`). This violates the "skip duplicates, log warnings, and show counts" acceptance criterion.

3. **Import command cannot resolve user-supplied paths**  
   `ImportCsv` insists on locating CSV files relative to the solution directory and unconditionally rewrites file names to `file + "csv"` when no extension is provided (`ReadingList.Infrastructure/Importers/ImportCsv.cs:24-118`). As a result `import file1` looks for `file1csv`, and paths such as `import C:\data\reading.csv` or files located next to the executable are rejected even though the spec states `import <file1.csv> [file2.csv ...]`. This makes the primary workflow unusable outside the dev machine.

4. **Export workflow ignores `export <format> <path>` and never asks about overwriting**  
   The CLI only accepts `export csv`/`export json` and always writes to an auto-generated timestamp under `./Exports`, never honoring a user-provided path or asking for confirmation when the target exists (`ReadingList.CLI/Menu/ExportMenu.cs:17-88`). The service layer keeps the same assumption and simply appends `.csv`/`.json` to whatever string it is given without checking for existing files (`ReadingList.Application/Services/ExportService.cs:17-39`). This contradicts the requirement: "`export json <path>` - async write ... Overwrite prompt on existing file (y/n)."

5. **List menu throws exceptions instead of handling empty results gracefully**  
   Every listing branch throws `new Exception(...)` when no rows are returned (`ReadingList.CLI/Menu/ListMenu.cs:45-160`). That bubbles out of the command loop, producing a crash/stack trace whenever the library is empty or a filter matches nothing, directly violating the non-functional requirement to "validate inputs and provide friendly errors" and the acceptance test "Given no books ... handles empty sequences without crashing".

6. **Ratings are forced to integers and lack validation**  
   The UI only accepts integer ratings (`ReadingList.CLI/Menu/UpdateMenu.cs:57-66`), and the repository method also takes an `int` and assigns it directly to `Book.Rating` without any bound checking (`ReadingList.Infrastructure/Persistance/InMemoryRepositroy.cs:97-109`). Sample data in `reqs.txt` uses fractional ratings (e.g., 4.5), so the current implementation cannot represent imported ratings accurately, nor can it enforce the 0-5 invariant the requirements describe.

7. **CSV parsing is not robust enough for the provided format**  
   `CsvParser` splits each line on `','` with no support for quoted commas or escaped characters and uses the current culture for `float.Parse` (`ReadingList.Infrastructure/Parsers/CsvParser.cs:23-43`). The sample CSVs already contain quoted strings, and ratings such as `4.5` will fail on locales that expect commas for decimals. Instead of logging and continuing, the parser aborts the entire import (see finding #2).

8. **No automated tests or extension helpers were delivered**  
   The solution file lists only the CLI, Application, Infrastructure, and Domain projects (`ReadingListAndStats.sln:1-26`); there is no `ReadingList.Tests` project or any other automated coverage, despite the testing plan/acceptance criteria calling for at least a handful of unit tests. Likewise, none of the proposed extension methods (for example, parsing helpers) were implemented - the parser even contains a `//TODO` comment acknowledging the gap. This leaves critical logic untested and makes future refactoring risky.

## Recommendations
- Revisit `ListAndQueryService` to match the contract (contains search, all books for rankings, explicit per-genre aggregates, and top-three authors). Extend `ReadingStats` so the CLI can print the requested sections.
- Adjust the parser/import pipeline to treat malformed rows and duplicate IDs as warnings: collect them, log/report counts back to the CLI, but continue importing valid records.
- Let `import` accept absolute or relative paths verbatim, fix the missing period when appending `.csv`, and stop assuming files live next to the solution.
- Update the export command signature to include a destination path, prompt before overwriting, and surface I/O failures clearly.
- Replace the exception-throwing branches in `ListMenu` with friendly messages; have the stats/query services return empty lists instead of `Result.Fail` when nothing matches.
- Model ratings as decimals, enforce the 0-5 invariant in the domain entity/repository, and allow users to enter fractional ratings that match the CSV schema.
- Swap in a proper CSV parser (or at least handle quoted fields and invariant numeric parsing) so the provided sample data loads reliably.
- Add the missing test project and start covering the repository, parser, importer, and query logic per the testing plan.
