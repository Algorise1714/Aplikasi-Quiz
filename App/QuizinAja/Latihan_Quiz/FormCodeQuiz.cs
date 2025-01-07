using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Latihan_Quiz
{
    public partial class FormCodeQuiz : Form
    {
        private SqlCommand cmd;
        private SqlDataReader rd;
        private Koneksi Konn = new Koneksi();

        public FormCodeQuiz()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {            
            if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("Harap masukkan kode kuis dan nama pengguna.","Peringatan",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }

            else
            {
                if (MessageBox.Show("Apakah Anda Yakin Masuk Ke Dalam Quiz dengan Code " + textBox1.Text + " Dan Dengan Nikcname " + textBox2.Text + "?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SqlConnection Conn = Konn.GetConn();
                    Conn.Open();
                    cmd = new SqlCommand("SELECT * FROM [Quiz] WHERE Code = @Code COLLATE SQL_Latin1_General_CP1_CS_AS", Conn);
                    cmd.Parameters.AddWithValue("@Code", textBox1.Text);
                    rd = cmd.ExecuteReader();
                    rd.Read();
                    if (rd.HasRows)
                    {
                        Class1.quizID = rd["Code"].ToString();
                        Class1.quizID = rd["ID"].ToString();
                        Class1.uname = textBox2.Text;
                        this.Hide();
                        FormQuiz frmQuiz = new FormQuiz();
                        frmQuiz.Show();


                        textBox1.Clear();
                        textBox2.Clear();
                    }

                    else
                    {
                        MessageBox.Show("Code Tidak Valid", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        textBox1.Clear();
                    }
                }
               
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormLogin frmLogin = new FormLogin();
            frmLogin.Show();
        }

        private void FormCodeQuiz_Load(object sender, EventArgs e)
        {
            this.ActiveControl = textBox1;
        }

        private void button1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                button1.PerformClick();
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if(String.IsNullOrEmpty(textBox2.Text))
                {
                    this.ActiveControl = textBox2;
                }
                else
                {
                    button1.PerformClick();
                }
               
            }
            else if (e.Alt && e.KeyCode == Keys.Left)
            {
                button2.PerformClick();
            }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1.PerformClick();
            }
            else if (e.Alt && e.KeyCode == Keys.Left)
            {
                button2.PerformClick();
            }
        }

        private void button2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt && e.KeyCode == Keys.Left)
            {
                button2.PerformClick();
            }
        }
    }
}
