using System;

namespace Application.DTOs
{
    public class ReceiptDocumentDto
    {
        public Guid Id { get; set; }
        public string Num { get; set; }
        public DateTime Date { get; set; }
        public List<ReceiptResourceDto>? ReceiptResources { get; set; }
    }

    public class ReceiptResourceDto
    {
        public Guid Id { get; set; }
        public string ResourceName { get; set; }
        public Guid ResourceId { get; set; }
        public string MeasureUnitName { get; set; }
        public Guid MeasureUnitId { get; set; }
        public int Count { get; set; }
    }
}