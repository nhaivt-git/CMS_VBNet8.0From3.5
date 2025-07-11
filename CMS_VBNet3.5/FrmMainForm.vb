Public Class FrmMainForm
    Public UserRole As String

    Private Sub LoadFormToContent(form As Form)
        pnlContent.Controls.Clear()
        form.TopLevel = False
        form.FormBorderStyle = FormBorderStyle.None
        form.Dock = DockStyle.Fill
        pnlContent.Controls.Add(form)
        form.Show()
    End Sub

    Private Sub mnUserList_Click(sender As Object, e As EventArgs) Handles mnUserList.Click
        LoadFormToContent(New FrmUserList(UserRole))
    End Sub

    Private Sub mnCustomerList_Click(sender As Object, e As EventArgs) Handles mnCustomerList.Click
        LoadFormToContent(New FrmCustomerLst(UserRole))
    End Sub

    Private Sub FrmMainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        MenuStrip1.BackColor = pnlMenu.BackColor
        MenuStrip1.RenderMode = ToolStripRenderMode.System
        lblUserName.Text = "Welcome, " & Environment.UserName
        'show the customer list by default
        Call mnCustomerList_Click(Nothing, EventArgs.Empty)

        'Role-based menu visibility
        If UserRole = "Admin" Then
            mnUserList.Visible = True
        Else
            mnUserList.Visible = False
        End If

    End Sub

    Private Sub lkLogout_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles lkLogout.LinkClicked
        Dim frmLogin As New FrmLogin()
        Me.Hide()
        frmLogin.ShowDialog()
        Me.Close()
    End Sub

End Class