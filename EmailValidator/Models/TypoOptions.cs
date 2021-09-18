namespace EmailValidator.Models
{
    using System.Collections.Generic;

    public class TypoOptions
    {
        /// <summary>
        /// A list of domains to check typo's of
        /// </summary>
        public List<string> Domains { get; set; } = new();
        /// <summary>
        /// A list of second level domains to check typos of
        /// </summary>
        public List<string> SecondLevelDomains { get; set; } = new();
        /// <summary>
        /// A list of top level domains to check typo's of
        /// </summary>  
        public List<string> TopLevelDomains { get; set; } = new();
        
        public int? DomainThreshold { get; set; }
        
        public int? SecondLevelThreshold { get; set; }
        
        public int? TopLevelThreshold { get; set; }
    }
}