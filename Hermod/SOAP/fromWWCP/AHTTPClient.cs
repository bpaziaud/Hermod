﻿/*
 * Copyright (c) 2010-2016, Achim 'ahzf' Friedland <achim.friedland@graphdefined.com>
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
using System.Xml.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace org.GraphDefined.Vanaheimr.Hermod.SOAP
{

    /// <summary>
    /// An abstract base class for all HTTP clients.
    /// </summary>
    public abstract class AHTTPClient : IDisposable
    {

        #region Data

        /// <summary>
        /// The default timeout for upstream queries.
        /// </summary>
        public static readonly TimeSpan  DefaultQueryTimeout   = TimeSpan.FromSeconds(180);

        /// <summary>
        /// The default HTTP user agent.
        /// </summary>
        public const           String    DefaultHTTPUserAgent  = "GraphDefined HTTP Client";

        /// <summary>
        /// The default remote TCP port to connect to.
        /// </summary>
        public static readonly IPPort    DefaultRemotePort     = IPPort.Parse(443);

        #endregion

        #region Properties

        /// <summary>
        /// A unqiue identification of this client.
        /// </summary>
        public String            ClientId                { get; }

        public String            Hostname                { get; }

        /// <summary>
        /// The remote TCP port to connect to.
        /// </summary>
        public IPPort            RemotePort              { get; }

        public String            HTTPVirtualHost         { get; }

        public String            UserAgent               { get; }

        /// <summary>
        /// The timeout for upstream requests.
        /// </summary>
        public TimeSpan?         RequestTimeout          { get; }

        /// <summary>
        /// The DNS client defines which DNS servers to use.
        /// </summary>
        public DNSClient         DNSClient               { get; }

        //   public X509Certificate2  ServerCert              { get; }

        public RemoteCertificateValidationCallback RemoteCertificateValidator { get; }

        public X509Certificate ClientCert { get; }

        //        public LocalCertificateSelectionCallback ClientCertificateSelector { get; set; }

        public Boolean          UseTLS                  { get; set; }

        #endregion

        #region Events

        #region OnException

        /// <summary>
        /// An event fired whenever an exception occured.
        /// </summary>
        public event OnExceptionDelegate OnException;

        #endregion

        #region OnHTTPError

        /// <summary>
        /// A delegate called whenever a HTTP error occured.
        /// </summary>
        public delegate void OnHTTPErrorDelegate(DateTime Timestamp, Object Sender, HTTPResponse HttpResponse);

        /// <summary>
        /// An event fired whenever a HTTP error occured.
        /// </summary>
        public event OnHTTPErrorDelegate OnHTTPError;

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an abstract HTTP client.
        /// </summary>
        /// <param name="ClientId">A unqiue identification of this client.</param>
        /// <param name="Hostname">The hostname to connect to.</param>
        /// <param name="RemotePort">The remote TCP port to connect to.</param>
        /// <param name="RemoteCertificateValidator">A delegate to verify the remote TLS certificate.</param>
        /// <param name="ClientCert">The TLS client certificate to use.</param>
        /// <param name="HTTPVirtualHost">An optional HTTP virtual host name to use.</param>
        /// <param name="UserAgent">An optional HTTP user agent to use.</param>
        /// <param name="QueryTimeout">An optional timeout for upstream queries.</param>
        /// <param name="DNSClient">An optional DNS client.</param>
        public AHTTPClient(String                               ClientId,
                           String                               Hostname,
                           IPPort                               RemotePort,
                           RemoteCertificateValidationCallback  RemoteCertificateValidator  = null,
                           X509Certificate                      ClientCert                  = null,
                           String                               HTTPVirtualHost             = null,
                           String                               UserAgent                   = DefaultHTTPUserAgent,
                           TimeSpan?                            QueryTimeout                = null,
                           DNSClient                            DNSClient                   = null)
        {

            #region Initial checks

            if (Hostname.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Hostname), "The given parameter must not be null or empty!");

            #endregion

            this.ClientId                    = ClientId;
            this.Hostname                    = Hostname;
            this.RemotePort                  = RemotePort ?? DefaultRemotePort;

            this.RemoteCertificateValidator  = RemoteCertificateValidator;
            this.ClientCert                  = ClientCert;

            this.HTTPVirtualHost             = HTTPVirtualHost.IsNotNullOrEmpty()
                                                    ? HTTPVirtualHost
                                                    : Hostname;

            this.UserAgent                   = UserAgent ?? DefaultHTTPUserAgent;

            this.RequestTimeout              = QueryTimeout != null
                                                  ? QueryTimeout.Value
                                                  : DefaultQueryTimeout;

            this.DNSClient                   = DNSClient == null
                                                  ? new DNSClient()
                                                  : DNSClient;

        }

        #endregion


        #region (protected) SendHTTPError(Timestamp, Sender, HttpResponse)

        /// <summary>
        /// Notify that an HTTP error occured.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the error received.</param>
        /// <param name="Sender">The sender of this error message.</param>
        /// <param name="HttpResponse">The HTTP response related to this error message.</param>
        protected void SendHTTPError(DateTime      Timestamp,
                                     Object        Sender,
                                     HTTPResponse  HttpResponse)
        {

            DebugX.Log("AOICPUpstreamService => HTTP Status Code: " + HttpResponse != null ? HttpResponse.HTTPStatusCode.ToString() : "<null>");

            OnHTTPError?.Invoke(Timestamp, Sender, HttpResponse);

        }

        #endregion

        #region (protected) SendException(Timestamp, Sender, Exception)

        /// <summary>
        /// Notify that an exception occured.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the exception.</param>
        /// <param name="Sender">The sender of this exception.</param>
        /// <param name="Exception">The exception itself.</param>
        protected void SendException(DateTime   Timestamp,
                                     Object     Sender,
                                     Exception  Exception)
        {

            DebugX.Log("AOICPUpstreamService => Exception: " + Exception.Message);

            OnException?.Invoke(Timestamp, Sender, Exception);

        }

        #endregion


        #region Dispose()

        /// <summary>
        /// Dispose this object.
        /// </summary>
        public virtual void Dispose()
        { }

        #endregion

    }

}
