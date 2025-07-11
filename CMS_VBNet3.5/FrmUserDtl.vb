Imports System.Data.Odbc
Imports System.Net
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel

Public Class FrmUserDtl
    Private connection As OdbcConnection
    Private isEditMode As Boolean = False
    Private currentUserId As Integer = -1
    Private connStr As String = "DSN=mysql_cms_vbnet3.5;Uid=root;Pwd=;"

    Private Sub FrmUserDtl_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        connection = New OdbcConnection(connStr)
        cbxRole.SelectedIndex = 0
        ClearForm()
    End Sub

    Public Sub LoadUser(userId As Integer)
        isEditMode = True
        currentUserId = userId
        Try
            connection.Open()
            Using cmd As New OdbcCommand("SELECT user_id, user_name, user_password, user_mail, user_role FROM users WHERE user_id = ?", connection)
                cmd.Parameters.AddWithValue("@user_id", userId)
                Using reader = cmd.ExecuteReader()
                    If reader.Read() Then
                        txtUserId.Text = reader("user_id").ToString()
                        txtUsrNm.Text = reader("user_name").ToString()
                        txtEmail.Text = reader("user_mail").ToString()
                        'txtPwd.Text = reader("user_Pass").ToString()
                        cbxRole.Text = reader("user_role").ToString()
                    Else
                        MessageBox.Show("User not found.")
                        'ClearForm()
                        'Return
                    End If
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        Finally
            connection.Close()
        End Try
        txtUserId.ReadOnly = True
        btnAdd.Enabled = False
        btnUpdate.Enabled = True
        btnDelete.Enabled = True
    End Sub

    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        'Check Username, Password, Mail, Role: Null
        If txtUsrNm.Text.Trim() = "" Or txtPwd.Text.Trim() = "" Or txtEmail.Text.Trim() = "" Or txtConfirm.Text.Trim() = "" Or cbxRole.Text.Trim() = "" Then
            MessageBox.Show("Please enter name, password, email and role.")
            Return
        Else
            'Check User unique
            If chkUser(txtUsrNm.Text.Trim()) Then
                MessageBox.Show("Username already exists. Please choose a different name.")
                Return
            End If

            'Check Length Pass < 6
            If txtPwd.Text.Trim().Length < 6 Then
                MessageBox.Show("Password must contain at least 6 characters.")
                Return
            Else

            End If
            'Check Mail type
            If Not IsValidEmail(txtEmail.Text.Trim()) Then
                MessageBox.Show("EInvalid email address.")
                Return
            End If
            'Check Password and Confirm Password
            If txtPwd.Text.Trim() <> txtConfirm.Text.Trim() Then
                MessageBox.Show("Password and Confirm Password do not match.")
                Return
            End If
        End If
        Dim hashedPassword As String = GetMD5(txtPwd.Text.Trim())
        Try
            connection.Open()
            Using cmd As New OdbcCommand("INSERT INTO users (user_name, user_password, user_mail, user_role) VALUES (?, ?, ?, ?)", connection)
                cmd.Parameters.AddWithValue("@user_name", txtUsrNm.Text.Trim())
                cmd.Parameters.AddWithValue("@user_password", hashedPassword)
                cmd.Parameters.AddWithValue("@user_mail", txtEmail.Text.Trim())
                cmd.Parameters.AddWithValue("@user_role", cbxRole.Text.Trim())
                cmd.ExecuteNonQuery()
            End Using
            MessageBox.Show("User added successfully.")
            ClearForm()
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        Finally
            connection.Close()
        End Try
    End Sub

    Private Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        Dim hashedPassword As String = GetMD5(txtPwd.Text.Trim())
        If currentUserId = -1 Then Return
        Try
            connection.Open()
            Using cmd As New OdbcCommand("UPDATE customers SET user_name=?, user_password=?, user_mail=?, user_role=? WHERE user_id=?", connection)
                cmd.Parameters.AddWithValue("@user_name", txtUsrNm.Text.Trim())
                cmd.Parameters.AddWithValue("@user_password", hashedPassword)
                cmd.Parameters.AddWithValue("@user_mail", txtEmail.Text.Trim())
                cmd.Parameters.AddWithValue("@user_role", cbxRole.Text.Trim())
                cmd.Parameters.AddWithValue("@user_id", currentUserId)
                cmd.ExecuteNonQuery()
            End Using
            MessageBox.Show("Customer updated successfully.")
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        Finally
            connection.Close()
        End Try
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        If currentUserId = -1 Then Return
        If MessageBox.Show("Are you sure to delete this user?", "Confirm", MessageBoxButtons.YesNo) = DialogResult.Yes Then
            Try
                connection.Open()
                Using cmd As New OdbcCommand("DELETE FROM users WHERE user_id=?", connection)
                    cmd.Parameters.AddWithValue("@user_id", currentUserId)
                    cmd.ExecuteNonQuery()
                End Using
                MessageBox.Show("Customer deleted successfully.")
                ClearForm()
            Catch ex As Exception
                MessageBox.Show("Error: " & ex.Message)
            Finally
                connection.Close()
            End Try
        End If
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub ClearForm()
        txtUserId.Text = ""
        txtUsrNm.Text = ""
        txtEmail.Text = ""
        txtPwd.Text = ""
        txtConfirm.Text = ""
        cbxRole.SelectedIndex = 0
        txtUserId.ReadOnly = True
        btnAdd.Enabled = True
        btnUpdate.Enabled = False
        btnDelete.Enabled = False
        isEditMode = False
        currentUserId = -1
    End Sub

    Private Function chkUser(ByVal username As String) As Boolean
        Try
            connection.Open()
            Dim sql As String = "SELECT COUNT(*) FROM users WHERE user_name = ?"

            Using dml As New OdbcCommand(sql, connection)
                dml.Parameters.AddWithValue("@user_name", username)
                Dim count As Integer = CInt(dml.ExecuteScalar())
                Return count > 0
            End Using
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
            Return False

        Finally
            connection.Close()
        End Try
    End Function

    Public Function IsValidEmail(ByVal email As String) As Boolean
        Dim pattern As String = "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"
        Dim regex As New Regex(pattern)
        Return regex.IsMatch(email)
    End Function

    Public Function GetMD5(ByVal str As String) As String
        Dim md5 As MD5 = MD5.Create()
        Dim inputBytes As Byte() = Encoding.ASCII.GetBytes(str)
        Dim hashBytes As Byte() = md5.ComputeHash(inputBytes)

        Dim sb As New StringBuilder()
        For i As Integer = 0 To hashBytes.Length - 1
            sb.Append(hashBytes(i).ToString("x2"))
        Next

        Return sb.ToString()
    End Function

End Class
