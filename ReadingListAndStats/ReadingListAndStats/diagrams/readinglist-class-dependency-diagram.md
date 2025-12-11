# ReadingList Class Dependency Diagram

This diagram highlights the core classes, interfaces, and how they depend on each other within the ReadingList application.

```mermaid
classDiagram
    direction LR

    class Program {
        +Main()
    }

    class AppLogger {
        <<static>>
        +Initialize()
        +Close()
    }

    class ConsoleGui {
        -ILogger logger
        -IImportService importer
        -IRepository<int, Book> bookRepository
        +RunAsync()
        +ShowImportMenuAsync()
        +ShowListAndQueryMenuAsync()
        +ShowUpdateMenu()
        +ShowExportMenu()
        +ShowHelpAndExitMenu()
    }

    class AppUtils {
        <<static>>
        +HandleImport(args, importer, repository)
        +ComputeUserStats(repository)
    }

    class CsvImporterService {
        +Import(files)
    }

    class CsvBookParserService {
        <<static>>
        +ParseCsvLines(lines, hasHeader, out error)
    }

    class InMemoryRepository~TKey, T~ {
        +Add(key, entity)
        +AddMultipleAsync(keys, entities)
        +GetAllAsync(ct)
        +GetAllWhen(predicate)
        +GetById(id)
    }

    class UpdateRepositoryService~TKey, T~ {
        +UpdateFinished(id, repository)
        +RateAsync(id, rating, repository)
    }

    class Book {
        +Id
        +Title
        +Author
        +Year
        +Pages
        +Genre
        +Finished
        +Rating
    }

    class Stats {
        +FinishedBooks
        +TotalBooks
        +AvgRating
        +PagesByGenre
        +TopAutorsByBookCount
    }

    class Result~T~ {
        +IsSuccess
        +Value
        +Error
        +Ok(value)
        +Fail(error)
    }

    class IConsoleGui {
        <<interface>>
        +ShowImportMenuAsync()
        +ShowListAndQueryMenuAsync()
        +ShowUpdateMenu()
        +ShowExportMenu()
        +ShowHelpAndExitMenu()
        +RunAsync()
    }

    class IImportService {
        <<interface>>
        +Import(files)
    }

    class IRepository~TKey, T~ {
        <<interface>>
        +GetById(id)
        +GetAllAsync(ct)
        +GetAllWhen(predicate)
        +Add(key, entity)
        +AddMultipleAsync(keys, entities)
        +UpdateFinishedAsync(id)
        +RateAsync(id)
        +DeleteAsync(id)
    }

    class IUpdateRepositoryService~TKey, T~ {
        <<interface>>
        +UpdateFinished(id, repository)
        +RateAsync(id, rating, repository)
    }

    class Serilog {
        <<external>>
    }

    Program --> AppLogger : init & shutdown
    Program --> IConsoleGui : run app
    ConsoleGui ..|> IConsoleGui
    ConsoleGui --> AppLogger : logging
    ConsoleGui --> AppUtils : helper calls
    ConsoleGui --> IImportService : import flow
    ConsoleGui --> IRepository~int, Book~ : persistence
    ConsoleGui --> UpdateRepositoryService~int, Book~ : updates
    ConsoleGui --> Stats : display stats
    ConsoleGui --> Book : display data
    AppUtils --> AppLogger : logging
    AppUtils --> IImportService : import orchestration
    AppUtils --> IRepository~int, Book~ : persistence
    AppUtils --> Result~List<Book>~ : import result
    AppUtils --> Stats : produce stats
    IImportService <|.. CsvImporterService
    CsvImporterService --> CsvBookParserService
    CsvImporterService --> Result~List<Book>~
    CsvImporterService --> Book
    CsvBookParserService --> Book
    CsvBookParserService --> AppLogger
    IRepository~TKey, T~ <|.. InMemoryRepository~TKey, T~
    InMemoryRepository~TKey, T~ --> Result~T~
    IUpdateRepositoryService~TKey, T~ <|.. UpdateRepositoryService~TKey, T~
    UpdateRepositoryService~TKey, T~ --> AppLogger
    UpdateRepositoryService~TKey, T~ --> Result~T~
    UpdateRepositoryService~int, Book~ --> Book
    AppLogger --> Serilog
    Result~T~ --> Book : used with Book
```

**Notes**
- `UpdateRepositoryService` is intended to implement `IUpdateRepositoryService`, but the current code references a non-existent `IUpdateRepository` type; aligning those names will restore the intended contract.
- External logging goes through Serilog via the static `AppLogger` facade.
