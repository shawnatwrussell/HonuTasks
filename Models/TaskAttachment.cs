using HonuTasks.Extensions;
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
    public class TaskAttachment
    {
        //Primary Key
        public int Id { get; set; }

        //FK
        [DisplayName("Task")]
        public int TaskId { get; set; }

        public string UserId { get; set; }

        [DisplayName("Description")]
        public string Description { get; set; }

        [DisplayName("Date Created")]
        public DateTimeOffset Created { get; set; }

        //File Properties
        [NotMapped]
        [DataType(DataType.Upload)]
        [DisplayName("Choose a File")]
        [MaxFileSize(2 * 1024 * 1024)]
        [AllowedExtensions(new string[] { ".jpg", ".png", ".doc", ".docx", ".xls", ".xlsx", ".pdf" })]
        public IFormFile FormFile { get; set; }

        [DisplayName("File Name")]
        public string FileName { get; set; }
        public byte[] FileData { get; set; }

        [Display(Name = "File Extension")]
        public string FileContentType { get; set; }


        //Navigational Properties
        public virtual Task Task { get; set; }
        //References the Parent-property: Task class

        public virtual HTUser User { get; set; }

    }
}
