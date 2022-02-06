using Microsoft.AspNetCore.Http;
using System;

namespace JWT.Dto
{
    public class Cookie
    {
		public string Name { get; set; } = "";
		public CookieOptions? Options { get; set; }

		public Cookie(string _Name)
		{
			Name = _Name;
		}
		public Cookie(string _Name, CookieOptions _Options)
		{
			Name = _Name;
			Options = _Options;
		}

		public void SetCookieValue(string value, HttpResponse response)
		{
			if (response == null)
				throw new ArgumentNullException(nameof(response));
			if (Options == null)
				throw new NullReferenceException($"{nameof(Options)} is null");

			response.Cookies.Append(Name, value, Options);
		}
		public string GetCookieValue(HttpRequest request)
		{
			if (request == null)
				throw new ArgumentNullException(nameof(request));

			return request.Cookies[Name];
		}
	}
}
