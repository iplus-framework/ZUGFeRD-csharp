using System.IO;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace s2industries.ZUGFeRD.Test
{
    [TestClass]
    public class HRInvoiceTests : TestBase
    {
        [TestMethod]
        public void TestHROperatorExtensions()
        {
            InvoiceDescriptor desc = InvoiceDescriptor.CreateInvoice("TEST-001", System.DateTime.Now, CurrencyCodes.EUR);
            desc.Type = InvoiceType.Invoice;

            desc.SetSeller(
                name: "Test Seller Ltd.",
                postcode: "10000",
                city: "Zagreb",
                street: "Ilica 1",
                country: CountryCodes.HR,
                id: "9934:12345678901",
                description: "Test Seller Ltd."
            );

            desc.SetBuyer(
                name: "Test Buyer Ltd.",
                postcode: "10000",
                city: "Zagreb",
                street: "Ilica 2",
                country: CountryCodes.HR,
                id: "9934:12345678902"
            );

            MemoryStream ms = new MemoryStream();
            desc.Save(ms, ZUGFeRDVersion.Version23, Profile.HRInvoice, ZUGFeRDFormats.UBL);
            ms.Position = 0;

            XmlDocument doc = new XmlDocument();
            doc.Load(ms);

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
            XmlNode accountingSupplierParty = doc.SelectSingleNode("//cac:AccountingSupplierParty", nsmgr);
            Assert.IsNotNull(accountingSupplierParty, "AccountingSupplierParty should be present");
        }

        [TestMethod]
        public void TestCreditNoteGeneratesCreditNoteRoot()
        {
            InvoiceDescriptor desc = InvoiceDescriptor.CreateInvoice("TEST-CN-001", System.DateTime.Now, CurrencyCodes.EUR);
            desc.Type = InvoiceType.CreditNote; // 381

            desc.SetSeller(name: "Seller", postcode: "10000", city: "Zagreb", street: "Ilica 1", country: CountryCodes.HR, id: "9934:12345678901");
            desc.SetBuyer(name: "Buyer", postcode: "10000", city: "Zagreb", street: "Ilica 2", country: CountryCodes.HR, id: "9934:12345678902");

            MemoryStream ms = new MemoryStream();
            desc.Save(ms, ZUGFeRDVersion.Version23, Profile.HRInvoice, ZUGFeRDFormats.UBL);
            ms.Position = 0;

            XmlDocument doc = new XmlDocument();
            doc.Load(ms);

            Assert.AreEqual("CreditNote", doc.DocumentElement.LocalName, "Root element should be CreditNote for type 381");
            Assert.IsFalse(doc.GetElementsByTagName("InvoiceTypeCode").Count > 0, "Should not have InvoiceTypeCode");
            Assert.IsTrue(doc.GetElementsByTagName("CreditNoteTypeCode").Count > 0, "Should have CreditNoteTypeCode");
        }
    }
}
