using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Zion.OFXParser.Modelos;

namespace Zion.OFXParse.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ImportarArquivoUnico()
        {
            var extraxt = Zion.OFXParser.Parser.ParseToExtract(@"C:\Laranjeiras\Temp\Extrato Conta Corrente-040120212303.ofx");
            Assert.IsFalse(extraxt.ImportingErrors.Any());
            Assert.IsNotNull(extraxt);
        }

        [TestMethod]
        public void ImportarPasta()
        {
            var files = Directory.GetFiles(@"C:\Laranjeiras\Temp", "*.ofx");
            var extracts = new List<Extract>();
            foreach (var file in files)
            {
                var extract = Zion.OFXParser.Parser.ParseToExtract(file);
                extracts.Add(extract);
            
            }
            Assert.IsFalse(extracts.Where(x => x.ImportingErrors.Any()).Any());
            Assert.IsFalse(extracts.Count == 0);
        }
    }
}
