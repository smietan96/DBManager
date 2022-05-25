namespace DBManager
{
    internal static class Prompt
    {
        internal static User? AddOrEditUserDialog(string caption, User? userToEdit = null)
        {
            Form prompt = new Form()
            {
                Width = 500,
                Height = 500,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };

            Label loginLbl = new Label() { Left = 10, Top = 20, Text = "Login" };
            TextBox loginTb = new TextBox() { Left = 120, Top = 20, Width = 300 };
            if (!string.IsNullOrEmpty(userToEdit?.Login)) { loginTb.Text = userToEdit.Login; }

            Label pwdLbl = new Label() { Left = 10, Top = 70, Text = "Password" };
            TextBox pwdTb = new TextBox() { Left = 120, Top = 70, Width = 300 };
            if (!string.IsNullOrEmpty(userToEdit?.Password)) { pwdTb.Text = userToEdit.Password; }

            Label ageLbl = new Label() { Left = 10, Top = 120, Text = "Age" };
            NumericUpDown ageNum = new NumericUpDown() { Left = 120, Top = 120, Width = 300 };
            if (userToEdit?.Age.HasValue == true) { ageNum.Value = userToEdit.Age.Value; }

            Button confirmation = new Button() { Left = 10, Top = 170, Width = 50, Text = "Ok", DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };

            prompt.Controls.AddRange(new Control[] { loginLbl, loginTb, pwdLbl, pwdTb, ageLbl, ageNum, confirmation });
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? new User 
            { 
                Login = loginTb.Text, 
                Password = pwdTb.Text, 
                RegisterDate = userToEdit != null ? userToEdit.RegisterDate : DateTime.Now, 
                Age = ageNum.Value == 0 ? null : Convert.ToInt32(ageNum.Value) 
            } : null;
        }

        internal static int GetUserByIdDialog(string caption)
        {
            Form prompt = new Form()
            {
                Width = 300,
                Height = 200,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };

            Label idLbl = new Label() { Left = 10, Top = 20, Text = "User ID" };
            NumericUpDown idNum = new NumericUpDown() { Left = 120, Top = 20, Width = 30 };

            Button confirmation = new Button() { Left = 10, Top = 120, Width = 50, Text = "Ok", DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };

            prompt.Controls.AddRange(new Control[] { idLbl, idNum, confirmation });
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? (Int32.TryParse(idNum.Value.ToString(), out int id) ? id : 0) : 0;
        }
    }
}
