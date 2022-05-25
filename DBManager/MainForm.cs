using System.Data;
using System.Data.SqlClient;

namespace DBManager
{
    public partial class MainForm : Form
    {
        private SqlConnection DBConnection;
        private DBConfiguration Configuration;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                Configuration = new DBConfiguration();

                if (!Configuration.IsValid())
                {
                    throw new Exception("Configuration is not valid");
                }

                DBConnection = new SqlConnection(Configuration.ConnectionString);
                DBConnection.Open();
                MessageBox.Show($"Connection opened");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error occurred: {ex.Message}");
                Application.Exit();
            }
        }

        private void MainForm_FormClosing(Object sender, FormClosingEventArgs e)
        {
            if (DBConnection?.State == ConnectionState.Open)
            {
                DBConnection.Close();
            }
            DBConnection?.Dispose();
            MessageBox.Show("Closing program");
        }

        private void btnGetUsers_Click(object sender, EventArgs e)
        {
            if (DBConnection == null || DBConnection.State != ConnectionState.Open)
            {
                MessageBox.Show("Database connection is not opened!");
                Application.Exit();
            }

            try
            {
                GetAllUsers();
                MessageBox.Show($"Result count: {dataGridView1.Rows.Count}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error occurred: {ex.Message}");
            }
        }

        private void GetAllUsers()
        {
            string getAllCommand = $"SELECT * FROM [{Configuration.TableName}]";
            SqlDataAdapter dataAdapter = new SqlDataAdapter(getAllCommand, DBConnection);
            DataTable dt = new DataTable();
            dataAdapter.Fill(dt);
            dataGridView1.DataSource = dt;
        }

        private User? GetUserById(int id)
        {
            string getCommand = $"SELECT * FROM [{Configuration.TableName}] WHERE [ID] = {id}";
            SqlDataAdapter dataAdapter = new SqlDataAdapter(getCommand, DBConnection);
            DataTable dt = new DataTable();
            dataAdapter.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                try
                {
                    return new User
                    {
                        ID = Convert.ToInt32(dt.Rows[0].ItemArray[0]),
                        Login = dt.Rows[0].ItemArray[1].ToString(),
                        Password = dt.Rows[0].ItemArray[2].ToString(),
                        RegisterDate = Convert.ToDateTime(dt.Rows[0].ItemArray[3]),
                        Age = Int32.TryParse(dt.Rows[0].ItemArray[4].ToString(), out int age) ? age : null,
                    };
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error while reading data from DB. Details: {ex.Message}");
                }
            }

            return null;
        }

        private void btnAddUser_Click(object sender, EventArgs e)
        {
            if (DBConnection == null || DBConnection.State != ConnectionState.Open)
            {
                MessageBox.Show("Database connection is not opened!");
                Application.Exit();
            }

            try
            {
                User? newUser = Prompt.AddOrEditUserDialog("Create user");

                if (newUser == null)
                {
                    MessageBox.Show("Cannot create user");
                    return;
                }

                string insertCommand = $"INSERT INTO [{Configuration.TableName}]([{Configuration.LoginColumnName}],[{Configuration.PasswordColumnName}],[{Configuration.RegisterDateColumnName}],[{Configuration.AgeColumnName}]) VALUES(@login,@password,@registerDate,@age)";

                using (SqlCommand cmd = new SqlCommand(insertCommand, DBConnection))
                {
                    cmd.Parameters.Add("@login", SqlDbType.NVarChar, 50).Value = !string.IsNullOrEmpty(newUser.Login) ? newUser.Login : DBNull.Value;
                    cmd.Parameters.Add("@password", SqlDbType.VarChar, 50).Value = !string.IsNullOrEmpty(newUser.Password) ? newUser.Password : DBNull.Value;
                    cmd.Parameters.Add("@registerDate", SqlDbType.DateTime).Value = newUser.RegisterDate;
                    cmd.Parameters.Add("@age", SqlDbType.Int).Value = newUser.Age.HasValue ? newUser.Age.Value : DBNull.Value;
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("New user created!");
                GetAllUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error occurred: {ex.Message}");
            }
        }

        private void btnDeleteUser_Click(object sender, EventArgs e)
        {
            if (DBConnection == null || DBConnection.State != ConnectionState.Open)
            {
                MessageBox.Show("Database connection is not opened!");
                Application.Exit();
            }

            try
            {
                int idToDelete = Prompt.GetUserByIdDialog("Delete user");
                User? userToDelete = GetUserById(idToDelete);

                if (userToDelete == null)
                {
                    MessageBox.Show($"Cannot find user with id = {idToDelete}");
                    return;
                }

                string deleteCommand = $"DELETE FROM [{Configuration.TableName}] WHERE [ID] = @id";

                using (SqlCommand cmd = new SqlCommand(deleteCommand, DBConnection))
                {
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = userToDelete.ID;
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("User deleted!");
                GetAllUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error occurred: {ex.Message}");
            }
        }

        private void btnEditUser_Click(object sender, EventArgs e)
        {
            if (DBConnection == null || DBConnection.State != ConnectionState.Open)
            {
                MessageBox.Show("Database connection is not opened!");
                Application.Exit();
            }

            try
            {
                int idToEdit = Prompt.GetUserByIdDialog("Edit user");
                User? userToEdit = GetUserById(idToEdit);

                if (userToEdit == null)
                {
                    MessageBox.Show($"Cannot find user with id = {idToEdit}");
                    return;
                }

                User? editedUser = Prompt.AddOrEditUserDialog("Edit user", userToEdit);

                if (editedUser == null)
                {
                    MessageBox.Show("Cannot edit user");
                    return;
                }

                string updateCommand = $"UPDATE [{Configuration.TableName}] SET [{Configuration.LoginColumnName}] = @login, [{Configuration.PasswordColumnName}] = @password, [{Configuration.AgeColumnName}] = @age WHERE [ID] = @id";

                using (SqlCommand cmd = new SqlCommand(updateCommand, DBConnection))
                {
                    cmd.Parameters.Add("@login", SqlDbType.NVarChar, 50).Value = !string.IsNullOrEmpty(editedUser.Login) ? editedUser.Login : DBNull.Value;
                    cmd.Parameters.Add("@password", SqlDbType.VarChar, 50).Value = !string.IsNullOrEmpty(editedUser.Password) ? editedUser.Password : DBNull.Value;
                    cmd.Parameters.Add("@age", SqlDbType.Int).Value = editedUser.Age.HasValue ? editedUser.Age.Value : DBNull.Value;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = userToEdit.ID;
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("User updated!");
                GetAllUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error occurred: {ex.Message}");
            }
        }
    }
}