using System;
using System.Collections.Generic;
using System.Text;

namespace ClassModel.convertdata.ccdv
{
    public class Ccdv_Dung_Tg
    {
        public int donvi_id { get; set; }
        public int nhomlc_id { get; set; }
        public int donvi_cha_id { get; set; }
        public string ten_dvql { get; set; }
        public int tong_pct { get; set; }


        public string ten_dv { get; set; }
        public DateTime ngaycn_bbbg { get; set; }
        public int soluong_khonghen_ccdv { get; set; }
        public int ok_khonghen_ccdv { get; set; }
        public int tregio_khonghen_ccdv { get; set; }
        public int soluong_cohen_ccdv { get; set; }
        public int ok_cohen_ccdv { get; set; }
        public int tregio_cohen_ccdv { get; set; }
        public Double tyle_ccdv { get; set; }
    }
}
