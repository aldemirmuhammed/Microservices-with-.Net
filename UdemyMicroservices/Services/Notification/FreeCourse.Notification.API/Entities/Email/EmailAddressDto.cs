using GreenPipes.Internals.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeCourse.Notification.API.Entities.Email
{
    public class EmailAddressDto 
    {

        public string Province { get; private set; }
        public string District { get; private set; }
        public string Street { get; private set; }
        public string ZipCode { get; private set; }
        public string Line { get; private set; }

        public EmailAddressDto(string province, string district, string street, string zipCode, string line)
        {
            Province = province;
            District = district;
            Street = street;
            ZipCode = zipCode;
            Line = line;
        }

        protected IEnumerable<object> GetEqualityComponents()
        {


            yield return Province;
            yield return District;
            yield return Street;
            yield return ZipCode;
            yield return Line;
        }

    }
}
