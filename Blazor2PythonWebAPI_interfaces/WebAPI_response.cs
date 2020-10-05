using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Geometry;
//using System.Text.Json;
//using System.Text.Json.Serialization;



namespace Blazor2PythonWebAPI_interfaces
{
    [Serializable]
    public class WebAPI_response
    {
        public ResponseCodes ResponseCode = ResponseCodes.OK;
        public WebAPI_error Error = new WebAPI_error();
        public string ResponseDescription = "";
        public Dictionary<string, string> StringAttributes = new Dictionary<string, string>();


        public string ToJSON()
        {
            string currObjectJSONstring = JsonConvert.SerializeObject(this, Formatting.Indented);

            return currObjectJSONstring;
        }
    }
}
