namespace Theatre.DataProcessor
{
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;
    using System.Xml.Serialization;
    using Theatre.Data;
    using Theatre.Data.Models;
    using Theatre.Data.Models.Enums;
    using Theatre.DataProcessor.ImportDto;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfulImportPlay
            = "Successfully imported {0} with genre {1} and a rating of {2}!";

        private const string SuccessfulImportActor
            = "Successfully imported actor {0} as a {1} character!";

        private const string SuccessfulImportTheatre
            = "Successfully imported theatre {0} with #{1} tickets!";



        public static string ImportPlays(TheatreContext context, string xmlString)
        {
            //using Data Transfer Object Class to map it with plays
            var serializer = new XmlSerializer(typeof(ImportPLaysDTO[]), new XmlRootAttribute("Plays"));

            //Deserialize method needs TextReader object to convert/map 
            using StringReader inputReader = new StringReader(xmlString);
            var playsArrayDTOs = (ImportPLaysDTO[])serializer.Deserialize(inputReader);

            //using StringBuilder to gather all info in one string
            StringBuilder sb = new StringBuilder();

            //creating List where all valid plays can be kept
            List<Play> playsXML = new List<Play>();

            //creating List where all valid genres can be kept
            List<string> validGenres = new List<string> { "Drama", "Comedy", "Romance", "Musical" };

            foreach (ImportPLaysDTO playDTO in playsArrayDTOs)
            {
                //validating duration of plays in given xml
                TimeSpan duration;                
                if (!TimeSpan.TryParseExact(playDTO.Duration, "c", CultureInfo.InvariantCulture, TimeSpanStyles.None, out duration))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                //validating info for play from data
                if (!IsValid(playDTO) || duration.TotalHours < 1)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                //checking for duplicates
                if (!validGenres.Contains(playDTO.Genre))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                //creating a valid play
                Play playToAdd = new Play
                {
                    Title = playDTO.Title,
                    Duration = duration,
                    Rating = playDTO.Raiting,
                    Genre = (Genre)Enum.Parse(typeof(Genre), playDTO.Genre), //
                    Description = playDTO.Description,
                    Screenwriter = playDTO.Screenwriter
                };

                playsXML.Add(playToAdd);
                sb.AppendLine(string.Format(SuccessfulImportPlay, playToAdd.Title, playToAdd.Genre,
                    playToAdd.Rating));
            }

            context.Plays.AddRange(playsXML);

            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportCasts(TheatreContext context, string xmlString)
        {
            var serializer = new XmlSerializer(typeof(ImportCastDTO[]), new XmlRootAttribute("Casts"));
            using StringReader inputReader = new StringReader(xmlString);
            var castsArrayDTOs = (ImportCastDTO[])serializer.Deserialize(inputReader);

            StringBuilder sb = new StringBuilder();
            List<Cast> castsXML = new List<Cast>();

            foreach (ImportCastDTO castDTO in castsArrayDTOs)
            {

                if (!IsValid(castDTO))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Cast castToAdd = new Cast
                {
                    FullName = castDTO.FullName,
                    IsMainCharacter = bool.Parse(castDTO.IsMainCharacter),
                    PhoneNumber = castDTO.PhoneNumber,
                    PlayId = castDTO.PlayId
                };

                string isMainOrLesser = "";

                if (castDTO.IsMainCharacter == "true")
                {
                    isMainOrLesser = "main";
                }
                else
                {
                    isMainOrLesser = "lesser";
                }

                castsXML.Add(castToAdd);
                sb.AppendLine(string.Format(SuccessfulImportActor, castToAdd.FullName, 
                    isMainOrLesser));
            }

            context.Casts.AddRange(castsXML);

            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportTtheatersTickets(TheatreContext context, string jsonString)
        {
            var theatresArray = JsonConvert.DeserializeObject<ImportTheatreDTO[]>(jsonString);

            StringBuilder sb = new StringBuilder();
            List<Theatre> theatresList = new List<Theatre>();

            foreach (ImportTheatreDTO theatreDTO in theatresArray)
            {

                if (!IsValid(theatreDTO))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Theatre theatreToAdd = new Theatre()
                {
                    Name = theatreDTO.Name,
                    NumberOfHalls = theatreDTO.NumberOfHalls,
                    Director = theatreDTO.Director
                };



                foreach (var ticketDTO in theatreDTO.Tickets)
                {
                    if (!IsValid(ticketDTO))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Ticket ticketToAdd = new Ticket()
                    {
                        Price = ticketDTO.Price,
                        RowNumber = ticketDTO.RowNumber,
                        PlayId = ticketDTO.PlayId
                    };

                    theatreToAdd.Tickets.Add(ticketToAdd);

                }

                theatresList.Add(theatreToAdd);
                sb.AppendLine(string.Format(SuccessfulImportTheatre, theatreToAdd.Name, theatreToAdd.Tickets.Count));
            }

            context.Theatres.AddRange(theatresList);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }


        private static bool IsValid(object obj)
        {
            var validator = new ValidationContext(obj);
            var validationRes = new List<ValidationResult>();

            var result = Validator.TryValidateObject(obj, validator, validationRes, true);
            return result;
        }
    }
}
