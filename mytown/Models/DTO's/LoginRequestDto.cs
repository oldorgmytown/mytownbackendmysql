namespace mytown.Models.DTO_s
{
    public class LoginRequestDto
    {
        public required string Email { get; init; }
        public required string Password { get; init; }

        //  NEW
        public required string Role { get; init; }  // Admin | Business | Shopper | CourierHead | CourierBranch
    }
}
