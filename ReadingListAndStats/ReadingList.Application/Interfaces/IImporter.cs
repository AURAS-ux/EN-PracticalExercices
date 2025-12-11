namespace ReadingList.src.Domain.Interfaces
{
    public interface IImporter
    {
        public Task<Result<List<Book>>> Import(string[] files);
    }
}
