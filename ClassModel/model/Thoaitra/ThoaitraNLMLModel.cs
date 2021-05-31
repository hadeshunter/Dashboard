using System;
using System.Collections.Generic;
using System.Text;

namespace ClassModel.model.ThoaitraNLML
{
    public class ThoaitraNLMLModel
    {
        public int donvi_id { get; set; }
        public string donvi { get; set; }
        public int thang { get; set; }
        public int thoaitra_docquyen { get; set; }
        public int thoaitra_nlml { get; set; }
    }
    public class ThoaitraNLMLModel_date
    {
        public int donvi_id { get; set; }
        public string donvi { get; set; }
        public int donvi_cha_id { get; set; }
        public string donvi_cha { get; set; }
        public int thoaitra_docquyen { get; set; }
        public int thoaitra_nlml { get; set; }
        public DateTime ngay_yc { get; set; }
        public int ngay_yc_so { get; set; }
        public int thang { get; set; }
    }
}
