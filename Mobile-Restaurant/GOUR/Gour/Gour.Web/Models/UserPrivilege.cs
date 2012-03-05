namespace Gour.Web.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class UserPrivilege
    {
        public UserPrivilege()
        {
            this.Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }

        public string UserId { get; set; }

        public string Privilege { get; set; }
    }
}