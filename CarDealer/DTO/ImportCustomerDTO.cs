﻿using System.Xml.Serialization;

namespace CarDealer.DTO
{
    [XmlType("Customer")]
    public class ImportCustomerDTO
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("birthDate")]
        public string BirthDate { get; set; }

        [XmlElement("isYoungDriver")]
        public bool IsYoungDriver { get; set; }
    }
}
