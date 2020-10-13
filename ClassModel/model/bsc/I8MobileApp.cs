using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ClassModel.model.bsc
{
    public class I8MobileApp
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int i8mbid { get; set; }
        public string ten_tt { get; set; }
        public int sl_login { get; set; }
        public int donvicha_id { get; set; }
        public string ten_dv { get; set; }
        public int dv_id { get; set; }
        public int tong_nv { get; set; }
        public double ty_le { get; set; }
        public DateTime ngay { get; set; }
    }
    public class UsageResponse
    {
        public int dv_id { get; set; }
        public string ten_dv { get; set; }
        public int ttvt_id { get; set; }
        public string ttvt { get; set; }
        public int login { get; set; }
        public int tong { get; set; }
        public int ty_le { get; set; }
        public long unix_time { get; set; }
    }
}
