using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CommonInterfaces
{
    public interface ICsvExportable
    {
        string CSVHeader(string prefix = "");
        string ToCSV(ref Dictionary<Type, List<MethodInfo>> dictAlreadyFoundExtensionsMethods);
    }



    public abstract class CsvExportable : ICsvExportable
    {


        public string CSVHeader(string prefix = "")
        {
            List<string> lStrPropertiesNames = GetPropertiesNamesOfClass(this);
            List<string> lStrFieldsNames = GetFieldsNamesOfClass(this);
            List<string> lStrHeaders = new List<string>();
            string retStr = "";

            foreach (string propertyName in lStrPropertiesNames)
            {
                List<Assembly> loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
                List<MethodInfo> thisAssemblyExtensionMethods = new List<MethodInfo>();
                Type thisPropertyType = GetType().GetProperty(propertyName).PropertyType;
                foreach (Assembly assembly in loadedAssemblies)
                {
                    thisAssemblyExtensionMethods.AddRange(GetExtensionMethods(assembly, thisPropertyType));
                }


                if (GetType().GetProperty(propertyName).PropertyType.GetInterfaces().Contains(typeof(ICsvExportable)))
                {
                    ICsvExportable propValue = Activator.CreateInstance((GetType().GetProperty(propertyName)).PropertyType) as ICsvExportable;
                    try
                    {
                        propValue = GetType().GetProperty(propertyName).GetValue(this, null) as ICsvExportable;
                    }
                    catch (Exception e)
                    {
                        ;
                    }
                    lStrHeaders.Add(propValue.CSVHeader(prefix + ((prefix == "") ? ("") : ("_")) + propertyName));
                }
                else if (thisAssemblyExtensionMethods.Any(meth => meth.Name == "CSVHeader"))
                {
                    object propValue = Activator.CreateInstance((GetType().GetProperty(propertyName)).PropertyType);

                    try
                    {
                        propValue = GetType().GetProperty(propertyName).GetValue(this, null);
                    }
                    catch (Exception e)
                    {
                        ;
                    }

                    MethodInfo method = thisAssemblyExtensionMethods.Find(mInfo => mInfo.Name == "CSVHeader");
                    object[] parameters = new object[] { propValue, prefix + ((prefix == "") ? ("") : ("_")) + propertyName };
                    string strCurrPropertyCSVHeader = (string)method.Invoke(propValue, parameters);

                    lStrHeaders.Add(strCurrPropertyCSVHeader);
                }
                else if (GetType().GetProperty(propertyName).GetValue(this, null) is IEnumerable<ICsvExportable> && !(GetType().GetProperty(propertyName).GetValue(this) is string))
                {
                    IEnumerable<ICsvExportable> ienum =
                        GetType().GetProperty(propertyName).GetValue(this, null) as IEnumerable<ICsvExportable>;

                    foreach (Assembly assembly in loadedAssemblies)
                    {
                        thisAssemblyExtensionMethods.AddRange(GetExtensionMethods(assembly,
                            typeof(IEnumerable<ICsvExportable>)));
                    }

                    MethodInfo method = thisAssemblyExtensionMethods.Find(mInfo => mInfo.Name == "CSVHeader");
                    object[] parameters = new object[] { ienum, prefix + ((prefix == "") ? ("") : ("_")) + propertyName };
                    string strCurrPropertyCSVHeader = (string)method.Invoke(ienum, parameters);

                    lStrHeaders.Add(strCurrPropertyCSVHeader);
                }
                else
                {
                    lStrHeaders.Add(prefix + ((prefix == "") ? ("") : ("_")) + propertyName);
                }
            }




            foreach (string fieldName in lStrFieldsNames)
            {
                List<Assembly> loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
                List<MethodInfo> thisAssemblyExtensionMethods = new List<MethodInfo>();
                Type thisFieldType = GetType().GetField(fieldName).FieldType;
                foreach (Assembly assembly in loadedAssemblies)
                {
                    thisAssemblyExtensionMethods.AddRange(GetExtensionMethods(assembly, thisFieldType));
                }


                if (GetType().GetField(fieldName).FieldType.GetInterfaces().Contains(typeof(ICsvExportable)))
                {
                    ICsvExportable fieldValue =
                        Activator.CreateInstance((GetType().GetField(fieldName)).FieldType) as ICsvExportable;
                    try
                    {
                        fieldValue = GetType().GetField(fieldName).GetValue(this) as ICsvExportable;
                    }
                    catch (Exception e)
                    {
                        ;
                    }

                    lStrHeaders.Add(fieldValue.CSVHeader(prefix + ((prefix == "") ? ("") : ("_")) + fieldName));
                }
                else if (thisAssemblyExtensionMethods.Any(meth => meth.Name == "CSVHeader"))
                {
                    object fieldValue = Activator.CreateInstance((GetType().GetField(fieldName)).FieldType);

                    try
                    {
                        fieldValue = GetType().GetField(fieldName).GetValue(this);
                    }
                    catch (Exception e)
                    {
                        ;
                    }


                    MethodInfo method = thisAssemblyExtensionMethods.Find(mInfo => mInfo.Name == "CSVHeader");
                    object[] parameters = new object[] { fieldValue, prefix + ((prefix == "") ? ("") : ("_")) + fieldName };
                    string strCurrFieldCSVHeader = (string)method.Invoke(fieldValue, parameters);

                    lStrHeaders.Add(strCurrFieldCSVHeader);
                }
                else if (GetType().GetField(fieldName).GetValue(this) is IEnumerable<ICsvExportable> && !(GetType().GetField(fieldName).GetValue(this) is string))
                {
                    IEnumerable<ICsvExportable> ienum =
                        GetType().GetField(fieldName).GetValue(this) as IEnumerable<ICsvExportable>;

                    foreach (Assembly assembly in loadedAssemblies)
                    {
                        thisAssemblyExtensionMethods.AddRange(GetExtensionMethods(assembly,
                            typeof(IEnumerable<ICsvExportable>)));
                    }

                    MethodInfo method = thisAssemblyExtensionMethods.Find(mInfo => mInfo.Name == "CSVHeader");
                    object[] parameters = new object[] { ienum, prefix + ((prefix == "") ? ("") : ("_")) + fieldName };
                    string strCurrPropertyCSVHeader = (string)method.Invoke(ienum, parameters);

                    lStrHeaders.Add(strCurrPropertyCSVHeader);
                }
                else
                {
                    lStrHeaders.Add(prefix + ((prefix == "") ? ("") : ("_")) + fieldName);
                }
            }

            retStr = String.Join(";", lStrHeaders.ToArray<string>());
            return retStr;
        }



        public static IEnumerable<MethodInfo> GetExtensionMethods(Assembly assembly, Type extendedType)
        {
            var query = from type in assembly.GetTypes()
                        where type.IsSealed && !type.IsGenericType && !type.IsNested
                        from method in type.GetMethods(BindingFlags.Static
                            | BindingFlags.Public | BindingFlags.NonPublic)
                        where method.IsDefined(typeof(ExtensionAttribute), false)
                        where method.GetParameters()[0].ParameterType == extendedType
                        select method;
            return query;
        }




        public static Dictionary<Type, List<MethodInfo>> GetExtensionsMethodsForType(Type t, Dictionary<Type, List<MethodInfo>> dictAlreadyFoundExtensionsMethods)
        {
            if (!dictAlreadyFoundExtensionsMethods.ContainsKey(t))
            {
                List<Assembly> loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
                List<MethodInfo> thisAssemblyExtensionMethods = new List<MethodInfo>();
                foreach (Assembly assembly in loadedAssemblies)
                {
                    thisAssemblyExtensionMethods.AddRange(GetExtensionMethods(assembly, t));
                }
                dictAlreadyFoundExtensionsMethods.Add(t, thisAssemblyExtensionMethods);
            }

            return dictAlreadyFoundExtensionsMethods;
        }



        public string ToCSV(ref Dictionary<Type, List<MethodInfo>> dictAlreadyFoundExtensionsMethods)
        {
            List<string> lStrPropertiesNames = GetPropertiesNamesOfClass(this);
            List<string> lStrFieldsNames = GetFieldsNamesOfClass(this);
            List<string> lStrValues = new List<string>();
            string retStr = "";

            foreach (string propertyName in lStrPropertiesNames)
            {
                Type thisPropertyType = GetType().GetProperty(propertyName).PropertyType;
                dictAlreadyFoundExtensionsMethods = GetExtensionsMethodsForType(thisPropertyType, dictAlreadyFoundExtensionsMethods);
                List<MethodInfo> thisAssemblyExtensionMethods = dictAlreadyFoundExtensionsMethods[thisPropertyType];

                if (GetType().GetProperty(propertyName).GetValue(this, null) is IEnumerable<ICsvExportable>)
                {
                    thisPropertyType = typeof(IEnumerable<ICsvExportable>);
                    dictAlreadyFoundExtensionsMethods = GetExtensionsMethodsForType(thisPropertyType, dictAlreadyFoundExtensionsMethods);
                    thisAssemblyExtensionMethods = dictAlreadyFoundExtensionsMethods[thisPropertyType];
                }


                object propValue = GetType().GetProperty(propertyName).GetValue(this, null);
                if (propValue is CsvExportable)
                {
                    CsvExportable currPropValueCSVexportable = propValue as CsvExportable;
                    lStrValues.Add(currPropValueCSVexportable.ToCSV(ref dictAlreadyFoundExtensionsMethods));
                }
                else if (thisAssemblyExtensionMethods.Any(meth => meth.Name == "ToCSV"))
                {
                    MethodInfo method = thisAssemblyExtensionMethods.Find(minfo => minfo.Name == "ToCSV");
                    object[] parameters = new object[] { propValue, dictAlreadyFoundExtensionsMethods };
                    string strCurrPropertyCSV = (string)method.Invoke(propValue, parameters);

                    lStrValues.Add(strCurrPropertyCSV);
                }
                else if ((propValue.GetType() == typeof(double)) || (propValue.GetType() == typeof(float)))
                {
                    lStrValues.Add(propValue.ToString().Replace(",", "."));
                }
                else if (propValue.GetType() == typeof(DateTime))
                {
                    DateTime dtObj = (DateTime)propValue;
                    lStrValues.Add(dtObj.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                else
                {
                    lStrValues.Add(propValue.ToString());
                }
            }




            foreach (string fieldName in lStrFieldsNames)
            {
                Type thisFieldType = GetType().GetField(fieldName).FieldType;
                dictAlreadyFoundExtensionsMethods = GetExtensionsMethodsForType(thisFieldType, dictAlreadyFoundExtensionsMethods);
                List<MethodInfo> thisAssemblyExtensionMethods = dictAlreadyFoundExtensionsMethods[thisFieldType];

                object fieldValue = GetType().GetField(fieldName).GetValue(this);
                if (fieldValue is CsvExportable)
                {
                    CsvExportable currFieldValueCSVexportable = fieldValue as CsvExportable;
                    lStrValues.Add(currFieldValueCSVexportable.ToCSV(ref dictAlreadyFoundExtensionsMethods));
                }
                else if (thisAssemblyExtensionMethods.Any(meth => meth.Name == "ToCSV"))
                {
                    MethodInfo method = thisAssemblyExtensionMethods.Find(minfo => minfo.Name == "ToCSV");
                    object[] parameters = new object[] { fieldValue, dictAlreadyFoundExtensionsMethods };
                    string strCurrFieldCSV = (string)method.Invoke(fieldValue, parameters);

                    lStrValues.Add(strCurrFieldCSV);
                }
                else if ((fieldValue.GetType() == typeof(double)) || (fieldValue.GetType() == typeof(float)))
                {
                    lStrValues.Add(fieldValue.ToString().Replace(",", "."));
                }
                else if (fieldValue.GetType() == typeof(DateTime))
                {
                    DateTime dtObj = (DateTime)fieldValue;
                    lStrValues.Add(dtObj.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                else
                {
                    lStrValues.Add(fieldValue.ToString());
                }
            }




            retStr = String.Join(";", lStrValues.ToArray<string>());
            return retStr;
        }



        private List<string> GetPropertiesNamesOfClass(object pObject)
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


        private List<string> GetFieldsNamesOfClass(object pObject)
        {
            List<string> fieldsList = new List<string>();
            if (pObject != null)
            {
                foreach (var prop in pObject.GetType().GetFields())
                {
                    fieldsList.Add(prop.Name);
                }
            }
            return fieldsList;
        }
    }
}
