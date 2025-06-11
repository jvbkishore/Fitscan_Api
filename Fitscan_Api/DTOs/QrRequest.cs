using System.ComponentModel.DataAnnotations;

namespace Fitscan_Api.DTOs
{
    public class QrRequest
    {
        public required string username { get; set; }
        public double? lat { get; set; }
        public double? lon { get; set; }
    }
}
