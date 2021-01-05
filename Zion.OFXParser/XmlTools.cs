using System;
using System.IO;
using System.Text;
using System.Xml;

namespace Zion.OFXParser
{
    internal class XmlTools
    {
        internal static XmlTextReader ParseFromFileToXml(string ofxSourceFile)
        {
            if(!File.Exists(ofxSourceFile))
                throw new FileNotFoundException("OFX source file not found: " + ofxSourceFile);

            try
            {
                if (!File.Exists(ofxSourceFile))
                    throw new FileNotFoundException("OFX source file not found: " + ofxSourceFile);

                StreamReader sr = File.OpenText(ofxSourceFile);
                return TranslateToXml(sr);
            }
            catch 
            {
                throw new FormatException($"Format of file is not valid: {ofxSourceFile}");
            }
        }

        internal static XmlTextReader ParseFromStringToXml(string ofxSource)
        {
            if (string.IsNullOrEmpty(ofxSource))
                throw new InvalidDataException("OFX source file invalid: " + ofxSource);

            try
            {
                var mr = new MemoryStream(Encoding.UTF8.GetBytes(ofxSource));
                var sr = new StreamReader(mr);
                return TranslateToXml(sr);
            }
            catch
            {
                throw new FormatException($"Format of file is not valid: {ofxSource}");
            }
        }

        internal static XmlTextReader ParseFromBytesToXml(byte[] ofx)
        {
            try
            {
                var mr = new MemoryStream(ofx);
                var sr = new StreamReader(mr);
                return TranslateToXml(sr);
            }
            catch
            {
                throw new FormatException($"Format of OFX file is not valid");
            }
        }

        private static XmlTextReader TranslateToXml(StreamReader sr)
        {
            StringBuilder result = new StringBuilder();
            string linha;

            while ((linha = sr.ReadLine()) != null)
            {
                linha = linha.Trim();

                if ((linha.StartsWith("</") && linha.EndsWith(">")) 
                    || linha.StartsWith("<") && linha.EndsWith(">"))
                {
                    result.Append(linha);
                }
                else if (linha.StartsWith("<") && !linha.EndsWith(">"))
                {
                    result.Append(linha);
                    result.Append(ReturnFinalTag(linha));
                }
            }
            sr.Close();

            var resultReader = new StringReader(result.ToString());
            return new XmlTextReader(resultReader);
        }

        private static string ReturnFinalTag(string content)
        {
            string returnFinal = "";

            if ((content.IndexOf("<") != -1) && (content.IndexOf(">") != -1))
            {
                int position1 = content.IndexOf("<");
                int position2 = content.IndexOf(">");
                if ((position2 - position1) > 2)
                {
                    returnFinal = content.Substring(position1, (position2 - position1) + 1);
                    returnFinal = returnFinal.Replace("<", "</");
                }
            }

            return returnFinal;
        }
    }
}
