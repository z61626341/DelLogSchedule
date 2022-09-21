using DelLogSchedule.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ProjectContext
{
    public partial class SysSystemLogContext : DbContext
    {
        public SysSystemLogContext() { }
        public SysSystemLogContext(DbContextOptions<SysSystemLogContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=Test;persist security info=True;user id=sa;password=Jim20220905;MultipleActiveResultSets=true;");
        }
        public virtual DbSet<SysSystemLog> SysSystemLogsContext { get; set; } = default!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Chinese_Taiwan_Stroke_CI_AS");

            modelBuilder.Entity<SysSystemLog>(entity =>
            {
                entity.HasKey(e => e.Uid)
                    .HasName("PK__SYS_ApiLog");

                entity.ToTable("SYS_SystemLog");

                entity.Property(e => e.IP)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.Action)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Target)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.IsSuccess)
                    .IsUnicode(false);

                entity.Property(e => e.Detail)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ErrMsg)
                    .IsUnicode(false);

                entity.Property(e => e.SQLLog)
                    .IsUnicode(false);

                entity.Property(e => e.CreateDateTime).HasColumnType("datetime");

                entity.Property(e => e.CreateUser)
                    .HasMaxLength(15)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
