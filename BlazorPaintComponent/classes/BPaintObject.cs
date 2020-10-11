using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BlazorSvgHelper.Classes.SubClasses;
using Geometry;
//using System.Xml;
//using System.Xml.Serialization;
//using System.Text.Json;
//using System.Text.Json.Serialization;
using Newtonsoft.Json;


namespace BlazorPaintComponent.classes
{
    [Serializable]
    //[XmlInclude(typeof(BPaintCircle))]
    //[XmlInclude(typeof(BPaintEllipse))]
    //[XmlInclude(typeof(BPaintHandDraw))]
    //[XmlInclude(typeof(BPaintLine))]
    //[XmlInclude(typeof(BPaintRectangle))]
    public class BPaintObject : BPBoundable, IBPaintObject
    {
        public string guid = Guid.NewGuid().ToString();

        [JsonIgnore]
        public int ObjectID { get; set; }

        [JsonIgnore]
        public bool Selected { get; set; }

        [JsonIgnore]
        public bool EditMode { get; set; }

        [JsonIgnore]
        public int SequenceNumber { get; set; }

        //[JsonIgnore]
        public string Color { get; set; }

        [JsonIgnore]
        public double LineWidth { get; set; }

        [JsonIgnore]
        public double validityTolerance { get; set; } = 10.0d;

        private BPaintVertex _position;
        public BPaintVertex Position
        {
            get { return _position; }
            set
            {
                _position = value;
                VerticesList.Add(_position);
            }
        }

        public PointD Scale { get; set; } = new PointD(1.0, 1.0);
        public BPaintOpbjectType ObjectType { get; set; }

        
        public List<BPaintVertex> VerticesList = new List<BPaintVertex>();

        [JsonIgnore]
        public SVGtransform transform { get; set; } = new SVGtransform();

        [JsonIgnore]
        public int MandatoryVerticesCount { get; set; }


        public BPaintObject()
        {

        }

        public override RectD BoundingRectD(bool padding = true)
        {
            throw new NotImplementedException();
        }




        public static string[] GetTypePropertyNames(object classObject, BindingFlags bindingFlags)
        {
            if (classObject == null)
            {
                throw new ArgumentNullException(nameof(classObject));
            }

            var type = classObject.GetType();
            var propertyInfos = type.GetProperties(bindingFlags);

            return propertyInfos.Select(propertyInfo => propertyInfo.Name).ToArray();
        }




        public void PostJsonDeserializationCleaning()
        {
#if DEBUG
            Console.WriteLine("Entered PostJsonDeserializationCleaning");
#endif
            Type t = this.GetType();

            List<string> verticesGUIDs = VerticesList.Select(v => v.guid).ToList();
#if DEBUG
            Console.WriteLine("found " + verticesGUIDs.Count.ToString() + " vertices GUIDs");
#endif
            List<string> verticesGUIDsUnique = verticesGUIDs.Distinct().ToList();
#if DEBUG
            Console.WriteLine("found " + verticesGUIDsUnique.Count.ToString() + " unique vertices GUIDs");

            Console.WriteLine("there were " + VerticesList.Count.ToString() + " vertices in VerticesList");
#endif
            List<BPaintVertex> uniqueVertices = verticesGUIDsUnique.Select(guid => VerticesList.First(v => v.guid == guid)).ToList();
            VerticesList = uniqueVertices;
#if DEBUG
            Console.WriteLine("there are " + VerticesList.Count.ToString() + " vertices now in VerticesList");
#endif


            PropertyInfo[] props = t.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            List<PropertyInfo> propsList = props.ToList();
#if DEBUG
            Console.WriteLine("Found properties: \n" + string.Join(";", propsList.Select(pi => pi.Name)));
#endif
            propsList = propsList.FindAll(p => {
                object v = p.GetValue(this);
                if (v == null) { return false; }
                else { return v.GetType().Equals(typeof(BPaintVertex)); } });
#if DEBUG
            Console.WriteLine("object type: " + t.ToString());
            Console.WriteLine("found BPaintVertex properties: \n" + string.Join(";", propsList.Select(pi => pi.Name)));
#endif
            foreach (PropertyInfo prop in propsList)
            {
                if (prop.PropertyType.Equals(typeof(BPaintVertex)))
                {
                    // search for a vetrex in the list VerticesList and reset it with the found one
#if DEBUG
                    Console.WriteLine("property name: " + prop.Name);
#endif
                    BPaintVertex currVertexObj = (BPaintVertex)prop.GetValue(this);
#if DEBUG
                    Console.WriteLine("searching for a vertex with guid " + currVertexObj.guid);
#endif
                    BPaintVertex foundVertex = VerticesList.Find(v => v.guid == currVertexObj.guid);
#if DEBUG
                    Console.WriteLine("found a vertex in VerticesList with guid " + foundVertex.guid);
#endif
                    VerticesList.RemoveAll(v => v.guid == foundVertex.guid);
                    prop.SetValue(this, foundVertex);
                }
            }
        }
    }
}
