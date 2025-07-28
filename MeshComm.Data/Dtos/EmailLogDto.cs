namespace MeshComm.Data.Dtos
{
    public class EmailLogDto
    {
        public int EmailLogId { get; set; }
        public string RequestId { get; set; } = null!;
        public string RequestingApplication { get; set; } = null!;
        public string EmailBody { get; set; } = null!;
        public List<string> RecipientEmail { get; set; } = null!;
        public List<string> Cc { get; set; } = null!;
        public List<string> Bcc { get; set; } = null!;
        public string ResponseObject { get; set; } = null!;
        public string EmailTransmissionState { get; set; } = null!;
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime Version { get; set; }
    }
}
