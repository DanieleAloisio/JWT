using System.Collections.Generic;
using System.Threading.Tasks;

namespace Email
{
    public interface IEmail
    {
        Task SendAsync(string from, string to, string subject, string html);
        Task SendAsync(string from, IEnumerable<string> to, string subject, string html);
    }
}
