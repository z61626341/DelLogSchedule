using Dapper;
using DelLogSchedule.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace DelLogSchedule
{
    public class LogService : IDisposable
    {
        //private readonly IDbConnection _conn;
        private SqlConnection? _conn = null;
        private IConfiguration? _config = null;
        private readonly string? _client = null;
        public string _connstr = "";
        public LogService(IConfiguration config)
        {
            this._config = config;
            if(config != null)
            {
                this._connstr = _config.GetConnectionString("DefaultConnection");
                this._conn = new SqlConnection(_connstr);
            }

            this._client = $"pid:{System.Diagnostics.Process.GetCurrentProcess().Id}";
        }

        public List<SysSystemLog> GetSystemLog(string query)
        {
            var result = new List<SysSystemLog>();
            string strSql = $"SELECT * FROM SYS_SystemLog WHERE {query}";
            result = _conn.Query<SysSystemLog>(strSql).ToList();
            return result;
        }

        public IEnumerable<SysSystemLog> GetSystemLogs()
        {
            var sql = $@"SELECT * FROM SYS_SystemLog WHERE CreateDateTime between '2023-02-01' and '2023-02-05'";
            return this._conn.Query<SysSystemLog>(
                sql,
                new
                {
                    client = this._client,
                });
        }
        public int DelSystemLogs(int topNum, int delBeforeMonth)
        {
            var sql = $@"DELETE TOP ({topNum}) FROM SYS_SystemLog WHERE DATEADD(m,-{delBeforeMonth},GETDATE()) > CreateDateTime";
            return this._conn.ExecuteScalar<int>(sql);
        }
        public int DelSystemLogsByTruncate()
        {
            var sql = $@"
IF OBJECT_ID('TruncateTable') IS NOT NULL
BEGIN
   DROP TABLE TruncateTable
END

CREATE TABLE TruncateTable(
	[Uid] [uniqueidentifier] NOT NULL,
	[Num] [int] IDENTITY,
	[IP] [varchar](15) NULL,
	[Action] [nvarchar](10) NULL,
	[Target] [nvarchar](50) NULL,
	[IsSuccess] [bit] NULL,
	[Detail] [nvarchar](50) NULL,
	[ErrMsg] [text] NULL,
	[SQLLog] [text] NULL,
	[CreateDateTime] [datetime] NULL,
	[CreateUser] [nvarchar](15) NULL,
 CONSTRAINT [PK_TruncateTable] PRIMARY KEY CLUSTERED 
(
	[Uid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

-- 將資料從[SYS_SystemLog]轉移到TruncateTable
ALTER TABLE [SYS_SystemLog]  SWITCH PARTITION 3 TO TruncateTable ; 
GO 

--刪除資料
TRUNCATE TABLE TruncateTable
GO

SELECT COUNT(*) FROM TruncateTable;
GO";
            return this._conn.ExecuteScalar<int>(sql);
        }
        public void Dispose()
        {
            this._conn.Close();
        }
    }
}
