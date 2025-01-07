using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Latihan_Quiz
{
    public partial class FormCreateAccount : Form
    {
        public FormCreateAccount()
        {
            InitializeComponent();
        }
        private SqlCommand cmd;
        private DataSet ds;
        private SqlDataAdapter da;
        private SqlDataReader rd;

        Koneksi Konn = new Koneksi();


        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide();
            FormLogin formLogin = new FormLogin();
            formLogin.ShowDialog();
        }

        void Clear()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String data = textBox3.Text;
            int counterr = 0, min = 4, max = 20;
            data = data.Trim();


            if (String.IsNullOrEmpty(textBox1.Text) || String.IsNullOrEmpty(textBox2.Text) || String.IsNullOrEmpty(dt1.Text) || String.IsNullOrEmpty(textBox3.Text) || String.IsNullOrEmpty(textBox4.Text))
            {
                MessageBox.Show("Data Tidak Boleh kosong");
            }
            else if (textBox3.Text != textBox4.Text)
            {
                MessageBox.Show("Password dan Retype Password Tidak Sesuai");
                textBox4.Clear();
            }
            else if (data.Length < 4)
            {
                counterr += 1;
                MessageBox.Show("Password Tidak Boleh Kurang Dari 4 karakter","Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox3.Clear();
                textBox4.Clear();
            }
            else if (data.Length > 20)
            {
                counterr += 1;
                MessageBox.Show("Password Tidak Boleh Lebih Dari 20 Karakter", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox3.Clear();
                textBox4.Clear();
            }
            else
            {
                SqlConnection Conn = Konn.GetConn();
                Conn.Open();
                cmd = new SqlCommand("Select Count(*) from [User] Where Username = @Username COLLATE SQL_Latin1_General_CP1_CS_AS", Conn);
                cmd.Parameters.AddWithValue("Username", textBox1.Text);
                int userExists = (int)cmd.ExecuteScalar();
                if (userExists > 0)
                {
                    MessageBox.Show("Username ini sudah Digunakan Silahkan Pilih Username Yang Lain");
                    textBox1.Clear();
                }
                else
                {
                    cmd = new SqlCommand("insert into [User] Values (@Username,@FullName,@DateOfBirth,@Password)", Conn);
                    cmd.Parameters.AddWithValue("@Username", textBox1.Text);
                    cmd.Parameters.AddWithValue("@FullName", textBox2.Text);
                    cmd.Parameters.AddWithValue("DateOfBirth", DateTime.Parse(dt1.Text));
                    cmd.Parameters.AddWithValue("@Password", textBox3.Text);
                    cmd.ExecuteNonQuery();
                    Conn.Close();

                    MessageBox.Show("Create Account Berhasil","Informasion",MessageBoxButtons.OK,MessageBoxIcon.Information);
                    Clear();

                    this.Hide();
                    new FormLogin().Show();
                }

            }

        }

        private void FormCreateAccount_Load(object sender, EventArgs e)
        {

        }

        private void dt1_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
