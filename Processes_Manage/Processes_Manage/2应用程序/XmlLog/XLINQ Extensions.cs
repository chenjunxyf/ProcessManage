using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data.Linq;
using System.Xml.Linq;
using System.Xml;

namespace Processes_Manage
{
    public static class CustomXElementExtensions
    {
        public static string SafeValue(this XElement input)
        {
            return (input == null) ? string.Empty : input.Value.ToString();
        }
    }
}
