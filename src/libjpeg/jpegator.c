/**
 * \file
 * \brief Additional wrappers for JPEGator.
 *
 * NOTE: this file is not part of the original IJG software package. It was created by Aleh Dzenisiuk
 * for the JPEGator library in order to handle .NET exceptions correctly.
 *
 */

#include "jinclude.h"
#include "jpeglib.h"

#undef EXTERN
#define EXTERN(type) type __declspec(dllexport) 

#define JMPBUF() (int*)((char*)(cinfo->err) + sizeof(struct jpeg_error_mgr))
#define ERROR_FLAG() *(int*)((char*)(cinfo->err) + sizeof(struct jpeg_error_mgr) + sizeof(jmp_buf))

#define SAVE_STACK_VOID() \
	if (setjmp(JMPBUF())) \
		return;

#define SAVE_STACK(r, e) \
	if (!setjmp(JMPBUF())) \
		return (r); \
	else \
		return (e);

EXTERN(void) 
jpegator_CreateCompress(j_compress_ptr cinfo, int version, size_t structsize)
{
	SAVE_STACK_VOID();
	jpeg_CreateCompress(cinfo, version, structsize);
}

EXTERN(void) 
jpegator_CreateDecompress(j_decompress_ptr cinfo, int version, size_t structsize)
{
	SAVE_STACK_VOID();
	jpeg_CreateDecompress(cinfo, version, structsize);
}

EXTERN(void) 
jpegator_destroy_compress(j_compress_ptr cinfo)
{
	SAVE_STACK_VOID();
	jpeg_destroy_compress(cinfo);
}

EXTERN(void) 
jpegator_destroy_decompress(j_decompress_ptr cinfo)
{
	SAVE_STACK_VOID();
	jpeg_destroy_decompress(cinfo);
}

EXTERN(void) 
jpegator_set_defaults(j_compress_ptr cinfo)
{
	SAVE_STACK_VOID();
	jpeg_set_defaults(cinfo);
}

EXTERN(void) 
jpegator_set_quality(j_compress_ptr cinfo, int quality, boolean force_baseline)
{
	SAVE_STACK_VOID();
	jpeg_set_quality(cinfo, quality, force_baseline);
}

EXTERN(void)
jpegator_start_compress(j_compress_ptr cinfo, boolean write_all_tables)
{
	SAVE_STACK_VOID();
	jpeg_start_compress(cinfo, write_all_tables);
}

EXTERN(JDIMENSION) 
jpegator_write_scanlines(j_compress_ptr cinfo, JSAMPARRAY scanlines, JDIMENSION num_lines)
{
	SAVE_STACK(jpeg_write_scanlines(cinfo, scanlines, num_lines), 0);
}

EXTERN(void) 
jpegator_finish_compress(j_compress_ptr cinfo)
{
	SAVE_STACK_VOID();
	jpeg_finish_compress(cinfo);
}

EXTERN(int) 
jpegator_read_header(j_decompress_ptr cinfo, boolean require_image)
{
	SAVE_STACK(jpeg_read_header(cinfo, require_image), 0);
}

EXTERN(boolean) 
jpegator_start_decompress(j_decompress_ptr cinfo)
{
	SAVE_STACK(jpeg_start_decompress(cinfo), 0);
}

EXTERN(JDIMENSION) 
jpegator_read_scanlines(j_decompress_ptr cinfo, JSAMPARRAY scanlines, JDIMENSION max_lines)
{
	SAVE_STACK(jpeg_read_scanlines(cinfo, scanlines, max_lines), 0);
}

EXTERN(boolean) 
jpegator_skip_scanlines(j_decompress_ptr cinfo, JDIMENSION lines_count)
{
	JSAMPROW rows[4];
	int scanline_size;
	int buffer_size;
	JDIMENSION last_output_scanline;
	char* buffer;
	int i;

	if (lines_count == 0)
		return 1;
		
	scanline_size = cinfo->out_color_components * ((cinfo->output_width + 3) & ~3) * sizeof(JSAMPLE);
	buffer_size = cinfo->rec_outbuf_height * scanline_size;
	buffer = (char*)malloc(buffer_size);
	if (!buffer)
		return 0;

	for (i = 0; i < sizeof(rows) / sizeof(rows[0]); i++)
		rows[i] = (JSAMPROW)(buffer + i * scanline_size);

	last_output_scanline = cinfo->output_scanline + lines_count;
	if (last_output_scanline > cinfo->output_height)
		last_output_scanline = cinfo->output_height;

	while (cinfo->output_scanline < last_output_scanline)
	{
		if (setjmp(JMPBUF()))
			break;
		jpeg_read_scanlines(cinfo, rows, cinfo->rec_outbuf_height);
	}

	free(buffer);
	return 1;
}

EXTERN(boolean) 
jpegator_finish_decompress(j_decompress_ptr cinfo)
{
	SAVE_STACK(jpeg_finish_decompress(cinfo), 0);
}

EXTERN(boolean) 
jpegator_resync_to_restart(j_decompress_ptr cinfo, int desired)
{
	SAVE_STACK(jpeg_resync_to_restart(cinfo, desired), 0);
}

EXTERN(void)
jpegator_error_exit(j_common_ptr cinfo)
{
	ERROR_FLAG() = cinfo->err->msg_code;
	longjmp(JMPBUF(), 1);
}

/*~
// Squeeze single scanline of source image
void SqueezeLine(unsigned char* line, int* destLine, int sourWidth, int destWidth)
{
	int stepW = sourWidth;
	int stepR = stepW % destWidth;
	int destWidth3 = destWidth * 3;

	int s1w = destWidth; // (s + 1) * destWidth
	int s1wend = sourWidth * destWidth;
	int d3 = 0;
	int s3 = 0;

	int l = 0;
	int nextW = stepW;
	int r = stepR;

	while (1)
	{
		// TODO: Get rid of this multiplication
		// Squeeze whole pixels
		while (s1w <= nextW)
		{
			destLine[d3 + 0] += line[s3++];
			destLine[d3 + 1] += line[s3++];
			destLine[d3 + 2] += line[s3++];
			s1w += destWidth;
		}

		if (s1w >= s1wend)
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

		s1w += destWidth;

		nextW += stepW;
	}
}

EXTERN(boolean)
jpegator_load_thumbnail(j_decompress_ptr cinfo, int destWidth, int destHeight, unsigned char* bitmapBits, int bitmapLineSize, int quality)
{
	int sourWidth = cinfo->image_width;
	int sourHeight = cinfo->image_height;

	int bitmapOffset = 0;

	// Allocating scanline buffer
	unsigned char *line = (unsigned char*)malloc(sourWidth * 3);
	unsigned char** linePtr = &line;
	if (!line)
		return 0;


	// Squeeze image depending on quality settings
	if (quality >= 2)
	{
		int stepH;
		int stepB;
		int nextH;
		int b;
		int i;
		int d;
		int t;


		//
		// High quality downscaling: using linear interpolation
		//

		// Getting shortcuts for some values (not sure if they can be optimized by JIT)
		int sourWidthHeight = sourWidth * sourHeight;
		int destWidthHeight = destWidth * destHeight;

		//
		// Prepare temporary buffers
		//
		int* destLine = (int*)malloc(destWidth * 3 * 4);       // Temp scanline of destination image
		unsigned char* destByteLine = (unsigned char*)malloc(destWidth * 3); // Ready for output scanline of destination image
		int* tempLine = (int*)malloc(destWidth * 3 * 4);

		if (!destLine || !destByteLine)
		{
			free(line);
			return 0;
		}

		//
		// Prepare scaling algorithm vars
		//
		stepH = sourHeight;
		stepB = sourHeight % destHeight;
		nextH = stepH;
		b = stepB;
		i = 0;
		d = 0;

		// Go through scanlines and squeeze them
		while (1)
		{
			// Squeeze scanlines that fit into single scanline of destination image
			while ((i + 1) * destHeight <= nextH)
			{				
				jpeg_read_scanlines(cinfo, linePtr, 1);
				//~decomp.ReadScanline(line);
				SqueezeLine(line, destLine, sourWidth, destWidth);
				i++;
			}

			//
			// Squeeze source scanline that partially belongs to two scanlines of destination image
			//

			// Squeeze first part
			
			if (i < sourHeight)
			{
				int offset; 

				jpeg_read_scanlines(cinfo, linePtr, 1);
				//~decomp.ReadScanline(line);

				SqueezeLine(line, tempLine, sourWidth, destWidth);
				offset = bitmapOffset;
				for (d = 0; d < destWidth * 3; d++)
				{
					bitmapBits[offset] = (unsigned char)((destLine[d] * destHeight + tempLine[d] * b) * destWidth / sourWidthHeight);
					offset++;
				}
				bitmapOffset += bitmapLineSize;
			}
			else
			{
				int offset = bitmapOffset;
				for (d = 0; d < destWidth * 3; d++)
				{
					bitmapBits[offset] = (unsigned char)(destLine[d] * destWidthHeight / sourWidthHeight);
					offset++;
				}

				break;
			}

			// Squeeze second part
			t = destHeight - b;
			for (d = 0; d < destWidth * 3; d++)
				destLine[d] = tempLine[d] * t / destHeight;

			b += stepB;
			if (b >= destHeight)
				b -= destHeight;

			i++;
			nextH += stepH;
		}
	}
	else
	{
		//
		// Low quality downscaling: just throw away some pixels of original image
		//
		int lineNumber = 0;
		int y = 0;
		int sourHeightDestHeight = sourHeight * destHeight;
		int sourWidthDestWidth = sourWidth * destWidth;
		while (1)
		{
			int x;
			int d;
			int nd;
			int linesToSkip;

			jpeg_read_scanlines(cinfo, linePtr, 1);
			//~decomp.ReadScanline(line);
			lineNumber += destHeight;

			x = 0;
			d = bitmapOffset;
			nd = destWidth;
			while (x < sourWidthDestWidth)
			{
				int xx = 3 * (x / destWidth);

				bitmapBits[d++] = line[xx];
				bitmapBits[d++] = line[xx + 1];
				bitmapBits[d++] = line[xx + 2];

				x += sourWidth;
			}

			bitmapOffset += bitmapLineSize;

			y += sourHeight;
			if (y >= sourHeightDestHeight)
				break;

			linesToSkip = (y - lineNumber) / destHeight;
			if (linesToSkip > 0)			
				jpegator_skip_scanlines(cinfo, linesToSkip);

			lineNumber += linesToSkip * destHeight;
		}
	}

	return 1;
}
*/