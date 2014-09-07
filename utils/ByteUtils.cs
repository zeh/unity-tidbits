using System;
using UnityEngine;

public class ByteUtils {

	public enum Endian {
		LITTLE_ENDIAN,
		BIG_ENDIAN
	}

	/**
	 * Writes a short (Int16) to a byte array.
	 * This is an aux function used when creating the WAV data.
	 */
	public static void writeShortToBytes(byte[] __bytes, ref int __position, short __newShort, Endian __endian) {
		writeBytes(__bytes, ref __position, new byte[2] { (byte)((__newShort >> 8) & 0xff), (byte)(__newShort & 0xff) }, __endian);
	}

	/**
	 * Writes a uint (UInt32) to a byte array.
	 * This is an aux function used when creating the WAV data.
	 */
	public static void writeUintToBytes(byte[] __bytes, ref int __position, uint __newUint, Endian __endian) {
		writeBytes(__bytes, ref __position, new byte[4] { (byte)((__newUint >> 24) & 0xff), (byte)((__newUint >> 16) & 0xff), (byte)((__newUint >> 8) & 0xff), (byte)(__newUint & 0xff) }, __endian);
	}

	/**
	 * Writes any number of bytes into a byte array, at a given position.
	 * This is an aux function used when creating the WAV data.
	 */
	public static void writeBytes(byte[] __bytes, ref int __position, byte[] __newBytes, Endian __endian) {
		// Writes __newBytes to __bytes at position __position, increasing the position depending on the length of __newBytes
		for (int i = 0; i < __newBytes.Length; i++) {
			__bytes[__position] = __newBytes[__endian == Endian.BIG_ENDIAN ? i : __newBytes.Length - i - 1];
			__position++;
		}
	}
}