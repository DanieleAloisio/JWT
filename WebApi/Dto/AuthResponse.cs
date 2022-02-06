using System;
using System.Collections.Generic;

namespace JWT.Dto
{
    public class AuthResponse: BaseResponse
    {
		public string Token { get; set; } = "";
		public string RefreshToken { get; set; } = "";
		public DateTime ExpireDate { get; set; }
		public bool RefreshTokenHidden { get; set; }

		public string UserName { get; set; } = "";
		public List<string> Roles { get; set; } = new List<string>();

		public AuthResponse() { }
		public AuthResponse(bool _success) : base(_success) { }

		public void HideRefreshToken()
		{
			RefreshToken = "";
			RefreshTokenHidden = true;
		}
	}
}
