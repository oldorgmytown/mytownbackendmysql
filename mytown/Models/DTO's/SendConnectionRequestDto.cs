using System.ComponentModel.DataAnnotations;

namespace mytown.DTOs
{
    public class SendConnectionRequestDto
    {
        [Range(1, int.MaxValue)]
        public int ShopperId { get; set; }           // the shopper sending the request

        [Range(1, int.MaxValue)]
        public int ReceiverShopperId { get; set; }   // the shopper receiving it
    }
}