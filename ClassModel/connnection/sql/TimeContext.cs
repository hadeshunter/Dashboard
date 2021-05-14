using ClassModel.model.timer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClassModel.connnection.sql
{
    public class TimeContext : DbContext
    {
        public TimeContext(DbContextOptions<TimeContext> options) : base(options)
        {
        }
        public DbSet<TimeChange> TimeChange { get; set; }
    }
    }
