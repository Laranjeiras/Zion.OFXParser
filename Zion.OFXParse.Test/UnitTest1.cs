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
            System.Console.WriteLine(extracts.Sum(x=>x.Transactions.Sum(x=>x.Amount)));

            var trans = extracts.Select(x => x.Transactions).ToList();
            foreach (var item in trans)
            {
                foreach (var tran in item)
                {
                    System.Console.WriteLine($"{tran.Amount}\t{tran.Description}");
                }
            }
            //45
            System.Console.WriteLine(extracts.Sum(x => x.Transactions.Count()));  
            System.Console.WriteLine(extracts.Sum(x => x.Transactions.Where(y=>y.TransactionType == OFXParser.TransactionType.CREDIT).Sum(x => x.Amount)));
            System.Console.WriteLine(extracts.Sum(x => x.Transactions.Where(y => y.TransactionType == OFXParser.TransactionType.DEBIT).Sum(x => x.Amount)));
        }
    }
}
