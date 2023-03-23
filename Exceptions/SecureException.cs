namespace ZetaTrading.Exceptions
{
    public class SecureException
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string? EventId { get; set; }
        public DateTime TimeStamp { get; set; }
        public string? QueryParameters { get; set; }
        public string? BodyParameters { get; set; }
        public string? StackTrace { get; set; }
        
    }
}
