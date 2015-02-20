' A demo for Bitmap utility functions
' Copyright (C) 2006-2009, Aleh Dzenisiuk
' http://dzenisiuk.info/jpegator/

Imports System.Drawing

Public Class MainForm
    Inherits System.Windows.Forms.Form
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents Label1 As System.Windows.Forms.Label

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        MyBase.Dispose(disposing)
    End Sub

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents PictureBox3 As System.Windows.Forms.PictureBox
    Friend WithEvents PictureBox2 As System.Windows.Forms.PictureBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.PictureBox3 = New System.Windows.Forms.PictureBox
        Me.PictureBox2 = New System.Windows.Forms.PictureBox
        Me.Label3 = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'PictureBox1
        '
        Me.PictureBox1.Location = New System.Drawing.Point(8, 24)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(95, 88)
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(8, 8)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(96, 16)
        Me.Label1.Text = "Initial:"
        '
        'Label2
        '
        Me.Label2.Location = New System.Drawing.Point(8, 120)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(216, 16)
        Me.Label2.Text = "To array, processed, then back:"
        '
        'PictureBox3
        '
        Me.PictureBox3.Location = New System.Drawing.Point(8, 136)
        Me.PictureBox3.Name = "PictureBox3"
        Me.PictureBox3.Size = New System.Drawing.Size(95, 88)
        '
        'PictureBox2
        '
        Me.PictureBox2.Location = New System.Drawing.Point(120, 24)
        Me.PictureBox2.Name = "PictureBox2"
        Me.PictureBox2.Size = New System.Drawing.Size(95, 88)
        '
        'Label3
        '
        Me.Label3.Location = New System.Drawing.Point(120, 8)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(112, 16)
        Me.Label3.Text = "To array and back:"
        '
        'MainForm
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.PictureBox2)
        Me.Controls.Add(Me.PictureBox3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.PictureBox1)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "MainForm"
        Me.Text = "BitmapBits"
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private CurWidth As Integer

    Private Sub CreateImages()
        ' Get width and height for quick reference, assuming all 3 images have the same size
        Dim width As Integer = Me.PictureBox1.Width
        Dim height As Integer = Me.PictureBox1.Height

        ' Get length in bytes of a single line of BGR24 data
        Dim strideLength As Integer = ((width * 3 + 3) And Not 3)
        ' How much bytes are padded to align single line on 4 byte boundary
        Dim stridePadding As Integer = strideLength - (width * 3)
        ' Total length of the image in bytes
        Dim length As Integer = strideLength * height

        '
        ' Create a byte array representing our image and fill it with something
        '
        Dim bits() As Byte
        bits = New Byte(length) {}

        Dim x, y As Integer
        Dim offset As Integer = 0
        For y = 0 To height - 1
            For x = 0 To width - 1
                bits(offset) = (x * 7 + y * 3) Mod 255
                offset = offset + 1
                bits(offset) = (x * 2 + y * 3) Mod 255
                offset = offset + 1
                bits(offset) = (x * 3 + y) Mod 255
                offset = offset + 1
            Next
            offset = offset + stridePadding
        Next

        ' Create a bitmap from our data
        Dim bitmap As System.Drawing.Bitmap = JPEGator.BitmapUtils.BitmapFromBGR24(bits, 0, width, height)
        ' And put it onto the picture box as the initial image
        Me.PictureBox1.Image = bitmap

        '
        ' Get data from the bitmap into array
        '
        Dim bitsDest() As Byte
        bitsDest = New Byte(length) {}
        JPEGator.BitmapUtils.BGR24FromBitmap(bitmap, bitsDest, 0)

        ' And then create bitmap back based on this array
        Me.PictureBox2.Image = JPEGator.BitmapUtils.BitmapFromBGR24(bitsDest, 0, width, height)

        ' Make some simple processing of the bitmap data 
        ' (making blue and green components darker)
        offset = 0
        For y = 0 To height - 1
            For x = 0 To width - 1
                bitsDest(offset) = bitsDest(offset) >> 1
                offset = offset + 1
                bitsDest(offset) = bitsDest(offset) >> 1
                offset = offset + 1
                bitsDest(offset) = bitsDest(offset)
                offset = offset + 1
            Next
            offset = offset + stridePadding
        Next

        ' Create a new bitmap from processed data
        Me.PictureBox3.Image = JPEGator.BitmapUtils.BitmapFromBGR24(bitsDest, 0, width, height)
    End Sub

    Private Sub MainForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CreateImages()
    End Sub

End Class
