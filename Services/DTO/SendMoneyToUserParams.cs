using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DTO
{
    public class SendMoneyToUserParams
    {
        public string Phone { get; set; }

        public int? UserId { get; set; }
    }
}
