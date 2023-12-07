using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace HEXPLAY
{
	public static class Hash
	{
		static MD5 md5 = MD5.Create();
		public static byte[] Md5(byte[] bytes)
		{
			return md5.ComputeHash(bytes);
		}

		static SHA1 sha1 = SHA1.Create();
		public static byte[] Sha1(byte[] bytes)
		{
			return sha1.ComputeHash(bytes);
		}

		static readonly uint[] tableCrc32 = new uint[]{
				0x00000000, 0x77073096, 0xEE0E612C, 0x990951BA, 0x076DC419, 0x706AF48F,
				0xE963A535, 0x9E6495A3, 0x0EDB8832, 0x79DCB8A4, 0xE0D5E91E, 0x97D2D988,
				0x09B64C2B, 0x7EB17CBD, 0xE7B82D07, 0x90BF1D91, 0x1DB71064, 0x6AB020F2,
				0xF3B97148, 0x84BE41DE, 0x1ADAD47D, 0x6DDDE4EB, 0xF4D4B551, 0x83D385C7,
				0x136C9856, 0x646BA8C0, 0xFD62F97A, 0x8A65C9EC, 0x14015C4F, 0x63066CD9,
				0xFA0F3D63, 0x8D080DF5, 0x3B6E20C8, 0x4C69105E, 0xD56041E4, 0xA2677172,
				0x3C03E4D1, 0x4B04D447, 0xD20D85FD, 0xA50AB56B, 0x35B5A8FA, 0x42B2986C,
				0xDBBBC9D6, 0xACBCF940, 0x32D86CE3, 0x45DF5C75, 0xDCD60DCF, 0xABD13D59,
				0x26D930AC, 0x51DE003A, 0xC8D75180, 0xBFD06116, 0x21B4F4B5, 0x56B3C423,
				0xCFBA9599, 0xB8BDA50F, 0x2802B89E, 0x5F058808, 0xC60CD9B2, 0xB10BE924,
				0x2F6F7C87, 0x58684C11, 0xC1611DAB, 0xB6662D3D, 0x76DC4190, 0x01DB7106,
				0x98D220BC, 0xEFD5102A, 0x71B18589, 0x06B6B51F, 0x9FBFE4A5, 0xE8B8D433,
				0x7807C9A2, 0x0F00F934, 0x9609A88E, 0xE10E9818, 0x7F6A0DBB, 0x086D3D2D,
				0x91646C97, 0xE6635C01, 0x6B6B51F4, 0x1C6C6162, 0x856530D8, 0xF262004E,
				0x6C0695ED, 0x1B01A57B, 0x8208F4C1, 0xF50FC457, 0x65B0D9C6, 0x12B7E950,
				0x8BBEB8EA, 0xFCB9887C, 0x62DD1DDF, 0x15DA2D49, 0x8CD37CF3, 0xFBD44C65,
				0x4DB26158, 0x3AB551CE, 0xA3BC0074, 0xD4BB30E2, 0x4ADFA541, 0x3DD895D7,
				0xA4D1C46D, 0xD3D6F4FB, 0x4369E96A, 0x346ED9FC, 0xAD678846, 0xDA60B8D0,
				0x44042D73, 0x33031DE5, 0xAA0A4C5F, 0xDD0D7CC9, 0x5005713C, 0x270241AA,
				0xBE0B1010, 0xC90C2086, 0x5768B525, 0x206F85B3, 0xB966D409, 0xCE61E49F,
				0x5EDEF90E, 0x29D9C998, 0xB0D09822, 0xC7D7A8B4, 0x59B33D17, 0x2EB40D81,
				0xB7BD5C3B, 0xC0BA6CAD, 0xEDB88320, 0x9ABFB3B6, 0x03B6E20C, 0x74B1D29A,
				0xEAD54739, 0x9DD277AF, 0x04DB2615, 0x73DC1683, 0xE3630B12, 0x94643B84,
				0x0D6D6A3E, 0x7A6A5AA8, 0xE40ECF0B, 0x9309FF9D, 0x0A00AE27, 0x7D079EB1,
				0xF00F9344, 0x8708A3D2, 0x1E01F268, 0x6906C2FE, 0xF762575D, 0x806567CB,
				0x196C3671, 0x6E6B06E7, 0xFED41B76, 0x89D32BE0, 0x10DA7A5A, 0x67DD4ACC,
				0xF9B9DF6F, 0x8EBEEFF9, 0x17B7BE43, 0x60B08ED5, 0xD6D6A3E8, 0xA1D1937E,
				0x38D8C2C4, 0x4FDFF252, 0xD1BB67F1, 0xA6BC5767, 0x3FB506DD, 0x48B2364B,
				0xD80D2BDA, 0xAF0A1B4C, 0x36034AF6, 0x41047A60, 0xDF60EFC3, 0xA867DF55,
				0x316E8EEF, 0x4669BE79, 0xCB61B38C, 0xBC66831A, 0x256FD2A0, 0x5268E236,
				0xCC0C7795, 0xBB0B4703, 0x220216B9, 0x5505262F, 0xC5BA3BBE, 0xB2BD0B28,
				0x2BB45A92, 0x5CB36A04, 0xC2D7FFA7, 0xB5D0CF31, 0x2CD99E8B, 0x5BDEAE1D,
				0x9B64C2B0, 0xEC63F226, 0x756AA39C, 0x026D930A, 0x9C0906A9, 0xEB0E363F,
				0x72076785, 0x05005713, 0x95BF4A82, 0xE2B87A14, 0x7BB12BAE, 0x0CB61B38,
				0x92D28E9B, 0xE5D5BE0D, 0x7CDCEFB7, 0x0BDBDF21, 0x86D3D2D4, 0xF1D4E242,
				0x68DDB3F8, 0x1FDA836E, 0x81BE16CD, 0xF6B9265B, 0x6FB077E1, 0x18B74777,
				0x88085AE6, 0xFF0F6A70, 0x66063BCA, 0x11010B5C, 0x8F659EFF, 0xF862AE69,
				0x616BFFD3, 0x166CCF45, 0xA00AE278, 0xD70DD2EE, 0x4E048354, 0x3903B3C2,
				0xA7672661, 0xD06016F7, 0x4969474D, 0x3E6E77DB, 0xAED16A4A, 0xD9D65ADC,
				0x40DF0B66, 0x37D83BF0, 0xA9BCAE53, 0xDEBB9EC5, 0x47B2CF7F, 0x30B5FFE9,
				0xBDBDF21C, 0xCABAC28A, 0x53B39330, 0x24B4A3A6, 0xBAD03605, 0xCDD70693,
				0x54DE5729, 0x23D967BF, 0xB3667A2E, 0xC4614AB8, 0x5D681B02, 0x2A6F2B94,
				0xB40BBE37, 0xC30C8EA1, 0x5A05DF1B, 0x2D02EF8D
			};

		public static uint Crc32(byte[] bytes)
		{
			unchecked
			{
				uint crc = (uint)(((uint)0) ^ (-1));

				int len = bytes.Length;
				for(int i = 0; i < len; i++)
					crc = (crc >> 8) ^ tableCrc32[(byte)(crc ^ bytes[i])];

				crc = (uint)(crc ^ (-1));

				if(crc < 0)
					crc += (uint)4294967296;

				return crc;
			}
		}

		public static byte[] Crc32Bytes(byte[] bytes)
		{
			return BitConverter.GetBytes(Crc32(bytes));
		}

		//http://mjs5.com/2016/03/17/c-standard-crc64-hashing-algorithm/
		static readonly ulong[] tableCrc64 = {
			0x0000000000000000, 0x7ad870c830358979,
			0xf5b0e190606b12f2, 0x8f689158505e9b8b,
			0xc038e5739841b68f, 0xbae095bba8743ff6,
			0x358804e3f82aa47d, 0x4f50742bc81f2d04,
			0xab28ecb46814fe75, 0xd1f09c7c5821770c,
			0x5e980d24087fec87, 0x24407dec384a65fe,
			0x6b1009c7f05548fa, 0x11c8790fc060c183,
			0x9ea0e857903e5a08, 0xe478989fa00bd371,
			0x7d08ff3b88be6f81, 0x07d08ff3b88be6f8,
			0x88b81eabe8d57d73, 0xf2606e63d8e0f40a,
			0xbd301a4810ffd90e, 0xc7e86a8020ca5077,
			0x4880fbd87094cbfc, 0x32588b1040a14285,
			0xd620138fe0aa91f4, 0xacf86347d09f188d,
			0x2390f21f80c18306, 0x594882d7b0f40a7f,
			0x1618f6fc78eb277b, 0x6cc0863448deae02,
			0xe3a8176c18803589, 0x997067a428b5bcf0,
			0xfa11fe77117cdf02, 0x80c98ebf2149567b,
			0x0fa11fe77117cdf0, 0x75796f2f41224489,
			0x3a291b04893d698d, 0x40f16bccb908e0f4,
			0xcf99fa94e9567b7f, 0xb5418a5cd963f206,
			0x513912c379682177, 0x2be1620b495da80e,
			0xa489f35319033385, 0xde51839b2936bafc,
			0x9101f7b0e12997f8, 0xebd98778d11c1e81,
			0x64b116208142850a, 0x1e6966e8b1770c73,
			0x8719014c99c2b083, 0xfdc17184a9f739fa,
			0x72a9e0dcf9a9a271, 0x08719014c99c2b08,
			0x4721e43f0183060c, 0x3df994f731b68f75,
			0xb29105af61e814fe, 0xc849756751dd9d87,
			0x2c31edf8f1d64ef6, 0x56e99d30c1e3c78f,
			0xd9810c6891bd5c04, 0xa3597ca0a188d57d,
			0xec09088b6997f879, 0x96d1784359a27100,
			0x19b9e91b09fcea8b, 0x636199d339c963f2,
			0xdf7adabd7a6e2d6f, 0xa5a2aa754a5ba416,
			0x2aca3b2d1a053f9d, 0x50124be52a30b6e4,
			0x1f423fcee22f9be0, 0x659a4f06d21a1299,
			0xeaf2de5e82448912, 0x902aae96b271006b,
			0x74523609127ad31a, 0x0e8a46c1224f5a63,
			0x81e2d7997211c1e8, 0xfb3aa75142244891,
			0xb46ad37a8a3b6595, 0xceb2a3b2ba0eecec,
			0x41da32eaea507767, 0x3b024222da65fe1e,
			0xa2722586f2d042ee, 0xd8aa554ec2e5cb97,
			0x57c2c41692bb501c, 0x2d1ab4dea28ed965,
			0x624ac0f56a91f461, 0x1892b03d5aa47d18,
			0x97fa21650afae693, 0xed2251ad3acf6fea,
			0x095ac9329ac4bc9b, 0x7382b9faaaf135e2,
			0xfcea28a2faafae69, 0x8632586aca9a2710,
			0xc9622c4102850a14, 0xb3ba5c8932b0836d,
			0x3cd2cdd162ee18e6, 0x460abd1952db919f,
			0x256b24ca6b12f26d, 0x5fb354025b277b14,
			0xd0dbc55a0b79e09f, 0xaa03b5923b4c69e6,
			0xe553c1b9f35344e2, 0x9f8bb171c366cd9b,
			0x10e3202993385610, 0x6a3b50e1a30ddf69,
			0x8e43c87e03060c18, 0xf49bb8b633338561,
			0x7bf329ee636d1eea, 0x012b592653589793,
			0x4e7b2d0d9b47ba97, 0x34a35dc5ab7233ee,
			0xbbcbcc9dfb2ca865, 0xc113bc55cb19211c,
			0x5863dbf1e3ac9dec, 0x22bbab39d3991495,
			0xadd33a6183c78f1e, 0xd70b4aa9b3f20667,
			0x985b3e827bed2b63, 0xe2834e4a4bd8a21a,
			0x6debdf121b863991, 0x1733afda2bb3b0e8,
			0xf34b37458bb86399, 0x8993478dbb8deae0,
			0x06fbd6d5ebd3716b, 0x7c23a61ddbe6f812,
			0x3373d23613f9d516, 0x49aba2fe23cc5c6f,
			0xc6c333a67392c7e4, 0xbc1b436e43a74e9d,
			0x95ac9329ac4bc9b5, 0xef74e3e19c7e40cc,
			0x601c72b9cc20db47, 0x1ac40271fc15523e,
			0x5594765a340a7f3a, 0x2f4c0692043ff643,
			0xa02497ca54616dc8, 0xdafce7026454e4b1,
			0x3e847f9dc45f37c0, 0x445c0f55f46abeb9,
			0xcb349e0da4342532, 0xb1eceec59401ac4b,
			0xfebc9aee5c1e814f, 0x8464ea266c2b0836,
			0x0b0c7b7e3c7593bd, 0x71d40bb60c401ac4,
			0xe8a46c1224f5a634, 0x927c1cda14c02f4d,
			0x1d148d82449eb4c6, 0x67ccfd4a74ab3dbf,
			0x289c8961bcb410bb, 0x5244f9a98c8199c2,
			0xdd2c68f1dcdf0249, 0xa7f41839ecea8b30,
			0x438c80a64ce15841, 0x3954f06e7cd4d138,
			0xb63c61362c8a4ab3, 0xcce411fe1cbfc3ca,
			0x83b465d5d4a0eece, 0xf96c151de49567b7,
			0x76048445b4cbfc3c, 0x0cdcf48d84fe7545,
			0x6fbd6d5ebd3716b7, 0x15651d968d029fce,
			0x9a0d8ccedd5c0445, 0xe0d5fc06ed698d3c,
			0xaf85882d2576a038, 0xd55df8e515432941,
			0x5a3569bd451db2ca, 0x20ed197575283bb3,
			0xc49581ead523e8c2, 0xbe4df122e51661bb,
			0x3125607ab548fa30, 0x4bfd10b2857d7349,
			0x04ad64994d625e4d, 0x7e7514517d57d734,
			0xf11d85092d094cbf, 0x8bc5f5c11d3cc5c6,
			0x12b5926535897936, 0x686de2ad05bcf04f,
			0xe70573f555e26bc4, 0x9ddd033d65d7e2bd,
			0xd28d7716adc8cfb9, 0xa85507de9dfd46c0,
			0x273d9686cda3dd4b, 0x5de5e64efd965432,
			0xb99d7ed15d9d8743, 0xc3450e196da80e3a,
			0x4c2d9f413df695b1, 0x36f5ef890dc31cc8,
			0x79a59ba2c5dc31cc, 0x037deb6af5e9b8b5,
			0x8c157a32a5b7233e, 0xf6cd0afa9582aa47,
			0x4ad64994d625e4da, 0x300e395ce6106da3,
			0xbf66a804b64ef628, 0xc5bed8cc867b7f51,
			0x8aeeace74e645255, 0xf036dc2f7e51db2c,
			0x7f5e4d772e0f40a7, 0x05863dbf1e3ac9de,
			0xe1fea520be311aaf, 0x9b26d5e88e0493d6,
			0x144e44b0de5a085d, 0x6e963478ee6f8124,
			0x21c640532670ac20, 0x5b1e309b16452559,
			0xd476a1c3461bbed2, 0xaeaed10b762e37ab,
			0x37deb6af5e9b8b5b, 0x4d06c6676eae0222,
			0xc26e573f3ef099a9, 0xb8b627f70ec510d0,
			0xf7e653dcc6da3dd4, 0x8d3e2314f6efb4ad,
			0x0256b24ca6b12f26, 0x788ec2849684a65f,
			0x9cf65a1b368f752e, 0xe62e2ad306bafc57,
			0x6946bb8b56e467dc, 0x139ecb4366d1eea5,
			0x5ccebf68aecec3a1, 0x2616cfa09efb4ad8,
			0xa97e5ef8cea5d153, 0xd3a62e30fe90582a,
			0xb0c7b7e3c7593bd8, 0xca1fc72bf76cb2a1,
			0x45775673a732292a, 0x3faf26bb9707a053,
			0x70ff52905f188d57, 0x0a2722586f2d042e,
			0x854fb3003f739fa5, 0xff97c3c80f4616dc,
			0x1bef5b57af4dc5ad, 0x61372b9f9f784cd4,
			0xee5fbac7cf26d75f, 0x9487ca0fff135e26,
			0xdbd7be24370c7322, 0xa10fceec0739fa5b,
			0x2e675fb4576761d0, 0x54bf2f7c6752e8a9,
			0xcdcf48d84fe75459, 0xb71738107fd2dd20,
			0x387fa9482f8c46ab, 0x42a7d9801fb9cfd2,
			0x0df7adabd7a6e2d6, 0x772fdd63e7936baf,
			0xf8474c3bb7cdf024, 0x829f3cf387f8795d,
			0x66e7a46c27f3aa2c, 0x1c3fd4a417c62355,
			0x935745fc4798b8de, 0xe98f353477ad31a7,
			0xa6df411fbfb21ca3, 0xdc0731d78f8795da,
			0x536fa08fdfd90e51, 0x29b7d047efec8728,
		};

		public static ulong Crc64(byte[] bytes, ulong crc = 0)
		{
			unchecked
			{
				int len = bytes.Length;
				for(int j = 0; j < len; j++)
					crc = tableCrc64[(byte)(crc ^ bytes[j])] ^ (crc >> 8);
			}

			return crc;
		}

		public static byte[] Crc64Bytes(byte[] bytes)
		{
			return BitConverter.GetBytes(Crc64(bytes));
		}

		// https://en.wikipedia.org/wiki/MurmurHash
		public static uint Murmur3(byte[] bytes, uint seed = 144)
		{
			const uint c1 = 0xcc9e2d51;
			const uint c2 = 0x1b873593;

			uint h1 = seed;
			uint k1 = 0;

			// TODO: Parallelize
			byte[] chunk = new byte[4];
			unchecked
			{
				for(int i = 0; i < bytes.Length; i += 4)
				{
					switch(bytes.Length - i)
					{
						case 3:
						k1 = (uint)
							(chunk[0]
							| chunk[1] << 8
							| chunk[2] << 16);
						k1 *= c1;
						k1 = rotl32(k1, 15);
						k1 *= c2;
						h1 ^= k1;
						break;

						case 2:
						k1 = (uint)
							(chunk[0]
							| chunk[1] << 8);
						k1 *= c1;
						k1 = rotl32(k1, 15);
						k1 *= c2;
						h1 ^= k1;
						break;

						case 1:
						k1 = (uint)(chunk[0]);
						k1 *= c1;
						k1 = rotl32(k1, 15);
						k1 *= c2;
						h1 ^= k1;
						break;

						default:
						chunk[0] = bytes[i];
						chunk[1] = bytes[i + 1];
						chunk[2] = bytes[i + 2];
						chunk[3] = bytes[i + 3];

						/* Get four bytes from the input into an uint */
						k1 = (uint)
							(chunk[0]
							| chunk[1] << 8
							| chunk[2] << 16
							| chunk[3] << 24);

						/* bitmagic hash */
						k1 *= c1;
						k1 = rotl32(k1, 15);
						k1 *= c2;

						h1 ^= k1;
						h1 = rotl32(h1, 13);
						h1 = h1 * 5 + 0xe6546b64;
						break;
					}
				}
			}

			// finalization, magic chants to wrap it all up
			h1 ^= (uint)bytes.Length;
			h1 = fmix(h1);

			return h1;
		}

		private static uint rotl32(uint x, byte r)
		{
			return unchecked((x << r) | (x >> (32 - r)));
		}

		private static uint fmix(uint h)
		{
			unchecked
			{
				h ^= h >> 16;
				h *= 0x85ebca6b;
				h ^= h >> 13;
				h *= 0xc2b2ae35;
				h ^= h >> 16;
			}
			return h;
		}
	}
}