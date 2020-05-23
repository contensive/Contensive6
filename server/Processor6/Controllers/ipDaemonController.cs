
//using System;
//using System.Net;
//using System.Net.Sockets;
//using System.Threading;
//using Contensive.Processor.Exceptions;

//namespace Contensive.Processor.Controllers {
//    //
//    public class IpDaemonController {
//        //
//        private dynamic cmdCallbackObject;
//        private int cmdListenPort;
//        private Thread cmdListenThread;
//        private const bool onThread = true;
//        //
//        //===============================================================================================
//        //   thread method for http Listener
//        //       decided on http because Contensive 4 used http, and it is easy to write clients
//        //       listens for commands on cmdPort
//        //
//        // short term fix -- calling object provides a call back routine ipDaemonCallBack(cmd, queryString, remoteIP)
//        //   must be one thread because it calls back into vb6
//        //
//        //   eventual solution -- the listener goes in the server, and creates an object to call into.
//        //
//        //===============================================================================================
//        //
//        private void thread_cmdListener()  {
//            //
//            string cmd = null;
//            string queryString = null;
//            string remoteIP = null;
//            string[] prefixes = null;
//            int prefixesCnt = 0;
//            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
//            IPHostEntry HostEntry = Dns.GetHostEntry(System.Net.Dns.GetHostName());
//            int ptr = 0;
//            HttpListenerContext context = null;
//            HttpListenerRequest request = null;
//            HttpListenerResponse response = null;
//            string responseString = null;
//            byte[] buffer = null;
//            System.IO.Stream output = null;
//            string Hint = "enter";
//            string hintPrefixes = "";
//            //
//            try {
//                //
//                if (!HttpListener.IsSupported) {
//                    throw new ArgumentException("This operating system does not support the required http listen methods");
//                } else {
//                    HttpListener cmdListener = null;
//                    //
//                    prefixes = new string[prefixesCnt + 1];
//                    prefixes[prefixesCnt] = "http://127.0.0.1:" + cmdListenPort + "/";
//                    prefixesCnt += 1;
//                    //
//                    Hint += ",building prefixes";
//                    foreach (IPAddress ipAddressInfo in HostEntry.AddressList) {
//                        if (ipAddressInfo.AddressFamily == AddressFamily.InterNetwork) {
//                            if (ipAddressInfo.ToString() != "127.0.0.1") {
//                                Array.Resize(ref prefixes, prefixesCnt + 1);
//                                prefixes[prefixesCnt] = "http://" + ipAddressInfo.ToString() + ":" + cmdListenPort + "/";
//                                prefixesCnt += 1;
//                            }
//                        }
//                    }
//                    if (prefixesCnt == 0) {
//                        throw new ArgumentException("No ip addresses are available");
//                    } else {
//                        //
//                        // Create a listener.
//                        //
//                        Hint += ",create listener";
//                        cmdListener = new HttpListener();
//                        for (ptr = 0; ptr < prefixesCnt; ptr++) {
//                            cmdListener.Prefixes.Add(prefixes[ptr]);
//                            hintPrefixes += "," + prefixes[ptr];
//                        }
//                        Hint += ",start";
//                        cmdListener.Start();
//                        do {
//                            context = cmdListener.GetContext();
//                            request = context.Request;
//                            response = context.Response;
//                            cmd = request.Url.LocalPath;
//                            queryString = request.Url.Query;
//                            Hint += ",cmd=[" + cmd + "],querystring=[" + queryString + "]";
//                            if (queryString.Length > 0) {
//                                if (queryString.left(1) == "?") {
//                                    queryString = queryString.Substring(1);
//                                }
//                            }
//                            remoteIP = request.RemoteEndPoint.Address.ToString();
//                            Hint += ",remoteIP=[" + remoteIP + "]";
//                            try {
//                                Hint += ",callback enter";
//                                responseString = cmdCallbackObject.ipDaemonCallback(cmd, queryString, remoteIP);
//                                Hint += ",callback exit";
//                            } catch (Exception ex) {
//                                //
//                                // should never return an error to the iDaemon
//                                //
//                                Microsoft.VisualBasic.FileIO.FileSystem.WriteAllText("C:\\clibIpDaemonDebug.log", Environment.NewLine + core.dateTimeMockable.ToString() + " Exception in callback, hintPrefixes=[" + hintPrefixes + "], hint=[" + Hint + "], ex=[" + ex.Message + "/" + ex.StackTrace + "]", true);
//                                responseString = "";
//                            }
//                            Hint += ",set buffer from responseString";
//                            if (responseString.Length <= 0) {
//                                buffer = System.Text.Encoding.Unicode.GetBytes("");
//                            } else {
//                                buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
//                            }
//                            Hint += ",write output from butter";
//                            response.ContentLength64 = buffer.Length;
//                            response.ContentType = "text/HTML";
//                            output = response.OutputStream;
//                            output.Write(buffer, 0, buffer.Length);
//                            output.Close();
//                        } while (true);
//                    }
//                }
//            } catch (HttpListenerException ex) {
//                //
//                //
//                //
//                Microsoft.VisualBasic.FileIO.FileSystem.WriteAllText("C:\\clibIpDaemonDebug.log", Environment.NewLine + core.dateTimeMockable.ToString() + " HttpListenerException, hintPrefixes=[" + hintPrefixes + "], hint=[" + Hint + "], ex=[" + ex.Message + "/" + ex.StackTrace + "]", true);
//            } catch (Exception ex) {
//                //
//                //
//                //
//                Microsoft.VisualBasic.FileIO.FileSystem.WriteAllText("C:\\clibIpDaemonDebug.log", Environment.NewLine + core.dateTimeMockable.ToString() + " Exception, hintPrefixes=[" + hintPrefixes + "], hint=[" + Hint + "], ex=[" + ex.Message + "/" + ex.StackTrace + "]", true);
//            }
//        }
//        //
//        //==========================================================================================
//        //   Stop listening
//        //==========================================================================================
//        //
//        public void startListening(object callbackObject, int listenPort) {
//            try {
//                //
//                //
//                //
//                cmdListenPort = listenPort;
//                cmdCallbackObject = callbackObject;
//                if (!onThread) {
//                    //
//                    // start on this thread and block
//                    //
//                } else {
//                    //
//                    // start on a new thread and return
//                    //
//                    cmdListenThread = new Thread(thread_cmdListener) {
//                        Name = "cmdListen",
//                        IsBackground = true
//                    };
//                    cmdListenThread.Start();
//                }
//            } catch (Exception) {
//                //
//                throw new GenericException("Error during ipDaemon.startListening");
//            }
//        }
//        //
//        //==========================================================================================
//        //   Stop listening
//        //==========================================================================================
//        //
//        public void stopListening()  {
//            try {
//                //
//                // abort sockets
//                //
//                if (!onThread) {
//                    //
//                    //
//                    //
//                } else if (cmdListenThread != null) {
//                    //
//                    //
//                    //
//                    cmdListenThread.Abort();
//                }
//            } catch (Exception) {
//                throw new GenericException("Error during ipDaemon.stopListening");
//            }
//        }
//    }
//}