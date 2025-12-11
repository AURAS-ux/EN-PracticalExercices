using ReadingList.src.Domain;

namespace ReadingList.Application.Interfaces
{
    public interface IUpdateable
    {
        Result<Book> MarkBookAsFinished(int bookId);
        Result<Book> RateBook(int bookId, int rating);
    }
}
