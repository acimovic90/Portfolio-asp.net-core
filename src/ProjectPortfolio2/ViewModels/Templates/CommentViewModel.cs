﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectPortfolio2.ViewModels.Templates
{
    public class CommentViewModel
    {
        public string Url { get; set; }
        public int Score { get; set; }
        public string Text { get; set; }
        public DateTime? CreateDate { get; set; }
        
    }
}
