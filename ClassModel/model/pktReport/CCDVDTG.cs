using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ClassModel.model.pktReport
{
    public class CCDVDTG
    {
        public int donvi_id { get; set; }
        public string ten_dv { get; set; }
        public int tong_pct { get; set; }
        public int soluong_khonghen_ccdv { get; set; }
        public int ok_khonghen_ccdv { get; set; }
        public int tregio_khonghen_ccdv { get; set; }
        public int soluong_cohen_ccdv { get; set; }
        public int ok_cohen_ccdv { get; set; }
        public int tregio_cohen_ccdv { get; set; }
        public double tyle_ccdv { get; set; }
    }
}
