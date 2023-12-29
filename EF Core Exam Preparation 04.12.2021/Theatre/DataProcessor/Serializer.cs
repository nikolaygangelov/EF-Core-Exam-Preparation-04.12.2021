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
            //turning needed info about theatres into a collection using anonymous object
            //using less data
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

            //Serialize method needs object to convert/map
	        //adding Formatting for better reading 
            return JsonConvert.SerializeObject(theatresAndTickets, Formatting.Indented);
        }

        public static string ExportPlays(TheatreContext context, double raiting)
        {
            //using Data Transfer Object Class to map it with plays
            XmlSerializer serializer = new XmlSerializer(typeof(ExportPlaysDTO[]), new XmlRootAttribute("Plays"));

            //using StringBuilder to gather all info in one string
            StringBuilder sb = new StringBuilder();

            //"using" automatically closes opened connections
            using var writer = new StringWriter(sb);

            var xns = new XmlSerializerNamespaces();

            //one way to display empty namespace in resulted file
            xns.Add(string.Empty, string.Empty);
            
            var playsAndActors = context.Plays
                .Where(p => p.Rating <= raiting)
                .Select(p => new ExportPlaysDTO
                {
                    //using identical properties in order to map successfully
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

            //Serialize method needs file, TextReader object and namespace to convert/map
            serializer.Serialize(writer, playsAndActors, xns);

            //explicitly closing connection in terms of reaching edge cases
            writer.Close();

            //using TrimEnd() to get rid of white spaces
            return sb.ToString().TrimEnd();
        }
    }
}
