using System;
using UnityEngine;

public class Texture2DUtils {

	/**
	 * Draws a non-aliased line to a texture
	 * Originally from http://wiki.unity3d.com/index.php?title=TextureDrawLine
	 */
	/*
	public static void drawLine(Texture2D __texture, int __x0, int __y0, int __x1, int __y1, Color __color, float __alpha = 1) {
		int dy = __y1 - __y0;
		int dx = __x1 - __x0;
		int stepx, stepy;
	 
		if (dy < 0) {
			dy = -dy;
			stepy = -1;
		} else {
			stepy = 1;
		}
		if (dx < 0) {
			dx = -dx;
			stepx = -1;
		} else {
			stepx = 1;
		}
		dy <<= 1;
		dx <<= 1;
	 
		float fraction = 0;
		Color32[] textureColors = __texture.GetPixels32();
		int iw = __texture.width;
		int ih = __texture.height;
		int pos;
	 
		__texture.SetPixel(__x0, __y0, __color);
		if (dx > dy) {
			fraction = dy - (dx >> 1);
			while (Mathf.Abs(__x0 - __x1) > 1) {
				if (fraction >= 0) {
					__y0 += stepy;
					fraction -= dx;
				}
				__x0 += stepx;
				fraction += dy;
				pos = __y0 * iw + __x0;
				textureColors[pos] = mixColors(textureColors[pos], __color, __alpha);
				__texture.SetPixel(__x0, __y0, col);
			}
		} else {
			fraction = dx - (dy >> 1);
			while (Mathf.Abs(__y0 - __y1) > 1) {
				if (fraction >= 0) {
					__x0 += stepx;
					fraction -= dy;
				}
				__y0 += stepy;
				fraction += dx;
				pos = __y0 * iw + __x0;
				textureColors[pos] = mixColors(textureColors[pos], __color, __alpha);
			}
		}
		
		__texture2D.SetPixels(textureColors);
	}
	*/


	public static void drawAntiAliasLine(Texture2D __texture, int __x0, int __y0, int __x1, int __y1, Color __color, float __alpha = 1) {
		// http://en.wikipedia.org/wiki/Xiaolin_Wu's_line_algorithm
		
		
		
	}
	
    // ================================================================================================================
    // INTERNAL INTERFACE ---------------------------------------------------------------------------------------------

	/**
	 * Applies an array of colors to a texture (allows pre-multiplication with alpha, and only applies to a certain rectangle)
	 */
	/*
	private static void applyColorsToTexture(Texture2D __texture, Color32[] __newColors, int __startX, int __startY, int __width, float __alpha = 1) {
		Color32[] textureColors = __texture.GetPixels32();
		
		int iw = __texture.width;
		int ih = __texture.height;
		
		int colorRectRight = __startX + __width;
		int colorRectBottom = __startY + (__newColors.Length / __width);

		int x, y;
		int pos;
		
		for (y = 0; y < ih; y++) { 
			for (x = 0; x < iw; x++) {
				if (x >= __startX && y >= __startY && x < colorRectRight && y < colorRectBottom) {
					// Inside the rectangle, so mix colors properly
					pos = y * iw + x;
					textureColors[pos] = mixColors(textureColors[pos], __newColors[x - __startX + (y - __startY) * __width], __alpha);
				}
			}
		}
		
		// Apply the actual colors to the texture
		__texture.SetPixels(textureColors);
		__texture.Apply();
	}
	*/

	/*
	private static Color32 mixColors(Color32 __oldColor, Color32 __newColor, float __alpha = 1) {
		// This is not an interpolated color because it must flatten, not just interpolate
		float oa = __oldColor.a / 255f;
		float na = (__newColor.a / 255f) * __alpha;
		float nna = 1f - na;

		float a = oa + (1 - oa) * na * 255f;
		float r = (__oldColor.r * nna + __newColor.r * na) / 2f;
		float g = (__oldColor.g * nna + __newColor.g * na) / 2f;
		float b = (__oldColor.b * nna + __newColorbr * na) / 2f;

		return new Color32(Math.round(r), Math.round(g), Math.round(b), Math.round(a));
	}
	*/
}