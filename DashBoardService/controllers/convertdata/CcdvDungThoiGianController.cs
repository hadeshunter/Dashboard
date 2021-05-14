using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DashBoardService.server.convertdata.ccdv;
using DashBoardService.server.convertdata.scdv;
using DashBoardService.server.convertdata.tk_khl;
using DashBoardService.server.convertdata.xlsc;
using Microsoft.AspNetCore.Mvc;

namespace DashBoardService.controllers.convertdata
{
    [Route("api/dashboard/[controller]")]
    [ApiController]
    public class CcdvDungThoiGianController : Controller
    {
        private ICcdvDungThoiGian m_ccdvDungThoiGian;
        private ISua_Chua_DV_Dung_TG_Quy_Dinh_New m_sua_Chua_DV_Dung_TG_Quy_Dinh_New;
        private ITyLeThoiGianDapUngXuLySuCo m_tyLeThoiGianDapUngXuLySuCo;
        private ITK_KhongHaiLong_CLDV m_tK_KhongHaiLong_CLDV;
        public CcdvDungThoiGianController(ICcdvDungThoiGian _ccdvDungThoiGian, ISua_Chua_DV_Dung_TG_Quy_Dinh_New _sua_Chua_DV_Dung_TG_Quy_Dinh_New, ITyLeThoiGianDapUngXuLySuCo _tyLeThoiGianDapUngXuLySuCo, ITK_KhongHaiLong_CLDV _tK_KhongHaiLong_CLDV)
        {
            m_ccdvDungThoiGian = _ccdvDungThoiGian;
            m_sua_Chua_DV_Dung_TG_Quy_Dinh_New = _sua_Chua_DV_Dung_TG_Quy_Dinh_New;
            m_tyLeThoiGianDapUngXuLySuCo = _tyLeThoiGianDapUngXuLySuCo;
            m_tK_KhongHaiLong_CLDV = _tK_KhongHaiLong_CLDV;
        }

        [HttpGet("test")]
        public dynamic test() {
            try
            {
                return m_tK_KhongHaiLong_CLDV.toDataConvert("05/03/2021", "07/03/2021");
            }
            catch(Exception e)
            {

            }
            
            return true;
        }
        [HttpGet("sua_Chua_DV_Dung_TG_Quy_Dinh_New")]
        public dynamic sua_Chua_DV_Dung_TG_Quy_Dinh_New()
        {
            try
            {
                return m_sua_Chua_DV_Dung_TG_Quy_Dinh_New.toDataConvert("05/03/2020", "07/03/2020");
            }
            catch (Exception e)
            {

            }

            return true;
        }
    }
}