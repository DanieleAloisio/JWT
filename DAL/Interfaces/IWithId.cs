using System;

namespace JWT.Areas.Identity.Interfaces
{
	public interface IWithId<T>
	{
		T Id { get; set; }
	}
}
