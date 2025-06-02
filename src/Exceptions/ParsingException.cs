using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedLib.LanguageModel.Exceptions
{
    internal class ParsingException :Exception
    {
        public ParsingException(string message)
            : base(message)
        {
        }
    }
}
