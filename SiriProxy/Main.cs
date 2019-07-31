using System;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using ARSoft.Tools.Net.Dns;
using System.Data;
using System.Xml;
using System.Data.SQLite;
using System.Runtime.Serialization.Plists;
using Ionic.Zlib;


namespace Sirification
{

    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        List<string> LogMessages = new List<string>();
        BackgroundWorker MainServerWorker;
        DnsServer DNSServer;
        X509Certificate ServerCertificate;
        int NumberOfThreads;
        int LogIndex = 0;
        bool DumpingEnabled = false;

        private void Start_Button_Click(object sender, EventArgs e)
        {
            Start_Button.Enabled = false;
            Stop_Button.Enabled = true;

            IntializeMainServer();
            //IntializeDnsServer();
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            Start_Button.Enabled = true;
            Stop_Button.Enabled = false;

            MainServerWorker.Dispose();
            DNSServer.Stop();
            MyTimer.Enabled = true;
            MyListBox.Items.Add(" ");
            MyListBox.Items.Add("Server Stoppage Requested...");
        }

        private void IntializeMainServer()
        {
            LogMessages.Add("Siri Server Started, Let's Rock'n Roll!");
            LogMessages.Add(" ");

            //ConnectedClients = new List<IPAddress>();
            MyTimer.Interval = (Double)MyNumericUpDown.Value;
            MyTimer.Enabled = true;
            MainServerWorker = new BackgroundWorker();
            MainServerWorker.WorkerReportsProgress = false;
            MainServerWorker.WorkerSupportsCancellation = true;
            MainServerWorker.DoWork += new DoWorkEventHandler(SiriServer);
            MainServerWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(SiriServerDone);

            MainServerWorker.RunWorkerAsync(MainServerWorker);
        }
        private void IntializeDnsServer()
        {
            DNSServer = new DnsServer(1, 0, DNSQuery);
            DNSServer.Start();
        }

        private void SiriServer(object Sender, DoWorkEventArgs Arguments)
        {
            ServerCertificate = new X509Certificate2(Sirification.Properties.Resources.Sirified, "12345");
            TcpListener SiriTcpListener = new TcpListener(IPAddress.Any, 443);
            SiriTcpListener.Start();

            BackgroundWorker LocalMainServerWorker = (BackgroundWorker)Arguments.Argument;

            //Reading Auth Data
            KeysManager.Intialize(ref LogMessages);

            while (true)
            {
                if (LocalMainServerWorker.CancellationPending)
                {
                    SiriTcpListener.Stop();
                    break;
                }

                NumberOfThreads++;

                BackgroundWorker SiriClientThread = new BackgroundWorker();
                SiriClientThread.WorkerReportsProgress = false;
                SiriClientThread.WorkerSupportsCancellation = true;
                SiriClientThread.DoWork += new DoWorkEventHandler(SiriClient);

                TcpClient SiriTcpClient = SiriTcpListener.AcceptTcpClient();

                LogMessages.Add("[" + SiriTcpClient.Client.RemoteEndPoint + "]" + " Server: This is Thread #" + NumberOfThreads.ToString());

                SiriClientThread.RunWorkerAsync(SiriTcpClient);
            }
        }

        private void SiriClient(object Sender, DoWorkEventArgs Arguments)
        {
            TcpClient SiriClient = (TcpClient)Arguments.Argument;
            TcpClient AppleServer = new TcpClient();
            KeysManager KeysManager = new KeysManager();

            try
            {
                LogMessages.Add("[" + SiriClient.Client.RemoteEndPoint + "]" + " Server: Connected.");
                SslStream ClientSSLStream = new SslStream(SiriClient.GetStream(), false);
                ClientSSLStream.AuthenticateAsServer(ServerCertificate, false, SslProtocols.Default, false);

                //Super Fast Respons ==========================================
                //ClientSSLStream.Write(Properties.Resources.AppleRawHeaders, 0, Properties.Resources.AppleRawHeaders.Length);
                //ClientSSLStream.Flush();

                //ClientSSLStream.Write(Properties.Resources.RawMagicCode, 0, Properties.Resources.RawMagicCode.Length);
                //ClientSSLStream.Flush();
                //Super Fast Respons ==========================================


                AppleServer = new TcpClient("guzzoni.apple.com", 443);
                SslStream AppleSSLStream = new SslStream(AppleServer.GetStream(), false);
                AppleSSLStream.AuthenticateAsClient("guzzoni.apple.com");

                ClientConnection ClientConnection = new ClientConnection(ref LogMessages, SiriClient.Client.RemoteEndPoint);
                AppleConnection AppleConnection = new AppleConnection(ref LogMessages, SiriClient.Client.RemoteEndPoint);
                ClientConnectionRewriter ClientConnectionRewriter = new ClientConnectionRewriter(ref ClientConnection, ref LogMessages);

                KeysManager.SetConnections(ref ClientConnection, ref AppleConnection, ref ClientConnectionRewriter);

                BackgroundWorker SiriWorker = (BackgroundWorker)Sender;
                Functions Tools = new Functions();
                Byte[] RawPacket;
                int PacketSize = 0;
                DateTime Timer = DateTime.Now;

                while (true)
                {
                    if (DateTime.Now.Subtract(Timer).Seconds > 40)
                    {
                        LogMessages.Add("[" + SiriClient.Client.RemoteEndPoint + "]" + " Server: Timed out.");

                        if (DumpingEnabled == true)
                        {
                            ClientConnection.WriteObjects();
                            AppleConnection.WriteObjects();
                        }

                        break;
                    }

                    Thread.Sleep(50);
                    if (SiriClient.GetStream().DataAvailable)
                    {
                        //Step 1: Read Packet from Client
                        RawPacket = new Byte[4096];
                        PacketSize = ClientSSLStream.Read(RawPacket, 0, RawPacket.Length);

                        //Step 1.1
                        RawPacket = Tools.ArrayCutter(ref RawPacket, 0, PacketSize - 1);

                        //Reading Packet
                        ClientConnection.Add(ref RawPacket);

                        //Rewritting Packet
                        RawPacket = ClientConnectionRewriter.Rewrite(ref RawPacket);
                        PacketSize = RawPacket.Length;

                        //Step 2: Send Packet to Apple
                        AppleSSLStream.Write(RawPacket, 0, PacketSize);
                        AppleSSLStream.Flush();

                        Timer = DateTime.Now;
                    }

                    Thread.Sleep(50);
                    if (AppleServer.GetStream().DataAvailable)
                    {
                        //Step 3: Read Packet from Apple
                        RawPacket = new Byte[4096];
                        PacketSize = AppleSSLStream.Read(RawPacket, 0, RawPacket.Length);

                        //Step 3.1
                        RawPacket = Tools.ArrayCutter(ref RawPacket, 0, PacketSize - 1);

                        //Step 4: Send Packet to Client
                        //if (AppleConnection.IsAppleOk && AppleConnection.IsMagicCode)
                        //{
                        ClientSSLStream.Write(RawPacket, 0, PacketSize);
                        ClientSSLStream.Flush();
                        // }

                        //Testing Reader
                        AppleConnection.Add(ref RawPacket);

                        Timer = DateTime.Now;
                    }
                }
            }
            catch (UnauthorizedAccessException Error)
            {
                //Database.ErrorsLog(SiriClient.Client.RemoteEndPoint, Error);
                LogMessages.Add("[" + SiriClient.Client.RemoteEndPoint + "]" + " Server: Login Error: " + Error.Message);
            }
            catch (SQLiteException Error)
            {
                Database.Connection.Close();
                Database.ErrorsLog(SiriClient.Client.RemoteEndPoint, Error);
                LogMessages.Add("[" + SiriClient.Client.RemoteEndPoint + "]" + " Server: Database Error: " + Error.Message);
            }
            catch (IOException Error)
            {
                if (Error.Message.Contains("Authentication failed"))
                {
                    Database.ErrorsLog(SiriClient.Client.RemoteEndPoint, new Exception("The Client Doesn't Trust Texnomic Certificate Authority.", Error.InnerException));
                    LogMessages.Add("[" + SiriClient.Client.RemoteEndPoint + "]" + " Server: SSL Error: The Client Doesn't Trust Texnomic Certificate Authority.");
                    LogMessages.Add("[" + SiriClient.Client.RemoteEndPoint + "]" + " Server: Disconnected.");
                    SiriClient.Close();
                    return;
                }
                else
                {
                    Database.ErrorsLog(SiriClient.Client.RemoteEndPoint, Error);
                }

                LogMessages.Add("[" + SiriClient.Client.RemoteEndPoint + "]" + " Server: A Party Closed Connection.");
            }
            catch (Exception Error)
            {
                Database.ErrorsLog(SiriClient.Client.RemoteEndPoint, Error);
                LogMessages.Add("[" + SiriClient.Client.RemoteEndPoint + "]" + " Server: Fatal Error: " + Error.Message + " | " + Error.StackTrace);
            }


            KeysManager.SaveKey();
            KeysManager.FinishKey();
            LogMessages.Add("[" + SiriClient.Client.RemoteEndPoint + "]" + " Server: Disconnected.");
            SiriClient.Close();
            AppleServer.Close();
            Database.Connection.Close();
        }

        private void SiriServerDone(object Sender, RunWorkerCompletedEventArgs Arguments)
        {
            BackgroundWorker LocalMainServerWorker = (BackgroundWorker)Sender;
            LocalMainServerWorker.Dispose();

            MyListBox.Items.Add("Server Stopped; Waiting for Existing Clients to Drop.");
            MyListBox.Items.Add(" ");
        }

        private DnsMessageBase DNSQuery(DnsMessageBase Message, IPAddress ClientIP, ProtocolType Protocol)
        {
            DnsMessage Query = (DnsMessage)Message;

            Message.IsQuery = false;

            if (Query != null && Query.Questions.Count == 1)
            {
                if (Query.Questions[0].Name.Equals("guzzoni.apple.com", StringComparison.InvariantCultureIgnoreCase))
                {
                    Query.AnswerRecords.Add(new ARecord("guzzoni.apple.com", 3600, IPAddress.Parse("10.0.0.15")));
                    Query.ReturnCode = ReturnCode.NoError;

                    LogMessages.Add("DNS: " + ClientIP.ToString() + " Spoofed.");

                    return Query;
                }
                else
                {
                    DnsMessage Answer = DnsClient.Default.Resolve(Query.Questions[0].Name, Query.Questions[0].RecordType, Query.Questions[0].RecordClass);

                    if (Answer != null)
                    {
                        foreach (DnsRecordBase Record in (Answer.AnswerRecords))
                        {
                            Query.AnswerRecords.Add(Record);
                        }

                        foreach (DnsRecordBase Record in (Answer.AdditionalRecords))
                        {
                            Query.AnswerRecords.Add(Record);
                        }

                        Query.ReturnCode = ReturnCode.NoError;
                        return Query;
                    }
                }
            }

            Query.ReturnCode = ReturnCode.ServerFailure;
            return Query;
        }

        private void MyListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (MyListBox.SelectedIndex != -1)
            {
                MyTextBox.Text = MyListBox.SelectedItem.ToString();
            }
        }

        private void MyNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            MyTimer.Interval = (Double)MyNumericUpDown.Value;
        }

        private void MyTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (ShowLogsCheckBox.Checked == true)
            {
                for (int i = LogIndex; i < LogMessages.Count; i++)
                {
                    MyListBox.Items.Add(LogMessages[i]);
                }
            }

            LogIndex = LogMessages.Count;
            MyListBox.SelectedIndex = MyListBox.Items.Count - 1;
        }

        private void MyDumpingCheckBox_CheckStateChanged(object sender, EventArgs e)
        {
            if (MyDumpingCheckBox.Checked == true)
            {
                DumpingEnabled = true;
            }
            else
            {
                DumpingEnabled = false;
            }
        }

        private void MyExportLogsButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog OFD = new SaveFileDialog();
            OFD.InitialDirectory = Environment.CurrentDirectory;
            OFD.Filter = "Log Messages File (.txt) | *.txt";
            OFD.FilterIndex = 1;
            OFD.FileName = "Log Messages " + DateTime.Now.ToString("dd.MM.yyyy HH.mm.ss") + ".txt";
            OFD.RestoreDirectory = true;

            if (OFD.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllLines(OFD.FileName, LogMessages);
            }
        }

        private void ExecuteButton_Click(object sender, EventArgs e)
        {
            MyDataGridView.DataSource = Database.ExecuteQuery(SQLTextBox.Text);
            MyDataGridView.AutoResizeColumns();
        }

        private void ClearLogButton_Click(object sender, EventArgs e)
        {
            MyListBox.Items.Clear();
        }

        private void InstallCAButton_Click(object sender, EventArgs e)
        {
            X509Store Store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
            Store.Open(OpenFlags.ReadWrite);
            Store.Add(new X509Certificate2(Properties.Resources.TexnomicCA, "12345"));
            Store.Close();
        }
    }

    class Functions
    {
        //public Byte[] Decompress(Byte[] Data)
        //{
        //    MemoryStream UnzippedStream = new MemoryStream();

        //    ZlibStream Zlib = new ZlibStream(new MemoryStream(Data), CompressionMode.Decompress, true);
        //    Zlib.FlushMode = FlushType.Sync;
        //    Zlib.CopyTo(UnzippedStream);

        //    return UnzippedStream.ToArray();
        //}

        //public Byte[] Compress(Byte[] Data)
        //{
        //    MemoryStream ZippedStream = new MemoryStream();

        //    ZlibStream Zlib = new ZlibStream(new MemoryStream(Data), CompressionMode.Compress, CompressionLevel.BestCompression, true);
        //    Zlib.FlushMode = FlushType.Sync;
        //    Zlib.CopyTo(ZippedStream);

        //    return ZippedStream.ToArray();
        //}

        /// <summary>
        /// A function for creating a new Byte Array and copies the range of elements between Start Index to Stop Index from Source Array to the new one. 
        /// </summary>
        /// <param name="SourceArray">Source Array to copy elements from.</param>
        /// <param name="Start">Start Index to start copiny elements from.</param>
        /// <param name="End">End Index to stop copying elements from. (Note: Element at End Index inclued in new array)</param>
        /// <returns>New Byte Arrya contaning the specificed range.</returns>
        public Byte[] ArrayCutter(ref Byte[] SourceArray, int Start, int End)
        {
            Byte[] DestinationArray = new Byte[(End - Start) + 1];
            Array.Copy(SourceArray, Start, DestinationArray, 0, (End - Start) + 1);
            return DestinationArray;
        }

        /// <summary>
        /// A function for convering Byte Array to Hexadecimal string.
        /// </summary>
        /// <param name="Data">Byte Array to be converted.</param>
        /// <returns>Hexadecimal string.</returns>
        public string ToHex(Byte[] Data)
        {
            string Hex = null;
            for (int i = 0; i < Data.Length; i++)
            {
                Hex += string.Format("{0:X2}", Data[i]);
            }
            return Hex;
        }

        public Byte[] FromHex(ref string Hex)
        {
            Byte[] Bytes = new Byte[Hex.Length / 2];
            string Test;
            for (int i = 0; i < Hex.Length / 2; i++)
            {
                Test = Hex.Substring(i + i, 2);
                Bytes[i] = Byte.Parse(Hex.Substring(i + i, 2), System.Globalization.NumberStyles.HexNumber);
            }

            return Bytes;
        }

        public IDictionary PlistReader(ref Byte[] BinaryPlist)
        {
            BinaryPlistReader BPR = new BinaryPlistReader();
            return BPR.ReadObject(new MemoryStream(BinaryPlist, false));
        }

        public Byte[] ArrayCompiner(ref Byte[] First, ref Byte[] Second)
        {
            Byte[] Temp = new Byte[First.Length + Second.Length];
            Array.Copy(First, Temp, First.Length);
            Array.Copy(Second, 0, Temp, First.Length, Second.Length);
            return Temp;
        }

        public Dictionary<string, string> CreatePlist(string Count, string Prefix)
        {
            Dictionary<string, string> PongPlist = new Dictionary<string, string>();
            PongPlist.Add("Plists Count", Count);
            PongPlist.Add("Ping", Prefix);
            return PongPlist;
        }
    }

    class ClientConnection
    {
        Functions Tools = new Functions();
        Zlib Decompressor = new Zlib(CompressionMode.Decompress);
        public Byte[] RawPacket = new Byte[0];
        public string HttpMethod = null;
        public Byte[] RawHeaders = new Byte[0];
        public int RawHeadersLastIndex = 0;
        public Dictionary<string, string> Headers = new Dictionary<string, string>(4);
        public Byte[] RawMagicCode = new Byte[0];
        public string MagicCode = null;
        public Byte[] RawData = new Byte[0];
        private Byte[] RawPrefix = new Byte[0];
        private string Prefix = null;
        private int PlistSize = 0;
        private Byte[] RawPlist = new Byte[0];
        public List<IDictionary> Plist = new List<IDictionary>();
        private BinaryPlistWriter BPW = new BinaryPlistWriter();
        private int Index = 0;
        private List<DateTime> TimeStamp = new List<DateTime>();
        public bool Is4S = false;
        public List<string> Pings = new List<string>();
        List<string> LogMessages;
        public EndPoint EndPoint;
        bool Reload = false;
        public X509Certificate Certifcate = new X509Certificate();

        public ClientConnection(ref List<string> Logs, EndPoint ClientEndPoint)
        {
            LogMessages = Logs;
            EndPoint = ClientEndPoint;
        }

        public void Add(ref Byte[] NewPacket)
        {
            if (RawPacket.Length > 0)
            {
                Decompressor.Write(NewPacket);
                ReadObjects();
            }
            else
            {
                RawPacket = NewPacket;
                ReadHeaders();
            }
        }

        public void ReadHeaders()
        {
            StreamReader Reader = new StreamReader(new MemoryStream(RawPacket), Encoding.ASCII);
            HttpMethod = Reader.ReadLine();
            RawHeadersLastIndex += HttpMethod.Length + 2;
            string Line = null;
            string[] Header = new string[0];
            while ((Line = Reader.ReadLine()) != string.Empty)
            {
                RawHeadersLastIndex += Line.Length + 2;
                Header = Line.Split(':');
                Headers.Add(Header[0], Header[1]);
                LogMessages.Add("[" + EndPoint.ToString() + "] Client: " + Line);
            }
            RawHeadersLastIndex += +1;

            if (!Headers["User-Agent"].Contains("Assistant"))
            {
                throw (new Exception("Not A Siri Client."));
            }

            if (Headers["User-Agent"].Contains("iPhone4,1"))
            {
                Is4S = true;
            }

            LogMessages.Add("[" + EndPoint.ToString() + "] Client: Siri Capable: " + Is4S.ToString());

            if (!Database.CheckLogUser(Headers["Host"], Headers["User-Agent"], EndPoint.ToString()))
            {
                if (!Is4S)
                {
                    throw (new UnauthorizedAccessException("Unauthorized Customer."));
                }
                else
                {
                    LogMessages.Add("[" + EndPoint.ToString() + "] Server: Unauthorized Donor.");
                }
            }

            RawHeaders = Tools.ArrayCutter(ref RawPacket, 0, RawHeadersLastIndex);
            RawMagicCode = Tools.ArrayCutter(ref RawPacket, RawHeadersLastIndex + 1, RawHeadersLastIndex + 4);
            MagicCode = Tools.ToHex(RawMagicCode);

            if (MagicCode != "AACCEE02")
            {
                throw (new Exception("[" + EndPoint.ToString() + "] Client: " + "Wrong Magic Code: " + MagicCode));
            }

            LogMessages.Add("[" + EndPoint.ToString() + "] Client: " + MagicCode);

            Decompressor.Write(Tools.ArrayCutter(ref RawPacket, RawHeadersLastIndex + 5, RawPacket.Length - 1));

            ReadObjects();
        }

        void ReadObjects()
        {
            RawData = Decompressor.Read();

            if (!Reload)
            {
                Index = 0;
            }

            do
            {
                if (RawData.Length > 0)
                {
                    RawPrefix = Tools.ArrayCutter(ref RawData, Index, Index + 4);
                    Prefix = Tools.ToHex(RawPrefix);
                    if (Prefix.Substring(0, 2) == "03") //Skip Ping!
                    {
                        Index = Index + 5;
                        Pings.Add(Prefix);
                        LogMessages.Add("[" + EndPoint.ToString() + "] Client: Ping " + Convert.ToInt32(Prefix.Substring(3), 16));
                        continue;
                    }
                    if (Prefix.Substring(0, 2) != "02")
                    {
                        throw (new Exception("[" + EndPoint.ToString() + "] Client: " + "Wrong Data Marker: " + Prefix));
                    }
                    PlistSize = Convert.ToInt32(Prefix.Substring(3), 16);
                    if (RawData.Length - 1 < Index + PlistSize + 4)
                    {
                        LogMessages.Add("[" + EndPoint.ToString() + "] Server: Partial Data Buffered.");
                        Decompressor.Reload();
                        Reload = true;
                        break;
                    }
                    RawPlist = Tools.ArrayCutter(ref RawData, Index + 5, Index + PlistSize + 4);
                    IDictionary NewPlist = Tools.PlistReader(ref RawPlist);
                    LogMessages.Add("[" + EndPoint.ToString() + "] Client: " + NewPlist["class"]);
                    Plist.Add(NewPlist);
                    TimeStamp.Add(DateTime.Now);
                    Index = Index + PlistSize + 5;
                    Reload = false;
                }
            }
            while (RawData.Length - 1 > Index);
        }

        public void WriteObjects()
        {
            string Folder = EndPoint.ToString().Replace(':', '-') + "\\";
            Directory.CreateDirectory("Dump\\" + Folder);

            for (int i = 0; i < Plist.Count; i++)
            {
                BPW.WriteObject("Dump\\" + Folder + i + " Client " + Plist[i]["class"].ToString() + ".plist", Plist[i]);
            }

            if (Pings.Count > 0)
            {
                File.AppendAllLines("Dump\\" + Folder + "Pings.txt", Pings);
            }

            File.AppendAllLines("Dump\\" + Folder + "Log.txt", LogMessages);
            BPW.WriteObject("Dump\\" + Folder + "Client Headers.plist", Headers);
        }
    }

    class AppleConnection
    {
        Functions Tools = new Functions();
        Zlib Decompressor = new Zlib(CompressionMode.Decompress);
        public string HttpMethod = null;
        public Byte[] RawHeaders = new Byte[0];
        public Dictionary<string, string> Headers = new Dictionary<string, string>(3);
        public Byte[] RawMagicCode = new Byte[0];
        public string MagicCode = null;
        public Byte[] RawPacket = new Byte[0];
        public Byte[] RawData = new Byte[0];
        Byte[] RawPrefix = new Byte[0];
        string Prefix = null;
        int PlistSize = 0;
        Byte[] RawPlist = new Byte[0];
        public List<IDictionary> Plist = new List<IDictionary>();
        int RawHeadersLastIndex = 0;
        public bool IsMagicCode = false;
        public bool IsAppleOk = false;
        BinaryPlistWriter BPW = new BinaryPlistWriter();
        int Index = 0;
        List<DateTime> TimeStamp = new List<DateTime>();
        List<string> Pong = new List<string>();
        List<string> LogMessages;
        EndPoint EndPoint;
        bool Reload = false;

        public AppleConnection(ref List<string> Logs, EndPoint ClientEndPoint)
        {
            LogMessages = Logs;
            EndPoint = ClientEndPoint;
        }

        public void Add(ref Byte[] NewPacket)
        {
            if (IsMagicCode)
            {
                Decompressor.Write(NewPacket);
                ReadObjects();
                return;
            }

            if (IsAppleOk)
            {
                RawPacket = NewPacket;
                ReadMagicCode();
                return;
            }

            RawPacket = NewPacket;
            ReadHeaders();
        }

        void ReadMagicCode()
        {
            RawMagicCode = RawPacket;
            MagicCode = Tools.ToHex(RawMagicCode);
            if (MagicCode != "AACCEE02")
            {
                throw (new Exception("[" + EndPoint.ToString() + "] Apple:" + "Wrong Magic Code: " + MagicCode));
            }
            IsMagicCode = true;
            LogMessages.Add("[" + EndPoint.ToString() + "] Apple: " + MagicCode);
        }

        void ReadHeaders()
        {
            RawHeaders = RawPacket;
            StreamReader Reader = new StreamReader(new MemoryStream(RawHeaders), Encoding.ASCII);
            HttpMethod = Reader.ReadLine();
            RawHeadersLastIndex += HttpMethod.Length + 2;
            string Line = null;
            string[] Header = new string[0];
            while ((Line = Reader.ReadLine()) != string.Empty)
            {
                RawHeadersLastIndex += Line.Length + 2;
                Header = Line.Split(':');
                Headers.Add(Header[0], Header[1]);
                LogMessages.Add("[" + EndPoint.ToString() + "] Apple: " + Line);
            }
            RawHeadersLastIndex += +1;

            IsAppleOk = true;
        }

        void ReadObjects()
        {
            RawData = Decompressor.Read();

            if (!Reload)
            {
                Index = 0;
            }

            do
            {
                if (RawData.Length > 0)
                {
                    RawPrefix = Tools.ArrayCutter(ref RawData, Index, Index + 4);
                    Prefix = Tools.ToHex(RawPrefix);
                    if (Prefix.Substring(0, 2) == "04") //Skip Pong!
                    {
                        Index = Index + 5;
                        Pong.Add(Prefix);
                        LogMessages.Add("[" + EndPoint.ToString() + "] Apple: Pong " + Convert.ToInt32(Prefix.Substring(3), 16));
                        continue;
                    }
                    if (Prefix.Substring(0, 2) != "02")
                    {
                        throw (new Exception("[" + EndPoint.ToString() + "] Apple:" + "Wrong Data Marker: " + Prefix));
                    }
                    PlistSize = Convert.ToInt32(Prefix.Substring(3), 16);
                    if (RawData.Length - 1 < Index + PlistSize + 4)
                    {
                        LogMessages.Add("[" + EndPoint.ToString() + "] Server: Partial Data Buffered.");
                        Decompressor.Reload();
                        Reload = true;
                        break;
                    }
                    RawPlist = Tools.ArrayCutter(ref RawData, Index + 5, Index + PlistSize + 4);
                    IDictionary NewPlist = Tools.PlistReader(ref RawPlist);
                    LogMessages.Add("[" + EndPoint.ToString() + "] Apple: " + NewPlist["class"]);
                    if ((string)NewPlist["class"] == "SpeechRecognized")
                    {
                        try
                        {
                            string Speech = AppleSystem.AddPlist(ref NewPlist).ToString();
                            Database.SpeechLog(EndPoint.ToString(), Speech);
                            LogMessages.Add("[" + EndPoint.ToString() + "] Apple: " + Speech);
                        }
                        catch
                        {

                        }
                    }
                    Plist.Add(NewPlist);
                    TimeStamp.Add(DateTime.Now);
                    Index = Index + PlistSize + 5;
                    Reload = false;
                }
            }
            while (RawData.Length - 1 > Index);

        }

        public void WriteObjects()
        {
            string Folder = EndPoint.ToString().Replace(':', '-') + "\\";
            Directory.CreateDirectory("Dump\\" + Folder);

            for (int i = 0; i < Plist.Count; i++)
            {
                BPW.WriteObject("Dump\\" + Folder + i + " Apple " + Plist[i]["class"].ToString() + ".plist", Plist[i]);
            }

            if (Pong.Count > 0)
            {
                File.AppendAllLines("Dump\\" + Folder + "Pongs.txt", Pong);
            }

            BPW.WriteObject("Dump\\" + Folder + "Apple Headers.plist", Headers);
        }
    }

    class Zlib
    {
        Functions Tools = new Functions();
        ZlibStream ZlibStream;
        MemoryStream Stream = new MemoryStream();
        int Index = 0;
        int LastIndex = 0;
        Byte[] Buffer;
        Byte[] Data;

        public Zlib(CompressionMode Mode)
        {
            ZlibStream = new ZlibStream(Stream, Mode, CompressionLevel.BestCompression, false);
            ZlibStream.FlushMode = FlushType.Sync;
        }

        public void Write(Byte[] Data)
        {
            ZlibStream.Write(Data, 0, Data.Length);
        }

        public Byte[] Read()
        {
            Buffer = Stream.ToArray();
            Data = Tools.ArrayCutter(ref Buffer, Index, Buffer.Length - 1);
            LastIndex = Index;
            Index = Buffer.Length;
            return Data;
        }

        public void Reload()
        {
            Index = LastIndex;
        }
    }

    class ClientConnectionRewriter
    {
        Functions Tools = new Functions();
        Zlib Compressor = new Zlib(CompressionMode.Compress);
        Byte[] NewPacket;
        MemoryStream Stream = new MemoryStream();
        BinaryPlistWriter BPW = new BinaryPlistWriter();
        Byte[] BinaryPlist;
        string PlistSize;
        Byte[] Prefix;
        Byte[] Data = new Byte[0];
        List<string> LogMessages;
        bool KeyObtained = false;
        public KeysManager.Key Key = new KeysManager.Key();
        ClientConnection ClientConnection;
        int PlistIndex = 0;
        int PingIndex = 0;
        bool AuthReplaced = false;
        bool HeaderReplaced = false;

        public ClientConnectionRewriter(ref ClientConnection Connection, ref List<string> Log)
        {
            LogMessages = Log;
            ClientConnection = Connection;
        }

        public Byte[] Rewrite(ref Byte[] CurrentPacket)
        {
            if (!ClientConnection.Is4S && !KeysManager.KeysExist)
            {
                throw (new Exception("No Keys Available for Unsupported Siri Clients."));
            }

            if (!ClientConnection.Is4S && !KeyObtained)
            {
                Key = KeysManager.GetKey();
                KeyObtained = true;
            }

            if (!ClientConnection.Is4S)
            {
                NewPacket = new Byte[0];

                if (!HeaderReplaced)
                {
                    NewPacket = Key.Headers;
                    NewPacket = Tools.ArrayCompiner(ref NewPacket, ref Key.MagicCode);
                    HeaderReplaced = true;
                    LogMessages.Add("[" + ClientConnection.EndPoint.ToString() + "] Server: \"X-ACE-HOST\" Replaced.");
                }

                ReplaceAuthData(PlistIndex);

                for (int i = PlistIndex; i < ClientConnection.Plist.Count; i++)
                {
                    Stream = new MemoryStream();
                    BPW.WriteObject(Stream, ClientConnection.Plist[i]);
                    BinaryPlist = Stream.ToArray();
                    PlistSize = (0x0200000000 + BinaryPlist.Length).ToString("X").PadLeft(10, '0');
                    Prefix = Tools.FromHex(ref PlistSize);
                    Compressor.Write(Tools.ArrayCompiner(ref Prefix, ref BinaryPlist));
                    PlistIndex = i + 1;
                }

                for (int i = PingIndex; i < ClientConnection.Pings.Count; i++)
                {
                    string Ping = ClientConnection.Pings[PingIndex];
                    Compressor.Write(Tools.FromHex(ref Ping));
                    PingIndex = i + 1;
                }

                Data = Compressor.Read();

                NewPacket = Tools.ArrayCompiner(ref NewPacket, ref Data);

                return NewPacket;
            }
            else
            {
                if (!HeaderReplaced)
                {
                    string Headers = Encoding.ASCII.GetString(ClientConnection.RawHeaders);
                    Headers = Headers.Replace(ClientConnection.Headers["Host"].Trim(), "guzzoni.apple.com");
                    ClientConnection.RawHeaders = Encoding.ASCII.GetBytes(Headers); //Saving Fixed Header for Saved Keys.
                    NewPacket = ClientConnection.RawHeaders;
                    NewPacket = Tools.ArrayCompiner(ref NewPacket, ref ClientConnection.RawMagicCode);
                    HeaderReplaced = true;
                    LogMessages.Add("[" + ClientConnection.EndPoint.ToString() + "] Server: Host Header Replaced.");
                }

                return CurrentPacket;
            }
        }

        private void ReplaceAuthData(int PlistIndex)
        {
            for (int i = PlistIndex; i < ClientConnection.Plist.Count; i++)
            {
                if (ClientConnection.Plist[i].Contains("properties"))
                {
                    AuthReplaced = false;

                    IDictionary NewPlist = (IDictionary)ClientConnection.Plist[i]["properties"];
                    IDictionary SavedPlist = (IDictionary)Key.Plist["properties"];

                    if (NewPlist.Contains("validationData"))
                    {
                        NewPlist["validationData"] = SavedPlist["sessionValidationData"];
                        AuthReplaced = true;
                        LogMessages.Add("[" + ClientConnection.EndPoint.ToString() + "] Server: \"validationData\" Replaced.");
                    }

                    if (NewPlist.Contains("sessionValidationData"))
                    {
                        NewPlist["sessionValidationData"] = SavedPlist["sessionValidationData"];
                        AuthReplaced = true;
                        LogMessages.Add("[" + ClientConnection.EndPoint.ToString() + "] Server: \"sessionValidationData\" Replaced.");
                    }

                    //if (NewPlist.Contains("assistantId"))
                    //{
                    //    NewPlist["assistantId"] = SavedPlist["assistantId"];
                    //    AuthReplaced = true;
                    //    LogMessages.Add("[" + ClientConnection.EndPoint.ToString() + "] Server: \"assistantId\" Replaced.");
                    //}

                    //if (NewPlist.Contains("speechId"))
                    //{
                    //    NewPlist["speechId"] = SavedPlist["speechId"];
                    //    AuthReplaced = true;
                    //    LogMessages.Add("[" + ClientConnection.EndPoint.ToString() + "] Server: \"speechId\" Replaced.");
                    //}

                    //if (NewPlist.Contains("sessionInfoRequest"))
                    //{
                    //    NewPlist["sessionInfoRequest"] = SavedPlist["sessionValidationData"];
                    //    AuthReplaced = true;
                    //    LogMessages.Add("[" + ClientConnection.EndPoint.ToString() + "] Server: \"sessionInfoRequest\" Replaced.");
                    //}

                    if (AuthReplaced)
                    {
                        ClientConnection.Plist[i]["properties"] = NewPlist;
                    }

                }
            }





        }

    }

    class KeysManager
    {
        public class Key
        {
            public Byte[] Validation;
            public IDictionary Plist;
            public Byte[] Headers;
            public Byte[] MagicCode = Properties.Resources.RawMagicCode;
        }

        static List<string> LogMessages;
        public static bool KeysExist = false;
        ClientConnection Client;
        AppleConnection Apple;
        ClientConnectionRewriter Rewriter;

        public KeysManager()
        {
            //Just Nothing
        }

        public static void Intialize(ref List<string> Logs)
        {
            LogMessages = Logs;
            CheckForKeys();
        }

        public void SetConnections(ref ClientConnection ClientConnection, ref AppleConnection AppleConnection, ref ClientConnectionRewriter ClientConnectionRewriter)
        {
            Client = ClientConnection;
            Apple = AppleConnection;
            Rewriter = ClientConnectionRewriter;
        }

        static void CheckForKeys()
        {
            int NumOfKeys = Database.CheckForKeys();

            if (NumOfKeys > 0)
            {
                KeysExist = true;
                LogMessages.Add("Server: " + NumOfKeys + " Keys Loaded.");
            }
            else
            {
                KeysExist = false;
                LogMessages.Add("Server: No Keys Found.");
            }
        }

        public void SaveKey()
        {
            if (Client.Is4S)
            {
                BinaryPlistReader BPR = new BinaryPlistReader();
                BinaryPlistWriter BPW = new BinaryPlistWriter();

                switch (CheckforClass("AssistantNotFound", Apple.Plist))
                {
                    case false:
                        {
                            IDictionary Key = GetPlist("LoadAssistant", Client.Plist);
                            IDictionary Properties = (IDictionary)Key["properties"];
                            MemoryStream Stream = new MemoryStream();
                            BPW.WriteObject(Stream, Key);

                            if (Database.SaveKey(
                                (string)Properties["assistantId"],
                                (string)Properties["speechId"],
                                (Byte[])Properties["sessionValidationData"],
                                Stream.ToArray(), Client.RawHeaders,
                                Client.Headers["Host"],
                                Client.Headers["User-Agent"]))
                            {
                                LogMessages.Add("[" + Client.EndPoint + "] " + "Server: Warning! Working in Single Key Mode.");
                                LogMessages.Add("[" + Client.EndPoint + "] " + "Server: New Key " + (string)Properties["assistantId"] + " Saved.");
                            }
                            else
                            {
                                LogMessages.Add("[" + Client.EndPoint + "] " + "Server: Key " + (string)Properties["assistantId"] + " Already Saved.");
                            }

                            KeysExist = true;

                            break;
                        }
                    case true:
                        {
                            IDictionary ClientKey = GetPlist("CreateAssistant", Client.Plist);
                            IDictionary AppleKey = GetPlist("AssistantCreated", Apple.Plist);
                            IDictionary AppleProperties = (IDictionary)AppleKey["properties"];
                            IDictionary Key = BPR.ReadObject(new MemoryStream(Sirification.Properties.Resources.Client_LoadAssistant));
                            IDictionary KeyProperties = (IDictionary)Key["properties"];
                            KeyProperties["assistantId"] = AppleProperties["assistantId"];
                            KeyProperties["speechId"] = AppleProperties["speechId"];
                            IDictionary ClientProperties = (IDictionary)ClientKey["properties"];
                            KeyProperties["sessionValidationData"] = ClientProperties["validationData"];
                            Key["properties"] = KeyProperties;
                            IDictionary Properties = (IDictionary)Key["properties"];
                            MemoryStream Stream = new MemoryStream();
                            BPW.WriteObject(Stream, Key);

                            if (Database.SaveKey(
                               (string)Properties["assistantId"],
                               (string)Properties["speechId"],
                               (Byte[])Properties["sessionValidationData"],
                               Stream.ToArray(), Client.RawHeaders,
                               Client.Headers["Host"],
                               Client.Headers["User-Agent"]))
                            {
                                LogMessages.Add("[" + Client.EndPoint + "] " + "Server: Warning! Working in Single Key Mode.");
                                LogMessages.Add("[" + Client.EndPoint + "] " + "Server: New Key " + (string)Properties["assistantId"] + " Saved.");
                            }
                            else
                            {
                                LogMessages.Add("[" + Client.EndPoint + "] " + "Server: Key " + (string)Properties["assistantId"] + " Already Saved.");
                            }

                            KeysExist = true;

                            break;
                        }
                    default:
                        break;
                }
            }
        }

        public static Key GetKey()
        {
            BinaryPlistReader BPR = new BinaryPlistReader();
            List<Byte[]> Data = Database.GetKey();
            if (Data.Capacity > 0)
            {
                Key Key = new Key();
                Key.Plist = BPR.ReadObject(new MemoryStream(Data[0]));
                Key.Headers = Data[1];
                Key.Validation = Data[2];
                IDictionary Properties = (IDictionary)Key.Plist["properties"];
                KeysExist = true;
                return Key;
            }
            KeysExist = false;
            return new Key();
        }

        public void FinishKey()
        {
            if (Client.Is4S == false && Rewriter.Key.Validation != null)
            {
                Database.FinishKey(Rewriter.Key.Validation);
            }
        }

        bool CheckforClass(string Name, List<IDictionary> Plists)
        {
            for (int i = 0; i < Plists.Count; i++)
            {
                if (Name == (string)Plists[i]["class"])
                {
                    return true;
                }
            }

            return false;
        }
        IDictionary GetPlist(string Name, List<IDictionary> Plists)
        {
            for (int i = 0; i < Plists.Count; i++)
            {
                if (Name == (string)Plists[i]["class"])
                {
                    return Plists[i];
                }
            }

            return new Dictionary<string, string>();
        }
    }

    static class Database
    {
        public static SQLiteConnection Connection = new SQLiteConnection("Data Source=Database.db");

        public static int CheckForKeys()
        {
            Connection.Open();
            SQLiteCommand SQL = new SQLiteCommand(Connection);
            SQL.CommandText = @"SELECT COUNT(ASSISTANT) FROM KEYS, SETTINGS WHERE VALID = 'YES' AND LOAD < MAXLOAD";
            int NumOfKeys = int.Parse(SQL.ExecuteScalar().ToString());
            Connection.Close();
            return NumOfKeys;
        }

        public static bool SaveKey(string Assistant, string Speech, Byte[] Validation, Byte[] Plist, Byte[] RawHeader, string Host, string UserAgent)
        {
            Connection.Open();
            SQLiteCommand SQL = new SQLiteCommand(Connection);
            SQL.CommandText = @"SELECT COUNT(VALIDATION) FROM KEYS WHERE VALIDATION = @VALIDATION";
            SQL.Parameters.Add("@VALIDATION", DbType.Binary).Value = Validation;
            int Result = int.Parse(SQL.ExecuteScalar().ToString());
            if (Result == 0)
            {
                //Single Key Mode
                SQL = new SQLiteCommand(Connection);
                SQL.CommandText = @"UPDATE KEYS SET VALID = 'NO'";
                SQL.ExecuteNonQuery();

                SQL = new SQLiteCommand(Connection);
                SQL.CommandText = @"INSERT INTO KEYS VALUES (@ASSISTANT, @SPEECH, @VALIDATION, @PLIST, @RAWHEADER, @HOST, @USERAGENT, @VALID, @CONCURRENT, @LOAD)";
                SQL.Parameters.Add("@ASSISTANT", DbType.String).Value = Assistant;
                SQL.Parameters.Add("@SPEECH", DbType.String).Value = Speech;
                SQL.Parameters.Add("@VALIDATION", DbType.Binary).Value = Validation;
                SQL.Parameters.Add("@PLIST", DbType.Binary).Value = Plist;
                SQL.Parameters.Add("@RAWHEADER", DbType.Binary).Value = RawHeader;
                SQL.Parameters.Add("@HOST", DbType.String).Value = Host;
                SQL.Parameters.Add("@USERAGENT", DbType.String).Value = UserAgent;
                SQL.Parameters.Add("@VALID", DbType.String).Value = "YES";
                SQL.Parameters.Add("@CONCURRENT", DbType.Int32).Value = 0;
                SQL.Parameters.Add("@LOAD", DbType.Int32).Value = 0;
                SQL.ExecuteNonQuery();
                Connection.Close();
                return true;
            }
            else
            {
                SQL = new SQLiteCommand(Connection);
                SQL.CommandText = @"SELECT ASSISTANT, SPEECH FROM KEYS WHERE VALIDATION = @VALIDATION";
                SQL.Parameters.Add("@VALIDATION", DbType.Binary).Value = Validation;
                DataTable Table = new DataTable();
                SQLiteDataReader Reader = SQL.ExecuteReader();
                Table.Load(Reader);
                Reader.Close();

                if ((string)Table.Rows[0][0] != Assistant)
                {
                    SQL = new SQLiteCommand(Connection);
                    SQL.CommandText = @"UPDATE KEYS SET ASSISTANT = @ASSISTANT WHERE VALIDATION = @VALIDATION";
                    SQL.Parameters.Add("@ASSISTANT", DbType.String).Value = Assistant;
                    SQL.Parameters.Add("@VALIDATION", DbType.Binary).Value = Validation;
                    SQL.ExecuteNonQuery();
                }

                if ((string)Table.Rows[0][1] != Speech)
                {
                    SQL = new SQLiteCommand(Connection);
                    SQL.CommandText = @"UPDATE KEYS SET SPEECH = @SPEECH WHERE VALIDATION = @VALIDATION";
                    SQL.Parameters.Add("@SPEECH", DbType.String).Value = Speech;
                    SQL.Parameters.Add("@VALIDATION", DbType.Binary).Value = Validation;
                    SQL.ExecuteNonQuery();
                }

            }
            Connection.Close();
            return false;
        }

        public static List<Byte[]> GetKey()
        {
            Connection.Open();
            SQLiteCommand SQL = new SQLiteCommand(Connection);
            SQL.CommandText = @"SELECT COUNT(ASSISTANT) FROM KEYS, SETTINGS WHERE VALID = 'YES' AND LOAD < MAXLOAD AND CONCURRENT = 
                              (SELECT MIN(CONCURRENT) FROM KEYS WHERE  VALID = 'YES' AND LOAD < MAXLOAD)";
            int Result = int.Parse(SQL.ExecuteScalar().ToString());
            if (Result != 0)
            {
                SQL = new SQLiteCommand(Connection);
                SQL.CommandText = @"SELECT PLIST, RAWHEADER, VALIDATION FROM KEYS, SETTINGS WHERE VALID = 'YES' AND LOAD < MAXLOAD AND CONCURRENT = 
                                  (SELECT MIN(CONCURRENT) FROM KEYS WHERE  VALID = 'YES' AND LOAD < MAXLOAD)";
                DataTable Table = new DataTable();
                SQLiteDataReader Reader = SQL.ExecuteReader();
                Table.Load(Reader);
                Reader.Close();
                List<Byte[]> Key = new List<Byte[]>(2);
                Key.Add((Byte[])Table.Rows[0][0]);
                Key.Add((Byte[])Table.Rows[0][1]);
                Key.Add((Byte[])Table.Rows[0][2]);

                SQL = new SQLiteCommand(Connection);
                SQL.CommandText = @"UPDATE KEYS SET CONCURRENT = CONCURRENT + 1, LOAD = LOAD + 1 WHERE VALIDATION = @VALIDATION";
                SQL.Parameters.Add("@VALIDATION", DbType.Binary).Value = (Byte[])Table.Rows[0][2];
                SQL.ExecuteNonQuery();
                Connection.Close();
                return Key;
            }
            Connection.Close();
            return new List<byte[]>(0);
        }

        public static void FinishKey(Byte[] Validation)
        {
            Connection.Open();
            SQLiteCommand SQL = new SQLiteCommand(Connection);
            SQL.CommandText = @"UPDATE KEYS SET CONCURRENT = CONCURRENT - 1 WHERE VALIDATION = @VALIDATION";
            SQL.Parameters.Add("@VALIDATION", DbType.Binary).Value = Validation;
            SQL.ExecuteNonQuery();
            Connection.Close();
        }

        public static bool CheckLogUser(string Host, string UserAgent, string EndPoint)
        {
            Connection.Open();
            SQLiteCommand SQL = new SQLiteCommand(Connection);
            SQL.CommandText = @"SELECT COUNT(HOST), EXPIREDATE FROM USERS WHERE HOST = @HOST AND USERAGENT = @USERAGENT";
            SQL.Parameters.Add("@HOST", DbType.String).Value = Host;
            SQL.Parameters.Add("@USERAGENT", DbType.String).Value = UserAgent;
            DataTable Table = new DataTable();
            SQLiteDataReader Reader = SQL.ExecuteReader();
            Table.Load(Reader);
            Reader.Close();
            Connection.Close();

            LogUser(Host, UserAgent, EndPoint);

            if (int.Parse(Table.Rows[0][0].ToString()) != 0)
            {
                if (int.Parse(DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0')) <= int.Parse(Table.Rows[0][1].ToString()))
                {
                    return true;
                }
            }
            return false;
        }

        static void LogUser(string Host, string UserAgent, string EndPoint)
        {
            Connection.Open();
            SQLiteCommand SQL = new SQLiteCommand(Connection);
            SQL.CommandText = @"INSERT INTO USERSLOG VALUES (@HOST, @USERAGENT, @DATETIME, @ENDPOINT)";
            SQL.Parameters.Add("@HOST", DbType.String).Value = Host;
            SQL.Parameters.Add("@USERAGENT", DbType.String).Value = UserAgent;
            SQL.Parameters.Add("@DATETIME", DbType.String).Value = DateTime.Now.ToString();
            SQL.Parameters.Add("@ENDPOINT", DbType.String).Value = EndPoint;
            SQL.ExecuteNonQuery();
            Connection.Close();
        }

        public static void SpeechLog(string EndPoint, string Speech)
        {
            Connection.Open();
            SQLiteCommand SQL = new SQLiteCommand(Connection);
            SQL.CommandText = @"INSERT INTO SPEECHLOG VALUES (@ENDPOINT, @SPEECH)";
            SQL.Parameters.Add("@ENDPOINT", DbType.String).Value = EndPoint;
            SQL.Parameters.Add("@SPEECH", DbType.String).Value = Speech;
            SQL.ExecuteNonQuery();
            Connection.Close();
        }

        public static void ErrorsLog(EndPoint EndPoint, Exception Error)
        {
            Connection.Open();
            SQLiteCommand SQL = new SQLiteCommand(Connection);
            SQL.CommandText = @"INSERT INTO ERRORSLOG VALUES (@ENDPOINT, @MESSAGE, @INNERMESSAGE, @STACKTRACE, @DATETIME)";
            SQL.Parameters.Add("@ENDPOINT", DbType.String).Value = EndPoint;
            SQL.Parameters.Add("@MESSAGE", DbType.String).Value = Error.Message;
            SQL.Parameters.Add("@INNERMESSAGE", DbType.String).Value = Error.InnerException.Message;
            SQL.Parameters.Add("@STACKTRACE", DbType.String).Value = Error.StackTrace;
            SQL.Parameters.Add("@DATETIME", DbType.String).Value = DateTime.Now;
            SQL.ExecuteNonQuery();
            Connection.Close();
        }

        public static DataTable ExecuteQuery(string Query)
        {
            Connection.Open();
            SQLiteDataAdapter Adaptor = new SQLiteDataAdapter(Query, Connection);
            DataTable Table = new DataTable();

            try
            {
                Adaptor.Fill(Table);
            }
            catch (Exception Error)
            {
                Connection.Close();
                MessageBox.Show(Error.Message);
            }
            Connection.Close();
            return Table;
        }
    }

    static class AppleSystem
    {
        public static object AddPlist(ref IDictionary Plist)
        {
            switch ((string)Plist["group"])
            {
                case "com.apple.ace.speech":
                    {
                        return Speech.AddSpeech(ref Plist);
                    }
                default:
                    return new object();
            }
        }

        static class Speech
        {
            public static object AddSpeech(ref IDictionary Plist)
            {
                switch ((string)Plist["class"])
                {
                    case "StartSpeechRequest":
                        {
                            return SpeechPacket.Intialize();
                        }
                    case "SpeechPacket":
                        {
                            return SpeechPacket.AddPacket((IDictionary)Plist["properties"]);
                        }
                    case "FinishSpeech":
                        {
                            return SpeechPacket.Finish((IDictionary)Plist["properties"]);
                        }
                    case "SpeechRecognized":
                        {
                            SpeechRecognized SpeechRecognized = new SpeechRecognized((IDictionary)Plist["properties"]);
                            return SpeechRecognized.Read();
                        }
                    default:
                        return new object();
                }
            }

            static class SpeechPacket
            {
                static Byte[] Speech = new Byte[0];
                static int PacketNumber = 0;

                public static bool Intialize()
                {
                    Speech = new Byte[0];
                    PacketNumber = 0;
                    return true;
                }

                public static bool AddPacket(IDictionary Plist)
                {
                    if ((Int16)Plist["packetNumber"] == PacketNumber)
                    {
                        object[] Packets = (object[])Plist["packets"];
                        for (int i = 0; i < Packets.Length; i++)
                        {
                            Speech = ArrayCompiner(ref Speech, (Byte[])Packets[i]);
                        }
                        PacketNumber++;
                        return true;
                    }
                    return false;
                }

                public static object Finish(IDictionary Plist)
                {
                    if ((Int16)Plist["packetCount"] == PacketNumber)
                    {
                        return Speech;
                    }
                    return false;
                }

                static Byte[] ArrayCompiner(ref Byte[] First, Byte[] Second)
                {
                    Byte[] Temp = new Byte[First.Length + Second.Length];
                    Array.Copy(First, Temp, First.Length);
                    Array.Copy(Second, 0, Temp, First.Length, Second.Length);
                    return Temp;
                }
            }

            class SpeechRecognized
            {
                string Text;

                public SpeechRecognized(IDictionary Plist)
                {
                    Text = SpeechRecognizedMethode(Plist);
                }

                public string Read()
                {
                    return Text;
                }

                string SpeechRecognizedMethode(IDictionary Plist)
                {
                    return Recognition((IDictionary)Plist["recognition"]);
                }

                string Recognition(IDictionary Plist)
                {
                    return Phrases((IDictionary)Plist["properties"]);
                }

                string Phrases(IDictionary Plist)
                {
                    List<IDictionary> Phrases = new List<IDictionary>();
                    object[] Dictionaries = (object[])Plist["phrases"];
                    for (int i = 0; i < Dictionaries.Length; i++)
                    {
                        Phrases.Add((IDictionary)Dictionaries[i]);
                    }
                    return Phrase(Phrases);
                }

                string Phrase(List<IDictionary> Plists)
                {
                    List<IDictionary> Interpretation = new List<IDictionary>();
                    for (int i = 0; i < Plists.Count; i++)
                    {
                        Interpretation.Add((IDictionary)Plists[i]["properties"]);
                    }
                    return Interpretations(Interpretation);
                }

                string Interpretations(List<IDictionary> Plists)
                {
                    List<IDictionary> Interpretation = new List<IDictionary>();
                    for (int i = 0; i < Plists.Count; i++)
                    {
                        object[] Dictionaries = (object[])Plists[i]["interpretations"];
                        //Taking First Interpretation Since it's most accurate
                        Interpretation.Add((IDictionary)((IDictionary)Dictionaries[0])["properties"]);
                    }
                    return Tokens(Interpretation);
                }

                string Tokens(List<IDictionary> Plists)
                {
                    List<IDictionary> Tokens = new List<IDictionary>();
                    for (int i = 0; i < Plists.Count; i++)
                    {
                        object[] Dictionaries = (object[])Plists[i]["tokens"];
                        for (int x = 0; x < Dictionaries.Length; x++)
                        {
                            Tokens.Add((IDictionary)((IDictionary)Dictionaries[x])["properties"]);
                        }
                    }
                    return Token(Tokens);
                }

                string Token(List<IDictionary> Plists)
                {
                    string Text = null;
                    for (int i = 0; i < Plists.Count; i++)
                    {
                        Text += " " + (string)Plists[i]["text"];
                        if ((bool)Plists[i]["removeSpaceBefore"])
                        {
                            Text = Text.TrimStart();
                        }
                        if ((bool)Plists[i]["removeSpaceAfter"])
                        {
                            Text = Text.TrimEnd();
                        }
                    }
                    return Text;
                }
            }
        }

    }

}

