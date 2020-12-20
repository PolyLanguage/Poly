using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyEditor
{
    public struct DocumentTag
    {
        public string DocumentLocation { get; set; }

        public DocumentTag(string fileLocation)
        {
            DocumentLocation = fileLocation;
        }
    }
}
