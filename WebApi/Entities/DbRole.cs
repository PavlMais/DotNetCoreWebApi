using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace WebApi.Entities
{
    public class DbRole : IdentityRole<long>
    {
        public virtual ICollection<DbUserRole> UserRoles { get; set; }
    }
}