using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentViewer
{
    public interface IIdentity
    {
        string ID { get; }
        string ShortName { get; }
        string LongName { get; }
    }
}
