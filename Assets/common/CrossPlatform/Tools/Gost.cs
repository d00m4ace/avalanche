using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace HEXPLAY
{
	public class Gost
	{
		public enum Cipher // Both ciphers use 256-bit key
		{
			Kuznyechik, //Kuznyechik handles 128-bit blocks
			Magma //Magma – 64-bit blocks
		};

		ICipherAlgorithm cipher;
		private byte[] state;

		public Gost(Cipher cipher)
		{
			this.cipher = cipher == Cipher.Kuznyechik ? (ICipherAlgorithm)new Kuznyechik() : (ICipherAlgorithm)new Magma();
			state = new byte[this.cipher.BlockSize];
		}

		public void SetKey(byte[] pbKey)
		{
			cipher.SetKey(pbKey);
		}

		public void SetIV(byte[] pbIV)
		{
			Array.Copy(pbIV, state, cipher.BlockSize);
		}

		public void Encrypt(byte[] src, byte[] encrypt, int length)
		{
			int blockSize = cipher.BlockSize;
			int blockCount = length / blockSize;
			int blockCountSize = blockCount * blockSize;

			for(int i = 0, offset = 0; i < blockCount; i++, offset += blockSize)
				TransformBlock(true, src, offset, blockSize, encrypt, offset);

			if(length - blockCount * blockSize > 0)
				TransformBlock(true, src, blockCountSize, length - blockCountSize, encrypt, blockCountSize);
		}

		public void Decrypt(byte[] src, byte[] decrypt, int length)
		{
			int blockSize = cipher.BlockSize;
			int blockCount = length / blockSize;
			int blockCountSize = blockCount * blockSize;

			for(int i = 0, offset = 0; i < blockCount; i++, offset += blockSize)
				TransformBlock(false, src, offset, blockSize, decrypt, offset);

			if(length - blockCountSize > 0)
				TransformBlock(false, src, blockCountSize, length - blockCountSize, decrypt, blockCountSize);
		}

		public int TransformBlock(bool encrypt, byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
		{
			if(inputCount == 0) return inputCount;

			byte[] dataBlock = new byte[inputCount];
			byte[] result = new byte[inputCount];

			Array.Copy(inputBuffer, inputOffset, dataBlock, 0, inputCount);

			byte[] gamma = cipher.Encrypt(state);

			for(int i = 0; i < dataBlock.Length; i++)
			{
				result[i] = (byte)(dataBlock[i] ^ gamma[i]);
			}

			Array.Copy(result, 0, outputBuffer, outputOffset, inputCount);
			Array.Copy(encrypt ? result : dataBlock, state, inputCount);

			return inputCount;
		}

		public void Test()
		{
			HEXInt hexInt = -0x7FFFFFFF;
			int unHexInt = hexInt;
			hexInt = 777;

			HEXInt hi = 5;
			int i_hi = hi;
			HEXInt hl = (HEXInt)5L;
			long l_hl = (long)hl;
			HEXInt hf = (HEXInt)5.0f;
			float f_hf = (float)hf;
			HEXInt hd = (HEXInt)5.0;
			double d_hd = (double)hd;

			{
				HEXInt test1 = hi + 5;
				int result1 = test1;
				HEXInt test2 = hi + hl;
				int result2 = test1;
				HEXInt test3 = 5 + hi;
				int result3 = test1;
			}

			{
				HEXInt test1 = hi - 5;
				int result1 = test1;
				HEXInt test2 = hi - hl;
				int result2 = test1;
				HEXInt test3 = 5 - hi;
				int result3 = test1;
			}

			{
				HEXInt test1 = hi * 5;
				int result1 = test1;
				HEXInt test2 = hi * hl;
				int result2 = test1;
				HEXInt test3 = 5 * hi;
				int result3 = test1;
			}

			{
				HEXInt test1 = hi / 5;
				int result1 = test1;
				HEXInt test2 = hi / hl;
				int result2 = test1;
				HEXInt test3 = 5 / hi;
				int result3 = test1;
			}

			{
				HEXInt test1 = hi % 5;
				int result1 = test1;
				HEXInt test2 = hi % hl;
				int result2 = test1;
				HEXInt test3 = 5 % hi;
				int result3 = test1;
			}

			{
				int i = 0;
				i++;
				++i;

				HEXInt h = 0;
				h++;
				int result1 = h;
				++h;
				int result2 = h;
			}

			int iii = 7;
			long l_iii = iii;
			float f_iii = iii;

			byte[] key = new byte[cipher.KeyLength];
			byte[] iv = new byte[cipher.BlockSize];

			Random rng = new Random();
			rng.NextBytes(key);
			rng.NextBytes(iv);

			SetKey(key);

			for(int i = 0; i < 10; i++)
			{
				byte[] plainText = Encoding.UTF8.GetBytes(string.Format("However {0}, the files are normally written {0} just beforehand in a buffered manner {0}.", i));
				byte[] encrypt = new byte[plainText.Length];
				byte[] decrypt = new byte[plainText.Length];

				SetIV(iv);
				Encrypt(plainText, encrypt, plainText.Length);

				SetIV(iv);
				Decrypt(encrypt, decrypt, plainText.Length);

				string decryptString = Encoding.UTF8.GetString(decrypt);

				byte[] bxor = new byte[decrypt.Length];
				Array.Copy(decrypt, bxor, decrypt.Length);

				string b64bxor0 = bxor.ToBase64();
				bxor.Xor(iv);
				string b64bxor1 = bxor.ToBase64();
				bxor.Xor(iv);
				string b64bxor2 = bxor.ToBase64();
				string bxorString = bxor.ToUTF8String();

				string md5PlainText = Hash.Md5(plainText).ToHexString();
				string md5DecryptString = Hash.Md5(Encoding.UTF8.GetBytes(decryptString)).ToHexString();

				string sha1PlainText = Hash.Sha1(plainText).ToHexString();
				string sha1DecryptString = Hash.Sha1(Encoding.UTF8.GetBytes(decryptString)).ToHexString();

				string hexPlainText = plainText.ToHexString();
				string hexDecryptString = Encoding.UTF8.GetBytes(decryptString).ToHexString();

				string b64PlainText = plainText.ToBase64();
				string b64DecryptString = decryptString.ToBase64();

				string unb64PlainText = b64PlainText.StringFromBase64();
				string unb64DecryptString = b64DecryptString.StringFromBase64();

				uint crc32PlainText = Hash.Crc32(plainText);
				uint crc32DecryptString = Hash.Crc32(Encoding.UTF8.GetBytes(decryptString));

				ulong crc64PlainText = Hash.Crc64(plainText);
				ulong crc64DecryptString = Hash.Crc64(Encoding.UTF8.GetBytes(decryptString));
			}
		}

		/// <summary>
		/// Common interface for all ciphers in the plugin.
		/// Allows user to separate cipher algorithm from plugin-related logic.
		/// </summary>
		public interface ICipherAlgorithm
		{
			/// <summary>
			/// Key length in bytes
			/// </summary>
			int KeyLength { get; }

			/// <summary>
			/// Block size in bytes
			/// </summary>
			int BlockSize { get; }

			/// <summary>
			/// Set encryption key for cipher instance
			/// </summary>
			/// <param name="key">Key as byte array</param>
			void SetKey(byte[] key);

			/// <summary>
			/// Encrypts block of data using key set before
			/// </summary>
			/// <param name="data">Plain text block</param>
			/// <returns>Cipher text block</returns>
			byte[] Encrypt(byte[] data);
		}

		class Magma : ICipherAlgorithm
		{
			private const int BLOCK_SIZE = 8;
			private const int KEY_LENGTH = 32;

			public int BlockSize
			{
				get
				{
					return BLOCK_SIZE;
				}
			}

			public void SetKey(byte[] key)
			{
				_subKeys = GetSubKeys(key);
			}

			public int KeyLength
			{
				get
				{
					return KEY_LENGTH;
				}
			}

			/*
			 * Magma cipher implementation
			 */

			/// <summary>
			/// Substitution Table
			/// id-tc26-gost-28147-param-Z
			/// </summary>
			private readonly byte[][] _sBox =
			{
            //            0     1     2     3     4     5     6     7     8     9     A     B     C     D     E     F
            new byte[] { 0x0C, 0x04, 0x06, 0x02, 0x0A, 0x05, 0x0B, 0x09, 0x0E, 0x08, 0x0D, 0x07, 0x00, 0x03, 0x0F, 0x01 },
			new byte[] { 0x06, 0x08, 0x02, 0x03, 0x09, 0x0A, 0x05, 0x0C, 0x01, 0x0E, 0x04, 0x07, 0x0B, 0x0D, 0x00, 0x0F },
			new byte[] { 0x0B, 0x03, 0x05, 0x08, 0x02, 0x0F, 0x0A, 0x0D, 0x0E, 0x01, 0x07, 0x04, 0x0C, 0x09, 0x06, 0x00 },
			new byte[] { 0x0C, 0x08, 0x02, 0x01, 0x0D, 0x04, 0x0F, 0x06, 0x07, 0x00, 0x0A, 0x05, 0x03, 0x0E, 0x09, 0x0B },
			new byte[] { 0x07, 0x0F, 0x05, 0x0A, 0x08, 0x01, 0x06, 0x0D, 0x00, 0x09, 0x03, 0x0E, 0x0B, 0x04, 0x02, 0x0C },
			new byte[] { 0x05, 0x0D, 0x0F, 0x06, 0x09, 0x02, 0x0C, 0x0A, 0x0B, 0x07, 0x08, 0x01, 0x04, 0x03, 0x0E, 0x00 },
			new byte[] { 0x08, 0x0E, 0x02, 0x05, 0x06, 0x09, 0x01, 0x0C, 0x0F, 0x04, 0x0B, 0x00, 0x0D, 0x0A, 0x03, 0x07 },
			new byte[] { 0x01, 0x07, 0x0E, 0x0D, 0x00, 0x05, 0x08, 0x03, 0x04, 0x0F, 0x0A, 0x06, 0x09, 0x0C, 0x0B, 0x02 }
			};

			private uint[] _subKeys;

			public byte[] Encrypt(byte[] data)
			{
				byte[] dataR = new byte[data.Length];
				Array.Copy(data, dataR, data.Length);
				Array.Reverse(dataR);

				uint a0 = BitConverter.ToUInt32(dataR, 0);
				uint a1 = BitConverter.ToUInt32(dataR, 4);

				byte[] result = new byte[8];

				for(int i = 0; i < 31; i++)
				{
					int keyIndex = unchecked((i < 24) ? i % 8 : 7 - (i % 8));
					uint round = a1 ^ funcG(a0, _subKeys[keyIndex]);

					a1 = a0;
					a0 = round;
				}

				a1 = a1 ^ funcG(a0, _subKeys[0]);

				Array.Copy(BitConverter.GetBytes(a0), 0, result, 0, 4);
				Array.Copy(BitConverter.GetBytes(a1), 0, result, 4, 4);

				Array.Reverse(result);
				return result;
			}

			private uint funcG(uint a, uint k)
			{
				uint c = a + k;
				uint tmp = funcT(c);
				return unchecked((tmp << 11) | (tmp >> 21));
			}

			private uint funcT(uint a)
			{
				uint res = 0;

				unchecked
				{
					res ^= _sBox[0][a & 0x0000000f];
					res ^= (uint)(_sBox[1][((a & 0x000000f0) >> 4)] << 4);
					res ^= (uint)(_sBox[2][((a & 0x00000f00) >> 8)] << 8);
					res ^= (uint)(_sBox[3][((a & 0x0000f000) >> 12)] << 12);
					res ^= (uint)(_sBox[4][((a & 0x000f0000) >> 16)] << 16);
					res ^= (uint)(_sBox[5][((a & 0x00f00000) >> 20)] << 20);
					res ^= (uint)(_sBox[6][((a & 0x0f000000) >> 24)] << 24);
					res ^= (uint)(_sBox[7][((a & 0xf0000000) >> 28)] << 28);
				}

				return res;
			}

			private uint[] GetSubKeys(byte[] key)
			{
				byte[] keyR = new byte[key.Length];
				uint[] subKeys = new uint[8];
				Array.Copy(key, keyR, key.Length);
				Array.Reverse(keyR);
				for(int i = 0; i < 8; i++)
				{
					subKeys[i] = BitConverter.ToUInt32(keyR, i * 4);
				}
				Array.Reverse(subKeys);
				return subKeys;
			}
		}

		class Kuznyechik : ICipherAlgorithm
		{
			private const int BLOCK_SIZE = 16;
			private const int KEY_LENGTH = 32;

			private const int SUB_LENGTH = KEY_LENGTH / 2;

			public int BlockSize
			{
				get
				{
					return BLOCK_SIZE;
				}
			}

			public int KeyLength
			{
				get
				{
					return KEY_LENGTH;
				}
			}

			public byte[] Encrypt(byte[] data)
			{
				byte[] block = new byte[BLOCK_SIZE];
				byte[] temp = new byte[BLOCK_SIZE];

				Array.Copy(data, block, BLOCK_SIZE);

				for(int i = 0; i < 9; i++)
				{
					LSX(ref temp, ref _subKeys[i], ref block);
					Array.Copy(temp, block, BLOCK_SIZE);
				}

				X(ref block, ref _subKeys[9]);

				return block;

			}

			private byte[][] _subKeys;

			public void SetKey(byte[] key)
			{
				/*
				 * Initialize SubKeys array
				 */

				_subKeys = new byte[10][];
				for(int i = 0; i < 10; i++)
				{
					_subKeys[i] = new byte[SUB_LENGTH];
				}

				byte[] x = new byte[SUB_LENGTH];
				byte[] y = new byte[SUB_LENGTH];

				byte[] c = new byte[SUB_LENGTH];

				/*
				 * SubKey[1] = k[255..128]
				 * SubKey[2] = k[127..0]
				 */

				for(int i = 0; i < SUB_LENGTH; i++)
				{
					_subKeys[0][i] = x[i] = key[i];
					_subKeys[1][i] = y[i] = key[i + 16];
				}

				unchecked
				{
					for(int k = 1; k < 5; k++)
					{

						for(int j = 1; j <= 8; j++)
						{
							C(ref c, 8 * (k - 1) + j);
							F(ref c, ref x, ref y);
						}

						Array.Copy(x, _subKeys[2 * k], SUB_LENGTH);
						Array.Copy(y, _subKeys[2 * k + 1], SUB_LENGTH);
					}
				}
			}

			/*
			 * Transformations
			 */

			private readonly byte[] _pi = {
			0xFC, 0xEE, 0xDD, 0x11, 0xCF, 0x6E, 0x31, 0x16, 	// 00..07
            0xFB, 0xC4, 0xFA, 0xDA, 0x23, 0xC5, 0x04, 0x4D, 	// 08..0F
            0xE9, 0x77, 0xF0, 0xDB, 0x93, 0x2E, 0x99, 0xBA, 	// 10..17
            0x17, 0x36, 0xF1, 0xBB, 0x14, 0xCD, 0x5F, 0xC1, 	// 18..1F
            0xF9, 0x18, 0x65, 0x5A, 0xE2, 0x5C, 0xEF, 0x21, 	// 20..27
            0x81, 0x1C, 0x3C, 0x42, 0x8B, 0x01, 0x8E, 0x4F, 	// 28..2F
            0x05, 0x84, 0x02, 0xAE, 0xE3, 0x6A, 0x8F, 0xA0, 	// 30..37
            0x06, 0x0B, 0xED, 0x98, 0x7F, 0xD4, 0xD3, 0x1F, 	// 38..3F
            0xEB, 0x34, 0x2C, 0x51, 0xEA, 0xC8, 0x48, 0xAB, 	// 40..47
            0xF2, 0x2A, 0x68, 0xA2, 0xFD, 0x3A, 0xCE, 0xCC, 	// 48..4F
            0xB5, 0x70, 0x0E, 0x56, 0x08, 0x0C, 0x76, 0x12, 	// 50..57
            0xBF, 0x72, 0x13, 0x47, 0x9C, 0xB7, 0x5D, 0x87, 	// 58..5F
            0x15, 0xA1, 0x96, 0x29, 0x10, 0x7B, 0x9A, 0xC7, 	// 60..67
            0xF3, 0x91, 0x78, 0x6F, 0x9D, 0x9E, 0xB2, 0xB1, 	// 68..6F
            0x32, 0x75, 0x19, 0x3D, 0xFF, 0x35, 0x8A, 0x7E, 	// 70..77
            0x6D, 0x54, 0xC6, 0x80, 0xC3, 0xBD, 0x0D, 0x57, 	// 78..7F
            0xDF, 0xF5, 0x24, 0xA9, 0x3E, 0xA8, 0x43, 0xC9, 	// 80..87
            0xD7, 0x79, 0xD6, 0xF6, 0x7C, 0x22, 0xB9, 0x03, 	// 88..8F
            0xE0, 0x0F, 0xEC, 0xDE, 0x7A, 0x94, 0xB0, 0xBC, 	// 90..97
            0xDC, 0xE8, 0x28, 0x50, 0x4E, 0x33, 0x0A, 0x4A, 	// 98..9F
            0xA7, 0x97, 0x60, 0x73, 0x1E, 0x00, 0x62, 0x44, 	// A0..A7
            0x1A, 0xB8, 0x38, 0x82, 0x64, 0x9F, 0x26, 0x41, 	// A8..AF
            0xAD, 0x45, 0x46, 0x92, 0x27, 0x5E, 0x55, 0x2F, 	// B0..B7
            0x8C, 0xA3, 0xA5, 0x7D, 0x69, 0xD5, 0x95, 0x3B, 	// B8..BF
            0x07, 0x58, 0xB3, 0x40, 0x86, 0xAC, 0x1D, 0xF7, 	// C0..C7
            0x30, 0x37, 0x6B, 0xE4, 0x88, 0xD9, 0xE7, 0x89, 	// C8..CF
            0xE1, 0x1B, 0x83, 0x49, 0x4C, 0x3F, 0xF8, 0xFE, 	// D0..D7
            0x8D, 0x53, 0xAA, 0x90, 0xCA, 0xD8, 0x85, 0x61, 	// D8..DF
            0x20, 0x71, 0x67, 0xA4, 0x2D, 0x2B, 0x09, 0x5B, 	// E0..E7
            0xCB, 0x9B, 0x25, 0xD0, 0xBE, 0xE5, 0x6C, 0x52, 	// E8..EF
            0x59, 0xA6, 0x74, 0xD2, 0xE6, 0xF4, 0xB4, 0xC0, 	// F0..F7
            0xD1, 0x66, 0xAF, 0xC2, 0x39, 0x4B, 0x63, 0xB6, 	// F8..FF
        };

			private readonly byte[] _lFactors = {
			0x94, 0x20, 0x85, 0x10, 0xC2, 0xC0, 0x01, 0xFB,
			0x01, 0xC0, 0xC2, 0x10, 0x85, 0x20, 0x94, 0x01
		};

			private byte[][] _gf_mul = init_gf256_mul_table();

			private static byte[][] init_gf256_mul_table()
			{
				byte[][] mul_table = new byte[256][];
				for(int x = 0; x < 256; x++)
				{
					mul_table[x] = new byte[256];
					for(int y = 0; y < 256; y++)
					{
						mul_table[x][y] = kuz_mul_gf256_slow((byte)x, (byte)y);
					}
				}
				return mul_table;
			}

			private static byte kuz_mul_gf256_slow(byte a, byte b)
			{
				byte p = 0;
				byte counter;
				byte hi_bit_set;
				unchecked
				{
					for(counter = 0; counter < 8 && a != 0 && b != 0; counter++)
					{
						if((b & 1) != 0)
							p ^= a;
						hi_bit_set = (byte)(a & 0x80);
						a <<= 1;
						if(hi_bit_set != 0)
							a ^= 0xc3; /* x^8 + x^7 + x^6 + x + 1 */
						b >>= 1;
					}
				}
				return p;
			}

			private void S(ref byte[] data)
			{
				for(int i = 0; i < data.Length; i++)
				{
					data[i] = _pi[data[i]];
				}
			}

			private void X(ref byte[] result, ref byte[] data)
			{
				unchecked
				{
					for(int i = 0; i < result.Length; i++)
					{
						result[i] ^= data[i];
					}
				}
			}

			private byte l(ref byte[] data)
			{
				byte x = data[15];

				unchecked
				{
					for(int i = 14; i >= 0; i--)
					{
						x ^= _gf_mul[data[i]][_lFactors[i]];
					}
				}
				return x;
			}

			private void R(ref byte[] data)
			{
				byte z = l(ref data);
				for(int i = 15; i > 0; i--)
				{
					data[i] = data[i - 1];
				}
				data[0] = z;
			}

			private void L(ref byte[] data)
			{
				for(int i = 0; i < 16; i++)
				{
					R(ref data);
				}
			}

			private void F(ref byte[] k, ref byte[] a1, ref byte[] a0)
			{
				byte[] temp = new byte[SUB_LENGTH];

				LSX(ref temp, ref k, ref a1);
				X(ref temp, ref a0);

				Array.Copy(a1, a0, SUB_LENGTH);
				Array.Copy(temp, a1, SUB_LENGTH);

			}

			private void LSX(ref byte[] result, ref byte[] k, ref byte[] a)
			{
				Array.Copy(k, result, BLOCK_SIZE);
				X(ref result, ref a);
				S(ref result);
				L(ref result);
			}

			private void C(ref byte[] c, int i)
			{
				Array.Clear(c, 0, SUB_LENGTH);
				c[15] = (byte)i;
				L(ref c);
			}
		}
	}
}