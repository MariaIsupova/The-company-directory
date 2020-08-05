using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Company_employee
{
    public partial class Form1 : Form
    {
        SqlConnection sqlConnection;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename='|DataDirectory|employees.mdf';Integrated Security=True;";
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Maria\source\repos\Company employee\Company employee\employees.mdf;Integrated Security=True;";
            sqlConnection = new SqlConnection(connectionString);
            sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            string query = "SELECT p.employeeID, pr.personName, pr.birthday, pr.phone, pr.address, positionName, departmentName, s.personName, p.adding FROM EMPLOYEES p LEFT JOIN PERSONAL_DATA s ON p.supervisor=s.personID LEFT JOIN POSITIONS ON p.position=POSITIONS.PositionID LEFT JOIN DEPARTMENTS ON p.department=DEPARTMENTS.DepartmentID LEFT JOIN PERSONAL_DATA pr ON p.employeeID=pr.personID;";
            SqlCommand command = new SqlCommand(query, sqlConnection);
            SqlDataReader reader = command.ExecuteReader();
            List<string[]> data = new List<string[]>();
            while (reader.Read())
            {
                data.Add(new string[9]);
                for (int i = 0; i < dataGridView1.ColumnCount; i++)
                    data[data.Count - 1][i] = reader[i].ToString();
            }
            reader.Close();

            foreach (string[] s in data)
                dataGridView1.Rows.Add(s);

            comboBox_data();
            //sqlConnection.Close();
        }

        private void Reload_data()
        {
            dataGridView1.Rows.Clear();

            string query = "SELECT p.employeeID, pr.personName, pr.birthday, pr.phone, pr.address, positionName, departmentName, s.personName, p.adding FROM EMPLOYEES p LEFT JOIN PERSONAL_DATA s ON p.supervisor=s.personID LEFT JOIN POSITIONS ON p.position=POSITIONS.PositionID LEFT JOIN DEPARTMENTS ON p.department=DEPARTMENTS.DepartmentID LEFT JOIN PERSONAL_DATA pr ON p.employeeID=pr.personID;";
            SqlCommand command = new SqlCommand(query, sqlConnection);
            SqlDataReader reader = command.ExecuteReader();
            List<string[]> data = new List<string[]>();
            while (reader.Read())
            {
                data.Add(new string[9]);
                for (int i = 0; i < dataGridView1.ColumnCount; i++)
                    data[data.Count - 1][i] = reader[i].ToString();
            }
            reader.Close();

            foreach (string[] s in data)
                dataGridView1.Rows.Add(s);
        }


        private void comboBox_data()
        {
            comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBox3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBox4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBox5.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            //comboBox1.Items.Clear();
            SqlCommand select_position = new SqlCommand("SELECT * FROM POSITIONS;", sqlConnection);
            DataTable postable = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(select_position);
            adapter.Fill(postable);
            comboBox1.DataSource = postable;
            comboBox1.DisplayMember = "PositionName";
            comboBox1.ValueMember = "PositionID";
            comboBox1.SelectedIndex = -1;

            SqlCommand select_supervisor = new SqlCommand("SELECT personID, personName FROM PERSONAL_DATA;", sqlConnection);
            SqlDataAdapter adapt = new SqlDataAdapter(select_supervisor);
            DataTable sptable = new DataTable();
            DataTable sptable2 = new DataTable();
            adapt.Fill(sptable);
            adapt.Fill(sptable2);
            sptable.Columns.Add("Combined", typeof(string), "personID +'-'+ personName");
            DataRow dataRow = sptable.NewRow();
            dataRow["personID"] = 0;
            sptable.Rows.InsertAt(dataRow, 0);

            sptable2.Columns.Add("Combined", typeof(string), "personID +'-'+ personName");
            comboBox2.DataSource = sptable;
            comboBox2.DisplayMember = "Combined";
            comboBox2.ValueMember = "personID";
            comboBox2.SelectedIndex = -1;   
            comboBox5.DataSource = sptable2;
            comboBox5.DisplayMember = "Combined";
            comboBox5.ValueMember = "personID";
            comboBox5.SelectedIndex = -1;

            SqlCommand select_department = new SqlCommand("SELECT * FROM DEPARTMENTS;", sqlConnection);
            SqlDataAdapter adap = new SqlDataAdapter(select_department);
            DataTable deptable = new DataTable();
            DataTable deptable2 = new DataTable();
            adap.Fill(deptable);
            adap.Fill(deptable2);
            comboBox3.DataSource = deptable;
            comboBox3.DisplayMember = "DepartmentName";
            comboBox3.ValueMember = "DepartmentID";
            comboBox3.SelectedIndex = -1;
            comboBox4.DataSource = deptable2;
            comboBox4.DisplayMember = "DepartmentName";
            comboBox4.ValueMember = "DepartmentID";
            comboBox4.SelectedIndex = -1;
        }

        private void Add_employee_Click(object sender, EventArgs e)
        {
            string name = textBoxName.Text;
            string date = dateTimePicker.Text;
            string phone = textBoxPhone.Text;
            string address = textBoxAddress.Text;
            int id = 0;
            int position_ID = Convert.ToInt32(comboBox1.SelectedValue);
            int department_ID = Convert.ToInt32(comboBox3.SelectedValue);
            int supervisor_ID = Convert.ToInt32(comboBox2.SelectedValue);

            if ((textBoxName.Text == string.Empty) || (textBoxPhone.Text == string.Empty) || (textBoxAddress.Text == string.Empty) || string.IsNullOrEmpty(comboBox1.Text) || string.IsNullOrEmpty(comboBox3.Text))
            {
                MessageBox.Show("Заполните данные о сотруднике");
                return;
            }

            string query = "INSERT INTO PERSONAL_DATA(personName,birthday,phone,address) OUTPUT Inserted.personID VALUES(N'" + name + "','" + date + "','" + phone + "','" + address + "');";
            SqlCommand comand = new SqlCommand(query, sqlConnection);
            id = (Int32)comand.ExecuteScalar();

            /* string find_id = "SELECT personID FROM PERSONAL_DATA WHERE PERSONAL_DATA.personName='" + name + "'AND PERSONAL_DATA.phone='"+phone+ "' AND PERSONAL_DATA.address='"+address+"';";
            SqlCommand command1 = new SqlCommand(find_id, sqlConnection);
            SqlDataReader reader = command1.ExecuteReader();
            reader.Read();
            id = Convert.ToInt32(reader[0].ToString());
            reader.Close();*/

            //string now;
            string query2 = "INSERT INTO EMPLOYEES (employeeID, position, department,supervisor, adding) VALUES(" + id + "," + position_ID + "," + department_ID + "," + supervisor_ID + ",GETDATE());";
            string query3 = "INSERT INTO EMPLOYEES (employeeID, position, department,supervisor, adding) VALUES(" + id + "," + position_ID + "," + department_ID + ",NULL,GETDATE());";
            if (supervisor_ID == 0)
            {
                SqlCommand command2 = new SqlCommand(query3, sqlConnection);
                command2.ExecuteNonQuery();
                //now = command2.ExecuteScalar().ToString();
                //SqlCommand insert_data = new SqlCommand("INSERT INTO ", sqlConnection);
            }
            else
            {
                SqlCommand command2 = new SqlCommand(query2, sqlConnection);
                command2.ExecuteNonQuery();
                //now = command2.ExecuteScalar().ToString();

            }
            //MessageBox.Show("name-" + name + "  date-" + date + "  phone-" + phone + "  ad-" + address + " pos-" + position_ID + "  dep-" + department_ID + "  sup-" + supervisor_ID);
            MessageBox.Show("Сотрудник добавлен, перейдите к списку сотрудников");
            Reload_data();
        }

        private void textBoxPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!Char.IsDigit(number) && number != 8 && number != 45 && number != 43 && number != 41 && number != 40) // цифры и клавиша BackSpace
            {
                e.Handled = true;
            }

        }

        private void textBoxName_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if ((c < 'А' || c > 'я') && c != 8 && (c <= 64 || c >= 91) && (c <= 96 || c >= 123) && c!=32 && c == 17)
            {
                e.Handled = true;
            }
        }

        private void SelectDepartmentEmployees(int selected)
        {
            string query = "SELECT p.employeeID, pr.personName, pr.birthday, pr.phone, pr.address, positionName, departmentName, s.personName,p.adding FROM EMPLOYEES p LEFT JOIN PERSONAL_DATA s ON p.supervisor=s.personID LEFT JOIN POSITIONS ON p.position=POSITIONS.PositionID LEFT JOIN DEPARTMENTS ON p.department=DEPARTMENTS.DepartmentID LEFT JOIN PERSONAL_DATA pr ON p.employeeID=pr.personID WHERE p.department = " + selected + ";";
            SqlCommand command = new SqlCommand(query, sqlConnection);
            SqlDataReader reader = command.ExecuteReader();
            List<string[]> data = new List<string[]>();
            dataGridView1.Rows.Clear();
            while (reader.Read())
            {
                data.Add(new string[9]);
                for (int i = 0; i < dataGridView1.ColumnCount; i++)
                    data[data.Count - 1][i] = reader[i].ToString();
            }
            reader.Close();

            foreach (string[] s in data)
                dataGridView1.Rows.Add(s);
        }

        private void SelectSubordinateEmployee(int selectedEmployee)
        {
            string query = "SELECT p.employeeID, pr.personName, pr.birthday, pr.phone, pr.address, positionName, departmentName, s.personName,p.adding FROM EMPLOYEES p LEFT JOIN PERSONAL_DATA s ON p.supervisor=s.personID LEFT JOIN POSITIONS ON p.position=POSITIONS.PositionID LEFT JOIN DEPARTMENTS ON p.department=DEPARTMENTS.DepartmentID LEFT JOIN PERSONAL_DATA pr ON p.employeeID=pr.personID WHERE p.supervisor = " + selectedEmployee + ";";
            SqlCommand command = new SqlCommand(query, sqlConnection);
            SqlDataReader reader = command.ExecuteReader();
            List<string[]> data = new List<string[]>();
            if (reader.HasRows) // если есть данные
            {
                while (reader.Read())
                {
                    data.Add(new string[9]);
                    for (int i = 0; i < dataGridView1.ColumnCount; i++)
                        data[data.Count - 1][i] = reader[i].ToString();

                }
                reader.Close();

                dataGridView1.Rows.Clear();
                foreach (string[] s in data)
                    dataGridView1.Rows.Add(s);
            }
            else
            {
                MessageBox.Show("У сотрудника нет подчинённых");
                reader.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Reload_data();
        }

        private void SubordinateEmployee_Click(object sender, EventArgs e)
        {
            int selected = Convert.ToInt32(comboBox5.SelectedValue);
            SelectSubordinateEmployee(selected);
        }

        private void DepartmentEmployees_Click(object sender, EventArgs e)
        {
            int selected = Convert.ToInt32(comboBox4.SelectedValue);
            SelectDepartmentEmployees(selected);
        }
    }
}
