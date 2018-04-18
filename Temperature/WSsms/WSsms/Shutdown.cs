using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace WSsms
{
    class Shutdown
    {
        public void ShutdownAll()
        {
            //testSaveList();
            

            ShutdownMembers mem = null;
            string path = @"C:\SMS_temp\members.xml";

            XmlSerializer serializer = new XmlSerializer(typeof(ShutdownMembers));

            StreamReader reader = new StreamReader(path);
            mem = (ShutdownMembers)serializer.Deserialize(reader);
            reader.Close();

            foreach (var member in mem.MemberList)
            {
                ShutdownPC(member.Name);
                //Console.WriteLine("Shutdown " + member.Name);
            }

        }

        private void ShutdownPC(string pcName)
        {
            // Start the child process.
            Process p = new Process();
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = @"C:\SMS_temp\ShutdownApp.exe";
            p.StartInfo.Arguments = pcName;
            p.Start();
            // Do not wait for the child process to exit before
            // reading to the end of its redirected stream.
            // p.WaitForExit();
            // Read the output stream first and then wait.
            //string output = p.StandardOutput.ReadToEnd();
            //p.WaitForExit();
        }

        //public string SerializeToString(object obj, string rootString)
        //{
        //    string rtnString = "";
        //    XmlRootAttribute xRoot = new XmlRootAttribute();
        //    xRoot.ElementName = rootString;

        //    //Leak
        //    //XmlSerializer serializer = new XmlSerializer(obj.GetType(), xRoot);
        //    XmlSerializer serializer = xmlSer.Create(obj.GetType(), xRoot);

        //    using (StingWriterEncode writer = new StingWriterEncode(Encoding.GetEncoding(1252)))
        //    {
        //        serializer.Serialize(writer, obj);
        //        rtnString = writer.ToString();
        //    }
        //    return rtnString;
        //}

        private void TestSerializeToFile(object obj, string rootString)
        {
            XmlRootAttribute xRoot = new XmlRootAttribute();
            xRoot.ElementName = rootString;

            System.Xml.Serialization.XmlSerializer writer =
                new System.Xml.Serialization.XmlSerializer(obj.GetType(), xRoot);

            System.IO.StreamWriter file = new System.IO.StreamWriter(
                @"C:\SMS_temp\members.xml");
            writer.Serialize(file, obj);
            file.Close();
        }

        private void testSaveList()
        {
            ShutdownMembers test = new ShutdownMembers();
            List<Member> MemberList = new List<Member>();
            Member mem1 = new Member();
            mem1.IP = "1.2.3.4";
            mem1.Name = "DRStestSQL";
            MemberList.Add(mem1);

            Member mem2 = new Member();
            mem2.IP = "8.8.8.8";
            mem2.Name = "google";
            MemberList.Add(mem2);

            test.MemberList = MemberList;

            //TestSerializeToFile(test, "Hej");
            XmlSerializer xsSubmit = new XmlSerializer(typeof(ShutdownMembers));
            var subReq = test;
            var xml = "";

            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    xsSubmit.Serialize(writer, subReq);
                    xml = sww.ToString(); // Your XML
                }
            }
        }

        public ShutdownMembers GetMemberList()
        {
            ShutdownMembers mem = null;
            string path = @"C:\SMS_temp\members.xml";

            XmlSerializer serializer = new XmlSerializer(typeof(ShutdownMembers));

            StreamReader reader = new StreamReader(path);
            mem = (ShutdownMembers)serializer.Deserialize(reader);
            reader.Close();

            return mem;

        }

    }

    public class ShutdownMembers
    {
        public List<Member> MemberList { get; set; }
    }

    public class Member
    {
        public string IP { get; set; }
        public string Name { get; set; }
        public bool Exclude { get; set; }
    }

    class StingWriterEncode : StringWriter
    {
        private readonly Encoding encoding;
        public StingWriterEncode(Encoding encoding)
        {
            this.encoding = encoding;
        }

        public override Encoding Encoding
        {
            get { return encoding; }
        }
    }
}
