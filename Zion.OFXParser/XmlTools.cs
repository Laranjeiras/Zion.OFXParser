using System;
using System.IO;
using System.Text;
using System.Xml;

namespace Zion.OFXParser
{
    internal class XmlTools
    {
        internal static XmlTextReader ParseToXml(string ofxSourceFile)
        {
            if(!File.Exists(ofxSourceFile))
                throw new FileNotFoundException("OFX source file not found: " + ofxSourceFile);

            try
            {
                StringBuilder ofxTranslated = TranslateToXml(ofxSourceFile);
                TextReader txt = new StringReader(ofxTranslated.ToString());
                return new XmlTextReader(txt);
            }
            catch 
            {
                throw new FormatException($"Format of file is not valid: {ofxSourceFile}");
            }
        }

        private static StringBuilder TranslateToXml(string ofxSourceFile)
        {
            StringBuilder result = new StringBuilder();
            string linha;

            if (!File.Exists(ofxSourceFile))
                throw new FileNotFoundException("OFX source file not found: " + ofxSourceFile);

            StreamReader sr = File.OpenText(ofxSourceFile);
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

            return result;
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
