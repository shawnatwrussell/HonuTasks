using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HonuTasks.Models
{
    public class HTUser : IdentityUser
    {
        [Required]
        [StringLength(50)]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [NotMapped]
        [DisplayName("Full Name")]
        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }

        //File Properties
        [NotMapped]
        [DataType(DataType.Upload)]
        //[MaxFileSize(1024 * 1024)]
        //[AllowedExtensions(new string[] {".jpg", ",png"})]
        [DisplayName("Choose a File")]
        public IFormFile AvatarImage { get; set; }

        [DisplayName("File Name")]
        public string AvatarFileName { get; set; }
        public byte[] AvatarFileData { get; set; }

        [DisplayName("File Extension")]
        public string ContentType { get; set; }


        public int? EventId { get; set; }

        public int? CreatorId { get; set; }

        //Navigational Properties
        public virtual ICollection<Events> Events { get; set; } =
            new HashSet<Events>();
    }
}
