using System.Runtime.Serialization;

namespace EnergyPortalApi.Domain.Models.CsvParsing
{
    [Serializable]
    public class CsvParsingException : Exception
    {
        public CsvParsingException()
        {
        }

        public CsvParsingException(string? message) : base(message)
        {
        }

        public CsvParsingException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

    }
}