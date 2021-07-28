using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPI.Models.System;

namespace WebAPI.Models.Common
{
    public class AnnouncementUserViewModel
    {
        public int AnnouncementId { get; set; }

        public string UserId { get; set; }

        public bool HasRead { get; set; }

        public virtual AppUserViewModel AppUser { get; set; }

        public virtual AnnouncementViewModel Announcement { get; set; }
    }
}