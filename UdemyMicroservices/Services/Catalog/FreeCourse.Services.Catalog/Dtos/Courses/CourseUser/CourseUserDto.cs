using Microsoft.AspNetCore.Identity;
using System;

namespace FreeCourse.Services.Catalog.Dtos.Courses.CourseUser
{
    public class CourseUserDto
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string City { get; set; }

        public byte[] ProfilePicture { get; set; }

    }
}
