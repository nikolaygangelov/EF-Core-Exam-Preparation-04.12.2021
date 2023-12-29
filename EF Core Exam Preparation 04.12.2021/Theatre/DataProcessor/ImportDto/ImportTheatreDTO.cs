using System.ComponentModel.DataAnnotations;
using Theatre.Data.Models;

namespace Theatre.DataProcessor.ImportDto
{
    public class ImportTheatreDTO
    {
        [Required]
        [MaxLength(30)]
        [MinLength(4)]
        public string Name { get; set; }

        [Required]
        [Range(1, 10)]
        public sbyte NumberOfHalls { get; set; }

        [Required]
        [MaxLength(30)]
        [MinLength(4)]
        public string Director { get; set; }

        public ImportTheatresTicketsDTO[] Tickets { get; set; }
    }
}
