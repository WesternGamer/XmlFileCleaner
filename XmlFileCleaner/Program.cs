using System.Xml;
using System.Text;

namespace XmlFileCleaner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            foreach (string arg in args)
            {
                try
                {
                    string xmlContent = File.ReadAllText(arg);
                    string modifiedXmlContent = RemoveCommentsAndSpaces(xmlContent);

                    // Write the modified XML content back to the file
                    File.WriteAllText(arg, modifiedXmlContent);

                    Console.WriteLine("Comments removed and spaces replaced successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }


        }

        static string RemoveCommentsAndSpaces(string xmlContent)
        {
            // Load XML content into an XmlDocument
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlContent);

            // Remove comments
            XmlNodeList commentNodes = xmlDoc.SelectNodes("//comment()");
            foreach (XmlNode node in commentNodes)
            {
                node.ParentNode.RemoveChild(node);
            }

            // Replace consecutive spaces with a single space
            StringBuilder sb = new StringBuilder();
            foreach (char c in xmlDoc.OuterXml)
            {
                if (char.IsWhiteSpace(c))
                {
                    if (sb.Length == 0 || !char.IsWhiteSpace(sb[sb.Length - 1]))
                    {
                        sb.Append(' ');
                    }
                }
                else
                {
                    sb.Append(c);
                }
            }

            // Convert the modified XML content back to a human-readable format
            XmlDocument modifiedXmlDoc = new XmlDocument();
            modifiedXmlDoc.LoadXml(sb.ToString());
            StringWriter sw = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ", // 2 spaces for indentation
                NewLineChars = "\n", // New line character
                NewLineHandling = NewLineHandling.Replace
            };

            using (XmlWriter writer = XmlWriter.Create(sw, settings))
            {
                modifiedXmlDoc.WriteTo(writer);
            }

            return sw.ToString();
        }
    }
}
