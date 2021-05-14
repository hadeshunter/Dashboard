using System;
using System.Collections.Generic;
using System.Text;

namespace ClassModel.model.BTS
{
    public class MLLBTS
    {
        public string ttvt { get; set; }
        public long sl_mll { get; set; }
        public long tg_mll_tong { get; set; }
        public double tg_mll_tb { get; set; }
        public string thangketthuc { get; set; }
    }

    public class MLLNN
    {
        public string ten_loi { get; set; }
        public long sl_mll { get; set; }
        public long tongcong { get; set; }
        public double ty_le { get; set; }
        public string thangketthuc { get; set; }
    }

    public class tk_mll_bts
    {
        public int donvi_id { get; set; }
        public string don_vi { get; set; }
        public int tong_bts { get; set; }
        public int tong_suco { get; set; }
        public int tong_tg_mll { get; set; }
        public double tb_tg_mll { get; set; }
        public double dokhadung { get; set; }
        public int may_phat_dien { get; set; }
        public int mat_ac_accu_yeu { get; set; }
        public int loi_tbi_nguon { get; set; }
        public int ibs_mat_dien_ac { get; set; }
        public int thay_tbi_nguon { get; set; }
        public int khac { get; set; }
        public int hu_dut_cap { get; set; }
        public int tb_access { get; set; }
        public int ttvt_chuyen_mang { get; set; }
        public int tb_core { get; set; }
        public int dhtt_chuyen_mang_tbi { get; set; }
        public int chay_cap { get; set; }
        public int mang_cap_viba_thue_sst_spt { get; set; }
        public int loi_viba { get; set; }
        public int bat_kha_khang { get; set; }
        public int khong_ro_nguyen_nhan { get; set; }
        public int tram_moi_di_doi { get; set; }
        public int hu_tbi_bts { get; set; }
        public int net_tac_dong_mang_luoi { get; set; }
        public string loai_mang { get; set; }
        public int so_ngay { get; set; }
        public DateTime thang_tk { get; set; }
    }

    public class tk_mll_bts_nn
    {
        public int donvi_id { get; set; }
        public string ten_dv { get; set; }
        public string loai_mang { get; set; }
        public string ten_loi { get; set; }
        public int sl_loi { get; set; }
        public int so_ngay { get; set; }
        public DateTime thang_tk { get; set; }
    }
}
