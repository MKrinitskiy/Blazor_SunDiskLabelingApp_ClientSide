using System;
using System.Collections.Generic;
using System.Text;

namespace Blazor2PythonWebAPI_interfaces
{
    [Serializable]
    public enum ErrorCodes
    {
        NoError = 0,
        GenericError = 1,
        UnknownError = 2,
        FileNotFoundError = 3,
        ClientIDnotFound = 4,
    }
}
