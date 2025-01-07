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
using System.Runtime.InteropServices;
using System.Diagnostics.Eventing.Reader;

namespace Latihan_Quiz
{
    public partial class FormAddQuiz : Form
    {
        public FormAddQuiz()
        {
            InitializeComponent();
            dt = new DataTable();
            dt.Columns.Add("Question", typeof(String));
            dt.Columns.Add("OptionA", typeof(String));
            dt.Columns.Add("OptionB", typeof(String));
            dt.Columns.Add("OptionC", typeof(String));
            dt.Columns.Add("OptionD", typeof(String));
            dt.Columns.Add("CorrectAnswer", typeof(String));

            dataGridView1.DataSource = dt;

            DataGridViewButtonColumn deleteBtnColumn = new DataGridViewButtonColumn();
            deleteBtnColumn.HeaderText = "Delete";
            deleteBtnColumn.Text = "Delete";
            deleteBtnColumn.UseColumnTextForButtonValue = true;
            dataGridView1.Columns.Add(deleteBtnColumn);

            DataGridViewButtonColumn updateBtnColumn = new DataGridViewButtonColumn();
            updateBtnColumn.HeaderText = "Update";
            updateBtnColumn.Text = "Update";
            updateBtnColumn.UseColumnTextForButtonValue = true;
            dataGridView1.Columns.Add(updateBtnColumn);
        }

        private string selectedAnswer;
        private SqlCommand cmd1;
        private SqlCommand cmd2;
        private DataSet ds;
        private SqlDataAdapter da;
        private SqlDataReader rd;
        private DataTable dt;

        private int? updatingRowIndex = null;

        Koneksi Konn = new Koneksi();

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                selectedAnswer = textBox5.Text;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                selectedAnswer = textBox6.Text;
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                selectedAnswer = textBox7.Text;
            }
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4.Checked)
            {
                selectedAnswer = textBox8.Text;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (updatingRowIndex.HasValue)
            {
                UpdateQuestion();
            }
            else
            {
                AddQuestion();
            }
        }

        private void AddQuestion()
        {
            if (string.IsNullOrEmpty(selectedAnswer))
            {
                MessageBox.Show("Harap Pilih Jawaban Yang Benar", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(textBox4.Text) || string.IsNullOrEmpty(textBox5.Text) || string.IsNullOrEmpty(textBox6.Text) ||
                string.IsNullOrEmpty(textBox7.Text) || string.IsNullOrEmpty(textBox8.Text))
            {
                MessageBox.Show("Data Tidak Boleh Kosong", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataRow newRow = dt.NewRow();
            newRow["Question"] = textBox4.Text;
            newRow["OptionA"] = textBox5.Text;
            newRow["OptionB"] = textBox6.Text;
            newRow["OptionC"] = textBox7.Text;
            newRow["OptionD"] = textBox8.Text;
            newRow["CorrectAnswer"] = selectedAnswer;

            dt.Rows.Add(newRow);
            dataGridView1.DataSource = dt;

            ClearForm();
        }

        private void UpdateQuestion()
        {
            if (updatingRowIndex.HasValue && updatingRowIndex.Value >= 0 && updatingRowIndex.Value < dt.Rows.Count)
            {
                DataRow row = dt.Rows[updatingRowIndex.Value];
                row["Question"] = textBox4.Text;
                row["OptionA"] = textBox5.Text;
                row["OptionB"] = textBox6.Text;
                row["OptionC"] = textBox7.Text;
                row["OptionD"] = textBox8.Text;
                row["CorrectAnswer"] = selectedAnswer;

                dataGridView1.DataSource = dt;
                updatingRowIndex = null;

                button1.Text = "Add Question";
                ClearForm();
            }
        }

        private void ClearForm()
        {
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
            textBox7.Clear();
            textBox8.Clear();

            radioButton1.Checked = false;
            radioButton2.Checked = false;
            radioButton3.Checked = false;
            radioButton4.Checked = false;

            selectedAnswer = null;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormMainMenu fmrMain = new FormMainMenu();
            fmrMain.Show();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                var column = dataGridView1.Columns[e.ColumnIndex];

                if (column is DataGridViewButtonColumn)
                {
                    if (column.HeaderText == "Delete")
                    {
                        if (MessageBox.Show("Apakah Anda Yakin Ingin Menghapus Pertanyaan ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            dataGridView1.Rows.RemoveAt(e.RowIndex);
                        }
                    }
                    else if (column.HeaderText == "Update")
                    {
                        LoadQuestionForUpdate(e.RowIndex);
                    }
                }
            }
        }

        private void LoadQuestionForUpdate(int rowIndex)
        {
            if (rowIndex >= 0 && rowIndex < dt.Rows.Count)
            {
                DataRow row = dt.Rows[rowIndex];

                textBox4.Text = row["Question"].ToString();
                textBox5.Text = row["OptionA"].ToString();
                textBox6.Text = row["OptionB"].ToString();
                textBox7.Text = row["OptionC"].ToString();
                textBox8.Text = row["OptionD"].ToString();

                string correctAnswer = row["CorrectAnswer"].ToString();
                if (correctAnswer == textBox5.Text)
                {
                    radioButton1.Checked = true;
                }
                else if (correctAnswer == textBox6.Text)
                {
                    radioButton2.Checked = true;
                }
                else if (correctAnswer == textBox7.Text)
                {
                    radioButton3.Checked = true;
                }
                else if (correctAnswer == textBox8.Text)
                {
                    radioButton4.Checked = true;
                }

                updatingRowIndex = rowIndex;
                button1.Text = "Update Question";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SqlConnection Conn = Konn.GetConn();
            Conn.Open();
            if (String.IsNullOrEmpty(textBox1.Text) || String.IsNullOrEmpty(textBox2.Text) || String.IsNullOrEmpty(textBox3.Text))
            {
                MessageBox.Show("Harap Isi Semua Data", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                cmd2 = new SqlCommand("Select Count(*) from [Quiz] Where Code = @Code COLLATE SQL_Latin1_General_CP1_CS_AS", Conn);
                cmd2.Parameters.AddWithValue("Code", textBox2.Text);
                int userExists = (int)cmd2.ExecuteScalar();

                if (userExists > 0)
                {
                    MessageBox.Show("Kode " + textBox2.Text + " Sudah Digunakan, Silahkan Masukkan Kode yang lain");
                }
                else
                {
                    cmd2 = new SqlCommand("Insert into [Quiz](UserID, Name, Code, Description, CreatedAt) values (@UserID, @Name, @Code, @Description, @CreatedAt); SELECT SCOPE_IDENTITY();", Conn);
                    cmd2.Parameters.AddWithValue("@UserID", Class1.userID);
                    cmd2.Parameters.AddWithValue("@Name", textBox1.Text);
                    cmd2.Parameters.AddWithValue("@Code", textBox2.Text);
                    cmd2.Parameters.AddWithValue("@Description", textBox3.Text);
                    cmd2.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                    object quizIdObj = cmd2.ExecuteScalar();
                    int quizId;

                    if (quizIdObj != null && int.TryParse(quizIdObj.ToString(), out quizId))
                    {
                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            if (!row.IsNewRow)
                            {
                                cmd2 = new SqlCommand("Insert into [Question](QuizID, Question, OptionA, OptionB, OptionC, OptionD, CorrectAnswer) Values (@QuizID,@Question, @OptionA, @OptionB, @OptionC, @OptionD, @CorrectAnswer)", Conn);
                                cmd2.Parameters.AddWithValue("QuizID", quizId);
                                cmd2.Parameters.AddWithValue("@Question", row.Cells["Question"].Value);
                                cmd2.Parameters.AddWithValue("@OptionA", row.Cells["OptionA"].Value);
                                cmd2.Parameters.AddWithValue("@OptionB", row.Cells["OptionB"].Value);
                                cmd2.Parameters.AddWithValue("@OptionC", row.Cells["OptionC"].Value);
                                cmd2.Parameters.AddWithValue("@OptionD", row.Cells["OptionD"].Value);
                                cmd2.Parameters.AddWithValue("@CorrectAnswer", row.Cells["CorrectAnswer"].Value);

                                cmd2.ExecuteNonQuery();
                            }
                        }
                        MessageBox.Show("Data berhasil dimasukkan", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    ClearQuiz();
                    ClearQuestion();
                    DataGridViewKosong();
                }
            }

            Conn.Close();
        }

        private void ClearQuiz()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
        }

        private void ClearQuestion()
        {
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
            textBox7.Clear();
            textBox8.Clear();
        }

        private void DataGridViewKosong()
        {
            dataGridView1.DataSource = null;
            dataGridView1.Rows.Clear();
        }

        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            this.dataGridView1.Rows[e.RowIndex].Cells["No"].Value = (e.RowIndex + 1).ToString();
        }

        private void radioButton1_CheckedChanged_1(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                selectedAnswer = textBox5.Text;
            }
        }

        private void radioButton4_CheckedChanged_1(object sender, EventArgs e)
        {
            if (radioButton4.Checked)
            {
                selectedAnswer = textBox6.Text;
            }
        }

        private void radioButton3_CheckedChanged_1(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                selectedAnswer = textBox7.Text;
            }
        }

        private void radioButton2_CheckedChanged_1(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                selectedAnswer = textBox8.Text;
            }
        }

        private void FormAddQuiz_Load(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
