' Simple JPEG scratchpad
' Copyright (C) 2006-2009, Aleh Dzenisiuk
' http://dzenisiuk.info/jpegator/

Imports JPEGator

Public Class MainForm
    Inherits System.Windows.Forms.Form
    Friend WithEvents FileMenuItem As System.Windows.Forms.MenuItem
    Friend WithEvents OpenMenuItem As System.Windows.Forms.MenuItem
    Friend WithEvents SaveMenuItem As System.Windows.Forms.MenuItem
    Friend WithEvents ExitMenuItem As System.Windows.Forms.MenuItem
    Friend WithEvents ToolsMenuItem As System.Windows.Forms.MenuItem
    Friend WithEvents AddTextMenuItem As System.Windows.Forms.MenuItem
    Friend WithEvents PencilMenuItem As System.Windows.Forms.MenuItem
    Friend WithEvents MenuItem2 As System.Windows.Forms.MenuItem
    Friend WithEvents MainMenu As System.Windows.Forms.MainMenu

    Private Image As System.Drawing.Bitmap    

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        MyBase.Dispose(disposing)
    End Sub

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents OpenFileDialog As System.Windows.Forms.OpenFileDialog
    Friend WithEvents PictureBox As System.Windows.Forms.PictureBox
    Friend WithEvents TextBox As System.Windows.Forms.TextBox
    Friend WithEvents SaveFileDialog As System.Windows.Forms.SaveFileDialog
    Private Sub InitializeComponent()
        Me.MainMenu = New System.Windows.Forms.MainMenu
        Me.FileMenuItem = New System.Windows.Forms.MenuItem
        Me.OpenMenuItem = New System.Windows.Forms.MenuItem
        Me.SaveMenuItem = New System.Windows.Forms.MenuItem
        Me.MenuItem2 = New System.Windows.Forms.MenuItem
        Me.ExitMenuItem = New System.Windows.Forms.MenuItem
        Me.ToolsMenuItem = New System.Windows.Forms.MenuItem
        Me.AddTextMenuItem = New System.Windows.Forms.MenuItem
        Me.PencilMenuItem = New System.Windows.Forms.MenuItem
        Me.OpenFileDialog = New System.Windows.Forms.OpenFileDialog
        Me.PictureBox = New System.Windows.Forms.PictureBox
        Me.TextBox = New System.Windows.Forms.TextBox
        Me.SaveFileDialog = New System.Windows.Forms.SaveFileDialog
        Me.SuspendLayout()
        '
        'MainMenu
        '
        Me.MainMenu.MenuItems.Add(Me.FileMenuItem)
        Me.MainMenu.MenuItems.Add(Me.ToolsMenuItem)
        '
        'FileMenuItem
        '
        Me.FileMenuItem.MenuItems.Add(Me.OpenMenuItem)
        Me.FileMenuItem.MenuItems.Add(Me.SaveMenuItem)
        Me.FileMenuItem.MenuItems.Add(Me.MenuItem2)
        Me.FileMenuItem.MenuItems.Add(Me.ExitMenuItem)
        Me.FileMenuItem.Text = "File"
        '
        'OpenMenuItem
        '
        Me.OpenMenuItem.Text = "Open..."
        '
        'SaveMenuItem
        '
        Me.SaveMenuItem.Text = "Save as..."
        '
        'MenuItem2
        '
        Me.MenuItem2.Text = "-"
        '
        'ExitMenuItem
        '
        Me.ExitMenuItem.Text = "Exit"
        '
        'ToolsMenuItem
        '
        Me.ToolsMenuItem.MenuItems.Add(Me.AddTextMenuItem)
        Me.ToolsMenuItem.MenuItems.Add(Me.PencilMenuItem)
        Me.ToolsMenuItem.Text = "Tools"
        '
        'AddTextMenuItem
        '
        Me.AddTextMenuItem.Text = "Add text"
        '
        'PencilMenuItem
        '
        Me.PencilMenuItem.Text = "Pencil"
        '
        'OpenFileDialog
        '
        Me.OpenFileDialog.Filter = "JPEG files (*.JPG, *.JPEG)|*.jpg,*.jpeg"
        Me.OpenFileDialog.InitialDirectory = "\My Documents\My Pictures"
        '
        'PictureBox
        '
        Me.PictureBox.Location = New System.Drawing.Point(8, 8)
        Me.PictureBox.Name = "PictureBox"
        Me.PictureBox.Size = New System.Drawing.Size(72, 56)
        '
        'TextBox
        '
        Me.TextBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 16.0!, System.Drawing.FontStyle.Bold)
        Me.TextBox.ForeColor = System.Drawing.Color.DodgerBlue
        Me.TextBox.Location = New System.Drawing.Point(16, 112)
        Me.TextBox.Name = "TextBox"
        Me.TextBox.Size = New System.Drawing.Size(176, 31)
        Me.TextBox.TabIndex = 0
        Me.TextBox.Text = "TextBox"
        Me.TextBox.Visible = False
        '
        'SaveFileDialog
        '
        Me.SaveFileDialog.FileName = "doc1"
        Me.SaveFileDialog.Filter = "JPEG files (*.JPG, *.JPEG)|*.jpg"
        '
        'MainForm
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 268)
        Me.Controls.Add(Me.TextBox)
        Me.Controls.Add(Me.PictureBox)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable
        Me.Menu = Me.MainMenu
        Me.Name = "MainForm"
        Me.Text = "JPEGPad"
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Sub ExitMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitMenuItem.Click
        Close()
    End Sub

    Private Sub OpenMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenMenuItem.Click        

        ' Offer user to select some JPEG file
        Dim Result As DialogResult = OpenFileDialog.ShowDialog()

        ' Load the file if user selected some file and have not cancelled the dialog
        If Result = Windows.Forms.DialogResult.OK Then

            ' TODO: Warn user about unsaved picture here, if any

            Try
                Cursor.Current = Cursors.WaitCursor

                ' Cleanup previously loaded image
                If Not IsNothing(Image) Then
                    PictureBox.Image = Nothing
                    Image.Dispose()
                    Image = Nothing
                    PictureBox.Enabled = False
                    PictureBox.Size = New Size(0, 0)
                End If

                Try
                    ' Load bitmap without any scaling using the JPEGator
                    Image = JPEGator.BitmapUtils.LoadBitmap(OpenFileDialog.FileName)

                    ' Setup PictureBox
                    PictureBox.Image = Image
                    PictureBox.Size = Image.Size
                    PictureBox.Location = New Point(0, 0)
                    PictureBox.Enabled = True

                Finally
                    Cursor.Current = Cursors.Default
                End Try

            Catch ex As Exception

                ' Show exception information, if any
                Windows.Forms.MessageBox.Show( _
                    String.Format("Unable to load file {0} ({1}): {2}", OpenFileDialog.FileName, ex.GetType().Name, ex.Message), _
                    "Error", _
                    MessageBoxButtons.OK, _
                    MessageBoxIcon.Hand, _
                    MessageBoxDefaultButton.Button1 _
                )
            End Try
        End If
    End Sub

    Private Sub PencilMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PencilMenuItem.Click
        ' Only one menu item can be active
        PencilMenuItem.Checked = Not PencilMenuItem.Checked
        AddTextMenuItem.Checked = Not PencilMenuItem.Checked

        Me.Focus()
    End Sub

    Private Sub AddTextMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddTextMenuItem.Click
        ' Only one menu item can be active
        AddTextMenuItem.Checked = Not AddTextMenuItem.Checked
        PencilMenuItem.Checked = Not AddTextMenuItem.Checked

        Me.Focus()
    End Sub

    ' True if user has pressed stylus and is moving it drawing something
    Dim Drawing As Boolean = False

    ' Position of pencil when previous MouseMove was received
    Dim LastPencilPos As Point

    ' True, if user is entering some data into text box right now
    Dim Editing As Boolean = False

    ' Used to control software input panel
    Dim InputPanel As New Microsoft.WindowsCE.Forms.InputPanel

    Private Sub PictureBox_MouseDown(ByVal sender As Object, ByVal e As MouseEventArgs) Handles PictureBox.MouseDown
        If PencilMenuItem.Checked Then
            ' This click starts drawing with the pencil, so set pencil position and raise drawing flag
            LastPencilPos = New Point(e.X, e.Y)
            Drawing = True
        ElseIf AddTextMenuItem.Checked Then            
            If Not Editing Then
                ' This click starts text editing

                ' Prepare text box
                TextBox.Location = New Point(e.X + PictureBox.Left, e.Y + PictureBox.Top)
                TextBox.Text = String.Empty
                TextBox.Visible = True
                TextBox.Focus()

                ' Popup the software input panel
                InputPanel.Enabled = True
                Editing = True
            Else
                ' Text editing was in progress while user clicked on PictureBox, so this click should end the editing
                Me.Focus()
            End If
        End If
    End Sub

    Private Sub DrawLineEx(ByVal g As Graphics, ByVal color As Color, ByVal Width As Integer, ByVal p1 As Point, ByVal p2 As Point)
        Dim dx As Double = p2.X - p1.X
        Dim dy As Double = p2.Y - p1.Y
        Dim l As Double = Math.Sqrt(dx * dx + dy * dy)
        If (Math.Abs(l) > 0.01) Then
            dx = dx * Width / (2 * l)
            dy = dy * Width / (2 * l)
            Dim polygon As Point() = { _
                New Point(CInt(Math.Round(p1.X - dy)), CInt(Math.Round(p1.Y + dx))), _
                New Point(CInt(Math.Round(p1.X + dy)), CInt(Math.Round(p1.Y - dx))), _
                New Point(CInt(Math.Round(p2.X + dy)), CInt(Math.Round(p2.Y - dx))), _
                New Point(CInt(Math.Round(p2.X - dy)), CInt(Math.Round(p2.Y + dx))) _
            }
            g.FillPolygon(New SolidBrush(color), polygon)
        End If
        Dim w2 As Double = Width / 2
        g.FillEllipse(New SolidBrush(color), CInt(Math.Round(p1.X - w2)), CInt(Math.Round(p1.Y - w2)), Width, Width)
        g.FillEllipse(New SolidBrush(color), CInt(Math.Round(p2.X - w2)), CInt(Math.Round(p2.Y - w2)), Width, Width)
    End Sub

    Private Sub PictureBox_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs) Handles PictureBox.MouseMove

        ' Check whether pencil mode is in effect and user started drawing
        If PencilMenuItem.Checked And Drawing Then

            ' Create Graphics for our picture
            Dim g As System.Drawing.Graphics = System.Drawing.Graphics.FromImage(Image)

            ' Set width of the pencil here
            Dim PencilWidth As Integer = 6

            ' Draw line that connects previous click position with current pencil position
            DrawLineEx(g, Color.Blue, PencilWidth, LastPencilPos, New Point(e.X, e.Y))

            ' Graphics objects is disposable, so dispose is as soon as it does not needed
            g.Dispose()

            ' Calculate rectangle of image area that has changed
            Dim left As Integer = CInt(Math.Min(LastPencilPos.X - PencilWidth, e.X))
            Dim right As Integer = CInt(Math.Max(LastPencilPos.X + PencilWidth, e.X))
            Dim top As Integer = CInt(Math.Min(LastPencilPos.Y - PencilWidth, e.Y))
            Dim bottom As Integer = CInt(Math.Max(LastPencilPos.Y + PencilWidth, e.Y))

            ' Mark changed rectangle as invalid
            PictureBox.Invalidate(New Rectangle(left, top, right - left + 1, bottom - top + 1))

            ' Remember current stylus position
            LastPencilPos.X = e.X
            LastPencilPos.Y = e.Y
        End If
    End Sub

    Private Sub PictureBox_MouseUp(ByVal sender As Object, ByVal e As MouseEventArgs) Handles PictureBox.MouseUp
        ' Drawing is over
        Drawing = False
    End Sub

    Private Sub TextEditBox_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles TextBox.LostFocus
        ' User tapped to some other control or switched to another application, so we should finish text editing
        If Editing Then
            ' If something was entered
            If TextBox.Modified Then

                ' Create graphics to draw on
                Dim g As Graphics = Graphics.FromImage(Image)

                ' Correct text position according to current position of PictureBox
                Dim x As Integer = TextBox.Left - PictureBox.Left
                Dim y As Integer = TextBox.Top - PictureBox.Top

                ' Calculate size of text that was drawn to correctly update needed area
                Dim TextSize As SizeF = g.MeasureString(TextBox.Text, TextBox.Font)
                
                Dim Rect As New Rectangle(x, y, CInt(TextSize.Width), CInt(TextSize.Height))
                
                ' Fill background for our label
                g.FillRectangle(New System.Drawing.SolidBrush(TextBox.BackColor), Rect)                
                
                ' Draw needed string. Assume that it is not multiline nor aligned 
                g.DrawString(TextBox.Text, TextBox.Font, New System.Drawing.SolidBrush(TextBox.ForeColor), x, y)
                                
                ' Mark changed area for redraw
                PictureBox.Invalidate(Rect)

                ' Dispose graphics asap
                g.Dispose()
            End If

            ' Hide text box and move it to the upper-left corner, so it does not affect scrollbars
            TextBox.Visible = False
            TextBox.Location = New Point(0, 0)

            ' Hide input panel
            InputPanel.Enabled = False

            ' Editing is finished
            Editing = False
        End If
    End Sub

    Private Sub TextBox_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles TextBox.KeyDown
        ' Enter key was pressed in text box, so finish editing too
        If e.KeyCode = Keys.Enter Then
            Me.Focus()
            e.Handled = True
        End If
    End Sub

    ' Quality of saved JPEG picture
    Private Const JPEGQuality As Integer = 85

    Private Sub SaveMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveMenuItem.Click
        Dim result As DialogResult = SaveFileDialog.ShowDialog()
        If result = DialogResult.OK Then
            Try
                JPEGator.BitmapUtils.SaveBitmap(Image, SaveFileDialog.FileName, JPEGQuality)
            Catch ex As Exception
                ' Show exception information, if any
                Windows.Forms.MessageBox.Show( _
                    String.Format("Unable to save file {0} ({1}): {2}", OpenFileDialog.FileName, ex.GetType().Name, ex.Message), _
                    "Error", _
                    MessageBoxButtons.OK, _
                    MessageBoxIcon.Hand, _
                    MessageBoxDefaultButton.Button1 _
                )
            End Try
        End If
    End Sub

End Class
