namespace EmailValidator.Models
{
    using System.Collections.Generic;
    using DnsClient.Protocol;

    public class DnsValidationResult : ValidationResult
    {
        public IEnumerable<DnsResourceRecord> RecordsFound { get; set; }
    }
}