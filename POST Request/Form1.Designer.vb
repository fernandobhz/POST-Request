<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.URL = New System.Windows.Forms.TextBox()
        Me.POSTDATA = New System.Windows.Forms.TextBox()
        Me.RESPONSE = New System.Windows.Forms.TextBox()
        Me.SEND = New System.Windows.Forms.Button()
        Me.BUILD_REQUEST = New System.Windows.Forms.Button()
        Me.MLS_REQUEST = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'URL
        '
        Me.URL.Location = New System.Drawing.Point(12, 12)
        Me.URL.Name = "URL"
        Me.URL.Size = New System.Drawing.Size(693, 20)
        Me.URL.TabIndex = 0
        '
        'POSTDATA
        '
        Me.POSTDATA.Location = New System.Drawing.Point(12, 38)
        Me.POSTDATA.Multiline = True
        Me.POSTDATA.Name = "POSTDATA"
        Me.POSTDATA.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.POSTDATA.Size = New System.Drawing.Size(693, 272)
        Me.POSTDATA.TabIndex = 1
        '
        'RESPONSE
        '
        Me.RESPONSE.Location = New System.Drawing.Point(12, 316)
        Me.RESPONSE.Multiline = True
        Me.RESPONSE.Name = "RESPONSE"
        Me.RESPONSE.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.RESPONSE.Size = New System.Drawing.Size(693, 158)
        Me.RESPONSE.TabIndex = 2
        '
        'SEND
        '
        Me.SEND.Location = New System.Drawing.Point(628, 480)
        Me.SEND.Name = "SEND"
        Me.SEND.Size = New System.Drawing.Size(75, 23)
        Me.SEND.TabIndex = 3
        Me.SEND.Text = "SEND"
        Me.SEND.UseVisualStyleBackColor = True
        '
        'BUILD_REQUEST
        '
        Me.BUILD_REQUEST.Location = New System.Drawing.Point(12, 480)
        Me.BUILD_REQUEST.Name = "BUILD_REQUEST"
        Me.BUILD_REQUEST.Size = New System.Drawing.Size(190, 23)
        Me.BUILD_REQUEST.TabIndex = 4
        Me.BUILD_REQUEST.Text = "Build Request To Google"
        Me.BUILD_REQUEST.UseVisualStyleBackColor = True
        '
        'MLS_REQUEST
        '
        Me.MLS_REQUEST.Location = New System.Drawing.Point(208, 480)
        Me.MLS_REQUEST.Name = "MLS_REQUEST"
        Me.MLS_REQUEST.Size = New System.Drawing.Size(190, 23)
        Me.MLS_REQUEST.TabIndex = 5
        Me.MLS_REQUEST.Text = "Build Request To Mozilla"
        Me.MLS_REQUEST.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(715, 507)
        Me.Controls.Add(Me.MLS_REQUEST)
        Me.Controls.Add(Me.BUILD_REQUEST)
        Me.Controls.Add(Me.SEND)
        Me.Controls.Add(Me.RESPONSE)
        Me.Controls.Add(Me.POSTDATA)
        Me.Controls.Add(Me.URL)
        Me.Name = "Form1"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "POST Request"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents URL As System.Windows.Forms.TextBox
    Friend WithEvents POSTDATA As System.Windows.Forms.TextBox
    Friend WithEvents RESPONSE As System.Windows.Forms.TextBox
    Friend WithEvents SEND As System.Windows.Forms.Button
    Friend WithEvents BUILD_REQUEST As System.Windows.Forms.Button
    Friend WithEvents MLS_REQUEST As System.Windows.Forms.Button

End Class
