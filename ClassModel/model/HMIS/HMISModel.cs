using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ClassModel.model.HMIS
{
    public class HMISModel
    {
        public int id { get; set; }
        public string donvi_banhang { get; set; }
        public int matinh { get; set; }
        public string csyt_quan_huyen { get; set; }
        public string ma_csyt_bhyt { get; set; }
        public string ma_csyt_ngoaibhyt { get; set; }
        public string tuyen { get; set; }
        public string ten_kh { get; set; }
        public string diachi { get; set; }
        public string to_khaibao_hethong { get; set; }
        public string ngay_khaibao_hethong { get; set; }
        public string ngay_trienkhai_his { get; set; }
        public string ngay_trienkhai_hmis { get; set; }
        public string biendong { get; set; }
        public string to_trienkhai { get; set; }
        public string ghichu { get; set; }
        public string ngay_cn { get; set; }
        public string user_cn { get; set; }

        public int da_trienkhai_hmis { get; set; }
        public int chua_trienkhai_hmis { get; set; }
    }
}
