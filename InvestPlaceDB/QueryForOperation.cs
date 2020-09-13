using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace InvestPlaceDB
{
    public class QueryForOperation
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime Date { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Summ { get; set; }

        public int CashId { get; set; }

        [ForeignKey(nameof(CashId))]
        [InverseProperty("QueryForOperation")]
        public virtual Cash Cash { get; set; }

        public bool IsCashOutput { get; set; }

        public int? CashQueryModeratorId { get; set; }

        [ForeignKey(nameof(CashQueryModeratorId))]
        [InverseProperty(nameof(ExtendedUser.CashQueryModerator))]
        public virtual ExtendedUser CashQueryModerator { get; set; }

        public bool? OperationModerate { get; set; }


        public CashQueryStatus Status { get; set; }


        public int? CashQueryClientProcessorId { get; set; }

        [ForeignKey(nameof(CashQueryClientProcessorId))]
        [InverseProperty(nameof(ExtendedUser.CashQueryClientProcessor))]
        public virtual ExtendedUser CashQueryClientProcessor { get; set; }
    }

    public enum CashQueryStatus
    {
        None = 0,
        ProcessByUser = 1,
        SendConfirm = 2,
        GetConfirm = 3
    }
}
