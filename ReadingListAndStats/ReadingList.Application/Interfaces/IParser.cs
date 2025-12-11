using ReadingList.src.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingList.Application.Interfaces
{
    public interface IParser<T> where T : class
    {
        public Result<List<T>> Parse(string[] lines,bool hasHeader);
    }
}
