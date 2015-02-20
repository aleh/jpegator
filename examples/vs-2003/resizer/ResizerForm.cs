// Simple JPEG resizer
// Copyright (C) 2006-2009, Aleh Dzenisiuk
// http://dzenisiuk.info/jpegator/

using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;

namespace resizer
{
	public class ResizerForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.SaveFileDialog saveFileDialog;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox sourceFileNameTextBox;
		private System.Windows.Forms.Button selectSourceButton;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button selectDestinationButton;
		private System.Windows.Forms.TextBox sourceWidthTextBox;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox sourceHeightTextBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Button goButton;
		private System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.NumericUpDown destWidthUpDown;
		private System.Windows.Forms.NumericUpDown destHeightUpDown;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.TextBox destinationFileNameTextBox;
		private System.Windows.Forms.NumericUpDown scaleUpDown;

		public ResizerForm()
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
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.label1 = new System.Windows.Forms.Label();
			this.sourceFileNameTextBox = new System.Windows.Forms.TextBox();
			this.selectSourceButton = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.destinationFileNameTextBox = new System.Windows.Forms.TextBox();
			this.selectDestinationButton = new System.Windows.Forms.Button();
			this.sourceWidthTextBox = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.sourceHeightTextBox = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.goButton = new System.Windows.Forms.Button();
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.scaleUpDown = new System.Windows.Forms.NumericUpDown();
			this.label8 = new System.Windows.Forms.Label();
			this.destWidthUpDown = new System.Windows.Forms.NumericUpDown();
			this.destHeightUpDown = new System.Windows.Forms.NumericUpDown();
			// 
			// openFileDialog
			// 
			this.openFileDialog.Filter = "JPEG images (*.JPG; *.JPEG)|*.JPG; *.JPEG";
			// 
			// saveFileDialog
			// 
			this.saveFileDialog.Filter = "JPEG images (*.JPG; *.JPEG)|*.JPG; *.JPEG";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Size = new System.Drawing.Size(224, 16);
			this.label1.Text = "Source file name:";
			// 
			// sourceFileNameTextBox
			// 
			this.sourceFileNameTextBox.Location = new System.Drawing.Point(8, 24);
			this.sourceFileNameTextBox.ReadOnly = true;
			this.sourceFileNameTextBox.Size = new System.Drawing.Size(200, 20);
			this.sourceFileNameTextBox.Text = "";
			// 
			// selectSourceButton
			// 
			this.selectSourceButton.Location = new System.Drawing.Point(208, 24);
			this.selectSourceButton.Size = new System.Drawing.Size(24, 20);
			this.selectSourceButton.Text = "...";
			this.selectSourceButton.Click += new System.EventHandler(this.selectSourceButton_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 88);
			this.label2.Size = new System.Drawing.Size(224, 16);
			this.label2.Text = "Destination file name:";
			// 
			// destinationFileNameTextBox
			// 
			this.destinationFileNameTextBox.Location = new System.Drawing.Point(8, 104);
			this.destinationFileNameTextBox.ReadOnly = true;
			this.destinationFileNameTextBox.Size = new System.Drawing.Size(200, 20);
			this.destinationFileNameTextBox.Text = "";
			// 
			// selectDestinationButton
			// 
			this.selectDestinationButton.Location = new System.Drawing.Point(208, 104);
			this.selectDestinationButton.Size = new System.Drawing.Size(24, 20);
			this.selectDestinationButton.Text = "...";
			// 
			// sourceWidthTextBox
			// 
			this.sourceWidthTextBox.Location = new System.Drawing.Point(72, 56);
			this.sourceWidthTextBox.ReadOnly = true;
			this.sourceWidthTextBox.Size = new System.Drawing.Size(72, 20);
			this.sourceWidthTextBox.Text = "";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(8, 56);
			this.label4.Size = new System.Drawing.Size(56, 16);
			this.label4.Text = "Size:";
			// 
			// sourceHeightTextBox
			// 
			this.sourceHeightTextBox.Location = new System.Drawing.Point(160, 56);
			this.sourceHeightTextBox.ReadOnly = true;
			this.sourceHeightTextBox.Size = new System.Drawing.Size(72, 20);
			this.sourceHeightTextBox.Text = "";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(144, 59);
			this.label3.Size = new System.Drawing.Size(16, 14);
			this.label3.Text = "x";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(8, 138);
			this.label5.Size = new System.Drawing.Size(56, 16);
			this.label5.Text = "Size:";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(144, 136);
			this.label6.Size = new System.Drawing.Size(16, 16);
			this.label6.Text = "x";
			this.label6.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// goButton
			// 
			this.goButton.Location = new System.Drawing.Point(8, 232);
			this.goButton.Size = new System.Drawing.Size(224, 20);
			this.goButton.Text = "Go!";
			this.goButton.Click += new System.EventHandler(this.goButton_Click);
			// 
			// progressBar
			// 
			this.progressBar.Location = new System.Drawing.Point(8, 256);
			this.progressBar.Size = new System.Drawing.Size(224, 8);
			this.progressBar.Visible = false;
			// 
			// scaleUpDown
			// 
			this.scaleUpDown.Increment = new System.Decimal(new int[] {
																		  5,
																		  0,
																		  0,
																		  0});
			this.scaleUpDown.Location = new System.Drawing.Point(72, 168);
			this.scaleUpDown.Minimum = new System.Decimal(new int[] {
																		1,
																		0,
																		0,
																		0});
			this.scaleUpDown.Size = new System.Drawing.Size(72, 20);
			this.scaleUpDown.Value = new System.Decimal(new int[] {
																	  100,
																	  0,
																	  0,
																	  0});
			this.scaleUpDown.ValueChanged += new System.EventHandler(this.scaleUpDown_ValueChanged);
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(144, 170);
			this.label8.Size = new System.Drawing.Size(88, 16);
			this.label8.Text = "%";
			// 
			// destWidthUpDown
			// 
			this.destWidthUpDown.Location = new System.Drawing.Point(72, 136);
			this.destWidthUpDown.Maximum = new System.Decimal(new int[] {
																			16384,
																			0,
																			0,
																			0});
			this.destWidthUpDown.Size = new System.Drawing.Size(72, 20);
			this.destWidthUpDown.ValueChanged += new System.EventHandler(this.destWidthUpDown_ValueChanged);
			// 
			// destHeightUpDown
			// 
			this.destHeightUpDown.Location = new System.Drawing.Point(160, 136);
			this.destHeightUpDown.Maximum = new System.Decimal(new int[] {
																			 16384,
																			 0,
																			 0,
																			 0});
			this.destHeightUpDown.Size = new System.Drawing.Size(72, 20);
			this.destHeightUpDown.ValueChanged += new System.EventHandler(this.destHeightUpDown_ValueChanged);
			// 
			// ResizerForm
			// 
			this.Controls.Add(this.destHeightUpDown);
			this.Controls.Add(this.destWidthUpDown);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.scaleUpDown);
			this.Controls.Add(this.progressBar);
			this.Controls.Add(this.goButton);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.sourceHeightTextBox);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.sourceWidthTextBox);
			this.Controls.Add(this.selectDestinationButton);
			this.Controls.Add(this.destinationFileNameTextBox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.selectSourceButton);
			this.Controls.Add(this.sourceFileNameTextBox);
			this.Controls.Add(this.label1);
			this.Menu = this.mainMenu1;
			this.MinimizeBox = false;
			this.Text = "Resizer";

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>

		static void Main() 
		{
			Application.Run(new ResizerForm());
		}

		private int sourceWidth;
		private int sourceHeight;

		private void selectSourceButton_Click(object sender, System.EventArgs e)
		{
			if (this.openFileDialog.ShowDialog() == DialogResult.OK)
			{	
				try
				{
					// Obtain source image size using decompressor
					using (JPEGator.Decompress decomp = new JPEGator.Decompress())
					{
						decomp.Start(System.IO.File.OpenRead(openFileDialog.FileName));
						sourceWidth = decomp.InputWidth;
						sourceHeight = decomp.InputHeight;
					}

					// Setup source controls
					sourceFileNameTextBox.Text = openFileDialog.FileName;
					sourceWidthTextBox.Text = sourceWidth.ToString();
					sourceHeightTextBox.Text = sourceHeight.ToString();

					// Initial setup for destination controls
					destinationFileNameTextBox.Text = System.IO.Path.ChangeExtension(openFileDialog.FileName, ".resized" + System.IO.Path.GetExtension(openFileDialog.FileName));								
					scaleUpDown_ValueChanged(this, EventArgs.Empty);				
				}
				catch (JPEGator.JpegException ex)
				{
					MessageBox.Show(
						string.Format(
							"Failed to open {0} to obtain its size: {1}",
							openFileDialog.FileName,
							ex.Message
						),
						"Resizer",
						MessageBoxButtons.OK,
						MessageBoxIcon.Hand,
						MessageBoxDefaultButton.Button1
					);
				}
			}
		}

		private bool lockChanges;

		// Handle changes in destination picture scale
		private void scaleUpDown_ValueChanged(object sender, System.EventArgs e)
		{
			if (lockChanges)
				return;
			lockChanges = true;
			this.destWidthUpDown.Value = sourceWidth * this.scaleUpDown.Value / 100;
			this.destHeightUpDown.Value = sourceHeight * this.scaleUpDown.Value / 100;
			lockChanges = false;
		}

		// Handle changes of destination picture width
		private void destWidthUpDown_ValueChanged(object sender, System.EventArgs e)
		{
			if (lockChanges)
				return;
			lockChanges = true;
			this.destHeightUpDown.Value = destWidthUpDown.Value * sourceHeight / sourceWidth;
			this.scaleUpDown.Value = 100 * destWidthUpDown.Value / sourceWidth;
			lockChanges = false;
		}

		// Handle changes of destination picture height
		private void destHeightUpDown_ValueChanged(object sender, System.EventArgs e)
		{
			if (lockChanges)
				return;
			lockChanges = true;
			this.destWidthUpDown.Value = destHeightUpDown.Value * sourceWidth / sourceHeight;
			this.scaleUpDown.Value = 100 * destHeightUpDown.Value / sourceHeight;
			lockChanges = false;
		}

		// Squeeze single scanline of source image
		private void SqueezeLine(byte[] line, uint[] destLine, uint sourWidth, uint destWidth)
		{
			uint stepW = sourWidth;
			uint stepR = (uint)(stepW % destWidth);
			uint destWidth3 = destWidth * 3;
			
			int s = 0;
			int d3 = 0;
			int s3 = 0;

			uint l = 0;
			uint nextW = stepW;					
			uint r = stepR;
					
			while (true)
			{	
				// Squeeze whole pixels
				while ((s + 1) * destWidth <= nextW)
				{							
					destLine[d3 + 0] += line[s3++];
					destLine[d3 + 1] += line[s3++];
					destLine[d3 + 2] += line[s3++];
					s++;
				}

				if (s >= sourceWidth)
					break;						

				// Add left part of partial pixel
				destLine[d3++] += line[s3 + 0] * r / destWidth;
				destLine[d3++] += line[s3 + 1] * r / destWidth;
				destLine[d3++] += line[s3 + 2] * r / destWidth;
						
				if (d3 >= destWidth3)
					break;

				l = destWidth - r;

				r += stepR;
				if (r >= destWidth)
					r -= destWidth;						

				// Set next pixel to the right part of partial pixel
				destLine[d3 + 0] += line[s3++] * l / destWidth;
				destLine[d3 + 1] += line[s3++] * l / destWidth;
				destLine[d3 + 2] += line[s3++] * l / destWidth;
				s++;

				nextW += stepW;
			}			
		}

		private void goButton_Click(object sender, System.EventArgs e)
		{
			try
			{
				progressBar.Value = 0;
				progressBar.Visible = true;
				try
				{
					// Create decompressor instance
					JPEGator.Decompress decomp = new JPEGator.Decompress();

					// Setup decompression progress handler
					decomp.Progress += new JPEGator.ProgressEventHandler(decomp_Progress);

					// Start decompression process (setup source stream, read source picture header)
					decomp.Start(System.IO.File.OpenRead(openFileDialog.FileName));

					// Create compressor instance
					JPEGator.Compress comp = new JPEGator.Compress(
						(int)this.destWidthUpDown.Value, 
						(int)this.destHeightUpDown.Value
					);
					// Start compression (prepare destination stream, setup color space and other compression parameters)
					comp.Start(this.destinationFileNameTextBox.Text, 75, JPEGator.ColorSpace.YCbCr);

					//
					// Getting shortcuts for some values (not sure if they can be optimized by JIT)
					//
					uint sourWidth = (uint)decomp.InputWidth;				
					uint sourHeight = (uint)decomp.InputHeight;
					uint sourWidthHeight = sourWidth * sourHeight;
					uint destWidth = (uint)comp.ImageWidth;
					uint destHeight = (uint)comp.ImageHeight;
					uint destWidthHeight = destWidth * destHeight;

					//
					// Prepare temporary buffers
					//
					byte[] line = new byte[sourWidth * 3];         // Scanline of source image
					uint[] destLine = new uint[destWidth * 3];       // Temp scanline of detination image
					byte[] destByteLine = new byte[destWidth * 3]; // Ready for output scanline of destination image

					//
					// Prepare scaling algorithm vars
					//
					uint stepH = sourHeight;
					uint stepB = (uint)(sourHeight % destHeight);
					uint nextH = stepH;
					uint b = stepB;

					int i = 0;
					int d = 0;				

					// Go through scanlines and squeeze them
					while (true)
					{							
						// Squeeze scanlines that fit into single scanline of destination image
						while ((i + 1) * destHeight <= nextH)
						{						
							decomp.ReadScanline(line);
							SqueezeLine(line, destLine, sourWidth, destWidth);											
							i++;												
						}										

						//
						// Squeeze source scanline that partially belongs to two scanlines of destination image
						//

						// Squeeze first part
						uint[] tempLine = new uint[destWidth * 3];
						if (i < sourHeight)																	
						{
							decomp.ReadScanline(line);
							
							SqueezeLine(line, tempLine, sourWidth, destWidth);
							for (d = 0; d < destWidth * 3; d++)
								destByteLine[d] = (byte)((destLine[d] * destHeight + tempLine[d] * b) * destWidth / sourWidthHeight);
							comp.WriteScanline(destByteLine, 0);
						}
						else
						{
							System.Diagnostics.Trace.Assert(b == 0);
							for (d = 0; d < destWidth * 3; d++)
								destByteLine[d] = (byte)(destLine[d] * destWidthHeight / sourWidthHeight);
							comp.WriteScanline(destByteLine, 0);
							break;
						}					

						// Squeeze second part
						uint t = destHeight - b;										
						for (d = 0; d < destWidth * 3; d++)
							destLine[d] = tempLine[d] * t / destHeight;

						b += stepB;
						if (b >= destHeight)
							b -= destHeight;
						
						i++;
						nextH += stepH;
					}				
				}
				finally
				{
					progressBar.Visible = false;
				}
			}
			catch (JPEGator.JpegException ex)
			{
				MessageBox.Show(
					string.Format(
						"Unable to resize {0} to {1}: {2}",
						openFileDialog.FileName,
						destinationFileNameTextBox.Text,
						ex.Message						
					),
					"Resizer",
					MessageBoxButtons.OK,
					MessageBoxIcon.Hand,
					MessageBoxDefaultButton.Button1
				);
			}
		}

		// Decompression progress handler
		private void decomp_Progress(object sender, JPEGator.ProgressEventArgs e)
		{
			progressBar.Value = e.Percent;
		}
	}
}
