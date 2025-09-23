using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using System.Xml;

namespace CoreMvvmLib.Helpers
{
    public class SerializeHelper
    {
        public static T ReadDataFromXmlFIle<T>(string fileName, bool useDataContractSerialize = false) where T : class
        {
			try
			{
				using (StreamReader sr = new StreamReader(fileName))
				{
					string xmlData = sr.ReadToEnd();
                    T result = useDataContractSerialize == true ? DataContractSerializeDeserialize<T>(xmlData) : XmlSerializerDeserialize<T>(xmlData);
                    return result;
                }
			}
			catch (Exception)
			{
				
				throw;
			}
        }
        public static T DataContractSerializeDeserialize<T>(string xmlData) where T : class
        {
            try
            {
                using (StringReader reader = new StringReader(xmlData))
                {
                    XmlReader xmlReader = XmlReader.Create(reader);
                    DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                    return serializer.ReadObject(xmlReader) as T;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static T XmlSerializerDeserialize<T>(string xmlData) where T : class
        {
            try
            {
                using (StringReader stringReader = new StringReader(xmlData))
                {
                    using (XmlTextReader xmlReader = new XmlTextReader(stringReader))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(T));
                        return serializer.Deserialize(xmlReader) as T;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static void SaveDataToXml<T>(string fileName, T target, bool useDataContractSerialize = false) where T : class
        {
            try
            {
                using (TextWriter streamWriter = new StreamWriter(fileName, false, Encoding.UTF8))
                {
                    string xmlData =
                    useDataContractSerialize ?
                    DataContractSerializerSerialize(target) : XmlSerializerSerialize(target);

                    streamWriter.Write(xmlData);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public static string DataContractSerializerSerialize<T>(T obj)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();

                XmlWriterSettings settings = new XmlWriterSettings()
                {
                    Indent = true,
                };
                using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, settings))
                {
                    DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(T));
                    dataContractSerializer.WriteObject(xmlWriter, obj);
                    xmlWriter.Flush();

                    return stringBuilder.ToString();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public static string XmlSerializerSerialize<T>(T obj)
        {
            try
            {
                XmlSerializer xmlSerializer = new(typeof(T));
                string xmlData;

                using (MemoryStream ms = new MemoryStream())
                {
                    XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
                    {
                        Indent = true,
                        IndentChars = new string(' ',4),
                        NewLineOnAttributes = false,
                        Encoding = Encoding.UTF8
                    };
                    using(XmlWriter xmlWriter = XmlWriter.Create(ms, xmlWriterSettings))
                    {
                        xmlSerializer.Serialize(xmlWriter, obj);
                    }
                    xmlData = Encoding.UTF8.GetString(ms.GetBuffer());
                    xmlData = xmlData.Substring(xmlData.IndexOf('<'));
                    xmlData = xmlData.Substring(0,xmlData.LastIndexOf('>') +1);
                }
                return xmlData;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
