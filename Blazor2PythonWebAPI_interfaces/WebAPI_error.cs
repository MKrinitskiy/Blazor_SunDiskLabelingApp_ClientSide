﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Blazor2PythonWebAPI_interfaces
{
    [Serializable]
    public class WebAPI_error
    {
        public ErrorCodes ErrorCode = ErrorCodes.NoError;
        public string ErrorDescription = "";
    }
}
