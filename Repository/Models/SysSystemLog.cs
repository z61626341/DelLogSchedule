using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelLogSchedule.Models
{
    public class SysSystemLog
    {
        public Guid Uid { get; set; }
        public string? IP { get; set; }
        public string? Action { get; set; }
        public string? Target { get; set; }
        public bool? IsSuccess { get; set; }
        public string? Detail { get; set; }
        public string? ErrMsg { get; set; }
        public string? SQLLog { get; set; }
        public DateTime CreateDateTime { get; set; }
        public string? CreateUser { get; set; }
    }
}
