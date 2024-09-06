﻿using System;
using System.ComponentModel.DataAnnotations;

namespace LoanServicingApi.Models
{
	public class Payment
	{
        [Key]
        public int Id { get; set; }
        public int LoanId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}

