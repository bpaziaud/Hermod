﻿/*
 * Copyright (c) 2010-2014, Achim 'ahzf' Friedland <achim@graphdefined.org>
 * This file is part of Hermod <http://www.github.com/Vanaheimr/Hermod>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using eu.Vanaheimr.Illias.Commons;
using eu.Vanaheimr.Hermod.Sockets.TCP;
using System.Net.Sockets;

#endregion

namespace eu.Vanaheimr.Hermod.Sockets.TCP
{

    public abstract class ACustomTCPServer : ITCPServer
    {

        #region Data

        protected internal readonly TCPServer TCPServer;

        #endregion

        #region Constructor(s)

        #region ACustomTCPServer(Port, ...)

        /// <summary>
        /// Initialize the TCP server using IPAddress.Any and the given parameters.
        /// </summary>
        /// <param name="Port">The listening port</param>
        /// <param name="ServerThreadName">The optional name of the TCP server thread.</param>
        /// <param name="ServerThreadPriority">The optional priority of the TCP server thread.</param>
        /// <param name="ServerThreadIsBackground">Whether the TCP server thread is a background thread or not.</param>
        /// <param name="ConnectionIdBuilder"></param>
        /// <param name="ConnectionThreadsNameCreator">An optional delegate to set the name of the TCP connection threads.</param>
        /// <param name="ConnectionThreadsPriority">The optional priority of the TCP connection threads.</param>
        /// <param name="ConnectionThreadsAreBackground">Whether the TCP conncection threads are background threads or not.</param>
        /// <param name="ConnectionTimeoutSeconds">The TCP client timeout for all incoming client connections in seconds.</param>
        /// <param name="Autostart">Start the TCP server thread immediately.</param>
        public ACustomTCPServer(IPPort                       Port,
                         String                       ServerThreadName                = null,
                         ThreadPriority               ServerThreadPriority            = ThreadPriority.AboveNormal,
                         Boolean                      ServerThreadIsBackground        = true,
                         Func<IPSocket, String>       ConnectionIdBuilder             = null,
                         Func<TCPConnection, String>  ConnectionThreadsNameCreator    = null,
                         ThreadPriority               ConnectionThreadsPriority       = ThreadPriority.AboveNormal,
                         Boolean                      ConnectionThreadsAreBackground  = true,
                         UInt64                       ConnectionTimeoutSeconds        = 30,
                         Boolean                      Autostart                       = false)

            : this(IPv4Address.Any,
                   Port,
                   ServerThreadName,
                   ServerThreadPriority,
                   ServerThreadIsBackground,
                   ConnectionIdBuilder,
                   ConnectionThreadsNameCreator,
                   ConnectionThreadsPriority,
                   ConnectionThreadsAreBackground,
                   ConnectionTimeoutSeconds,
                   Autostart)

        { }

        #endregion

        #region ACustomTCPServer(IIPAddress, Port, ...)

        /// <summary>
        /// Initialize the TCP server using the given parameters.
        /// </summary>
        /// <param name="IIPAddress">The listening IP address(es)</param>
        /// <param name="Port">The listening port</param>
        /// <param name="Mapper">A delegate to transform the incoming TCP connection data into custom data structures.</param>
        /// <param name="ServerThreadName">The optional name of the TCP server thread.</param>
        /// <param name="ServerThreadPriority">The optional priority of the TCP server thread.</param>
        /// <param name="ServerThreadIsBackground">Whether the TCP server thread is a background thread or not.</param>
        /// <param name="ConnectionIdBuilder"></param>
        /// <param name="ConnectionThreadsNameCreator">An optional delegate to set the name of the TCP connection threads.</param>
        /// <param name="ConnectionThreadsPriority">The optional priority of the TCP connection threads.</param>
        /// <param name="ConnectionThreadsAreBackground">Whether the TCP conncection threads are background threads or not.</param>
        /// <param name="ConnectionTimeoutSeconds">The TCP client timeout for all incoming client connections in seconds.</param>
        /// <param name="Autostart">Start the TCP server thread immediately.</param>
        public ACustomTCPServer(IIPAddress                   IIPAddress,
                                IPPort                       Port,
                                String                       ServerThreadName                = null,
                                ThreadPriority               ServerThreadPriority            = ThreadPriority.AboveNormal,
                                Boolean                      ServerThreadIsBackground        = true,
                                Func<IPSocket, String>       ConnectionIdBuilder             = null,
                                Func<TCPConnection, String>  ConnectionThreadsNameCreator    = null,
                                ThreadPriority               ConnectionThreadsPriority       = ThreadPriority.AboveNormal,
                                Boolean                      ConnectionThreadsAreBackground  = true,
                                UInt64                       ConnectionTimeoutSeconds        = 30,
                                Boolean                      Autostart                       = false)
        {

            //this.TCPServer = new TCPServer<TConnection>(IIPAddress,
            //                                            Port,
            //                                            ServerThreadName,
            //                                            ServerThreadPriority,
            //                                            ServerThreadIsBackground,
            //                                            ConnectionIdBuilder,
            //                                            ConnectionThreadsNameCreator,
            //                                            ConnectionThreadsPriority,
            //                                            ConnectionThreadsAreBackground,
            //                                            ConnectionTimeoutSeconds,
            //                                            Autostart);

        }

        #endregion

        #region ACustomTCPServer(IPSocket, ...)

        /// <summary>
        /// Initialize the TCP server using IPAddress.Any and the given parameters.
        /// </summary>
        /// <param name="IPSocket">The IP socket to listen.</param>
        /// <param name="ServerThreadName">The optional name of the TCP server thread.</param>
        /// <param name="ServerThreadPriority">The optional priority of the TCP server thread.</param>
        /// <param name="ServerThreadIsBackground">Whether the TCP server thread is a background thread or not.</param>
        /// <param name="ConnectionIdBuilder"></param>
        /// <param name="ConnectionThreadsNameCreator">An optional delegate to set the name of the TCP connection threads.</param>
        /// <param name="ConnectionThreadsPriority">The optional priority of the TCP connection threads.</param>
        /// <param name="ConnectionThreadsAreBackground">Whether the TCP conncection threads are background threads or not.</param>
        /// <param name="ConnectionTimeoutSeconds">The TCP client timeout for all incoming client connections in seconds.</param>
        /// <param name="Autostart">Start the TCP server thread immediately.</param>
        public ACustomTCPServer(IPSocket                     IPSocket,
                         String                       ServerThreadName                = null,
                         ThreadPriority               ServerThreadPriority            = ThreadPriority.AboveNormal,
                         Boolean                      ServerThreadIsBackground        = true,
                         Func<IPSocket, String>       ConnectionIdBuilder             = null,
                         Func<TCPConnection, String>  ConnectionThreadsNameCreator    = null,
                         ThreadPriority               ConnectionThreadsPriority       = ThreadPriority.AboveNormal,
                         Boolean                      ConnectionThreadsAreBackground  = true,
                         UInt64                       ConnectionTimeoutSeconds        = 30,
                         Boolean                      Autostart                       = false)

            : this(IPSocket.IPAddress,
                   IPSocket.Port,
                   ServerThreadName,
                   ServerThreadPriority,
                   ServerThreadIsBackground,
                   ConnectionIdBuilder,
                   ConnectionThreadsNameCreator,
                   ConnectionThreadsPriority,
                   ConnectionThreadsAreBackground,
                   ConnectionTimeoutSeconds,
                   Autostart)

        { }

        #endregion

        #endregion

        #region Events

        public event NewConnectionHandler    OnNewConnection;

        public event ConnectionClosedHandler OnConnectionClosed;

        #endregion






        protected internal void SendNewConnection(DateTime ServerTimestamp,
                                                  IPSocket RemoteSocket)
        {

            var OnNewConnectionLocal = OnNewConnection;
            if (OnNewConnectionLocal != null)
                OnNewConnectionLocal(this, ServerTimestamp, IPSocket, RemoteSocket, "...");

        }






        public String ServerThreadName
        {
            get
            {
                return TCPServer.ServerThreadName;
            }
            set
            {
                TCPServer.ServerThreadName = value;
            }
        }

        public ThreadPriority ServerThreadPriority
        {
            get
            {
                return TCPServer.ServerThreadPriority;
            }
            set
            {
                TCPServer.ServerThreadPriority = value;
            }
        }

        public bool ServerThreadIsBackground
        {
            get
            {
                return TCPServer.ServerThreadIsBackground;
            }
            set
            {
                TCPServer.ServerThreadIsBackground = value;
            }
        }

        public Func<IPSocket, String> ConnectionIdBuilder
        {
            get
            {
                return TCPServer.ConnectionIdBuilder;
            }
            set
            {
                TCPServer.ConnectionIdBuilder = value;
            }
        }

        public String ConnectionThreadsNameCreator
        {
            get
            {
                return TCPServer.ConnectionThreadsNameCreator;
            }
            set
            {
                TCPServer.ConnectionThreadsNameCreator = value;
            }
        }

        public ThreadPriority ConnectionThreadsPriority
        {
            get
            {
                return TCPServer.ConnectionThreadsPriority;
            }
            set
            {
                TCPServer.ConnectionThreadsPriority = value;
            }
        }

        public Boolean ConnectionThreadsAreBackground
        {
            get
            {
                return TCPServer.ConnectionThreadsAreBackground;
            }
            set
            {
                TCPServer.ConnectionThreadsAreBackground = value;
            }
        }

        public TimeSpan ConnectionTimeout
        {
            get
            {
                return TCPServer.ConnectionTimeout;
            }
            set
            {
                TCPServer.ConnectionTimeout = value;
            }
        }

        public ulong NumberOfClients
        {
            get
            {
                return TCPServer.NumberOfClients;
            }
        }

        public uint MaxClientConnections
        {
            get
            {
                return TCPServer.MaxClientConnections;
            }
        }

        public bool IsRunning
        {
            get
            {
                return TCPServer.IsRunning;
            }
        }

        public IIPAddress IPAddress
        {
            get
            {
                return TCPServer.IPAddress;
            }
        }

        public IPPort Port
        {
            get
            {
                return TCPServer.Port;
            }
        }

        public IPSocket IPSocket
        {
            get
            {
                return TCPServer.IPSocket;
            }
        }

        public string ServiceBanner
        {
            get
            {
                return TCPServer.ServiceBanner;
            }
            set
            {
                TCPServer.ServiceBanner = value;
            }
        }

        public void Start()
        {
            TCPServer.Start();
        }

        public void Start(TimeSpan Delay, bool InBackground = true)
        {
            TCPServer.Start(Delay, InBackground);
        }

        public void Shutdown(string Message = null, bool Wait = true)
        {
            TCPServer.Shutdown(Message, Wait);
        }

        public bool StopRequested
        {
            get
            {
                return TCPServer.StopRequested;
            }
        }

        public event Styx.Arrows.StartedEventHandler OnStarted
        {
            add
            {
                TCPServer.OnStarted += value;
            }
            remove
            {
                TCPServer.OnStarted -= value;
            }
        }

        public event Styx.Arrows.CompletedEventHandler OnCompleted
        {
            add
            {
                TCPServer.OnCompleted += value;
            }
            remove
            {
                TCPServer.OnCompleted -= value;
            }
        }

        public event Styx.Arrows.ExceptionOccuredEventHandler OnExceptionOccured
        {
            add
            {
                TCPServer.OnExceptionOccured += value;
            }
            remove
            {
                TCPServer.OnExceptionOccured -= value;
            }
        }

        public void Dispose()
        {
            TCPServer.Dispose();
        }

    }

}
