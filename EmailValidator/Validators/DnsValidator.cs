namespace EmailValidator.Validators
{
    using System.Linq;
    using DnsClient;
    using DnsClient.Protocol;
    using Models;

    public class DnsValidator
    {
        private readonly ILookupClient _client;
        private readonly ValidationOptions _options;
        
        public DnsValidator(ILookupClient client, ValidationOptions options)
        {
            _options = options ?? new ValidationOptions();
            _client = client ?? new LookupClient();
        }

        public DnsValidationResult Query(string email)
        {
            var domain = GetEmailDomain(email);

            var lookupResult = _client.Query(domain, QueryType.ANY);

            var aRecords = lookupResult.Answers.Where(x => x.RecordType == ResourceRecordType.A).ToList();
            var mxRecords = lookupResult.Answers.Where(x => x.RecordType == ResourceRecordType.MX).ToList();
            var allMxAndARecords = aRecords.Concat(mxRecords).ToList();

            var response = new DnsValidationResult
            {
                RecordsFound = allMxAndARecords
            };

            if (!mxRecords.Any() && aRecords.Any())
            {
                if (!aRecords.Any())
                {
                    response.Message = "Domain contains no DNS responses for A records or MX records";
                    response.IsValid = false;
                    return response;
                }
                response.Message =
                    "Domain contains A record(s) but no Mx Record. While email send might work it can not be guaranteed";
                if (_options.IsStrict)
                {
                    response.IsValid = false;
                    return response;
                }

                response.IsValid = true;
                return response;
            }

            response.Message = "Mx Record Exists";
            response.IsValid = true;
            return response;
        }

        private static string GetEmailDomain(string email)
        {
            return email.Split('@')[1];
        }
    }
}