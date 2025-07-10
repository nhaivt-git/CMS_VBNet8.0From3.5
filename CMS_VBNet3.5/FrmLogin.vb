
Imports System.Data.Common
Imports System.Data.Odbc
Imports System.Security.Cryptography
Imports System.Text


Public Class FrmLogin

    Private connection As OdbcConnection

    Private Sub connect_db()
        Dim connStr As String = "DSN=mysql_cms_vbnet3.5;Uid=root;Pwd=;"
        connection = New OdbcConnection(connStr)
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


    Private Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
        Dim username As String = txtUsrNm.Text.Trim()
        Dim password As String = txtPwd.Text.Trim()
        Dim hashedPassword As String = GetMD5(password)
        Try
            connection.Open()
            Dim sql As String = "SELECT COUNT(*) FROM users WHERE user_name = ? AND user_password = ?"

            Using dml As New OdbcCommand(sql, connection)

                dml.Parameters.AddWithValue("@user_name", username)
                dml.Parameters.AddWithValue("@user_password", hashedPassword)

                Dim count As Integer = CInt(dml.ExecuteScalar())
                If count > 0 Then
                    MessageBox.Show("Login successful!")
                    Dim frm As New FrmMainForm()
                    Me.Hide()
                    frm.ShowDialog()
                    Me.Close()
                Else
                    MessageBox.Show("Invalid username or password.")
                End If

            End Using
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        Finally
            connection.Close()
        End Try

    End Sub
    Private Sub Login_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        connect_db()
    End Sub

    Private Sub linkRegister_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles lkRegister.LinkClicked
        Dim frmRegister As New FrmRegister()
        frmRegister.Owner = Me
        frmRegister.ShowDialog()
        Me.Hide()

    End Sub
End Class