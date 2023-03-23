using ZetaTrading.Data;
using ZetaTrading.Exceptions;

namespace ZetaTrading.Services
{
    public class JournalService : IJournalService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<JournalService> _logger;
        public JournalService(ApplicationDbContext dbContext, ILogger<JournalService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        public SecureException AddExceptionToJournal(Exception exception, string[] requestParams)
        {
            var journal = new SecureException
            {
                EventId = Guid.NewGuid().ToString(),
                TimeStamp = DateTime.Now,
                Type = requestParams[0],
                QueryParameters = requestParams[1],
                BodyParameters = exception.Message,
                StackTrace = exception.StackTrace,
               
            };

            _dbContext.ExceptionJournal.Add(journal);
            try
            {
                _dbContext.SaveChanges();
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while saving exception to journal");
            }
            return journal;
        }

    }
}
