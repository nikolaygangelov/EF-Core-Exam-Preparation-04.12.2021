using System.ComponentModel.DataAnnotations;
using Theatre.Data.Models.Enums;

namespace Theatre.Data.Models
{
    public class Play
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Title { get; set; }

        [Required]
        [Range(typeof(TimeSpan), "01:00:00", "23:59:59")]
        public TimeSpan Duration { get; set; }

        [Required]
        [Range(0.00, 10.00)]
        public float Rating { get; set; }

        [Required]
        [EnumDataType(typeof(Genre))]
        public Genre Genre { get; set; }

        [Required]
        [MaxLength(700)]
        public string Description  { get; set; }

        [Required]
        [MaxLength(30)]
        public string Screenwriter  { get; set; }

        public ICollection<Cast> Casts { get; set; } = new HashSet<Cast>();
        public ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();
    }
}
