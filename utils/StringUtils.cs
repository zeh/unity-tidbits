using System;
using UnityEngine;

public class StringUtils {

	public static string getRandomAlphanumericString(int __length = 1) {
		// Returns a random alphanumeric string with the specific number of chars
		string chars = "0123456789AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz";
		int i = 0;

		string str = "";

		for (i = 0; i < __length; i++) {
			str += chars.Substring((int)Math.Round((float)UnityEngine.Random.Range(0, chars.Length-1)), 1);
		}

		return str;
	}

	// http://wiki.unity3d.com/index.php?title=MD5
	public static string MD5(string strToEncrypt) {
		System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
		byte[] bytes = ue.GetBytes(strToEncrypt);
 
		// encrypt bytes
		System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
		byte[] hashBytes = md5.ComputeHash(bytes);
 
		// Convert the encrypted bytes back to a string (base 16)
		string hashString = "";
 
		for (int i = 0; i < hashBytes.Length; i++) {
			hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
		}
 
		return hashString.PadLeft(32, '0');
	}

	// Encrypts  a string using a key and the RC4 algorithm
	// http://entitycrisis.blogspot.com/2011/04/encryption-between-python-and-c.html
	// Test: http://rc4.online-domain-tools.com/
	public static byte[] encodeRC4(string __data, string __skey) {
		byte[] bytes = System.Text.ASCIIEncoding.ASCII.GetBytes(__data);
        var key = System.Text.ASCIIEncoding.ASCII.GetBytes(__skey);
        byte[] s = new byte[256];
        byte[] k = new byte[256];
        byte temp;
        int i, j;

        for (i = 0; i < 256; i++) {
            s[i] = (byte)i;
            k[i] = key[i % key.GetLength (0)];
        }

        j = 0;
        for (i = 0; i < 256; i++) {
            j = (j + s[i] + k[i]) % 256;
            temp = s[i];
            s[i] = s[j];
            s[j] = temp;
        }

        i = j = 0;
        for (int x = 0; x < bytes.GetLength (0); x++) {
            i = (i + 1) % 256;
            j = (j + s[i]) % 256;
            temp = s[i];
            s[i] = s[j];
            s[j] = temp;
            int t = (s[i] + s[j]) % 256;
            bytes[x] ^= s[t];
        }

		return bytes;
    }

	public static string convertBytesToString(byte[] bytes) {
		// Converts bytes to a string representation (e.g. ffd4a0)
		string result = "";
		for (int i = 0; i < bytes.Length; i++) {
			result += bytes[i].ToString("x2");
		}
		return result;
	}

	public static string convertIntToCustomBase(int __number, string __charset, int __padSize = 0) {
		// Convert a number to any base given a charset
		// Radix, as per http://www.newgrounds.com/wiki/creator-resources/newgrounds-apis/encryption-process

		int baseLen = __charset.Length;
		int currValue = __number;
		int rest = 0;
		string result = "";
		while (currValue >= baseLen) {
			rest = currValue % baseLen;
			currValue = (currValue - rest) / baseLen;
			result = __charset.Substring(rest, 1) + result;
		}

		rest = currValue % baseLen;
		result = __charset.Substring(rest, 1) + result;

		while (result.Length < __padSize) result = __charset.Substring(0,1) + result;

		return result;
	}
}



