using System;
using System.Runtime.InteropServices;

namespace WSsms
{

	public class DllWrapper
	{
		//constructor
		public DllWrapper(short port, short chan)
		{
			ushort th_type = 1; //1 for EL015 temperature sensor
			ushort filter_factor = 1; //A filter_factor of 1 means add all of the difference (effectively no filtering)

			if((th03_open_unit(port)) == 0)
				Console.WriteLine("Open tempCom fail");
			else
				Console.WriteLine("Open tempCom GOOD");
			
			th03_set_channel (port, chan, th_type, filter_factor);
		}
		//deconstructor
		~DllWrapper()
		{
		
		}

		[DllImport("th0332.dll")]
		public static extern short th03_open_unit (short port);

		[DllImport("th0332.dll")]
		public static extern void th03_close_unit (short port);

		[DllImport("th0332.dll")]
		public static extern void th03_poll_driver ();

		[DllImport("th0332.dll")]
		public static extern unsafe short th03_get_cycle(long *cycle, short port);

		[DllImport("th0332.dll")]
		public static extern void th03_set_channel (short port, short channel, ushort th_type, ushort filter_factor);

		[DllImport("th0332.dll", SetLastError=true)]
		public static extern short th03_get_temp (ref long temp, short port, short channel, short filtered);

		[DllImport("th0332.dll")]
		public static extern short th03_set_ref_update (short port, short update_interval);

		[DllImport("th0332.dll")]
		public static extern unsafe short th03_get_version (short *version, short port);
	}
}
