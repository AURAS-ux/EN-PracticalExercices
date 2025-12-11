using ReadingList.src.App;
using ReadingList.src.Domain.Interfaces;
using ReadingList.src.Infrastructure;
using ReadingList.src.Infrastructure.Services;
Console.OutputEncoding = System.Text.Encoding.UTF8;
try
{
    AppLogger.Initialize();

    AppLogger.Logger.Information("Starting ReadingList App...");

    ITextGui app = new ConsoleGui();
    await app.RunAsync();

    AppLogger.Logger.Information("Application shutting down.");
}
catch (Exception ex)
{
    AppLogger.Logger?.Fatal(ex, "Fatal error");
}
finally
{
    AppLogger.Close();
}
