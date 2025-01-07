using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Latihan_Quiz
{
    internal class Koneksi
    {
        public SqlConnection GetConn()
        {
            SqlConnection Conn = new SqlConnection();
            Conn.ConnectionString = "Data Source=DESKTOP-3NBPQDA\\MSSQLSERVER01;Initial Catalog=QuizinAja;Integrated Security=True;Encrypt=False";
            return Conn;
        }
    }
}
