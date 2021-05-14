using ClassModel.connnection.reponsitory.impl;
using ClassModel.connnection.sql;
using ClassModel.model.timer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerConvert.service.timer.impl
{
    public class TimeChangeImpl : TimeReponsitory<TimeChange>, ITimeChange
    {
        public TimeChangeImpl(TimeContext context) : base(context)
        {
        }
    }
}
