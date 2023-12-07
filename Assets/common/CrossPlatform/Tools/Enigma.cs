using System;

namespace HEXPLAY
{
	public static class Enigma
	{
		static PseudoRandom pr = PseudoRandom.GetRest();

		static void Xor(byte[] bytes, byte[] key)
		{
			unchecked
			{
				for(int i = 0; i < bytes.Length; i++)
				{
					int iKey = i % key.Length;
					byte b = bytes[i];
					bytes[i] ^= key[iKey];
					key[iKey] = (byte)(b ^ ~(key[iKey] - 1));
				}
			}
		}

		static void UnXor(byte[] bytes, byte[] key)
		{
			unchecked
			{
				for(int i = 0; i < bytes.Length; i++)
				{
					int iKey = i % key.Length;
					bytes[i] ^= key[iKey];
					byte b = bytes[i];
					key[iKey] = (byte)(b ^ ~(key[iKey] - 1));
				}
			}
		}

		static byte[] Code(byte[] data, byte[] key, Func<byte[], byte[]> hash)
		{
			byte[] dataHash = hash(data);

			byte[] salt = BitConverter.GetBytes(pr.Random());
			byte[] keySalted = key.BlockCopy().Xor(salt);

			byte[] dataCoded = new byte[dataHash.Length + data.Length];
			Buffer.BlockCopy(dataHash, 0, dataCoded, 0, dataHash.Length);
			Buffer.BlockCopy(data, 0, dataCoded, dataHash.Length, data.Length);

			Xor(dataCoded, keySalted);

			byte[] result = new byte[salt.Length + dataCoded.Length];
			Buffer.BlockCopy(salt, 0, result, 0, salt.Length);
			Buffer.BlockCopy(dataCoded, 0, result, salt.Length, dataCoded.Length);

			return result;
		}

		static byte[] Decode(byte[] data, byte[] key, Func<byte[], byte[]> hash, int hashSize)
		{
			byte[] salt = new byte[8];
			Buffer.BlockCopy(data, 0, salt, 0, salt.Length);
			byte[] keySalted = key.BlockCopy().Xor(salt);

			byte[] dataEncoded = new byte[data.Length - 8];
			Buffer.BlockCopy(data, 8, dataEncoded, 0, dataEncoded.Length);
			UnXor(dataEncoded, keySalted);

			byte[] resultData = new byte[dataEncoded.Length - hashSize];
			Buffer.BlockCopy(dataEncoded, hashSize, resultData, 0, resultData.Length);

			byte[] resultDataHash = hash(resultData);

			byte[] dataEncodedHash = new byte[hashSize];
			Buffer.BlockCopy(dataEncoded, 0, dataEncodedHash, 0, dataEncodedHash.Length);

			if(resultDataHash.IsEqual(dataEncodedHash))
				return resultData;

			return null;
		}

		public static byte[] CodeMd5(byte[] data, byte[] key)
		{
			return Code(data, key, Hash.Md5);
		}

		public static byte[] DecodeMd5(byte[] data, byte[] key)
		{
			return Decode(data, key, Hash.Md5, 16);
		}

		public static byte[] CodeCrc32(byte[] data, byte[] key)
		{
			return Code(data, key, Hash.Crc32Bytes);
		}

		public static byte[] DecodeCrc32(byte[] data, byte[] key)
		{
			return Decode(data, key, Hash.Crc32Bytes, 4);
		}

		public static byte[] CodeCrc64(byte[] data, byte[] key)
		{
			return Code(data, key, Hash.Crc64Bytes);
		}

		public static byte[] DecodeCrc64(byte[] data, byte[] key)
		{
			return Decode(data, key, Hash.Crc64Bytes, 8);
		}
	}
}