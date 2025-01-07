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
    public partial class FormViewReport : Form
    {
        private SqlCommand cmd;
        private DataSet ds;
        private SqlDataAdapter da;
        private SqlDataReader rd;
        private DataTable dt;

        Koneksi Konn = new Koneksi();

        public FormViewReport()
        {
            InitializeComponent();
            SqlConnection Conn = Konn.GetConn();
            Conn.Open();
            

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
        private void Combo()
        {
            SqlConnection Conn = Konn.GetConn();
            Conn.Open();
            cmd = new SqlCommand("SELECT ID, Name FROM [Quiz] WHERE UserID = @UserID", Conn);
            cmd.Parameters.AddWithValue("@UserID", Class1.userID);
            rd = cmd.ExecuteReader();

            while (rd.Read())
            {
                comboBox1.Items.Add(new KeyValuePair<int, string>(Convert.ToInt32(rd["ID"]), rd["Name"].ToString()));
            }

            rd.Close();
            Conn.Close();

            comboBox1.DisplayMember = "Value";
            comboBox1.ValueMember = "Key";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != -1)
            {
                var selectedQuiz = (KeyValuePair<int, string>)comboBox1.SelectedItem;
                int quizID = selectedQuiz.Key;
                LoadDataGridView(quizID);
            }
        }

        private void LoadDataGridView(int quizID)
        {
            SqlConnection Conn = Konn.GetConn();
            Conn.Open();

            cmd = new SqlCommand("SELECT P.ParticipantNickname, P.TimeTaken, " +
                "(SELECT COUNT(*) FROM [ParticipantAnswer] PA " +
                "INNER JOIN [Question] Q ON PA.QuestionID = Q.ID " +
                "WHERE PA.ParticipantID = P.ID AND PA.Answer = Q.CorrectAnswer) AS CorrectAnswers, " +
                "(SELECT COUNT(*) FROM [Question] WHERE QuizID = @QuizID) AS TotalQuestions " +
                "FROM [Participant] P WHERE P.QuizID = @QuizID", Conn);

            cmd.Parameters.AddWithValue("@QuizID", quizID);
            da = new SqlDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);

            dt.Columns.Add("Time Taken", typeof(string));
            dt.Columns.Add("CorrectPercentage", typeof(string));

            int totalSeconds = 0;
            int totalPercentage = 0;
            foreach (DataRow row in dt.Rows)
            {
                int timeTakenInSeconds = Convert.ToInt32(row["TimeTaken"]);
                TimeSpan time = TimeSpan.FromSeconds(timeTakenInSeconds);
                totalSeconds += timeTakenInSeconds;
                row["Time Taken"] = time.ToString(@"hh\:mm\:ss");

                int correctAnswers = Convert.ToInt32(row["CorrectAnswers"]);
                int totalQuestions = Convert.ToInt32(row["TotalQuestions"]);
                int correctPercentage = totalQuestions > 0 ? (correctAnswers * 100) / totalQuestions : 0;

                row["CorrectPercentage"] = correctPercentage.ToString() + "%";

                totalPercentage += correctPercentage;
            }

            int participantCount = dt.Rows.Count;
            int averageSeconds = participantCount > 0 ? totalSeconds / participantCount : 0;
            TimeSpan averageTime = TimeSpan.FromSeconds(averageSeconds);
            label6.Text = averageTime.ToString(@"hh\:mm\:ss");

            int averagePercentage = participantCount > 0 ? totalPercentage / participantCount : 0;
            label7.Text = averagePercentage.ToString() + "%";

            int totalParticipants = dt.Rows.Count;
            label8.Text = totalParticipants.ToString() + " Participant(s)";

            dt.Columns.Remove("TimeTaken");
            dt.Columns.Remove("CorrectAnswers");
            dt.Columns.Remove("TotalQuestions");

            dataGridView1.DataSource = dt;

            Conn.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormMainMenu frmMenu = new FormMainMenu();
            frmMenu.Show();
        }

        private void FormViewReport_Load_1(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            dataGridView1.DataSource = null;
            dataGridView1.Rows.Clear();

            Combo();

        }
    }
}
