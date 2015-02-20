// Shows how to use JPEGator.Compress class
// Copyright (C) 2006-2009, Aleh Dzenisiuk
// http://dzenisiuk.info/jpegator/

using System;
using System.Drawing;
using System.Windows.Forms;

namespace triangles
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class TrianglesForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.Label progressLabel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button startButton;

		public TrianglesForm()
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
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.progressLabel = new System.Windows.Forms.Label();
			this.startButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			// 
			// progressBar
			// 
			this.progressBar.Location = new System.Drawing.Point(8, 128);
			this.progressBar.Size = new System.Drawing.Size(224, 8);
			// 
			// progressLabel
			// 
			this.progressLabel.Location = new System.Drawing.Point(8, 104);
			this.progressLabel.Size = new System.Drawing.Size(224, 16);
			this.progressLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// startButton
			// 
			this.startButton.Location = new System.Drawing.Point(80, 144);
			this.startButton.Text = "Start";
			this.startButton.Click += new System.EventHandler(this.startButton_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Size = new System.Drawing.Size(224, 88);
			this.label1.Text = "This sample demonstrates how to use JPEGator to synthesize some image and store i" +
				"t directly on disk. It creates several JPEG pictures with dif" +
				"ferent quality settings in the root directory of device.";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// TrianglesForm
			// 
			this.Controls.Add(this.label1);
			this.Controls.Add(this.startButton);
			this.Controls.Add(this.progressLabel);
			this.Controls.Add(this.progressBar);
			this.MinimizeBox = false;
			this.Text = "Triangles";

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>

		static void Main() 
		{
			Application.Run(new TrianglesForm());
		}

		// Total count of generated images
		private const int ImagesCount = 3;

		// Index of current image
		private int imageIndex;

		// Called each time when compression progress is changed
		private void OnProgress(object sender, JPEGator.ProgressEventArgs e)
		{			
			progressBar.Value = (imageIndex - 1) * 100 + e.Percent;
		}

		// Generate all images on start button click
		private void startButton_Click(object sender, System.EventArgs e)
		{
			// Disable start button just in case
			startButton.Enabled = false;

			try
			{
				// Setup maximum progress, every image requires 100 points
				progressBar.Maximum = ImagesCount * 100;

				// Create instance of compressor object and setup size of destination image				
				JPEGator.Compress c = new JPEGator.Compress(1023, 1023);

				// Subscribe on progress notifications
				c.Progress += new JPEGator.ProgressEventHandler(OnProgress);
				
				// Prepare buffer for one line of source image, buffer format is RGB, one byte per color component
				// As you can see, only buffer of size 3 * width bytes of memory required to generate relative big image
				byte[] line = new byte[c.ImageWidth * 3];

				// Generate ImagesCount pictures
				for (imageIndex = 1; imageIndex <= ImagesCount; imageIndex++)
				{			
					// File name for current image
					string fileName = string.Format("Triangles-{0}.jpg", imageIndex);

					// Change text on progress label
					progressLabel.Text = string.Format("Creating {0}...", fileName);
					progressLabel.Refresh();					

					// Starting compression. Using file as output, scale quality linearly depending on current image number
					c.Start(fileName, (100 / ImagesCount) * imageIndex, JPEGator.ColorSpace.YCbCr);

					// Go through all lines of image	
					for (int i = 0; i < c.ImageHeight; i++)
					{
						// Fill one line of image using some function
						for (int j = 0; j < c.ImageWidth; j++)
						{
							line[3 * j] = (byte)((7 * i - 5 * j) / 2);
							line[3 * j + 1] = (byte)((7 * i - j) / 2);
							line[3 * j + 2] = (byte)((2 * i + 3 * j) / 2);
						}

						// Write generated line to the destination
						c.WriteScanline(line, 0);
					}
				}	

				// Update controls finally
				progressLabel.Text = "Done";
				progressBar.Value = 0;
			}
			catch (Exception ex)
			{
				// Show info about exception to user
				MessageBox.Show(
					string.Format(
						"Exception of type {0} occured during images generation:\r\n{1}",
						ex.GetType().FullName, 
						ex.Message
					),
					"Error",
					MessageBoxButtons.OK, 
					MessageBoxIcon.Exclamation,
					MessageBoxDefaultButton.Button1
				);
			}
			finally
			{
				// Enable button back
				startButton.Enabled = true;
			}
		}
	}
}
