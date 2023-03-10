using System.ComponentModel.DataAnnotations;

namespace Net.Core.Authentication.Models
{
    public class Request
    {
        [DataType(DataType.Text, ErrorMessage = "value must be string"), Required(ErrorMessage = "request_id is required")]
        public string request_id { get; set; }

        [DataType(DataType.Text, ErrorMessage = "value must be string"), Required(ErrorMessage = "account_number is required")]
        public string account_number { get; set; }

        public string full_name { get; set; }
    }
}
