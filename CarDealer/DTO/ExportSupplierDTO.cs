using System.Xml.Serialization;

namespace CarDealer.DTO
{
    [XmlType("suppliers")]
    public class ExportSupplierDTO
    {
        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("parts-count")]
        public int PartsCount { get; set; }
    }
}
