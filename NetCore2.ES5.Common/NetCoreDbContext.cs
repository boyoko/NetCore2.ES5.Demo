using Microsoft.EntityFrameworkCore;
using NetCore2.ES5.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore2.ES5.Common
{
    public class NetCoreDbContext : DbContext
    {

        public virtual DbSet<CPAT_IN_PATIENT> CPAT_IN_PATIENT { get; set; }
        public virtual DbSet<CPAT_EMR_RECORD> CPAT_EMR_RECORD { get; set; }
        //public virtual DbSet<SCQ_CPATS> SCQ_CPATS { get; set; }
        public virtual DbSet<SD_CPATS> SD_CPATS { get; set; }
        public virtual DbSet<SD_CPAT_DETAIL> SD_CPAT_DETAIL { get; set; }
        //public virtual DbSet<CPAT_CHECK_RECORD> CPAT_CHECK_RECORD { get; set; }

        public NetCoreDbContext()
            : base()
        {
        }

        public NetCoreDbContext(DbContextOptions<NetCoreDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CPAT_EMR_RECORD>(entity =>
            {
                entity.HasKey(e => e.EMR_REC_ID);
            });

            modelBuilder.Entity<CPAT_IN_PATIENT>(entity =>
            {
                entity.HasKey(e => e.PATIENT_NO);
            });

            //modelBuilder.Entity<SCQ_CPATS>(entity =>
            //{
            //    //复合主键
            //    entity.HasKey(e => new { e.SCQ_ID, e.SD_CPAT_NO });
            //});

            modelBuilder.Entity<SD_CPATS>(entity =>
            {
                entity.HasKey(e => e.SD_CPAT_NO);
            });

            modelBuilder.Entity<SD_CPAT_DETAIL>(entity =>
            {
                entity.HasKey(e => e.DETAIL_ID);
            });

            //modelBuilder.Entity<CPAT_CHECK_RECORD>(entity =>
            //{
            //    entity.HasKey(e => e.CHECK_ID);
            //});

        }
    }
}
