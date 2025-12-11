using ReadingList.src.Domain.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingList.src.Infrastructure.Services
{
    public class ExportService
    {
        private IExportStrategy _strategy;
        private ILogger logger = AppLogger.Logger;

        public ExportService()
        {
            _strategy = new JsonExportStrategyService();
            logger.Information($"Initialtied export using {_strategy.GetType().Name}");
        }
        public ExportService(IExportStrategy strategy)
        {
            logger.Information($"Initialtied export using {strategy.GetType().Name}");
            _strategy = strategy;
        }
    }
}
