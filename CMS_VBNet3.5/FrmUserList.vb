Imports System.Data.Odbc

Public Class FrmUserList
    Private connection As OdbcConnection
    Private pageSize As Integer = 10
    Private currentPage As Integer = 1
    Private totalRows As Integer = 0
    Private totalPages As Integer = 1

    Private Sub FrmUserList_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim connStr As String = "DSN=mysql_cms_vbnet3.5;Uid=root;Pwd=;"
        connection = New OdbcConnection(connStr)

        dgvUsers.AutoGenerateColumns = False
        LoadUserList()

        'set background color for DataGridView
        With dgvUsers
            .EnableHeadersVisualStyles = False
            .ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray
            .ColumnHeadersDefaultCellStyle.ForeColor = Color.Black
            .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 10, FontStyle.Bold)
        End With
    End Sub

    Private Sub btnPrev_Click(sender As Object, e As EventArgs) Handles btnPrev.Click
        If currentPage > 1 Then
            currentPage -= 1
            LoadUserList()
        End If
    End Sub

    Private Sub btnNext_Click(sender As Object, e As EventArgs) Handles btnNext.Click
        If currentPage < totalPages Then
            currentPage += 1
            LoadUserList()
        End If
    End Sub

    Private Sub LoadUserList()
        'Clear existing columns in the DataGridView
        If dgvUsers.Columns.Count > 0 Then
            dgvUsers.Columns.Clear()
        End If
        'Set up DataGridView columns
        'user_id column
        Dim colId As New DataGridViewTextBoxColumn()
        colId.Name = "user_id"
        colId.HeaderText = "UserID"
        colId.DataPropertyName = "user_id"
        dgvUsers.Columns.Add(colId)

        'user_name column
        Dim colName As New DataGridViewTextBoxColumn()
        colName.Name = "user_name"
        colName.HeaderText = "Username"
        colName.DataPropertyName = "user_name"
        dgvUsers.Columns.Add(colName)

        'user_mail column
        Dim colMail As New DataGridViewTextBoxColumn()
        colMail.Name = "user_mail"
        colMail.HeaderText = "Email"
        colMail.DataPropertyName = "user_mail"
        colMail.Width = 250
        dgvUsers.Columns.Add(colMail)

        'user_role column
        Dim colRole As New DataGridViewTextBoxColumn()
        colRole.Name = "user_role"
        colRole.HeaderText = "Role"
        colRole.DataPropertyName = "user_role"
        dgvUsers.Columns.Add(colRole)

        Dim username As String = txtUsername.Text.Trim()
        Dim email As String = txtEmail.Text.Trim()
        Dim role As String = cbxRole.Text.Trim()
        Dim sql As String = "SELECT SQL_CALC_FOUND_ROWS user_id, user_name, user_mail, user_role FROM users WHERE 1=1"
        If username <> "" Then
            sql &= " AND user_name LIKE ?"
        End If
        If email <> "" Then
            sql &= " AND user_mail LIKE ?"
        End If
        If role <> "" Then
            sql &= " AND user_role LIKE ?"
        End If
        sql &= " LIMIT ? OFFSET ?"
        Try
            connection.Open()
            Using cmd As New OdbcCommand(sql, connection)
                If username <> "" Then cmd.Parameters.AddWithValue("@user_name", "%" & username & "%")
                If email <> "" Then cmd.Parameters.AddWithValue("@user_mail", "%" & email & "%")
                If role <> "" Then cmd.Parameters.AddWithValue("@user_role", "%" & role & "%")
                cmd.Parameters.AddWithValue("@limit", pageSize)
                cmd.Parameters.AddWithValue("@offset", (currentPage - 1) * pageSize)
                Using reader = cmd.ExecuteReader()
                    Dim dt As New DataTable()
                    dt.Load(reader)
                    dgvUsers.DataSource = dt
                End Using
            End Using
            Using cmdCount As New OdbcCommand("SELECT FOUND_ROWS()", connection)
                totalRows = Convert.ToInt32(cmdCount.ExecuteScalar())
            End Using
            totalPages = Math.Ceiling(totalRows / pageSize)
            If totalPages = 0 Then totalPages = 1
            lblPageInfo.Text = "Page " & currentPage & "/" & totalPages
            btnPrev.Enabled = currentPage > 1
            btnNext.Enabled = currentPage < totalPages
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        Finally
            connection.Close()
        End Try
    End Sub

    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        currentPage = 1
        LoadUserList()
    End Sub

    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        txtEmail.Text = ""
        txtUsername.Text = ""
        cbxRole.SelectedIndex = -1
    End Sub

End Class

