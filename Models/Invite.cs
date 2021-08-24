using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HonuTasks.Models
{
    public class Invite
    {
        public int Id { get; set; }

        [Display(Name = "User")]
        public int? UserId { get; set; }

        [DisplayName("Event")]
        public int? ProjectId { get; set; }

        [DisplayName("Date")]
        public DateTimeOffset InviteDate { get; set; }

        [DisplayName("Code")]
        public Guid CompanyToken { get; set; }

        [DisplayName("Invitee")]
        public string InviteeId { get; set; }

        [DisplayName("Invitor")]
        public string InvitorId { get; set; }

        [DisplayName("Email")]
        public string InviteeEmail { get; set; }

        [DisplayName("First Name")]
        public string InviteeFirstName { get; set; }

        [DisplayName("Last Name")]
        public string InviteeLastName { get; set; }

        public bool IsValid { get; set; }


        //Navigational Properties
        public virtual HTUser User { get; set; }

        public virtual HTUser Invitor { get; set; }

        public virtual HTUser Invitee { get; set; }

        public virtual Events Event { get; set; }


    }
}
