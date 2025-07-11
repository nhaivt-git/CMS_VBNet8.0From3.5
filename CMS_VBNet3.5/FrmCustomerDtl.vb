Imports System.Data.Odbc

Public Class FrmCustomerDtl
    Private connection As OdbcConnection
    Private isEditMode As Boolean = False
    Private currentCustomerId As Integer = -1
    Private connStr As String = "DSN=mysql_cms_vbnet3.5;Uid=root;Pwd=;"

    Private Sub FrmCustomerDtl_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        connection = New OdbcConnection(connStr)
        cbxCustomerGender.SelectedIndex = 0
        ClearForm()
    End Sub

    Public Sub LoadCustomer(customerId As Integer)
        isEditMode = True
        currentCustomerId = customerId
        Try
            connection.Open()
            Using cmd As New OdbcCommand("SELECT customer_id, customer_name, customer_address, customer_phone, customer_gender FROM customers WHERE customer_id = ?", connection)
                cmd.Parameters.AddWithValue("@customer_id", customerId)
                Using reader = cmd.ExecuteReader()
                    If reader.Read() Then
                        txtCustomerId.Text = reader("customer_id").ToString()
                        txtCustomerName.Text = reader("customer_name").ToString()
                        txtCustomerAddress.Text = reader("customer_address").ToString()
                        txtCustomerPhone.Text = reader("customer_phone").ToString()
                        cbxCustomerGender.Text = reader("customer_gender").ToString()
                    Else
                        MessageBox.Show("Customer not found.")
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
        txtCustomerId.ReadOnly = True
        btnAdd.Enabled = False
        btnUpdate.Enabled = True
        btnDelete.Enabled = True
    End Sub

    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        If txtCustomerName.Text.Trim() = "" Or txtCustomerAddress.Text.Trim() = "" Then
            MessageBox.Show("Please enter name and address.")
            Return
        End If
        Try
            connection.Open()
            Using cmd As New OdbcCommand("INSERT INTO customers (customer_name, customer_address, customer_phone, customer_gender) VALUES (?, ?, ?, ?)", connection)
                cmd.Parameters.AddWithValue("@customer_name", txtCustomerName.Text.Trim())
                cmd.Parameters.AddWithValue("@customer_address", txtCustomerAddress.Text.Trim())
                cmd.Parameters.AddWithValue("@customer_phone", txtCustomerPhone.Text.Trim())
                cmd.Parameters.AddWithValue("@customer_gender", cbxCustomerGender.Text.Trim())
                cmd.ExecuteNonQuery()
            End Using
            MessageBox.Show("Customer added successfully.")
            ClearForm()
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        Finally
            connection.Close()
        End Try
    End Sub

    Private Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        If currentCustomerId = -1 Then Return
        Try
            connection.Open()
            Using cmd As New OdbcCommand("UPDATE customers SET customer_name=?, customer_address=?, customer_phone=?, customer_gender=? WHERE customer_id=?", connection)
                cmd.Parameters.AddWithValue("@customer_name", txtCustomerName.Text.Trim())
                cmd.Parameters.AddWithValue("@customer_address", txtCustomerAddress.Text.Trim())
                cmd.Parameters.AddWithValue("@customer_phone", txtCustomerPhone.Text.Trim())
                cmd.Parameters.AddWithValue("@customer_gender", cbxCustomerGender.Text.Trim())
                cmd.Parameters.AddWithValue("@customer_id", currentCustomerId)
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
        If currentCustomerId = -1 Then Return
        If MessageBox.Show("Are you sure to delete this customer?", "Confirm", MessageBoxButtons.YesNo) = DialogResult.Yes Then
            Try
                connection.Open()
                Using cmd As New OdbcCommand("DELETE FROM customers WHERE customer_id=?", connection)
                    cmd.Parameters.AddWithValue("@customer_id", currentCustomerId)
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
        txtCustomerId.Text = ""
        txtCustomerName.Text = ""
        txtCustomerAddress.Text = ""
        txtCustomerPhone.Text = ""
        cbxCustomerGender.SelectedIndex = 0
        txtCustomerId.ReadOnly = True
        btnAdd.Enabled = True
        btnUpdate.Enabled = False
        btnDelete.Enabled = False
        isEditMode = False
        currentCustomerId = -1
    End Sub
End Class
