Imports System.Data.Odbc
Imports System.Windows.Forms.VisualStyles

Public Class FrmCustomerLst
    Private connection As OdbcConnection
    Private pageSize As Integer = 10
    Private currentPage As Integer = 1
    Private totalRows As Integer = 0
    Private totalPages As Integer = 1

    Private Role As String
    Public Sub New(userRole As String)
        InitializeComponent()
        Me.Role = userRole
    End Sub

    Private Sub FrmCustomerLst_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim connStr As String = "DSN=mysql_cms_vbnet3.5;Uid=root;Pwd=;"
        connection = New OdbcConnection(connStr)
        LoadCustomerList()

        'Role-based button visibility
        If Role = "Admin" Then
            btnAdd.Visible = True
            dgvCustomers.Columns("action").Visible = True
        Else
            btnAdd.Visible = False
            dgvCustomers.Columns("action").Visible = False
        End If

        'set background color and border for Header of DataGridView
        With dgvCustomers
            .EnableHeadersVisualStyles = False
            .ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray
            .ColumnHeadersDefaultCellStyle.ForeColor = Color.Black
            .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 10, FontStyle.Bold)
        End With
    End Sub

    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        currentPage = 1
        LoadCustomerList()
    End Sub

    Private Sub btnPrev_Click(sender As Object, e As EventArgs) Handles btnPrev.Click
        If currentPage > 1 Then
            currentPage -= 1
            LoadCustomerList()
        End If
    End Sub

    Private Sub btnNext_Click(sender As Object, e As EventArgs) Handles btnNext.Click
        If currentPage < totalPages Then
            currentPage += 1
            LoadCustomerList()
        End If
    End Sub

    Private Sub LoadCustomerList()
        If dgvCustomers.Columns.Count > 0 Then
            dgvCustomers.Columns.Clear()
        End If
        'Set up DataGridView columns
        Dim colId As New DataGridViewTextBoxColumn()
        colId.Name = "customer_id"
        colId.HeaderText = "ID"
        colId.DataPropertyName = "customer_id"
        dgvCustomers.Columns.Add(colId)

        Dim colName As New DataGridViewTextBoxColumn()
        colName.Name = "customer_name"
        colName.HeaderText = "Name"
        colName.DataPropertyName = "customer_name"
        dgvCustomers.Columns.Add(colName)

        Dim colAddress As New DataGridViewTextBoxColumn()
        colAddress.Name = "customer_address"
        colAddress.HeaderText = "Address"
        colAddress.DataPropertyName = "customer_address"
        dgvCustomers.Columns.Add(colAddress)

        Dim colPhone As New DataGridViewTextBoxColumn()
        colPhone.Name = "customer_phone"
        colPhone.HeaderText = "Phone"
        colPhone.DataPropertyName = "customer_phone"
        dgvCustomers.Columns.Add(colPhone)

        Dim colGender As New DataGridViewTextBoxColumn()
        colGender.Name = "customer_gender"
        colGender.HeaderText = "Gender"
        colGender.DataPropertyName = "customer_gender"
        dgvCustomers.Columns.Add(colGender)

        'icon edit column
        Dim colIcon As New DataGridViewImageColumn()
        colIcon.Name = "action"
        colIcon.HeaderText = ""
        colIcon.Image = My.Resources.Icon_Edit
        dgvCustomers.Columns.Add(colIcon)

        Dim name As String = txtName.Text.Trim()
        Dim address As String = txtAddress.Text.Trim()
        Dim phone As String = txtPhone.Text.Trim()
        Dim gender As String = cbxGender.Text.Trim()
        Dim sql As String = "SELECT SQL_CALC_FOUND_ROWS customer_id, customer_name, customer_address, customer_phone, customer_gender FROM customers WHERE 1=1"
        If name <> "" Then
            sql &= " AND customer_name LIKE ?"
        End If
        If address <> "" Then
            sql &= " AND customer_address LIKE ?"
        End If
        If phone <> "" Then
            sql &= " AND customer_phone LIKE ?"
        End If
        If gender <> "" Then
            sql &= " AND customer_gender = ?"
        End If
        sql &= " LIMIT ? OFFSET ?"
        Try
            connection.Open()
            Using cmd As New OdbcCommand(sql, connection)
                If name <> "" Then cmd.Parameters.AddWithValue("@customer_name", "%" & name & "%")
                If address <> "" Then cmd.Parameters.AddWithValue("@customer_address", "%" & address & "%")
                If phone <> "" Then cmd.Parameters.AddWithValue("@customer_phone", "%" & phone & "%")
                If gender <> "" Then cmd.Parameters.AddWithValue("@customer_gender", gender)
                cmd.Parameters.AddWithValue("@limit", pageSize)
                cmd.Parameters.AddWithValue("@offset", (currentPage - 1) * pageSize)
                Using reader = cmd.ExecuteReader()
                    Dim dt As New DataTable()
                    dt.Load(reader)
                    dgvCustomers.DataSource = dt
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


            'Role-based button visibility
            If Role = "Admin" Then
                dgvCustomers.Columns("action").Visible = True
            Else
                dgvCustomers.Columns("action").Visible = False
            End If
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        Finally
            connection.Close()
        End Try
    End Sub

    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        txtAddress.Clear()
        txtName.Clear()
        txtPhone.Clear()
        cbxGender.SelectedIndex = -1
    End Sub

    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        Dim frmDetail As New FrmCustomerDtl()
        frmDetail.ShowDialog(Me)
    End Sub


    'Set the cursor effect
    Private Sub dgvCustomers_CellMouseMove(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgvCustomers.CellMouseMove
        If e.ColumnIndex >= 0 AndAlso dgvCustomers.Columns(e.ColumnIndex).Name = "action" Then
            dgvCustomers.Cursor = Cursors.Hand
        Else
            dgvCustomers.Cursor = Cursors.Default
        End If
    End Sub

    Private Sub dgvCustomers_CellMouseLeave(sender As Object, e As DataGridViewCellEventArgs) Handles dgvCustomers.CellMouseLeave
        dgvCustomers.Cursor = Cursors.Default
    End Sub

    'click on Edit icon 
    Private Sub dgvCustomers_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvCustomers.CellContentClick
        If e.ColumnIndex >= 0 AndAlso dgvCustomers.Columns(e.ColumnIndex).Name = "action" AndAlso e.RowIndex >= 0 Then
            ' Get customer_id from  clicked row
            Dim customerId As Integer = Convert.ToInt32(dgvCustomers.Rows(e.RowIndex).Cells("customer_id").Value)
            ' Open form Customer Detail and pass customerId parameter
            Dim frmDetail As New FrmCustomerDtl()
            frmDetail.Show()
            frmDetail.LoadCustomer(customerId)
            ' MessageBox.Show("Edit customer with ID: " & customerId)
        End If
    End Sub

End Class
