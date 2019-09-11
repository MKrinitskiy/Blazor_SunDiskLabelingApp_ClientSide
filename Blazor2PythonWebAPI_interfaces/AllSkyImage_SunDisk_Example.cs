using System;
using System.Collections.Generic;
using System.Text;

namespace Blazor2PythonWebAPI_interfaces
{
    [Serializable]
    public class AllSkyImage_SunDisk_Example : IMarkable
    {
        public List<Label> Labels { get; set; }

    }
}
