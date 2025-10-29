namespace Flow_Api.Dtos.SuperAdmin.Response
{
    public class PendingRegistrationDto
    {
        public Guid Id { get; set; }
        public string ShopName { get; set; } = string.Empty;
        public string OwnerName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string BusinessType { get; set; } = string.Empty;
        public DateTime RegisteredDate { get; set; }
        public int DaysPending { get; set; }
        public string IpAddress { get; set; } = string.Empty;
    }
}
