 
namespace blacklist.Application.Common.DTOs
{
	public class UserRefreshTokenDto
	{

		public long Id { get; set; }
		[Required]
		public string UserName { get; set; }
		[Required]
		public string RefreshToken { get; set; }
		public bool IsActive { get; set; } = true;
	}
}
