// JPEGator .NET Compact Framework Library.
// Copyright (C) 2005-2009, Aleh Dzenisiuk. All Rights Reserved.
// http://dzenisiuk.info/jpegator/
//
// When redistributing JPEGator source code the above copyright notice and 
// this messages should be left intact. In case changes are made by you in 
// this source code it should be clearly indicated in the beginning of each 
// changed file.

using System;
using System.Runtime.InteropServices;

namespace JPEGator
{
	internal class JpegDll
	{
		public const string DllName = "JPEGator.jpeg.dll";

		public const int Version = 62;

		[DllImport("coredll.dll")]
		private static extern int wvsprintfW(char[] buf, string format, int args);

		private static void CheckError(IntPtr commonPtr)
		{
			IntPtr errorMgrPtr = (IntPtr)Marshal.ReadInt32(commonPtr);
			int errorCode = Marshal.ReadInt32(errorMgrPtr, ErrorMgr.StructSize - 4);
			if (errorCode != 0)
			{
				Marshal.WriteInt32(errorMgrPtr, ErrorMgr.StructSize - 4, 0);

				if (errorCode < 0 || errorCode >= errorMessages.Length)
					throw new JpegException(string.Format("Unknown error code: {0}", errorCode), errorCode);
				else
				{
					char[] buf = new char[1024];
					int length = wvsprintfW(buf, errorMessages[errorCode], (int)errorMgrPtr + 6 * 4);
					throw new JpegException(new string(buf, 0, length), errorCode);
				}
			}
		}

		public static void CreateCompress(IntPtr compressInfo, int version, int structureSize)
		{
			NativeMethods.CreateCompress(compressInfo, version, structureSize);
			CheckError(compressInfo);
		}

		public static void CreateDecompress(IntPtr decompressInfo, int version, int structureSize)
		{
			NativeMethods.CreateDecompress(decompressInfo, version, structureSize);
			CheckError(decompressInfo);
		}

		public static void DestroyCompress(IntPtr compressInfo)
		{
			NativeMethods.DestroyCompress(compressInfo);
			CheckError(compressInfo);
		}

		public static void DestroyDecompress(IntPtr decompressInfo)
		{
			NativeMethods.DestroyDecompress(decompressInfo);
			CheckError(decompressInfo);
		}

		public static void SetDefaults(IntPtr compressInfo)
		{
			NativeMethods.SetDefaults(compressInfo);
			CheckError(compressInfo);
		}

		public static void SetQuality(IntPtr compressInfo, int quality, bool forceBaseline)
		{
			NativeMethods.SetQuality(compressInfo, quality, forceBaseline);
			CheckError(compressInfo);
		}

		public static void StartCompress(IntPtr compressInfo, bool writeAllTables)
		{
			NativeMethods.StartCompress(compressInfo, writeAllTables);
			CheckError(compressInfo);
		}

		public static unsafe int WriteScanlines(IntPtr compressInfo, byte** bits, int numLines)
		{
			int result = NativeMethods.WriteScanlines(compressInfo, bits, numLines);
			CheckError(compressInfo);
			return result;
		}

		public static void FinishCompress(IntPtr compressInfo)
		{
			NativeMethods.FinishCompress(compressInfo);
			CheckError(compressInfo);
		}

		public static int ReadHeader(IntPtr decompressInfo, bool requireImage)
		{
			int result = NativeMethods.ReadHeader(decompressInfo, requireImage);
			CheckError(decompressInfo);
			return result;
		}

		public static bool StartDecompress(IntPtr decompressInfo)
		{
			bool result = NativeMethods.StartDecompress(decompressInfo);
			CheckError(decompressInfo);
			return result;
		}

		public static unsafe int ReadScanlines(IntPtr decompressInfo, byte** bits, int maxLines)
		{
			int result = NativeMethods.ReadScanlines(decompressInfo, bits, maxLines);
			CheckError(decompressInfo);
			return result;
		}

		public static bool FinishDecompress(IntPtr decompressInfo)
		{
			bool result = NativeMethods.FinishDecompress(decompressInfo);
			CheckError(decompressInfo);
			return result;
		}

		public static bool ResyncToRestart(IntPtr decompressInfo, int desired)
		{
			bool result = NativeMethods.ResyncToRestart(decompressInfo, desired);
			CheckError(decompressInfo);
			return result;
		}

		public static void SkipScanlines(IntPtr decompressInfo, int scanlinesCount)
		{
			bool result = NativeMethods.SkipScanlines(decompressInfo, scanlinesCount);
			if (!result)
				throw new OutOfMemoryException();
			CheckError(decompressInfo);
		}
/*~
#if NETCF1
		public static void LoadThumbnail(
			IntPtr decompressInfo,
			int destWidth, int destHeight,
			byte[] bitmapBits,
			int bitmapLineSize,
			ThumbnailQuality quality
		)
		{
			unsafe
			{
				fixed (byte* bitmapBitsPtr = bitmapBits)
				{
					bool result = NativeMethods.LoadThumbnail(
						decompressInfo,
						destWidth, destHeight,
						bitmapBitsPtr,
						bitmapLineSize,
						quality
					);
					if (!result)
						throw new OutOfMemoryException();
				}
			}
			CheckError(decompressInfo);
		}
#elif NETCF2
		unsafe public static void LoadThumbnail(
			IntPtr decompressInfo,
			int destWidth, int destHeight,
			byte* bitmapBits,
			int bitmapLineSize,
			ThumbnailQuality quality
		)
		{
			bool result = NativeMethods.LoadThumbnail(
				decompressInfo,
				destWidth, destHeight,
				bitmapBits,
				bitmapLineSize,
				quality
			);
			if (!result)
				throw new OutOfMemoryException();
			CheckError(decompressInfo);
		}
#endif
*/
		private sealed class NativeMethods
		{
			// void jpeg_CreateCompress (j_compress_ptr cinfo, int version, size_t structsize)
			[DllImport(DllName, EntryPoint="jpegator_CreateCompress")]
			public static extern void CreateCompress(IntPtr compressInfo, int version, int structureSize);

			// void jpeg_CreateDecompress (j_decompress_ptr cinfo, int version, size_t structsize)
			[DllImport(DllName, EntryPoint="jpegator_CreateDecompress")]
			public static extern void CreateDecompress(IntPtr decompressInfo, int version, int structureSize);

			// void jpeg_destroy_compress(j_compress_ptr cinfo)
			[DllImport(DllName, EntryPoint="jpegator_destroy_compress")]
			public static extern void DestroyCompress(IntPtr compressInfo);

			// void jpeg_destroy_decompress(j_decompress_ptr cinfo)
			[DllImport(DllName, EntryPoint="jpegator_destroy_decompress")]
			public static extern void DestroyDecompress(IntPtr decompressInfo);

			// void jpeg_set_defaults(j_compress_ptr cinfo)
			[DllImport(DllName, EntryPoint="jpegator_set_defaults")]
			public static extern void SetDefaults(IntPtr compressInfo);

			// void jpeg_set_quality(j_compress_ptr cinfo, int quality, boolean force_baseline)
			[DllImport(DllName, EntryPoint="jpegator_set_quality")]
			public static extern void SetQuality(IntPtr compressInfo, int quality, bool forceBaseline);

			// void jpeg_start_compress(j_compress_ptr cinfo, boolean write_all_tables)
			[DllImport(DllName, EntryPoint="jpegator_start_compress")]
			public static extern void StartCompress(IntPtr compressInfo, bool writeAllTables);

			// JDIMENSION jpeg_write_scanlines(j_compress_ptr cinfo, JSAMPARRAY scanlines, JDIMENSION num_lines)
			[DllImport(DllName, EntryPoint="jpegator_write_scanlines")]
			public static extern unsafe int WriteScanlines(IntPtr compressInfo, byte** bits, int numLines);

			// void jpeg_finish_compress(j_compress_ptr cinfo)
			[DllImport(DllName, EntryPoint="jpegator_finish_compress")]
			public static extern void FinishCompress(IntPtr compressInfo);

			// int jpeg_read_header(j_decompress_ptr cinfo, boolean require_image)
			[DllImport(DllName, EntryPoint="jpegator_read_header")]
			public static extern int ReadHeader(IntPtr decompressInfo, bool requireImage);

			// boolean jpeg_start_decompress(j_decompress_ptr cinfo)
			[DllImport(DllName, EntryPoint="jpegator_start_decompress")]
			public static extern bool StartDecompress(IntPtr decompressInfo);

			// JDIMENSION jpeg_read_scanlines(j_decompress_ptr cinfo, JSAMPARRAY scanlines, JDIMENSION max_lines)
			[DllImport(DllName, EntryPoint="jpegator_read_scanlines")]
			public static extern unsafe int ReadScanlines(IntPtr decompressInfo, byte** bits, int maxLines);

			// boolean jpeg_finish_decompress(j_decompress_ptr cinfo)
			[DllImport(DllName, EntryPoint="jpegator_finish_decompress")]
			public static extern bool FinishDecompress(IntPtr decompressInfo);

			// boolean jpeg_resync_to_restart(j_decompress_ptr cinfo, int desired)
			[DllImport(DllName, EntryPoint="jpegator_resync_to_restart")]
			public static extern bool ResyncToRestart(IntPtr decompressInfo, int desired);

			[DllImport(DllName, EntryPoint="jpegator_skip_scanlines")]
			public static extern bool SkipScanlines(IntPtr decompressInfo, int scanlinesCount);
/*~
			[DllImport(DllName, EntryPoint="jpegator_load_thumbnail")]
			unsafe public static extern bool LoadThumbnail(
				IntPtr decompressInfo,
				int destWidth, int destHeight,
				byte* bitmapBits,
				int bitmapLineSize,
				ThumbnailQuality quality
			);
*/
		}

		private static string[] errorMessages = {
			"Bogus message code %d",
			"Sorry, there are legal restrictions on arithmetic coding",
			"ALIGN_TYPE is wrong, please fix",
			"MAX_ALLOC_CHUNK is wrong, please fix",
			"Bogus buffer control mode",
			"Invalid component ID %d in SOS",
			"DCT coefficient out of range",
			"IDCT output block size %d not supported",
			"Bogus Huffman table definition",
			"Bogus input colorspace",
			"Bogus JPEG colorspace",
			"Bogus marker length",
			"Wrong JPEG library version: library is %d, caller expects %d",
			"Sampling factors too large for interleaved scan",
			"Invalid memory pool code %d",
			"Unsupported JPEG data precision %d",
			"Invalid progressive parameters Ss=%d Se=%d Ah=%d Al=%d",
			"Invalid progressive parameters at scan script entry %d",
			"Bogus sampling factors",
			"Invalid scan script at entry %d",
			"Improper call to JPEG library in state %d",
			"JPEG parameter struct mismatch: library thinks size is %u, caller expects %u",
			"Bogus virtual array access",
			"Buffer passed to JPEG library is too small",
			"Suspension not allowed here",
			"CCIR601 sampling not implemented yet",
			"Too many color components: %d, max %d",
			"Unsupported color conversion request",
			"Bogus DAC index %d",
			"Bogus DAC value 0x%x",
			"Bogus DHT index %d",
			"Bogus DQT index %d",
			"Empty JPEG image (DNL not supported)",
			"Read from EMS failed",
			"Write to EMS failed",
			"Didn't expect more than one scan",
			"Input file read error",
			"Output file write error --- out of disk space?",
			"Fractional sampling not implemented yet",
			"Huffman code size table overflow",
			"Missing Huffman code table entry",
			"Maximum supported image dimension is %u pixels",
			"Empty input file",
			"Premature end of input file",
			"Cannot transcode due to multiple use of quantization table %d",
			"Scan script does not transmit all data",
			"Invalid color quantization mode change",
			"Not implemented yet",
			"Requested feature was omitted at compile time",
			"Backing store not supported",
			"Huffman table 0x%02x was not defined",
			"JPEG datastream contains no image",
			"Quantization table 0x%02x was not defined",
			"Not a JPEG file: starts with 0x%02x 0x%02x",
			"Insufficient memory (case %d)",
			"Cannot quantize more than %d color components",
			"Cannot quantize to fewer than %d colors",
			"Cannot quantize to more than %d colors",
			"Invalid JPEG file structure: two SOF markers",
			"Invalid JPEG file structure: missing SOS marker",
			"Unsupported JPEG process: SOF type 0x%02x",
			"Invalid JPEG file structure: two SOI markers",
			"Invalid JPEG file structure: SOS before SOF",
			"Failed to create temporary file %s",
			"Read failed on temporary file",
			"Seek failed on temporary file",
			"Write failed on temporary file --- out of disk space?",
			"Application transferred too few scanlines",
			"Unsupported marker type 0x%02x",
			"Virtual array controller messed up",
			"Image too wide for this implementation",
			"Read from XMS failed",
			"Write to XMS failed",
			"Copyright (C) 1998, Thomas G. Lane",
			"6b  27-Mar-1998",
			"Caution: quantization tables are too coarse for baseline JPEG",
			"Adobe APP14 marker: version %d, flags 0x%04x 0x%04x, transform %d",
			"Unknown APP0 marker (not JFIF), length %u",
			"Unknown APP14 marker (not Adobe), length %u",
			"Define Arithmetic Table 0x%02x: 0x%02x",
			"Define Huffman Table 0x%02x",
			"Define Quantization Table %d  precision %d",
			"Define Restart Interval %u",
			"Freed EMS handle %u",
			"Obtained EMS handle %u",
			"End Of Image",
			"        %3d %3d %3d %3d %3d %3d %3d %3d",
			"JFIF APP0 marker: version %d.%02d, density %dx%d  %d",
			"Warning: thumbnail image size does not match data length %u",
			"JFIF extension marker: type 0x%02x, length %u",
			"    with %d x %d thumbnail image",
			"Miscellaneous marker 0x%02x, length %u",
			"Unexpected marker 0x%02x",
			"        %4u %4u %4u %4u %4u %4u %4u %4u",
			"Quantizing to %d = %d*%d*%d colors",
			"Quantizing to %d colors",
			"Selected %d colors for quantization",
			"At marker 0x%02x, recovery action %d",
			"RST%d",
			"Smoothing not supported with nonstandard sampling ratios",
			"Start Of Frame 0x%02x: width=%u, height=%u, components=%d",
			"    Component %d: %dhx%dv q=%d",
			"Start of Image",
			"Start Of Scan: %d components",
			"    Component %d: dc=%d ac=%d",
			"  Ss=%d, Se=%d, Ah=%d, Al=%d",
			"Closed temporary file %s",
			"Opened temporary file %s",
			"JFIF extension marker: JPEG-compressed thumbnail image, length %u",
			"JFIF extension marker: palette thumbnail image, length %u",
			"JFIF extension marker: RGB thumbnail image, length %u",
			"Unrecognized component IDs %d %d %d, assuming YCbCr",
			"Freed XMS handle %u",
			"Obtained XMS handle %u",
			"Unknown Adobe color transform code %d",
			"Inconsistent progression sequence for component %d coefficient %d",
			"Corrupt JPEG data: %u extraneous bytes before marker 0x%02x",
			"Corrupt JPEG data: premature end of data segment",
			"Corrupt JPEG data: bad Huffman code",
			"Warning: unknown JFIF revision number %d.%02d",
			"Premature end of JPEG file",
			"Corrupt JPEG data: found marker 0x%02x instead of RST%d",
			"Invalid SOS parameters for sequential JPEG",
			"Application transferred too many scanlines"
		};
	}
}
