using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Runtime.Remoting.Messaging;
using System.Runtime.InteropServices;

namespace Latihan_Quiz
{
    public partial class FormMainMenu : Form
    {
        private SqlCommand cmd;
        private DataSet ds;
        private SqlDataAdapter da;
        private SqlDataReader rd;
        private DataTable dt;

        Koneksi Konn = new Koneksi();

        public FormMainMenu()
        {
            InitializeComponent();
        }

        void data()
        {
            SqlConnection Conn = Konn.GetConn();
            Conn.Open();
            cmd = new SqlCommand("Select Name,Code,Description,CreatedAt from [Quiz] where UserID=@UserID", Conn);
            cmd.Parameters.AddWithValue("UserID", Class1.userID);
            da = new SqlDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;

        }

        private void FormMainMenu_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'quizinAjaDataSet1.Quiz' table. You can move, or remove it, as needed.
            this.quizTableAdapter.Fill(this.quizinAjaDataSet1.Quiz);
            lblUser.Text = Class1.uname;
            data();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormAddQuiz frmAdd = new FormAddQuiz();
            frmAdd.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Apakah Anda Yakin Keluar dari Akun Ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Hide();
                FormLogin frmLogin = new FormLogin();
                frmLogin.Show();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormViewReport frmView = new FormViewReport();
            frmView.Show();

        }

        private void lblUser_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Delete")
            {
                if (e.RowIndex >= 0)
                {
                    DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                   

                    if (MessageBox.Show("Apakah anda yakin ingin menghapus data ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        SqlConnection Conn = Konn.GetConn();
                        Conn.Open();
                        SqlTransaction ts = null;

                        try
                        {
                            ts = Conn.BeginTransaction();

                            string selectedQuizCode = row.Cells[1].Value.ToString();

                            SqlCommand cmd = new SqlCommand("DELETE FROM [ParticipantAnswer] WHERE QuestionID IN (SELECT ID FROM [Question] WHERE QuizID = (SELECT ID FROM [Quiz] WHERE Code = @QuizCode))", Conn, ts);
                            cmd.Parameters.AddWithValue("@QuizCode", selectedQuizCode);
                            cmd.ExecuteNonQuery();

                            cmd = new SqlCommand("DELETE FROM [Participant] WHERE QuizID = (SELECT ID FROM [Quiz] WHERE Code = @QuizCode)", Conn, ts);
                            cmd.Parameters.AddWithValue("@QuizCode", selectedQuizCode);
                            cmd.ExecuteNonQuery();

                            cmd = new SqlCommand("DELETE FROM [Question] WHERE QuizID = (SELECT ID FROM [Quiz] WHERE Code = @QuizCode)", Conn, ts);
                            cmd.Parameters.AddWithValue("@QuizCode", selectedQuizCode);
                            cmd.ExecuteNonQuery();

                            cmd = new SqlCommand("DELETE FROM [Quiz] WHERE Code = @QuizCode", Conn, ts);
                            cmd.Parameters.AddWithValue("@QuizCode", selectedQuizCode);
                            cmd.ExecuteNonQuery();

                            ts.Commit();
                            MessageBox.Show("Quiz berhasil dihapus.");
                        }
                        catch (Exception ex)
                        {
                            ts.Rollback();
                            MessageBox.Show("Terjadi kesalahan: " + ex.Message);
                        }
                        finally
                        {
                            Conn.Close();
                        }

                        this.quizTableAdapter.Fill(this.quizinAjaDataSet1.Quiz);
                        data();
                    }
                }
            }
        }

    }
}


