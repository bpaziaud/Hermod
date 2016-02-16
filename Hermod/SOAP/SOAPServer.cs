﻿/*
 * Copyright (c) 2010-2016, Achim 'ahzf' Friedland <achim@graphdefined.org>
 * This file is part of Vanaheimr Hermod <http://www.github.com/Vanaheimr/Hermod>
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
using System.Linq;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Sockets;
using org.GraphDefined.Vanaheimr.Hermod.Sockets.TCP;

#endregion

namespace org.GraphDefined.Vanaheimr.Hermod.SOAP
{

    /// <summary>
    /// A HTTP/SOAP/XML server.
    /// </summary>
    public class SOAPServer : HTTPServer
    {

        #region Data

        /// <summary>
        /// The default HTTP content type used for all SOAP requests/responses.
        /// </summary>
        public static readonly HTTPContentType DefaultSOAPContentType = HTTPContentType.XMLTEXT_UTF8;

        #endregion

        #region Properties

        #region SOAPContentType

        private readonly HTTPContentType _SOAPContentType;

        /// <summary>
        /// The SOAP XML HTTP content type.
        /// </summary>
        public HTTPContentType SOAPContentType
        {
            get
            {
                return _SOAPContentType;
            }
        }

        #endregion

        #region SOAPDispatchers

        private readonly Dictionary<String, SOAPDispatcher> _SOAPDispatchers;

        /// <summary>
        /// All registered SOAP dispatchers.
        /// </summary>
        public ILookup<String, SOAPDispatcher> SOAPDispatchers
        {
            get
            {
                return _SOAPDispatchers.ToLookup(_ => _.Key, _ => _.Value);
            }
        }

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initialize the SOAP server using the given parameters.
        /// </summary>
        /// <param name="TCPPort">An IP port to listen on.</param>
        /// <param name="DefaultServerName">The default HTTP servername, used whenever no HTTP Host-header had been given.</param>
        /// <param name="SOAPContentType">The default HTTP content type used for all SOAP requests/responses.</param>
        /// <param name="X509Certificate">Use this X509 certificate for TLS.</param>
        /// <param name="CallingAssemblies">A list of calling assemblies to include e.g. into embedded ressources lookups.</param>
        /// <param name="ServerThreadName">The optional name of the TCP server thread.</param>
        /// <param name="ServerThreadPriority">The optional priority of the TCP server thread.</param>
        /// <param name="ServerThreadIsBackground">Whether the TCP server thread is a background thread or not.</param>
        /// <param name="ConnectionIdBuilder">An optional delegate to build a connection identification based on IP socket information.</param>
        /// <param name="ConnectionThreadsNameBuilder">An optional delegate to set the name of the TCP connection threads.</param>
        /// <param name="ConnectionThreadsPriorityBuilder">An optional delegate to set the priority of the TCP connection threads.</param>
        /// <param name="ConnectionThreadsAreBackground">Whether the TCP connection threads are background threads or not (default: yes).</param>
        /// <param name="ConnectionTimeout">The TCP client timeout for all incoming client connections in seconds (default: 30 sec).</param>
        /// <param name="MaxClientConnections">The maximum number of concurrent TCP client connections (default: 4096).</param>
        /// <param name="DNSClient">The DNS client to use.</param>
        /// <param name="Autostart">Start the HTTP server thread immediately (default: no).</param>
        public SOAPServer(IPPort                            TCPPort                           = null,
                          String                            DefaultServerName                 = DefaultHTTPServerName,
                          HTTPContentType                   SOAPContentType                   = null,
                          X509Certificate2                  X509Certificate                   = null,
                          IEnumerable<Assembly>             CallingAssemblies                 = null,
                          String                            ServerThreadName                  = null,
                          ThreadPriority                    ServerThreadPriority              = ThreadPriority.AboveNormal,
                          Boolean                           ServerThreadIsBackground          = true,
                          ConnectionIdBuilder               ConnectionIdBuilder               = null,
                          ConnectionThreadsNameBuilder      ConnectionThreadsNameBuilder      = null,
                          ConnectionThreadsPriorityBuilder  ConnectionThreadsPriorityBuilder  = null,
                          Boolean                           ConnectionThreadsAreBackground    = true,
                          TimeSpan?                         ConnectionTimeout                 = null,
                          UInt32                            MaxClientConnections              = TCPServer.__DefaultMaxClientConnections,
                          DNSClient                         DNSClient                         = null,
                          Boolean                           Autostart                         = false)

            : base(TCPPort,
                   DefaultServerName,
                   X509Certificate,
                   CallingAssemblies,
                   ServerThreadName,
                   ServerThreadPriority,
                   ServerThreadIsBackground,
                   ConnectionIdBuilder,
                   ConnectionThreadsNameBuilder,
                   ConnectionThreadsPriorityBuilder,
                   ConnectionThreadsAreBackground,
                   ConnectionTimeout,
                   MaxClientConnections,
                   DNSClient,
                   false)

        {

            this._SOAPContentType  = SOAPContentType != null ? SOAPContentType : DefaultSOAPContentType;
            this._SOAPDispatchers  = new Dictionary<String, SOAPDispatcher>();

            if (Autostart)
                Start();

        }

        #endregion


        #region RegisterSOAPDelegate(URITemplate, Description, SOAPMatch, Delegate)

        /// <summary>
        /// Register a SOAP delegate.
        /// </summary>
        /// <param name="URITemplate">The URI template.</param>
        /// <param name="Description">A description of this SOAP delegate.</param>
        /// <param name="SOAPMatch">A delegate to check whether this dispatcher matches the given XML.</param>
        /// <param name="Delegate">A delegate to process a matching XML.</param>
        public void RegisterSOAPDelegate(String        URITemplate,
                                         String        Description,
                                         SOAPMatch     SOAPMatch,
                                         SOAPDelegate  Delegate)
        {

            SOAPDispatcher _SOAPDispatcher = null;

            // Check if there are other SOAP dispatchers at the given URI template.
            var _Handler = GetHandler(HTTPHostname.Any,
                                      URITemplate,
                                      HTTPMethod.POST,
                                      ContentTypes => _SOAPContentType);

            if (_Handler == null)
            {

                _SOAPDispatcher = new SOAPDispatcher(URITemplate);
                _SOAPDispatchers.Add(URITemplate, _SOAPDispatcher);

                // Register a new SOAP dispatcher
                AddMethodCallback(HTTPMethod.POST,
                                  URITemplate,
                                  _SOAPContentType,
                                  HTTPDelegate: _SOAPDispatcher.Invoke);

                // Register some information text for people using HTTP GET
                AddMethodCallback(HTTPMethod.GET,
                                  URITemplate,
                                  _SOAPContentType,
                                  HTTPDelegate: _SOAPDispatcher.EndpointTextInfo);

            }

            else
                _SOAPDispatcher = _Handler.Target as SOAPDispatcher;


            _SOAPDispatcher.RegisterSOAPDelegate(Description,
                                                 SOAPMatch,
                                                 Delegate);

        }

        #endregion

    }

}