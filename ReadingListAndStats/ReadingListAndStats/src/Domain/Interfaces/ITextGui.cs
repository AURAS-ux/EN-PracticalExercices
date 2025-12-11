using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingList.src.Domain.Interfaces
{
    public interface ITextGui
    {
        public Task ShowImportMenuAsync();
        public Task ShowListAndQueryMenuAsync();
        public void ShowUpdateMenu();
        public void ShowExportMenu();
        public void ShowHelpAndExitMenu();
        public Task RunAsync();
    }
}
