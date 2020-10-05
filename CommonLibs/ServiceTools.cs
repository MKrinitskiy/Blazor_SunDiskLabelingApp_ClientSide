using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
//using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
//using System.Xml;
//using System.Xml.Serialization;
using Microsoft.CSharp;

namespace CommonLibs
{
    public class ServiceTools
    {
        public static object getPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }


        public static void FlushMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }


        //public static int CurrentCodeLineNumber(
        //    [CallerMemberName] string callingMethod = "",
        //    [CallerFilePath] string callingFilePath = "",
        //    [CallerLineNumber] int callingFileLineNumber = 0)
        //{
        //    return callingFileLineNumber;
        //}


        //public static string CurrentCodeLineDescription(
        //    [CallerMemberName] string callingMethod = "",
        //    [CallerFilePath] string callingFilePath = "",
        //    [CallerLineNumber] int callingFileLineNumber = 0)
        //{
        //    string str = "line " + callingFileLineNumber + Environment.NewLine
        //                 + "in method " + callingMethod + Environment.NewLine
        //                 + "in file " + callingFilePath;
        //    return str;
        //}



        public static string GetExceptionMessages(Exception e, string msgs = "")
        {
            if (e == null) return String.Empty;
            if (msgs == "") msgs = e.Message;
            if (e.InnerException != null)
                msgs += "\r\nInnerException: " + GetExceptionMessages(e.InnerException);
            return msgs;
        }



        //public static Dictionary<string, object> ReadDictionaryFromXMLstring(string dataString)
        //{
        //    Dictionary<string, object> retDict = new Dictionary<string, object>();
        //    TextReader strReader = new StringReader(dataString);
        //    DataSet readingDataSet = new DataSet("DataSet");
        //    try
        //    {
        //        readingDataSet.ReadXml(strReader);
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //        throw;
        //    }

        //    foreach (DataTable table in readingDataSet.Tables)
        //    {
        //        foreach (DataRow row in table.Rows)
        //        {
        //            retDict.Add(row[0] as string, row[1]);
        //        }
        //    }
        //    readingDataSet.Dispose();
        //    return retDict;
        //}



        //public static string WriteDictionaryToXmlstring(Dictionary<string, object> dictToWrite)
        //{
        //    DataSet dsToWrite = new DataSet("DataSet");
        //    dsToWrite.Namespace = "NetFrameWork";
        //    DataTable table = new DataTable("table");

        //    DataColumn keyColumn = new DataColumn("key", Type.GetType("System.String"));

        //    DataColumn valueColumn = new DataColumn("value");
        //    table.Columns.Add(keyColumn);
        //    table.Columns.Add(valueColumn);
        //    dsToWrite.Tables.Add(table);

        //    DataRow newRow;

        //    foreach (KeyValuePair<string, object> pair in dictToWrite)
        //    {
        //        newRow = table.NewRow();
        //        newRow["key"] = pair.Key;
        //        newRow["value"] = pair.Value;
        //        table.Rows.Add(newRow);
        //    }
        //    dsToWrite.AcceptChanges();

            
        //    TextWriter strWriter = new StringWriter(new StringBuilder());
        //    dsToWrite.WriteXml(strWriter);
        //    string retstr = strWriter.ToString();
        //    return retstr;
        //}


        //public static string XmlSerializeToString(object objectInstance)
        //{
        //    XmlSerializer serializer = new XmlSerializer(objectInstance.GetType());
        //    StringBuilder sb = new StringBuilder();

        //    using (TextWriter writer = new StringWriter(sb))
        //    {
        //        serializer.Serialize(writer, objectInstance);
        //    }
        //    return sb.ToString();
        //}


        //public static T XmlDeserializeFromString<T>(string objectData)
        //{
        //    return (T)XmlDeserializeFromString(objectData, typeof(T));
        //}


        //public static object XmlDeserializeFromString(string objectData, Type type)
        //{
        //    var serializer = new XmlSerializer(type);
        //    object result;

        //    using (TextReader reader = new StringReader(objectData))
        //    {
        //        result = serializer.Deserialize(reader);
        //    }

        //    return result;
        //}



        public static string ErrorTextDescription(CompilerError error)
        {
            string strErrDescription = "[" + error.ErrorNumber + "]" + Environment.NewLine;
            strErrDescription += "file: " + error.FileName + Environment.NewLine;
            strErrDescription += "line:column: " + error.Line + " : " + error.Column + Environment.NewLine;
            strErrDescription += "message: " + error.ErrorText + Environment.NewLine;

            return strErrDescription;
        }



        public static Assembly CompileAssemblyFromExternalCodeSource(string code, out CompilerResults results)
        {
            string codeSource = code;
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            Dictionary<string, string> providerOptions = new Dictionary<string, string>
            {
                {"CompilerVersion", "v4.0"}
            };
            CSharpCodeProvider provider = new CSharpCodeProvider(providerOptions);
            CompilerParameters compilerParams = new CompilerParameters
            {
                GenerateInMemory = true,
                GenerateExecutable = false
            };
            //compilerParams.CompilerOptions = "/lib:" + Directory.GetCurrentDirectory() + "/libs/";

            compilerParams.ReferencedAssemblies.Add(executingAssembly.Location);
            foreach (AssemblyName assemblyName in executingAssembly.GetReferencedAssemblies())
            {
                compilerParams.ReferencedAssemblies.Add(Assembly.Load(assemblyName).Location);
            }
            results = provider.CompileAssemblyFromSource(compilerParams, codeSource);
            if (results.Errors.Count != 0)
            {
                return null;
            }

            return results.CompiledAssembly;
        }



        public static List<string> GetPropertiesNamesOfClass(object pObject)
        {
            List<string> propertyList = new List<string>();
            if (pObject != null)
            {
                foreach (var prop in pObject.GetType().GetProperties())
                {
                    propertyList.Add(prop.Name);
                }
            }
            return propertyList;
        }
    }
}
