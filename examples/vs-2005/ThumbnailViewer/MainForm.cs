// Shows how to use thumbnail functionality of the JPEGator
// Copyright (C) 2006-2009, Aleh Dzenisiuk
// http://dzenisiuk.info/jpegator/

using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Data;

using System.Runtime.InteropServices;

namespace ThumbnailViewer
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class MainForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.PictureBox pictureBox;
		private System.Windows.Forms.Label timeLabel;
		private System.Windows.Forms.TextBox timeTextBox;
		private System.Windows.Forms.MenuItem openMenuItem;
		private System.Windows.Forms.MainMenu mainMenu;
		private System.Windows.Forms.MenuItem fileMenuItem;
		private System.Windows.Forms.ComboBox qualityComboBox;
		private System.Windows.Forms.Label qualityLabel;
		private System.Windows.Forms.CheckBox proportionsCheckBox;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem exitMenuItem;
		private System.Windows.Forms.TextBox memTextBox;
		private System.Windows.Forms.Label memLabel;
		private System.Windows.Forms.Timer memTimer;
		private System.Windows.Forms.OpenFileDialog openFileDialog;

		public MainForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			base.Dispose( disposing );
		}
		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.mainMenu = new System.Windows.Forms.MainMenu();
			this.fileMenuItem = new System.Windows.Forms.MenuItem();
			this.openMenuItem = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.exitMenuItem = new System.Windows.Forms.MenuItem();
			this.pictureBox = new System.Windows.Forms.PictureBox();
			this.timeLabel = new System.Windows.Forms.Label();
			this.timeTextBox = new System.Windows.Forms.TextBox();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.qualityComboBox = new System.Windows.Forms.ComboBox();
			this.qualityLabel = new System.Windows.Forms.Label();
			this.proportionsCheckBox = new System.Windows.Forms.CheckBox();
			this.memTextBox = new System.Windows.Forms.TextBox();
			this.memLabel = new System.Windows.Forms.Label();
			this.memTimer = new System.Windows.Forms.Timer();
			// 
			// mainMenu
			// 
			this.mainMenu.MenuItems.Add(this.fileMenuItem);
			// 
			// fileMenuItem
			// 
			this.fileMenuItem.MenuItems.Add(this.openMenuItem);
			this.fileMenuItem.MenuItems.Add(this.menuItem1);
			this.fileMenuItem.MenuItems.Add(this.exitMenuItem);
			this.fileMenuItem.Text = "File";
			// 
			// openMenuItem
			// 
			this.openMenuItem.Text = "Open...";
			this.openMenuItem.Click += new System.EventHandler(this.openMenuItem_Click);
			// 
			// menuItem1
			// 
			this.menuItem1.Text = "-";
			// 
			// exitMenuItem
			// 
			this.exitMenuItem.Text = "Exit";
			this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
			// 
			// pictureBox
			// 
			this.pictureBox.Location = new System.Drawing.Point(36, 8);
			this.pictureBox.Size = new System.Drawing.Size(168, 168);
			this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			// 
			// timeLabel
			// 
			this.timeLabel.Location = new System.Drawing.Point(8, 227);
			this.timeLabel.Size = new System.Drawing.Size(120, 16);
			this.timeLabel.Text = "Loading time (ms):";
			// 
			// timeTextBox
			// 
			this.timeTextBox.Location = new System.Drawing.Point(128, 225);
			this.timeTextBox.ReadOnly = true;
			this.timeTextBox.Size = new System.Drawing.Size(104, 20);
			this.timeTextBox.Text = "";
			// 
			// openFileDialog
			// 
			this.openFileDialog.Filter = "JPEG files (*.jpg; *.jpeg) |*.jpg;*.jpeg";
			// 
			// qualityComboBox
			// 
			this.qualityComboBox.Items.Add("Lowest");
			this.qualityComboBox.Items.Add("Low");
			this.qualityComboBox.Items.Add("High");
			this.qualityComboBox.Location = new System.Drawing.Point(128, 179);
			this.qualityComboBox.Size = new System.Drawing.Size(104, 21);
			this.qualityComboBox.SelectedIndexChanged += new System.EventHandler(this.qualityComboBox_SelectedIndexChanged);
			// 
			// qualityLabel
			// 
			this.qualityLabel.Location = new System.Drawing.Point(8, 181);
			this.qualityLabel.Size = new System.Drawing.Size(120, 16);
			this.qualityLabel.Text = "Thumbnail quality:";
			// 
			// proportionsCheckBox
			// 
			this.proportionsCheckBox.Checked = true;
			this.proportionsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.proportionsCheckBox.Location = new System.Drawing.Point(8, 204);
			this.proportionsCheckBox.Size = new System.Drawing.Size(224, 16);
			this.proportionsCheckBox.Text = "Retain proportions";
			this.proportionsCheckBox.CheckStateChanged += new System.EventHandler(this.proportionsCheckBox_CheckStateChanged);
			// 
			// memTextBox
			// 
			this.memTextBox.Location = new System.Drawing.Point(128, 246);
			this.memTextBox.ReadOnly = true;
			this.memTextBox.Size = new System.Drawing.Size(104, 20);
			this.memTextBox.Text = "";
			// 
			// memLabel
			// 
			this.memLabel.Location = new System.Drawing.Point(8, 246);
			this.memLabel.Size = new System.Drawing.Size(120, 16);
			this.memLabel.Text = "Avaiable memory:";
			// 
			// memTimer
			// 
			this.memTimer.Enabled = true;
			this.memTimer.Interval = 500;
			this.memTimer.Tick += new System.EventHandler(this.memTimer_Tick);
			// 
			// MainForm
			// 
			this.Controls.Add(this.memLabel);
			this.Controls.Add(this.memTextBox);
			this.Controls.Add(this.proportionsCheckBox);
			this.Controls.Add(this.qualityLabel);
			this.Controls.Add(this.qualityComboBox);
			this.Controls.Add(this.timeTextBox);
			this.Controls.Add(this.timeLabel);
			this.Controls.Add(this.pictureBox);
			this.MaximizeBox = false;
			this.Menu = this.mainMenu;
			this.MinimizeBox = false;
			this.Text = "Thumbnail Viewer";
			this.Load += new System.EventHandler(this.MainForm_Load);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>

		static void Main() 
		{
			Application.Run(new MainForm());
		}

		// File name or current image
		private string fileName;

		// Current thumbnail quality
		private JPEGator.ThumbnailQuality quality;

		// Specifies whether to retain proportions of original image
		private bool proportions = true;

		private void openMenuItem_Click(object sender, System.EventArgs e)
		{
			DialogResult result = openFileDialog.ShowDialog();
			if (result == DialogResult.OK)			
			{
				fileName = openFileDialog.FileName;			
				Reload();
			}
		}

		private void qualityComboBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			quality = (JPEGator.ThumbnailQuality)qualityComboBox.SelectedIndex;
			Reload();
		}

		private void Reload()
		{
			if (!System.IO.File.Exists(fileName))
				return;

			Cursor.Current = Cursors.WaitCursor;
			try
			{
				// Remember current tick count to measure loading speed
				int startTicks = Environment.TickCount;

				// Create a thumbnail image
				pictureBox.Image = JPEGator.BitmapUtils.LoadThumbnail(
					// Put filename of your picture here
					fileName,
					// Put here the maximum size of thumbnail image you want to obtain.
					// If proportional flag is false then this will be an exact size 
					// of thumbnail image, if not then this will be the size corrected 
					// to retain proportions of original image
					pictureBox.Width, pictureBox.Height, 
					// Select thumbnail quality here: image scaling method and DCT method depend on it;
					// the lower is quality, the better is speed.
					quality, 
					// Indicates whether thumbnail image will retain proportions of original image
					proportions
					);
				
				// Measure the time in milliseconds, that was needed to load and scale the picture
				timeTextBox.Text = (Environment.TickCount - startTicks).ToString();
			}
			catch (JPEGator.JpegException ex)
			{
				MessageBox.Show(
					string.Format(
						"Failed to open {0}: {1}",
						fileName,
						ex.Message
					),
					"ThumbnailViewer",
					MessageBoxButtons.OK,
					MessageBoxIcon.Hand,
					MessageBoxDefaultButton.Button1
				);
			}
			finally
			{
				Cursor.Current = Cursors.Default;
			}
		}

		private void proportionsCheckBox_CheckStateChanged(object sender, System.EventArgs e)
		{
			proportions = proportionsCheckBox.Checked;
			Reload();
		}

		private void MainForm_Load(object sender, System.EventArgs e)
		{
			qualityComboBox.SelectedIndex = 0;
		}

		private void exitMenuItem_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void memTimer_Tick(object sender, System.EventArgs e)
		{						
			MemoryStatus memStatus = new MemoryStatus();
			memStatus.Length = Marshal.SizeOf(memStatus);
			GlobalMemoryStatus(ref memStatus);
			memTextBox.Text = string.Format(
				"{0} Kb", 
				memStatus.AvailVirtual / 1024
			);
		}

		private struct MemoryStatus
		{
			public int Length; 
			public int MemoryLoad; 
			public int TotalPhys; 
			public int AvailPhys; 
			public int TotalPageFile; 
			public int AvailPageFile; 
			public int TotalVirtual; 
			public int AvailVirtual; 			
		}

		[DllImport("coredll.dll")]
		private static extern void GlobalMemoryStatus(ref MemoryStatus status);
	}
}
