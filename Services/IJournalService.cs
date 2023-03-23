using ZetaTrading.Exceptions;

namespace ZetaTrading.Services
{
    public interface IJournalService
    {
        SecureException AddExceptionToJournal(Exception exception, string[] requestParams);
    }
}
