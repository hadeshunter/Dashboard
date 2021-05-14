using ClassModel.convertdata.ccdv;
using ClassModel.model.RqGrafana;
using DashBoardService.server.convertdata.ccdv;
using DashBoardService.server.pktReport.detail;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace DashBoardService.server.pktReport.impl
{
    public class PktReportSQLImpl : IPktReportSQL
    {
        private IConfiguration m_configuration;
        private ICCDV m_ccdv;
        private ISCDV m_scdv;
        private ICLDV m_cldv;
        private ICLPV m_clpv;
        private IXLSC m_xlsc;
        private IHSSDC m_hssdc;
        private IThoaiTra m_thoaitra;
        private ITonLDFiber m_tonLDFiber;
        private IMLLBTS m_mllbts;
        public PktReportSQLImpl(IConfiguration configuration, ICCDV ccdv, ISCDV scdv, ICLDV cldv, ICLPV clpv, IXLSC xlsc, IHSSDC hssdc, IThoaiTra thoaitra, ITonLDFiber tonLDFiber, IMLLBTS mllbts)
        {
            m_configuration = configuration;
            m_ccdv = ccdv;
            m_scdv = scdv;
            m_cldv = cldv;
            m_clpv = clpv;
            m_xlsc = xlsc;
            m_hssdc = hssdc;
            m_thoaitra = thoaitra;
            m_tonLDFiber = tonLDFiber;
            m_mllbts = mllbts;
        }

        public dynamic getStatic(RqGrafana rq)
        {
            List<dynamic> data = new List<dynamic>();
            // gType: 1-NLML; 2-CLML
            if ((int)rq.scopedVars.gType.value == 1)
            {
                switch ((int)rq.scopedVars.criteria.value)
                {
                    case 1: //CCDV
                        data = m_ccdv.getCCDV(rq);
                        break;
                    case 3: //HSSDC
                        data = m_hssdc.getHSSDC(rq);
                        break;                    
                    case 7: //Thoai tra PCT
                        data = m_thoaitra.getThoaiTraPTC(rq);
                        break;                   
                }
            } else if ((int)rq.scopedVars.gType.value == 2)
            {
                switch ((int)rq.scopedVars.criteria.value)
                {
                    
                    case 2: //SCDV
                        data = m_scdv.getSCDV(rq);
                        break;
                    case 4: //XLSC
                        data = m_xlsc.getXLSC(rq);
                        break;
                    case 5: //CLDV
                        data = m_cldv.getCLDV(rq);
                        break;
                    case 6: //CLPV
                        data = m_clpv.getCLPV(rq);
                        break;
                    case 8: //Ton LD Fiber
                        data = m_tonLDFiber.getTonLDFiber(rq);
                        break;
                    case 9: //MLL BTS TG
                        data = m_mllbts.getMLLBTS_TG(rq);
                        break;
                    case 11: //MLL BTS NN
                        data = m_mllbts.getMLLBTS_NN(rq);
                        break;
                }
            }
            
            return data;
        }
    }
}
