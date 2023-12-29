namespace Theatre.DataProcessor
{
    using Newtonsoft.Json;
    using System;
    using System.Globalization;
    using System.Text;
    using System.Xml.Serialization;
    using Theatre.Data;
    using Theatre.DataProcessor.ExportDto;

    public class Serializer
    {
        public static string ExportTheatres(TheatreContext context, int numbersOfHalls)
        {
            var theatresAndTickets = context.Theatres
               .Where(t =>t.NumberOfHalls >= numbersOfHalls && t.Tickets.Count >= 20)
               .Select(t => new
               {
                   Name = t.Name,
                   Halls = t.NumberOfHalls,
                   TotalIncome = t.Tickets.Where(ti => ti.RowNumber >= 1 && ti.RowNumber <= 5).Sum(ti => ti.Price),
                   Tickets = t.Tickets
                   .Where(ti => ti.RowNumber >= 1 && ti.RowNumber <= 5)
                   .Select(ti => new
                   {
                       Price = ti.Price,
                       RowNumber = ti.RowNumber
                   })
                   .OrderByDescending(ti => ti.Price)
                   .ToArray()
               })
               .OrderByDescending(t => t.Halls)
               .ThenBy(t => t.Name)
               .ToArray();

            return JsonConvert.SerializeObject(theatresAndTickets, Formatting.Indented);
        }

        public static string ExportPlays(TheatreContext context, double raiting)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ExportPlaysDTO[]), new XmlRootAttribute("Plays"));

            StringBuilder sb = new StringBuilder();

            using var writer = new StringWriter(sb);

            var xns = new XmlSerializerNamespaces();
            xns.Add(string.Empty, string.Empty);
            
            var playsAndActors = context.Plays
                .Where(p => p.Rating <= raiting)
                .Select(p => new ExportPlaysDTO
                {
                    Title = p.Title,
                    Duration = p.Duration.ToString("c"),
                    Rating = p.Rating == 0 ? "Premier" : p.Rating.ToString(),
                    Genre = p.Genre,
                    Actors = p.Casts
                    .Where(c => c.IsMainCharacter == true)
                    .Select(a => new ExportPlaysActorsDTO
                    {
                        FullName = a.FullName,
                        MainCharacter = $"Plays main character in '{p.Title}'."
                    })
                    .OrderByDescending(a => a.FullName)
                    .ToArray()
                })
                .OrderBy(p => p.Title)
                .ThenByDescending(p => p.Genre)
                .ToArray();

            serializer.Serialize(writer, playsAndActors, xns);
            writer.Close();

            return sb.ToString();
        }
    }
}
