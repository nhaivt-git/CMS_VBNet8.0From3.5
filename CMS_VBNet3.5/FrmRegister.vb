Imports System.Data.Odbc
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.RegularExpressions

Public Class FrmRegister

    Private connection As OdbcConnection

    Private Sub connect_db()
        Dim connStr As String = "DSN=mysql_cms_vbnet3.5;Uid=root;Pwd=;"
        connection = New OdbcConnection(connStr)
    End Sub

    Private Sub actClear()
        txtUsrNm.Clear()
        txtPwd.Clear()
        txtConfirm.Clear()
        txtEmail.Clear()
        cbxRole.SelectedIndex = -1
    End Sub

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

    Private Sub btnRegister_Click(sender As Object, e As EventArgs) Handles btnRegister.Click
        Dim username As String = txtUsrNm.Text.Trim()
        Dim password As String = txtPwd.Text.Trim()
        Dim mail As String = txtEmail.Text.Trim()
        Dim role As String = cbxRole.Text.Trim()

        'Check Username, Password, Mail, Role: Null
        If String.IsNullOrEmpty(username) Or String.IsNullOrEmpty(password) Or String.IsNullOrEmpty(mail) Or String.IsNullOrEmpty(role) Then
            MessageBox.Show("Username, Password, E-mail, Role is not null.")
            Return
        Else
            'Check User unique
            If chkUser(username) Then
                MessageBox.Show("Username already exists. Please choose a different name.")
                Return
            End If

            'Check Length Pass < 6
            If password.Length < 6 Then
                MessageBox.Show("Password must contain at least 6 characters.")
                Return
            Else

            End If
            'Check Mail type
            If Not IsValidEmail(mail) Then
                MessageBox.Show("EInvalid email address.")
                Return
            End If
            'Check Password and Confirm Password
            If password <> txtConfirm.Text.Trim() Then
                MessageBox.Show("Password and Confirm Password do not match.")
                Return
            End If
        End If

        Dim hashedPassword As String = GetMD5(password)
        Try
            connection.Open()
            Dim sql As String = "INSERT INTO users (user_name, user_password, user_mail, user_role) VALUES (?, ?, ?, ?)"

            Using dml As New OdbcCommand(sql, connection)

                dml.Parameters.AddWithValue("@user_name", username)
                dml.Parameters.AddWithValue("@user_password", hashedPassword)
                dml.Parameters.AddWithValue("@user_mail", mail)
                dml.Parameters.AddWithValue("@user_role", role)

                dml.ExecuteNonQuery()
                MessageBox.Show("Register successful!")
                actClear()
                Dim frm As New FrmLogin
                Me.Hide()
                frm.ShowDialog()
                Me.Close()
            End Using
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        Finally
            connection.Close()
        End Try

    End Sub

    Private Sub FrmRegister_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        connect_db()
    End Sub

    Private Sub btnBack_Click(sender As Object, e As EventArgs) Handles btnBack.Click
        Me.Owner.Show()
        Me.Close()
    End Sub
End Class
