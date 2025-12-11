namespace ReadingList.src.Domain.Interfaces
{
    public interface IImportService
    {
        public Task<Result<List<Book>>> Import(string[] files);
    }
}
