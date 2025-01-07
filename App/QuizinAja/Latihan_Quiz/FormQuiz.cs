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

namespace Latihan_Quiz
{
    public partial class FormQuiz : Form
    {
        private SqlCommand cmd;
        private SqlDataReader rd;
        private Koneksi Konn = new Koneksi();
        private List<Question> questionList = new List<Question>();
        private int currentQuestionIndex = 0;
        private List<string> userAnswers = new List<string>();
        private int participantID = 0;


        TimeSpan elapsedTime = TimeSpan.Zero;
        public FormQuiz()
        {
            InitializeComponent();
            
        }

        private void LoadQuestions()
        {
            using (SqlConnection Conn = Konn.GetConn())
            {
                Conn.Open();
                cmd = new SqlCommand("SELECT QuizID, ID, Question, OptionA, OptionB, OptionC, OptionD, CorrectAnswer FROM [Question] WHERE QuizID=@QuizID", Conn);
                cmd.Parameters.AddWithValue("@QuizID", Class1.quizID);
                rd = cmd.ExecuteReader();

                int questionIndex = 0;
                while (rd.Read())
                {
                    Question question = new Question
                    {
                        ID = Convert.ToInt32(rd["ID"]),
                        QuestionText = rd["Question"].ToString(),
                        OptionA = rd["OptionA"].ToString(),
                        OptionB = rd["OptionB"].ToString(),
                        OptionC = rd["OptionC"].ToString(),
                        OptionD = rd["OptionD"].ToString(),
                        CorrectAnswer = rd["CorrectAnswer"].ToString()
                    };
                    questionList.Add(question);
                    userAnswers.Add("");

                    Button btn = new Button
                    {
                        Text = (questionIndex + 1).ToString(),
                        Width = 50,
                        Height = 50,
                        Tag = questionIndex
                    };
                    btn.Click += new EventHandler(NavigateToQuestion);

                    flowLayoutPanel1.Controls.Add(btn);
                    questionIndex++;
                }
            }
        }

        private void NavigateToQuestion(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                int questionIndex = (int)btn.Tag;
                currentQuestionIndex = questionIndex;
                DisplayQuestion(currentQuestionIndex);
            }
        }

        private void DisplayQuestion(int index)
        {
            if (index >= 0 && index < questionList.Count)
            {
                var currentQuestion = questionList[index];

                lblQuestion.Text = currentQuestion.QuestionText;
                rbOptionA.Text = currentQuestion.OptionA;
                rbOptionB.Text = currentQuestion.OptionB;
                rbOptionC.Text = currentQuestion.OptionC;
                rbOptionD.Text = currentQuestion.OptionD;

                rbOptionA.Checked = false;
                rbOptionB.Checked = false;
                rbOptionC.Checked = false;
                rbOptionD.Checked = false;

                if (userAnswers[index] == currentQuestion.OptionA) rbOptionA.Checked = true;
                else if (userAnswers[index] == currentQuestion.OptionB) rbOptionB.Checked = true;
                else if (userAnswers[index] == currentQuestion.OptionC) rbOptionC.Checked = true;
                else if (userAnswers[index] == currentQuestion.OptionD) rbOptionD.Checked = true;

                UpdateButtonColors();

                btnPrev.Enabled = index > 0;
                btnNext.Text = (index == questionList.Count - 1) ? "Submit" : "Next";
            }
        }

        private void SaveParticipant()
        {
            using (SqlConnection Conn = Konn.GetConn())
            {
                Conn.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO [Participant] (QuizID, ParticipationDate, TimeTaken, ParticipantNickname) OUTPUT INSERTED.ID VALUES (@QuizID, @ParticipationDate, @TimeTaken, @ParticipantNickname)", Conn);

                cmd.Parameters.AddWithValue("@QuizID", Class1.quizID);
                cmd.Parameters.AddWithValue("@ParticipationDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@TimeTaken", (int)elapsedTime.TotalSeconds);
                cmd.Parameters.AddWithValue("@ParticipantNickname", Class1.uname);

                participantID = (int)cmd.ExecuteScalar();
            }
        }

        private void SaveAllAnswersToDB()
        {
            using (SqlConnection Conn = Konn.GetConn())
            {
                Conn.Open();
                for (int i = 0; i < questionList.Count; i++)
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO [ParticipantAnswer] (ParticipantID, QuestionID, Answer) VALUES (@ParticipantID, @QuestionID, @Answer)", Conn);
                    cmd.Parameters.AddWithValue("@ParticipantID", participantID);
                    cmd.Parameters.AddWithValue("@QuestionID", questionList[i].ID);

                    cmd.Parameters.AddWithValue("@Answer", userAnswers[i]);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void saveAnswer()
        {
            string selectedAnswer = "";

            if (rbOptionA.Checked) selectedAnswer = rbOptionA.Text;
            else if (rbOptionB.Checked) selectedAnswer = rbOptionB.Text;
            else if (rbOptionC.Checked) selectedAnswer = rbOptionC.Text;
            else if (rbOptionD.Checked) selectedAnswer = rbOptionD.Text;

            userAnswers[currentQuestionIndex] = selectedAnswer;
        }

        private void rbOption_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb != null && rb.Checked)
            {
                if (rb == rbOptionA) userAnswers[currentQuestionIndex] = rbOptionA.Text;
                else if (rb == rbOptionB) userAnswers[currentQuestionIndex] = rbOptionB.Text;
                else if (rb == rbOptionC) userAnswers[currentQuestionIndex] = rbOptionC.Text;
                else if (rb == rbOptionD) userAnswers[currentQuestionIndex] = rbOptionD.Text;

                UpdateButtonColors();
            }
        }

        private void UpdateButtonColors()
        {
            for (int i = 0; i < flowLayoutPanel1.Controls.Count; i++)
            {
                Button btn = flowLayoutPanel1.Controls[i] as Button;
                if (btn != null)
                {
                    if (!string.IsNullOrEmpty(userAnswers[i]))
                    {
                        btn.BackColor = Color.LightGreen;
                    }
                    else
                    {
                        btn.BackColor = Color.LightGray;
                    }
                }
            }
        }

        private void FormQuiz_Load(object sender, EventArgs e)
        {
            timer1.Interval = 1000;
            timer1.Tick += timer1_Tick;
            timer1.Tick -= timer1_Tick;
            timer1.Start();

            LoadQuestions();
            DisplayQuestion(currentQuestionIndex);
            label1.Text = Class1.uname;
        }

        private class Question
        {
            public int ID { get; set; }
            public string QuestionText { get; set; }
            public string OptionA { get; set; }
            public string OptionB { get; set; }
            public string OptionC { get; set; }
            public string OptionD { get; set; }
            public string CorrectAnswer { get; set; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            new FormCodeQuiz().Show();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            saveAnswer();

            // Cek apakah ada pertanyaan yang belum dijawab
            bool allAnswered = userAnswers.All(answer => !string.IsNullOrEmpty(answer));

            if (currentQuestionIndex < questionList.Count - 1)
            {
                // Pindah ke pertanyaan berikutnya jika masih ada pertanyaan
                currentQuestionIndex++;
                DisplayQuestion(currentQuestionIndex);
            }
            else
            {
                if (!allAnswered)
                {
                    // Jika ada pertanyaan yang belum dijawab, tampilkan pesan peringatan
                    int unansweredIndex = userAnswers.FindIndex(answer => string.IsNullOrEmpty(answer));
                    MessageBox.Show("HARAP JAWAB SEMUA PERTANYAAN", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    // Arahkan ke pertanyaan yang belum dijawab
                    currentQuestionIndex = unansweredIndex;
                    DisplayQuestion(currentQuestionIndex);
                    return;
                }

                // Tampilkan konfirmasi saat semua pertanyaan telah dijawab
                var result = MessageBox.Show("Apakah Anda yakin ingin mengirim jawaban Anda?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Simpan data peserta dan jawaban ke database
                    SaveParticipant();
                    SaveAllAnswersToDB();

                    MessageBox.Show("Quiz selesai, jawaban Anda telah dikirim.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Tutup form dan kembali ke form utama
                    this.Close();
                    new FormCodeQuiz().Show();
                }
                else
                {
                    // Jika user menekan tombol 'No', hentikan eksekusi
                    return;
                }
            }
        }


        private void btnPrev_Click(object sender, EventArgs e)
        {
            saveAnswer();
            if (currentQuestionIndex > 0)
            {
                currentQuestionIndex--;
                DisplayQuestion(currentQuestionIndex);
            }
        }

        private void btnNext_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt && e.KeyCode == Keys.Right)
            {
                btnNext.PerformClick();
            }
        }

        private void btnPrev_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt && e.KeyCode == Keys.Left)
            {
                btnPrev.PerformClick();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            elapsedTime = elapsedTime.Add(TimeSpan.FromSeconds(1));
            label3.Text = elapsedTime.ToString(@"hh\:mm\:ss") + " Seconds";
        }
    }
}
