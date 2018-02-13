using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyEvernote.Entities
{
    [Table("EvernoteUsers")]
    public class EvernoteUser : MyEntityBase
    {
        [DisplayName("Ad"), StringLength(25)]
        public string Name { get; set; }

        [DisplayName("Soyad"), StringLength(25)]
        public string Surname { get; set; }

        [DisplayName("Kullanıcı Adı"), Required, StringLength(25)]
        public string Username { get; set; }

        [DisplayName("E-posta"), Required, StringLength(70)]
        public string Email { get; set; }

        [DisplayName("Şifre"), Required, StringLength(25)]
        public string Password { get; set; }

        [StringLength(30), ScaffoldColumn(false)]
        public string ProfileImageFileName { get; set; }

        [DisplayName("Aktif Mi")]
        public bool IsActive { get; set; }

        [DisplayName("Admin Mi")]
        public bool IsAdmin { get; set; }

        [Required, ScaffoldColumn(false)]
        public Guid ActivateGuid { get; set; }

        public virtual List<Note> Notes { get; set; }
        public virtual List<Comment> Comments { get; set; }
        public virtual List<Liked> Likes { get; set; }
    }
}
