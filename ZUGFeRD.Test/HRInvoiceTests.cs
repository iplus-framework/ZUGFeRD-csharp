/*
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
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
            // Create a minimal HR invoice descriptor
            InvoiceDescriptor desc = InvoiceDescriptor.CreateInvoice("TEST-001", System.DateTime.Now, CurrencyCodes.EUR);
            desc.Type = InvoiceType.Invoice;
            desc.HROperatorTag = "John Doe";
            desc.HROperatorOIB = "12345678901";

            // Set seller
            desc.SetSeller(
                name: "Test Seller Ltd.",
                postcode:  "10000",
                city: "Zagreb",
                street: "Ilica 1",
                country: CountryCodes.HR,
                id: "9934:12345678901",
                description: "Test Seller Ltd."
            );

            // Set buyer
            desc.SetBuyer(
                name: "Test Buyer Ltd.",
                postcode: "10000",
                city: "Zagreb",
                street: "Ilica 2",
                country: CountryCodes.HR,
                id: "9934:12345678902"
            );

            // Save to XML
            MemoryStream ms = new MemoryStream();
            desc.Save(ms, ZUGFeRDVersion.Version23, Profile.HRInvoice, ZUGFeRDFormats.UBL);
            ms.Position = 0;

            // Load and verify
            XmlDocument doc = new XmlDocument();
            doc.Load(ms);

            // Check that the HR extension elements are present
            XmlNodeList operatorTagNodes = doc.GetElementsByTagName("OperatorTag", "urn:hzn.hr:schema:xsd:HRExtensionAggregateComponents-1");
            XmlNodeList operatorOIBNodes = doc.GetElementsByTagName("OperatorOIB", "urn:hzn.hr:schema:xsd:HRExtensionAggregateComponents-1");

            Assert.IsTrue(operatorTagNodes.Count > 0, "OperatorTag element should be present");
            Assert.IsTrue(operatorOIBNodes.Count > 0, "OperatorOIB element should be present");
            Assert.AreEqual("John Doe", operatorTagNodes[0].InnerText);
            Assert.AreEqual("12345678901", operatorOIBNodes[0].InnerText);
        }
    }
}
