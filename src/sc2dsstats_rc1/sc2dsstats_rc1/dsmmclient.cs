﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Windows;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Threading;

namespace sc2dsstats_rc1
{

    public class dsmmclient
    {
        private const int port = 7891;
        public bool STATUS { get; set; } = false;


        Win_mm MW { get; set; }
        List<KeyValuePair<int, string>> PLPOS { get; set; }
        public string INFO { get; set; }

        public dsmmclient()
        {
            PLPOS = new List<KeyValuePair<int, string>>();
        }

        public dsmmclient(Win_mm mw) : this()
        {
            MW = mw;
        }


        public void StopClient(Socket sender)
        {
            try
            {
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
            }
            catch { }
        }

        public string StartClient(Win_mm wm, string player, string result)
        {
            Socket sender = null;
            byte[] bytes = new byte[1024];
            string response = null;

            try
            {
                // Connect to a Remote server  
                // Get Host IP Address that is used to establish a connection  
                // In this case, we get one IP address of localhost that is IP : 127.0.0.1  
                // If a host has multiple addresses, you will get a list of addresses  

                //IPHostEntry ipHostInfo = Dns.GetHostEntry("userver4");
                //IPAddress ipAddress = IPAddress.Parse("192.168.178.28");

                IPHostEntry ipHostInfo = Dns.GetHostEntry("pax77.org");
                IPAddress ipAddress = IPAddress.Parse("144.76.58.9");
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP  socket.    
                sender = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.    
                try
                {
                    // Connect to Remote EndPoint  
                    sender.Connect(remoteEP);

                    Console.WriteLine("Socket connected to {0}",
                        sender.RemoteEndPoint.ToString());

                    // Create the preBuffer data.
                    string string1 = "Hello from [" + player + "]: " + result + ";" + "\r\n";
                    byte[] preBuf = Encoding.UTF8.GetBytes(string1);

                    int bytesSent = sender.Send(preBuf);

                    // Receive the response from the remote device.    
                    int bytesRec = sender.Receive(bytes);
                    Console.WriteLine("Echoed test = {0}",
                        Encoding.UTF8.GetString(bytes, 0, bytesRec));

                    string pattern = @"^sc2dsmm: Result: (.*)";
                    string ent = Encoding.UTF8.GetString(bytes, 0, bytesRec);
                    string elo = null;
                    foreach (Match m in Regex.Matches(ent, pattern))
                    {
                        elo = m.Groups[1].Value.ToString();
                    }
                    if (elo != null)
                    {
                        //wm.Dispatcher.Invoke(() =>
                        //{
                        //wm.PresentResult(elo);
                        //});
                        response = elo;
                    }

                    string pattern1 = @"^sc2dsmm: Matchup: (.*)";
                    string ent1 = Encoding.UTF8.GetString(bytes, 0, bytesRec);
                    string matchup = null;
                    foreach (Match m in Regex.Matches(ent1, pattern1))
                    {
                        matchup = m.Groups[1].Value.ToString();
                    }
                    if (matchup != null)
                    {
                        response = matchup;
                    }

                    string pattern2 = @"^sc2dsmm: Ladder: (.*)";
                    string ent2 = Encoding.UTF8.GetString(bytes, 0, bytesRec);
                    string ladder = null;
                    foreach (Match m in Regex.Matches(ent2, pattern2))
                    {
                        ladder = m.Groups[1].Value.ToString();
                    }
                    if (ladder != null)
                    {
                        response = ladder;
                    }

                    // Create the preBuffer data.
                    string string2 = "Hello from [" + player + "]: " + "fin" + ";" + "\r\n";
                    byte[] preBuf1 = Encoding.UTF8.GetBytes(string2);

                    int bytesSent1 = sender.Send(preBuf1);

                    // Release the socket.    
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();



                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return response;
        }

        public Socket StartClient(Win_mm mw, string player, string mode, string num, string skill, string server)
        {
            STATUS = false;
            Socket sender = null;
            byte[] bytes = new byte[1024];

            try
            {
                // Connect to a Remote server  
                // Get Host IP Address that is used to establish a connection  
                // In this case, we get one IP address of localhost that is IP : 127.0.0.1  
                // If a host has multiple addresses, you will get a list of addresses  

                //IPHostEntry ipHostInfo = Dns.GetHostEntry("userver4");
                //IPAddress ipAddress = IPAddress.Parse("192.168.178.28");

                IPHostEntry ipHostInfo = Dns.GetHostEntry("pax77.org");
                IPAddress ipAddress = IPAddress.Parse("144.76.58.9");
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP  socket.    
                sender = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                mw.Dispatcher.Invoke(() =>
                {
                    mw.CLIENT = sender;
                });

                // Connect the socket to the remote endpoint. Catch any errors.    
                try
                {
                    // Connect to Remote EndPoint  
                    sender.Connect(remoteEP);

                    Console.WriteLine("Socket connected to {0}",
                        sender.RemoteEndPoint.ToString());

                    // Create the preBuffer data.
                    string string1 = "Hello from [" + player + "]: Letmeplay: " + mode + ";" + num + ";" + skill + ";" + server + ";" + "\r\n";
                    byte[] preBuf = Encoding.UTF8.GetBytes(string1);

                    int bytesSent = sender.Send(preBuf);

                    // Receive the response from the remote device.    
                    int bytesRec = sender.Receive(bytes);
                    Console.WriteLine("Echoed test = {0}",
                        Encoding.UTF8.GetString(bytes, 0, bytesRec));

                    if (sender != null) PingPong(Encoding.UTF8.GetString(bytes, 0, bytesRec), player, sender, "0");


                    // Release the socket.    
                    //sender.Shutdown(SocketShutdown.Both);
                    //sender.Close();



                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return sender;
        }

        private static bool RemoveEmpty(String s)
        {
            return s == "";
        }

        public string PingPong(string pong, string player, Socket client, string mmid)
        {
            string response = "";
            string answer = "";
            //string mmid = "0";

            string pattern = @"^sc2dsmm: (.*)";
            foreach (Match m in Regex.Matches(pong, pattern))
            {
                answer = m.Groups[1].ToString();
            }
            if (CheckPong(answer))
            {
                string pt_letmeplay = @"^Letmeplay: (.*)";
                string pt_accept = @"^Accept: (.*)";
                string pt_decline = @"^Decline: (.*)";
                string pt_findgame = @"^Findgame: (.*)";
                string pt_ready = @"^Ready: (.*)";
                string pt_status = @"^Status: (.*)";

                // Matchup ready to go
                foreach (Match m_ready in Regex.Matches(answer, pt_ready))
                {
                    string ent = m_ready.Groups[1].ToString();
                    if (ent == "0")
                    {
                        // Someone declined / timed out :(
               
                        MW.Dispatcher.Invoke(() =>
                        {
                            MW.ALL_DECLINED = true;
                        });

                        // Waiting again
                        // We need to responde too
                        if (MW.WAITING == false && MW.DECLINED == false)
                        {
                            Random rngesus = new Random();
                            int rng = rngesus.Next(0, 3000);
                            int mysleep = 5000 + rng;
                            Thread.Sleep(mysleep);
                            // response = "Findgame: 1";
                            response = "Letmeplay: 1";
                            mmid = "0";
                            //Console.WriteLine("PP: " + mmid + " " + player + " pt_ready => Someone declined => waiting again");
                        } else
                        {
                            // we accepted
                            if (MW.ACCEPTED == true && STATUS == false)
                            {
                                response = "accept: " + mmid;
                                STATUS = true;
                                //Console.WriteLine("PP: " + mmid + " " + player + " pt_ready => Someone declined => we accepted");
                            
                            }
                            // we declined
                            else if (MW.DECLINED == true && STATUS == false)
                            {
                                response = "decline: " + mmid;
                                STATUS = true;
                                mmid = "0";
                                //Console.WriteLine("PP: " + mmid + " " + player + " pt_ready => Someone declined => we declined");
                            }
                            else
                            {
                                Thread.Sleep(200);
                                response = "Status: " + mmid;
                                //Console.WriteLine("PP: " + mmid + " " + player + " pt_ready => Someone declined => wait(Status)");
                            }
                        }
                    }
                    else
                    {
                        // all accepted :)
                        MW.Dispatcher.Invoke(() =>
                        {
                            MW.ALL_ACCEPTED = true;
                            MW.SetupPos(ent, player);
                        });
                        response = "fin";
                        //Console.WriteLine("PP: " + mmid + " " + player + " pt_ready => Someone declined => all accepted");
                    }

                    goto Response;
                }

                // How many players accepted?
                foreach (Match m_status in Regex.Matches(answer, pt_status))
                {
                    response = "Status: " + mmid;

                    int i = 0;
                    int j = 0;

                    MW.Dispatcher.Invoke(() =>
                    {
                        var pl_cb = MW.gr_mmacc.Children.OfType<CheckBox>().Where(x => x.Name.StartsWith("mmcb_acc"));
                        CheckBox cb_last = null;

                        foreach (string acc in m_status.Groups[1].ToString().Split(';'))
                        {
                            i++;
                            if (pl_cb != null)
                            {
                                List<CheckBox> llcb = pl_cb.Where(x => x.Name.Contains(i.ToString())).ToList();
                                CheckBox lcb = null;
                                if (llcb.Count > 0) lcb = llcb.ElementAt(0);
                                if (lcb != null)
                                {
                                    if (acc == "1")
                                    {
                                        lcb.IsChecked = true;
                                        j++;
                                    }
                                    else if (acc == "-1") 
                                    {
                                        lcb.IsChecked = false;
                                    }
                                    if (i == 6)
                                    {
                                        cb_last = lcb;
                                    }
                                }
                            }
                        }
                        if (cb_last != null) cb_last.Content = j.ToString() + "/6";
                    });

                    // we accepted
                    if (MW.ACCEPTED == true && STATUS == false)
                    {
                        response = "accept: " + mmid;
                        STATUS = true;
                        //Console.WriteLine("PP: " + mmid + " " + player + " pt_status => we accepted");
                     // we declined
                    } else if (MW.DECLINED == true && STATUS == false)
                    {
                        response = "decline: " + mmid;
                        STATUS = true;
                        mmid = "0";
                        //Console.WriteLine("PP: " + mmid + " " + player + " pt_status => we declined");
                    } else
                    {
                        Thread.Sleep(400);
                    }

                    goto Response;
                }

                // We are ready!
                foreach (Match m_accept in Regex.Matches(answer, pt_accept))

                {
                    mmid = m_accept.Groups[1].ToString();
                    response = "Ready_v2: " + mmid;

                    if (mmid == "0")
                    {
                        // Someone  already declined / timed out :(
                        MW.Dispatcher.Invoke(() =>
                        {
                            MW.ALL_DECLINED = true;
                        });

                        // Waiting again
                        Random rngesus = new Random();
                        int rng = rngesus.Next(0, 3000);
                        int mysleep = 5000 + rng;
                        Thread.Sleep(mysleep);
                        response = "Letmeplay: 1";
                        mmid = "0";
                        //Console.WriteLine("PP: " + mmid + " " + player + " pt_accept => already declined => wait");
                    }

                    goto Response;
                }
                // Searching ...
                foreach (Match m_findgame in Regex.Matches(answer, pt_findgame))
                {
                    string ent = m_findgame.Groups[1].ToString();
                    if (ent == "0")
                    {
                        // Waiting
                        Random rngesus = new Random();
                        int rng = rngesus.Next(0, 3000);
                        int mysleep = 5000 + rng;
                        Thread.Sleep(mysleep);

                        response = "Findgame: 0";

                        //Console.WriteLine("PP: " + mmid + " " + player + " pt_findgame => Searching");
                    }
                    else if (ent.StartsWith("Games"))
                    {
                        MW.Dispatcher.Invoke(() =>
                        {
                            MW.tb_info.Text = ent;
                        });
                        // Waiting
                        Random rngesus = new Random();
                        int rng = rngesus.Next(0, 3000);
                        int mysleep = 5000 + rng;
                        Thread.Sleep(mysleep);
                        response = "Findgame: 0";
                        //Console.WriteLine("PP: " + mmid + " " + player + " pt_findgame => Games");
                    } else  
                    {
                        mmid = ent;
                        // Matchup ready - waiting for Accept
                        MW.Dispatcher.Invoke(() =>
                        {
                            MW.GameFound(mmid);
                        });
                        // response = "wait";
                        response = "Status: " + mmid;
                        //Console.WriteLine("PP: " + mmid + " " + player + " pt_findgame => GameFound");
                    }
                    goto Response;
                }

                // we declined :(
                foreach (Match m_decline in Regex.Matches(answer, pt_decline))
                {
                    response = "fin";
                    mmid = "0";
                    MW.Dispatcher.Invoke(() =>
                    {
                        MW.DECLINED = false;
                        MW.bt_show.IsEnabled = true;
                        MW.tb_gamefound.Visibility = Visibility.Visible;
                        MW.tb_gamefound.Text = "Ready process declined :(";
                        try
                        {
                            MW._timer.Stop();
                        }
                        catch { }
                    });
                    //Console.WriteLine("PP: " + mmid + " " + player + " pt_decline => we declined");
                    goto Response;
                }
                // Find game
                foreach (Match m_letmeplay in Regex.Matches(answer, pt_letmeplay))
                {
                    //mmid = "0";
                    response = "Findgame: 0";
                    MW.Dispatcher.Invoke(() =>
                    {
                        MW.tb_gamefound.Text = "Searching ...";
                        MW.mmcb_ample.IsChecked = true;
                        MW.mmcb_ample.Content = "Online";
                    });
                    //Console.WriteLine("PP: " + mmid + " " + player + " pt_letmeplay => Searching");
                    goto Response;
                }
            }

            Console.WriteLine("ERR: " + answer);

                        

            Response:        

            //Console.WriteLine(player + ": " + pong + " => " + response);

            if (client != null)
            {

                    if (response != "wait")
                    {
                        response = "Hello from [" + player + "]: " + response + "\r\n";
                        byte[] msgBuf = Encoding.UTF8.GetBytes(response);
                        try
                        {
                            client.Send(msgBuf);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Client closed: {0}", e.ToString());
                        }
                    }




                    if (response == "fin")
                    {
                        try
                        {
                            StopClient(client);
                            client.Dispose();
                            client = null;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Client closed: {0}", e.ToString());
                        }
                        MW.Dispatcher.Invoke(() =>
                        {
                            MW.mmcb_ample.IsChecked = false;
                            MW.mmcb_ample.Content = "Offline";
                        });
                    }
                    else
                    {
                        byte[] bytes = new byte[1024];
                        string fin = "";
                        int bytesRec = 0;
                        try
                        {
                            bytesRec = client.Receive(bytes);
                            fin = Encoding.UTF8.GetString(bytes, 0, bytesRec);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Client closed: {0}", e.ToString());
                        }

                        if (bytesRec > 0) PingPong(Encoding.UTF8.GetString(bytes, 0, bytesRec), player, client, mmid);
                    }

                }

            else
            {
                MW.Dispatcher.Invoke(() =>
                {
                    MW.mmcb_ample.IsChecked = false;
                    MW.mmcb_ample.Content = "Offline";
                });
            }

            return response;
        }



    

        public bool CheckPong (string msg)
        {
            bool check = false;
            if (msg.Length < 2000)
            {
                check = true;
            } else
            {
                Console.WriteLine("Response too long :(");
            }
            return check;
        }

    }


    public class dsmmid
    {
        public int MMID { get; set; }
        public List<KeyValuePair<int, string>> PLAYERS { get; set; } = new List<KeyValuePair<int, string>>();
        public List<dsplayer> REPORTS { get; set; } = new List<dsplayer>();
        public string SERVER { get; set; }
        public string MOD { get; set; }
        public string NUM { get; set; }
        public int NEED { get; set; } = 0;
        public int REPORTED { get; set; } = 0;
        public int DURATION { get; set; } = 0;
        public dsreplay REPLAY {get; set; }

        public dsmmid()
        {
        }

    }

    public class dsmmladder
    {
        public int POS { get; set; }
        public string NAME { get; set; }
        public int GAMES { get; set; } = 0;
        public double ELO { get; set; } = 0;
        public double SIGMA { get; set; } = 0;
        public double EXP { get; set; } = 0;
    }
}

