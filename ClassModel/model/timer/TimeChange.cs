using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ClassModel.model.timer
{
    public class TimeChange
    {
        [Key]
        public int timeid { get; set; }
        public string name { get; set; }
        public DateTime starttime { get; set; }
        public DateTime endtime { get; set; }
        public DateTime starttimeupdate { get; set; }
        public DateTime endtimeupdate { get; set; }
    }
}
