using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Theatre.DataProcessor.ImportDto
{
    [XmlType("Cast")]
    public class ImportCastDTO
    {
        [Required]
        [MaxLength(30)]
        [MinLength(4)]
        [XmlElement("FullName")]
        public string FullName { get; set; }

        [Required]
        [RegularExpression(@"^(true|false)\b")]
        [XmlElement("IsMainCharacter")]
        public string IsMainCharacter { get; set; }

        //public string IsMainOrLesser { get; set; }

        [Required]
        [RegularExpression(@"^(\+44-\d{2}-\d{3}-\d{4})\b")]
        [XmlElement("PhoneNumber")]
        public string PhoneNumber { get; set; }

        [Required]
        [XmlElement("PlayId")]
        public int PlayId { get; set; }
    }
}
