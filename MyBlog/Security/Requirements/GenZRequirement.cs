using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Security.Requirements
{
    public class GenZRequirement: IAuthorizationRequirement
    {
        public GenZRequirement(int fromYear = 1997, int toYear = 2012)
        {
            FromYear = fromYear;
            ToYear = toYear;
            
        }

        public int FromYear { set; get; }
        public int ToYear { set; get; }

    }
    

   
}
