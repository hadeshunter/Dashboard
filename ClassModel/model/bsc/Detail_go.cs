using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ClassModel.model.bsc
{
    public class Detail_go
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int hdtb_id { get; set; }
	    public int hdkh_id { get; set; }
        public int thuebao_id { get; set; }
        public string ma_tb { get; set; }
        public string ten_tb { get; set; }
        public string diachi_ld { get; set; }
        public int dichvuvt_id { get; set; }
        public int loai_tb_id { get; set; }
        public DateTime ngaycn_bbbg { get; set; }
        public DateTime ngay_bbbg { get; set; }
        public int lydohuy_id { get; set; }
        public string lydohuy { get; set; }
        public DateTime ngay_yc { get; set; }
        public int kieuld_id { get; set; }
        public string ten_kieuld { get; set; }
        public DateTime ngay_ttdhtt_nhan { get; set; }
        public DateTime ngay_ttvt_nhan { get; set; }
        public int tovt { get; set; }
        public int donvi_id { get; set; }
        public int tthd_id { get; set; }
        public string ten_dv { get; set; }
    }
}
