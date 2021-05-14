using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ClassModel.model.ThoaitraPCT
{
    public class ThoaitraPCTModel
    {
        [Key]
        public int donvi_id { get; set; }
        public string donvi { get; set; }
        public int thang { get; set; }
        public int pct_thoaitra_ttvt { get; set; }
        public int pct_thoaitra_ttkd { get; set; }
    }

    public class ThoaitraPTC
    {
        public int donvi_id { get; set; }
        public string donvi { get; set; }
        public DateTime ngay_yc { get; set; }
        public int pct_thoaitra_ttvt { get; set; }
        public int pct_thoaitra_ttkd { get; set; }
    }
}