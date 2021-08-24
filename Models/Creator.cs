using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HonuTasks.Models
{
    public class Creator
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [DisplayName("Creator Name")]
        public string Name { get; set; }

        [DisplayName("Description")]
        public string Description { get; set; }

        //Image File Properties
        [NotMapped]
        [DataType(DataType.Upload)]
        //[MaxFileSize(1024 * 1024)]
        //[AllowedExtensions(new string[] {".jpg", ",png"})]
        [DisplayName("Choose a File")]
        public IFormFile ImageFormFile { get; set; }

        [DisplayName("File Name")]
        public string ImageFileName { get; set; }
        public byte[] ImageFileData { get; set; }

        [DisplayName("File Extension")]
        public string ImageFileContentType { get; set; }


        //Navigational Properties
        public virtual ICollection<HTUser> Members { get; set; } =
            new HashSet<HTUser>();

        public virtual ICollection<Events> Events { get; set; } =
            new HashSet<Events>();

        public virtual ICollection<Invite> Invites { get; set; } =
            new HashSet<Invite>();
    }
}
