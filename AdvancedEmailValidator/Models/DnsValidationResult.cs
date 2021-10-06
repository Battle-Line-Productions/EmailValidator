namespace AdvancedEmailValidator.Models
{
    using System.Collections.Generic;
    using DnsClient.Protocol;

    public class DnsValidationResult
    {
        public IEnumerable<DnsResourceRecord> RecordsFound { get; set; }
    }
}