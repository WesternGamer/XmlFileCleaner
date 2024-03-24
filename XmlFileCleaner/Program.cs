using System.Xml;
using System.Text;
using System.Security.Cryptography;

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
                    Console.WriteLine($"For: {arg}");
                    Console.WriteLine($"SHA256 Checksum: {SHA256CheckSum(arg)}");
                    Console.WriteLine("");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred when parsing: {arg}");
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("");
                }
            }

            Console.ReadLine();
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

        public static string SHA256CheckSum(string filePath)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                using (FileStream fileStream = File.OpenRead(filePath))
                {
                    return BitConverter.ToString(sha256.ComputeHash(fileStream)).Replace("-", "");
                }
            }
        }
    }
}
