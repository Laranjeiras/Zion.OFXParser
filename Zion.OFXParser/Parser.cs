using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Zion.OFXParser.Modelos;

namespace Zion.OFXParser
{
    public class Parser
    {
        public static Extract ParseToExtract(string ofxSourceFile)
        {
            var settings = new ParserSettings();

            bool temCabecalho = false;
            bool temDadosConta = false;

            string elementoSendoLido = string.Empty;
            Transaction transacaoAtual = null;
            
            BankAccount conta = new BankAccount();
            HeaderExtract cabecalho = new HeaderExtract();
            Extract extrato = new Extract(cabecalho, conta, "");

            var meuXml = XmlTools.ParseToXml(ofxSourceFile);
            try
            {
                while (meuXml.Read())
                {
                    if (meuXml.NodeType == XmlNodeType.EndElement)
                    {
                        switch (meuXml.Name)
                        {
                            case "STMTTRN":
                                if (transacaoAtual != null)
                                {
                                    extrato.AddTransaction(transacaoAtual);
                                    transacaoAtual = null;
                                }
                                break;
                        }
                    }
                    if (meuXml.NodeType == XmlNodeType.Element)
                    {
                        elementoSendoLido = meuXml.Name;

                        switch (elementoSendoLido)
                        {
                            case "STMTTRN":
                                transacaoAtual = new Transaction();
                                break;
                        }
                    }
                    if (meuXml.NodeType == XmlNodeType.Text)
                    {
                        switch (elementoSendoLido)
                        {
                            case "DTSERVER":
                                cabecalho.ServerDate = ConvertOfxDateToDateTime(meuXml.Value, extrato);
                                temCabecalho = true;
                                break;
                            case "LANGUAGE":
                                cabecalho.Language = meuXml.Value;
                                temCabecalho = true;
                                break;
                            case "ORG":
                                cabecalho.BankName = meuXml.Value;
                                temCabecalho = true;
                                break;
                            case "DTSTART":
                                extrato.InitialDate = ConvertOfxDateToDateTime(meuXml.Value, extrato);
                                break;
                            case "DTEND":
                                extrato.FinalDate = ConvertOfxDateToDateTime(meuXml.Value, extrato);
                                break;
                            case "BANKID":
                                conta.Bank = new Bank(GetBankId(meuXml.Value, extrato), "");
                                temDadosConta = true;
                                break;
                            case "BRANCHID":
                                conta.AgencyCode = meuXml.Value;
                                temDadosConta = true;
                                break;
                            case "ACCTID":
                                conta.AccountCode = meuXml.Value;
                                temDadosConta = true;
                                break;
                            case "ACCTTYPE":
                                conta.AccType = meuXml.Value;
                                temDadosConta = true;
                                break;
                            case "TRNTYPE":
                                transacaoAtual.TrnType = meuXml.Value;
                                break;
                            case "DTPOSTED":
                                transacaoAtual.Date = ConvertOfxDateToDateTime(meuXml.Value, extrato);
                                break;
                            case "TRNAMT":
                                transacaoAtual.Amount = GetTransactionValue(meuXml.Value, extrato);
                                break;
                            case "FITID":
                                transacaoAtual.Id = meuXml.Value;
                                break;
                            case "CHECKNUM":
                                transacaoAtual.Checksum = Convert.ToInt64(meuXml.Value);
                                break;
                            case "MEMO":
                                transacaoAtual.Description = string.IsNullOrEmpty(meuXml.Value) ? "" : meuXml.Value.Trim().Replace("  ", " ");
                                break;
                        }
                    }
                }
            }
            catch (XmlException)
            {
                throw new FormatException("Invalid OFX file!");
            }
            finally
            {
                meuXml.Close();
            }

            if ((settings.IsValidateHeader && temCabecalho == false) ||
                (settings.IsValidateAccountData && temDadosConta == false))
            {
                throw new FormatException("Invalid OFX file!");
            }

            return extrato;
        }

        private static int GetPartOfOfxDate(string ofxDate, PartDateTime partDateTime)
        {
            int result = 0;

            if (partDateTime == PartDateTime.YEAR)
                result = Int32.Parse(ofxDate.Substring(0, 4));
            else if (partDateTime == PartDateTime.MONTH)
                result = Int32.Parse(ofxDate.Substring(4, 2));
            if (partDateTime == PartDateTime.DAY)
                result = Int32.Parse(ofxDate.Substring(6, 2));
            if (partDateTime == PartDateTime.HOUR)
            {
                if (ofxDate.Length >= 10)
                    result = Int32.Parse(ofxDate.Substring(8, 2));
                else
                    result = 0;
            }
            if (partDateTime == PartDateTime.MINUTE)
            {
                if (ofxDate.Length >= 12)
                    result = Int32.Parse(ofxDate.Substring(10, 2));
                else
                    result = 0;
            }
            if (partDateTime == PartDateTime.SECOND)
            {
                if (ofxDate.Length >= 14)
                    result = Int32.Parse(ofxDate.Substring(12, 2));
                else
                    result = 0;
            }
            return result;
        }

        private static DateTime ConvertOfxDateToDateTime(string ofxDate, Extract extract)
        {
            DateTime dateTimeReturned = DateTime.MinValue;
            try
            {
                int year = GetPartOfOfxDate(ofxDate, PartDateTime.YEAR);
                int month = GetPartOfOfxDate(ofxDate, PartDateTime.MONTH);
                int day = GetPartOfOfxDate(ofxDate, PartDateTime.DAY);
                int hour = GetPartOfOfxDate(ofxDate, PartDateTime.HOUR);
                int minute = GetPartOfOfxDate(ofxDate, PartDateTime.MINUTE);
                int second = GetPartOfOfxDate(ofxDate, PartDateTime.SECOND);

                dateTimeReturned = new DateTime(year, month, day, hour, minute, second);
            }
            catch (Exception)
            {
                extract.ImportingErrors.Add(string.Format("Invalid datetime {0}", ofxDate));
            }
            return dateTimeReturned;
        }

        private static int GetBankId(string value, Extract extract)
        {
            int bankId;
            if (!int.TryParse(value, out bankId))
            {
                extract.ImportingErrors.Add(string.Format("Bank id isn't numeric value: {0}", value));
                bankId = 0;
            }
            return bankId;
        }

        private static double GetTransactionValue(string value, Extract extract)
        {
            double returnValue = 0;
            try
            {
                returnValue = Convert.ToDouble(value.Replace('.', ','));
            }
            catch (Exception)
            {
                extract.ImportingErrors.Add(string.Format("Invalid transaction value: {0}", value));
            }
            return returnValue;
        }
    }

    public class ParserSettings
    {
        public bool IsValidateHeader { get; set; }
        public bool IsValidateAccountData { get; set; }
    }
}
