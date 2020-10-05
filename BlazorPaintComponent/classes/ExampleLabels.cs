using System;
using Geometry;
using Blazor2PythonWebAPI_interfaces;
//using System.Xml;
//using System.Xml.Serialization;
//using System.Text.Json;
//using System.Text.Json.Serialization;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;


namespace BlazorPaintComponent.classes
{
    [Serializable]
    public class ExampleLabels
    {
        //public string ClassName = "ExampleLabels";
        public List<BPaintObject> LabelsList;
        public string strBaseImageFilename;
        public SizeD PresentedImageSize;


        public ExampleLabels()
        {
            this.strBaseImageFilename = "";
            this.LabelsList = new List<BPaintObject>();
            this.PresentedImageSize = new SizeD(0.0, 0.0);
        }


        public ExampleLabels(string strBaseImageFilename, List<BPaintObject> LabelsList, SizeD PresentedImageSize)
        {
            this.strBaseImageFilename = strBaseImageFilename;
            this.LabelsList = LabelsList;
            this.PresentedImageSize = PresentedImageSize;
        }
    }
}
