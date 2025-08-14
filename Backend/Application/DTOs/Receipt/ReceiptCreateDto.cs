using System;

namespace Application.DTOs
{
    public class ReceiptDocumentCreateDto
    {
        public string Num { get; set; }
        public DateTime Date { get; set; }
        public List<ReceiptResourceCreateDto>? ReceiptResources { get; set; }
    }

    public class ReceiptResourceCreateDto
    {
        public Guid ResourceId { get; set; }
        public Guid MeasureUnitId { get; set; }
        public int Count { get; set; }
    }
}