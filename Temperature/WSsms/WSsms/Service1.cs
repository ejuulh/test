using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;

namespace WSsms
{
	public class Temperature : System.ServiceProcess.ServiceBase
	{
		private System.Timers.Timer timer1;

		IniFile iniTempObj;
		long temp = 0;

		//Ini variabler
		short startmaxTemp = 0;
		short maxTemp1 = 0;
		short maxTemp2 = 0;
		short chan = 0;
		short tempComport = 0;
		int tlf1 = 0;
		int tlf2 = 0;

        AzureIOT iotHUB;
        SMTPClass mail;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

		public Temperature()
		{
			// This call is required by the Windows.Forms Component Designer.
			InitializeComponent();
            this.timer1 = new System.Timers.Timer(10000);
            this.timer1.Enabled = true;
            this.timer1.Interval = 10000;
            this.timer1.Elapsed += new System.Timers.ElapsedEventHandler(this.timer1_Elapsed);
            //this.EventLog.WriteEntry("Temperature", EventLogEntryType.Information);
        }

        // The main entry point for the process
        static void Main(string[] args)
        {
            if ((args != null) && (args.Length > 0) && (args[0].ToLower().StartsWith("/cmd")))
            {

                Temperature svc = new Temperature();
                svc.CmdStart(args);
                Console.WriteLine("Press ENTER to Exit");
                Console.ReadLine();
                svc.CmdStop();

            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                    new Temperature()
                };
                ServiceBase.Run(ServicesToRun);
            }
        }

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
		{
            //this.EventLog.WriteEntry("InitializeComponent", EventLogEntryType.Information);
            components = new System.ComponentModel.Container();
            this.ServiceName = "Temperatur";

        }

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			//comObj.closeComport();
			DllWrapper.th03_close_unit(tempComport);
			
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		/// <summary>
		/// Set things in motion so your service can do its work.
		/// </summary>
		protected override void OnStart(string[] args)
		{

            // TODO: Add code here to start your service.
            //AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            //this.EventLog.WriteEntry("Start", EventLogEntryType.Information);

            this.timer1.Enabled = true;
            this.timer1.Start();
            //mail.SendMail("mail.unigate.dk", false, true, "", "", "", "Temperature@goapplicate.com", "ejh@goapplicate.com", "", "Temperatur", "Temperatur overvågning startet.", null, null);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            this.EventLog.WriteEntry(e.ExceptionObject.ToString(), EventLogEntryType.Error);
        }
        /// <summary>
        /// Stop this service.
        /// </summary>
        protected override void OnStop()
		{
			// TODO: Add code here to perform any tear-down necessary to stop your service.
		}


        public void CmdStart(string[] args)
        {
            OnStart(args);
        }
        public void CmdStop()
        {
            OnStop();
        }

        bool first = true;
        private void timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			//----------------------------------------------------------------------
			// TEMPERATUR PROGRAM
			//----------------------------------------------------------------------
			
			//Console.WriteLine("Temp tjeckes");
            //this.EventLog.WriteEntry("timer1_Elapsed", EventLogEntryType.Information);
            short filtered = 1;
			short ok=0;

            if (first==null || first == true)
            {
                
                first = false;

                //Ini fil variabler
                iniTempObj = new IniFile("C:\\SMS_temp\\iniTemp.ini");
                tempComport = Convert.ToInt16(iniTempObj.IniReadValue("SETTINGS", "TEMP_COMPORT"));
                startmaxTemp = Convert.ToInt16(iniTempObj.IniReadValue("SETTINGS", "maxTemp1"));
                maxTemp2 = Convert.ToInt16(iniTempObj.IniReadValue("SETTINGS", "maxTemp2"));
                chan = Convert.ToInt16(iniTempObj.IniReadValue("SETTINGS", "CHANNEL"));
                tlf1 = Convert.ToInt32(iniTempObj.IniReadValue("SETTINGS", "TLFNR1"));
                tlf2 = Convert.ToInt32(iniTempObj.IniReadValue("SETTINGS", "TLFNR2"));

                

                maxTemp1 = startmaxTemp;
                try
                {
                    iotHUB = new AzureIOT();
                    iotHUB.init();
                }
                catch { }

                this.EventLog.WriteEntry("tempComport " + tempComport.ToString(), EventLogEntryType.Information);

                ////Der læses settings fra InitModem filen
                //iniModemObj = new IniFile("C:\\SMS_temp\\iniModem.ini");
                //modemComport = Convert.ToInt16(iniModemObj.IniReadValue("SETTINGS", "MODEM_COMPORT"));

                ////Comporten åbnes
                //comObj = new Comport("COM" + modemComport + ":");

                //Wrapper class
                ushort th_type = 1; //1 for EL015 temperature sensor
                ushort filter_factor = 10; //A filter_factor of 1 means add all of the difference (effectively no filtering)

                string comport = "OK";
                if ((DllWrapper.th03_open_unit(tempComport) == 0))
                {
                    Console.WriteLine("Open tempCom fail");
                    comport = "Open tempCom fail";
                }

                //this.EventLog.WriteEntry("first_3", EventLogEntryType.Information);

                DllWrapper.th03_set_channel(tempComport, chan, th_type, filter_factor);

                this.EventLog.WriteEntry("first_4", EventLogEntryType.Information);

                mail = new SMTPClass();

                mail.SendMail("mail.unigate.dk", false, true, "", "", "", "Temperature@goapplicate.com", "ejh@goapplicate.com", "", "Temperatur", "Temperatur overvågning startet. " + comport, null, null);
                this.EventLog.WriteEntry("Mail sent", EventLogEntryType.Information);
            }

            //if(doubleSMS == true) // 2 telefonnumre skal have besked
            //{
            //	iniModemObj.IniWriteValue("ACTION", "SENDRDY", "1");
            //	iniModemObj.IniWriteValue("ACTION", "TEXT", "Temperaturen er: " + (temp/100) + " grader.");
            //	iniModemObj.IniWriteValue("ACTION", "TLFNR", tlf2.ToString());
            //	doubleSMS = false;
            //}

            try
			{
				ok = DllWrapper.th03_get_temp(ref temp, tempComport, chan, filtered); // Temperaturen hentes
			}
			catch(Exception ex)
			{
				System.Diagnostics.Trace.WriteLine("get temp: "+ex.Message);
				System.Diagnostics.Trace.WriteLine("OK: "+ok.ToString());
			}


			
			if(ok == 1 && temp > 500 && temp < 10000) // 500 = 5 grader | 10000 = 100 grader
			{
                //iotHUB.SendDeviceToCloudMessagesAsync((double)temp / 100.00);

                //Console.WriteLine("Aflaest temp: " + (temp/100));
				//Console.WriteLine("Max temp: " + maxTemp1);


                /*if((temp/100) > maxTemp2)
				{
					// Gå til ekstremer hvis den går over temp2?
				}*/
                int tempA = (int)(temp / 100);
                if (tempA > maxTemp1)
                {
                    //iniModemObj.IniWriteValue("ACTION", "SENDRDY", "1");
                    //iniModemObj.IniWriteValue("ACTION", "TEXT", "Temperaturen er: " + (temp / 100) + " grader.");
                    //iniModemObj.IniWriteValue("ACTION", "TLFNR", tlf1.ToString());
                    maxTemp1 += 5;
                    mail.SendMail("mail.unigate.dk", false, true, "", "", "", "Temperature@goapplicate.com", "ejh@goapplicate.com", "", "Temperatur", "Temperaturen i serverrummet er: " + tempA.ToString()+ " grader.", null, null);
                    this.EventLog.WriteEntry("Mail sent", EventLogEntryType.Information);
                    //Shutdown shut = new Shutdown();
                    //shut.ShutdownAll();

                    //if (tlf2 > 0)
                    //    doubleSMS = true;
                }
                else if (maxTemp1 > startmaxTemp && (startmaxTemp - 2) > (temp / 100)) //maxTemp1 sænkes hvis temperaturen falder igen
                {
                    maxTemp1 = startmaxTemp;
                    mail.SendMail("mail.unigate.dk", false, true, "", "", "", "Temperature@goapplicate.com", "ejh@goapplicate.com", "", "Temperatur", "Temperaturen i serverrummet er: " + tempA.ToString() + " grader.", null, null);
                    this.EventLog.WriteEntry("Mail sent", EventLogEntryType.Information);
                }

            }
			else
			{
				//Console.WriteLine("Read tempCom fail");
	
				//if(readStatus == true)
				//{
				//	iniModemObj.IniWriteValue("ACTION", "SENDRDY", "1");
				//	iniModemObj.IniWriteValue("ACTION", "TEXT", "Fejl ved aflaesning af temperaturmaaler.");
				//	iniModemObj.IniWriteValue("ACTION", "TLFNR", tlf1.ToString());
				//	readStatus = false;
				//}
			}
		}


		
	}
}
