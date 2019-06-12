using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace BihuApiCore.Infrastructure.Extensions.UserClientExtensions
{
    public class MatchExpression
    {
        public List<Regex> Regexes { get; set; }

        public Action<Match, object> Action { get; set; }
    }
}
