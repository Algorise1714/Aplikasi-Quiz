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
using System.Diagnostics.Eventing.Reader;

namespace Latihan_Quiz
{
    public partial class FormLogin : Form
    {
        private SqlCommand cmd;
        private DataSet ds;
        private SqlDataAdapter da;
        private SqlDataReader rd;

        Koneksi Konn = new Koneksi();

        public FormLogin()
        {
            InitializeComponent();

            this.KeyPreview = true;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide();
            FormCreateAccount frmCreate = new FormCreateAccount();
            frmCreate.Show();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide();
            FormCodeQuiz frmJoin = new FormCodeQuiz();
            frmJoin.Show();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtUser.Text) && String.IsNullOrEmpty(txtPass.Text))
            {
                MessageBox.Show("Harap Isi Username Dan Password Terlebih Dahulu!!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (String.IsNullOrEmpty(txtUser.Text))
            {
                MessageBox.Show("Harap Isi Username Terlebih dahulu !", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (String.IsNullOrEmpty(txtPass.Text))
            {
                MessageBox.Show("Harap Isi Password terlebih Dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                SqlConnection Conn = Konn.GetConn();
                Conn.Open();

                cmd = new SqlCommand("SELECT * FROM [User] WHERE Username = @Username COLLATE SQL_Latin1_General_CP1_CS_AS AND Password = @Password COLLATE SQL_Latin1_General_CP1_CS_AS", Conn);

                cmd.Parameters.AddWithValue("@Username", txtUser.Text);
                cmd.Parameters.AddWithValue("@Password", txtPass.Text);
                rd = cmd.ExecuteReader();

                if (rd.Read())
                {
                    if (rd["Password"].ToString() == txtPass.Text)
                    {
                        Class1.userID = rd["ID"].ToString();
                        Class1.uname = rd["FullName"].ToString();
                        this.Hide();
                        FormMainMenu frmMenu = new FormMainMenu();
                        frmMenu.Show();
                    }
                }
                else
                {
                    MessageBox.Show("Username dan Password Salah!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }


        }

        private void cbShow_CheckedChanged(object sender, EventArgs e)
        {
            if(cbShow.Checked)
            {
                txtPass.UseSystemPasswordChar = false;
            }
            else
            {
                txtPass.UseSystemPasswordChar = true;
            }
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            this.ActiveControl = txtUser;
        }

        private void btnLogin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnLogin.PerformClick();
            }

        }

        private void txtUser_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
               if(String.IsNullOrEmpty(txtPass.Text))
                {
                    this.ActiveControl = txtPass;
                }
                else
                {
                    btnLogin.PerformClick();
                }
            }
        }

        private void txtPass_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                btnLogin.PerformClick();
            }

        }
    }
}
