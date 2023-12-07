#if false
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace FixedPoint
{
	public struct FixedPointTest
	{
		public static string Dump(Fixed value)
		{
			StringBuilder buffer = new StringBuilder();

			buffer.Append("toString=" + value.ToString());
			buffer.Append(" rawValue=" + value.raw.ToString());
			buffer.Append(" binary=" + value.ToBinaryString());
			buffer.Append(" (long)=" + ((long)value).ToString());
			buffer.Append(" (float)=" + ((float)value).ToString());
			buffer.Append(" (dounle)=" + ((double)value).ToString());
			buffer.Append(" (decimal)=" + ((decimal)value).ToString());

			return buffer.ToString();
		}

		public static string Dump(float value)
		{
			StringBuilder buffer = new StringBuilder();

			buffer.Append("toString=" + value.ToString());
			buffer.Append(" (long)=" + ((long)value).ToString());

			return buffer.ToString();
		}

		public static void Test()
		{
			float f = -1.5555f;
			Fixed fp1 = (Fixed)f;

			Console.WriteLine(Dump(fp1));
			Console.WriteLine(Dump(f));

			long l = -9L;
			Fixed fp2 = (Fixed)l;

			Console.WriteLine(Dump(fp2));
			Console.WriteLine(l);

			Fixed fp3 = 2 * fp2 * 5;
			Console.WriteLine(Dump(fp3));

			{
				Console.WriteLine("*");

				Fixed fp4 = fp3 * fp2;
				Console.WriteLine(Dump(fp4));

				Fixed fp5 = fp3 * -9;
				Console.WriteLine(Dump(fp5));

				Fixed fp6 = -90 * fp2;
				Console.WriteLine(Dump(fp6));
			}

			{
				Console.WriteLine("/");

				Fixed fp4 = fp3 / fp2;
				Console.WriteLine(Dump(fp4));

				Fixed fp5 = fp3 / -9;
				Console.WriteLine(Dump(fp5));

				Fixed fp6 = -90 / fp2;
				Console.WriteLine(Dump(fp6));
			}

			{
				Console.WriteLine("%");

				Fixed fp4 = fp3 % fp2;
				Console.WriteLine(Dump(fp4));

				Fixed fp5 = fp3 % -9;
				Console.WriteLine(Dump(fp5));

				Fixed fp6 = -90 % fp2;
				Console.WriteLine(Dump(fp6));
			}

			{
				Console.WriteLine("+");
				Fixed fp4 = fp3 + fp2;
				Console.WriteLine(Dump(fp4));

				Fixed fp5 = fp3 + -9;
				Console.WriteLine(Dump(fp5));

				Fixed fp6 = -90 + fp2;
				Console.WriteLine(Dump(fp6));
			}

			{
				Console.WriteLine("-");
				Fixed fp4 = fp3 - fp2;
				Console.WriteLine(Dump(fp4));

				Fixed fp5 = fp3 - -9;
				Console.WriteLine(Dump(fp5));

				Fixed fp6 = -90 - fp2;
				Console.WriteLine(Dump(fp6));
			}

			{
				Console.WriteLine("-fp");
				Fixed fp4 = -fp2;
				Console.WriteLine(Dump(fp4));
			}

			{
				Console.WriteLine("fp<<5");
				Fixed fp4 = fp2 << 5;
				Console.WriteLine(Dump(fp4));
			}

			{
				Console.WriteLine("fp>>5");
				Fixed fp4 = fp2 >> 5;
				Console.WriteLine(Dump(fp4));
			}

			{
				Fixed fp4 = fp3;
				Console.WriteLine("==");
				Console.WriteLine(fp3 == fp4);
				Console.WriteLine(-90 == fp3);
				Console.WriteLine(fp3 == -90);
			}

			{
				Fixed fp4 = fp3;
				Console.WriteLine("!=");
				Console.WriteLine(fp3 != fp4);
				Console.WriteLine(-90 != fp3);
				Console.WriteLine(fp3 != -90);
			}

			/*
					{
						long result = 0;
						int count = 100000000;
						PerformanceMeter.Sart();
						for(int repeat = 0; repeat < count; repeat++)
							result = (long)fp1;
						PerformanceMeter.Stop();
						Console.WriteLine(result);
					}
			*/

			{
				float e = 100010001;// (float)Math.PI;
				Fixed fp = (Fixed)e;

				Fixed result = (Fixed)0;
				float eresult = 0;
				int count = 1000000;

				PerformanceMeter.Prepare();

				PerformanceMeter.Sart(false);
				for(int repeat = 0; repeat < count; repeat++)
					result = Math.Sqrt(fp);
				PerformanceMeter.Stop(false);

				PerformanceMeter.Sart(false);
				for(int repeat = 0; repeat < count; repeat++)
					eresult = (float)System.Math.Sqrt(e);
				PerformanceMeter.Stop(false);

				PerformanceMeter.UnPrepare();

				Console.WriteLine("{0:#,000.00000}", fp);
				Console.WriteLine("{0:#,000.00000}", result);
				Console.WriteLine("{0:#,000.00000}", eresult);
			}

			{
				float e = (float)System.Math.PI / 2 - (float)System.Math.PI * 700;
				Fixed fp = (Fixed)e;

				Fixed result = (Fixed)0;
				float eresult = 0;
				int count = 1000000;

				PerformanceMeter.Prepare();

				PerformanceMeter.Sart(false);
				for(int repeat = 0; repeat < count; repeat++)
					result = Math.Sin(fp);
				PerformanceMeter.Stop(false);

				PerformanceMeter.Sart(false);
				for(int repeat = 0; repeat < count; repeat++)
					eresult = (float)System.Math.Sin(e);
				PerformanceMeter.Stop(false);

				PerformanceMeter.UnPrepare();

				Console.WriteLine("{0:#,000.00000}", Math.Epsilon);
				Console.WriteLine("{0:#,000.00000}", Math.PI);
				Console.WriteLine("{0:#,000.00000}", Math.E);

				Console.WriteLine("{0:#,000.00000}", fp);
				Console.WriteLine("{0:#,000.00000}", result);
				Console.WriteLine("{0:#,000.00000}", eresult);

				Console.WriteLine(Vector2.XYAxis.ToString());
			}


			{
				float e = (float)System.Math.PI / 2 - (float)System.Math.PI * 700;
				Fixed fp = (Fixed)e;

				Fixed result = (Fixed)0;
				float eresult = 0;
				int count = 1000000;

				PerformanceMeter.Prepare();

				PerformanceMeter.Sart(false);
				for(int repeat = 0; repeat < count; repeat++)
					result = Math.Atan(fp);
				PerformanceMeter.Stop(false);

				PerformanceMeter.Sart(false);
				for(int repeat = 0; repeat < count; repeat++)
					eresult = (float)System.Math.Atan(e);
				PerformanceMeter.Stop(false);

				PerformanceMeter.UnPrepare();

				Console.WriteLine("{0:#,000.00000}", fp);
				Console.WriteLine("{0:#,000.00000}", result);
				Console.WriteLine("{0:#,000.00000}", eresult);
			}


			{
				float e = (float)System.Math.PI / 5;// - (float)Math.PI * 700;
				Fixed fp = (Fixed)e;

				Fixed result = (Fixed)0;
				float eresult = 0;
				int count = 1000000;

				PerformanceMeter.Prepare();

				PerformanceMeter.Sart(false);
				for(int repeat = 0; repeat < count; repeat++)
					result = Math.Cos(fp);
				PerformanceMeter.Stop(false);

				PerformanceMeter.Sart(false);
				for(int repeat = 0; repeat < count; repeat++)
					eresult = (float)System.Math.Cos(e);
				PerformanceMeter.Stop(false);

				PerformanceMeter.UnPrepare();


				Console.WriteLine("{0:#,000.00000}", fp);
				Console.WriteLine("{0:#,000.00000}", result);
				Console.WriteLine("{0:#,000.00000}", eresult);
			}

			{
				float e = -1.5f;
				Fixed fp = (Fixed)e;

				Fixed result = Math.Round(fp);
				float eresult = (float)System.Math.Round(e);

				Console.WriteLine("{0:#,000.00000}", fp);
				Console.WriteLine("{0:#,000.00000}", result);
				Console.WriteLine("{0:#,000.00000}", eresult);
			}

			{
				float e = -1.5f;
				Fixed fp = (Fixed)e;

				Fixed result = Math.Ceiling(fp);
				float eresult = (float)System.Math.Ceiling(e);

				Console.WriteLine("{0:#,000.00000}", fp);
				Console.WriteLine("{0:#,000.00000}", result);
				Console.WriteLine("{0:#,000.00000}", eresult);
			}

			{
				float e = -1.5f;
				Fixed fp = (Fixed)e;

				Fixed result = Math.Floor(fp);
				float eresult = (float)System.Math.Floor(e);

				Console.WriteLine("{0:#,000.00000}", fp);
				Console.WriteLine("{0:#,000.00000}", result);
				Console.WriteLine("{0:#,000.00000}", eresult);
			}

			{
				Fixed angle = -Math.PI / 4;

				Vector2 r = Vector2.XAxis * (Fixed)10;
				Vector2 v = Vector2.Polar(angle);
				Vector2 rv = r.Rotate(v);

				Fixed av = v.Angle;
				Fixed arv = rv.Angle;

				Console.WriteLine(angle);
				Console.WriteLine(r);
				Console.WriteLine(v);
				Console.WriteLine(rv);
				Console.WriteLine(av);
				Console.WriteLine(arv);
			}
		}
	}
}
#endif