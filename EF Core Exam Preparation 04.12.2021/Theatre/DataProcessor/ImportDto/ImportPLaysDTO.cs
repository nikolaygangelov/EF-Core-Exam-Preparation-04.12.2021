using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using Theatre.Data.Models.Enums;

namespace Theatre.DataProcessor.ImportDto
{
    [XmlType("Play")]
    public class ImportPLaysDTO
    {

        [Required]
        [MaxLength(50)]
        [MinLength(4)]
        [XmlElement("Title")]
        public string Title { get; set; }

        [Required]
        [XmlElement("Duration")]
        public string Duration { get; set; }

        [Required]
        [Range(0.00, 10.00)]
        [XmlElement("Raiting")]
        public float Raiting { get; set; }

        [Required]
        [XmlElement("Genre")]
        public string Genre { get; set; }

        [Required]
        [MaxLength(700)]
        [XmlElement("Description")]
        public string Description { get; set; }

        [Required]
        [MaxLength(30)]
        [MinLength(4)]
        [XmlElement("Screenwriter")]
        public string Screenwriter { get; set; }
    }
}
