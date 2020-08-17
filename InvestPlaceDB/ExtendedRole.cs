using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Text;

namespace InvestPlaceDB
{
    public class ExtendedRole : IdentityRole<int>
    {
        // в AccountDropdownMenu все равно текстом
        public const string NORMAL = "Normal";
        public const string MODERATOR = "Moderator";
        public const string ADMIN = "Administrator";
    }
}
