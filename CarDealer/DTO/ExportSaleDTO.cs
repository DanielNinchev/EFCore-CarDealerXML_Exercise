using System.Xml.Serialization;

namespace CarDealer.DTO
{
    [XmlType("sale")]
    public class ExportSaleDTO
    {
        [XmlElement("car")]
        public ExportCarDTO Car { get; set; }

        [XmlElement("discount")]
        public decimal Discount { get; set; }

        [XmlElement("customer-name")]
        public string CustomerName { get; set; }

        [XmlElement("price")]
        public decimal Price { get; set; }

        [XmlElement("price-with-discount")]
        public decimal PriceWithDiscount { get; set; }

        [XmlElement("customer")]
        public ExportCustomerDTO Customer { get; set; }
    }

    [XmlType("car")]
    public class ExportCarDTO
    {
        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlAttribute("make")]
        public string Make { get; set; }

        [XmlAttribute("model")]
        public string Model { get; set; }

        [XmlAttribute("travelled-distance")]
        public long TravelledDistance { get; set; }

        [XmlArray("parts")]
        public ExportPartDTO[] Parts { get; set; }
    }

    [XmlType("part")]
    public class ExportPartDTO
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("price")]
        public decimal Price { get; set; }
    }
}
