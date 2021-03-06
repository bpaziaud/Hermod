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
using System.IO;
using System.Web;
using System.Linq;
using System.Threading;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Hermod.HTTP
{

    /// <summary>
    /// A read-only HTTP request header.
    /// </summary>
    public class HTTPRequest : AHTTPPDU
    {

        #region HTTPServer

        private readonly HTTPServer _HTTPServer;

        /// <summary>
        /// The HTTP server of this request.
        /// </summary>
        public HTTPServer HTTPServer
        {
            get
            {
                return _HTTPServer;
            }
        }

        #endregion

        #region First PDU line

        #region HTTPMethod

        private readonly HTTPMethod _HTTPMethod;

        /// <summary>
        /// The HTTP method.
        /// </summary>
        public HTTPMethod HTTPMethod
        {
            get
            {
                return _HTTPMethod;
            }
        }

        #endregion

        #region FakeURIPrefix

        private String _FakeURIPrefix;

        /// <summary>
        /// Add this prefix to the URI before sending the request.
        /// </summary>
        public String FakeURIPrefix
        {

            get
            {
                return _FakeURIPrefix;
            }

            internal set
            {
                _FakeURIPrefix = value;
            }

        }

        #endregion

        #region URI

        /// <summary>
        /// The minimal URI (this means e.g. without the query string).
        /// </summary>
        public String URI { get; }

        #endregion

        #region ParsedURIParameters

        private String[] _ParsedURIParameters;

        /// <summary>
        /// The parsed URI parameters of the best matching URI template.
        /// Set by the HTTP server.
        /// </summary>
        public String[] ParsedURIParameters
        {

            get
            {
                return _ParsedURIParameters;
            }

            internal set
            {
                if (value != null)
                    _ParsedURIParameters = value;
            }

        }

        #endregion

        #region QueryString

        private readonly QueryString _QueryString;

        /// <summary>
        /// The HTTP query string.
        /// </summary>
        public QueryString QueryString
        {
            get
            {
                return _QueryString;
            }
        }

        #endregion

        #region BestMatchingAcceptType

        private HTTPContentType _BestMatchingAcceptType;

        /// <summary>
        /// The best matching accept type.
        /// Set by the HTTP server.
        /// </summary>
        public HTTPContentType BestMatchingAcceptType
        {

            get
            {
                return _BestMatchingAcceptType;
            }

            internal set
            {
                if (value != null)
                    _BestMatchingAcceptType = value;
            }

        }

        #endregion

        #region ProtocolName

        private readonly String _ProtocolName;

        /// <summary>
        /// The HTTP protocol name field.
        /// </summary>
        public String ProtocolName
        {
            get
            {
                return _ProtocolName;
            }
        }

        #endregion

        #region ProtocolVersion

        private readonly HTTPVersion _ProtocolVersion;

        /// <summary>
        /// The HTTP protocol version.
        /// </summary>
        public HTTPVersion ProtocolVersion
        {
            get
            {
                return _ProtocolVersion;
            }
        }

        #endregion


        #region EntireRequestHeader

        /// <summary>
        /// Construct the entire HTTP header.
        /// </summary>
        public String EntireRequestHeader
        {
            get
            {

                return HTTPMethod.ToString() + " " + this.FakeURIPrefix + this.URI + QueryString + " " + ProtocolName + "/" + ProtocolVersion +
                       Environment.NewLine +
                       ConstructedHTTPHeader;

            }
        }

        #endregion

        #endregion

        #region Standard request header fields

        #region Accept

        protected AcceptTypes _Accept;

        /// <summary>
        /// The http content types accepted by the client.
        /// </summary>
        public AcceptTypes Accept
        {

            get
            {

                _Accept = GetHeaderField<AcceptTypes>("Accept");
                if (_Accept != null)
                    return _Accept;

                var _AcceptString = GetHeaderField<String>("Accept");

                if (!_AcceptString.IsNullOrEmpty())
                {
                    _Accept = new AcceptTypes(_AcceptString);
                    SetHeaderField("Accept", _Accept);
                    return _Accept;
                }

                else
                    return new AcceptTypes();

            }

        }

        #endregion

        #region Accept-Charset

        public String AcceptCharset
        {
            get
            {
                return GetHeaderField(HTTPHeaderField.AcceptCharset);
            }
        }

        #endregion

        #region Accept-Encoding

        public String AcceptEncoding
        {
            get
            {
                return GetHeaderField(HTTPHeaderField.AcceptEncoding);
            }
        }

        #endregion

        #region Accept-Language

        public String AcceptLanguage
        {
            get
            {
                return GetHeaderField(HTTPHeaderField.AcceptLanguage);
            }
        }

        #endregion

        #region Accept-Ranges

        public String AcceptRanges
        {
            get
            {
                return GetHeaderField(HTTPHeaderField.AcceptRanges);
            }
        }

        #endregion

        #region Authorization

        /// <summary>
        /// The HTTP basic authentication.
        /// </summary>
        public HTTPBasicAuthentication Authorization
        {
            get
            {

                var _Authorization = GetHeaderField<HTTPBasicAuthentication>("Authorization");
                if (_Authorization != null)
                    return _Authorization;

                var _AuthString = GetHeaderField<String>("Authorization");

                if (_AuthString == null)
                    return null;

                if (HTTPBasicAuthentication.TryParse(_AuthString, out _Authorization))
                    SetHeaderField("Authorization", _Authorization);

                return _Authorization;

            }
        }

        #endregion

        #region Depth

        public String Depth
        {
            get
            {
                return GetHeaderField(HTTPHeaderField.Depth);
            }
        }

        #endregion

        #region Destination

        public String Destination
        {
            get
            {
                return GetHeaderField(HTTPHeaderField.Destination);
            }
        }

        #endregion

        #region Expect

        public String Expect
        {
            get
            {
                return GetHeaderField(HTTPHeaderField.Expect);
            }
        }

        #endregion

        #region From

        public String From
        {
            get
            {
                return GetHeaderField(HTTPHeaderField.From);
            }
        }

        #endregion

        #region Host

        public HTTPHostname Host
            => HTTPHostname.Parse(GetHeaderField(HTTPHeaderField.Host));

        #endregion

        #region If

        public String If
        {
            get
            {
                return GetHeaderField(HTTPHeaderField.If);
            }
        }

        #endregion

        #region If-Match

        public String IfMatch
        {
            get
            {
                return GetHeaderField(HTTPHeaderField.IfMatch);
            }
        }

        #endregion

        #region If-Modified-Since

        public String IfModifiedSince
        {
            get
            {
                return GetHeaderField(HTTPHeaderField.IfModifiedSince);
            }
        }

        #endregion

        #region If-None-Match

        public String IfNoneMatch
        {
            get
            {
                return GetHeaderField(HTTPHeaderField.IfNoneMatch);
            }
        }

        #endregion

        #region If-Range

        public String IfRange
        {
            get
            {
                return GetHeaderField(HTTPHeaderField.IfRange);
            }
        }

        #endregion

        #region If-Unmodified-Since

        public String IfUnmodifiedSince
        {
            get
            {
                return GetHeaderField(HTTPHeaderField.IfUnmodifiedSince);
            }
        }

        #endregion

        #region Lock-Token

        public String LockToken
        {
            get
            {
                return GetHeaderField(HTTPHeaderField.LockToken);
            }
        }

        #endregion

        #region MaxForwards

        public UInt64? MaxForwards
        {
            get
            {
                return GetHeaderField_UInt64(HTTPHeaderField.MaxForwards);
            }
        }

        #endregion

        #region Overwrite

        public String Overwrite
        {
            get
            {
                return GetHeaderField(HTTPHeaderField.Overwrite);
            }
        }

        #endregion

        #region Proxy-Authorization

        public String ProxyAuthorization
        {
            get
            {
                return GetHeaderField(HTTPHeaderField.ProxyAuthorization);
            }
        }

        #endregion

        #region Range

        public String Range
            => GetHeaderField(HTTPHeaderField.Range);

        #endregion

        #region Referer

        public String Referer
            => GetHeaderField(HTTPHeaderField.Referer);

        #endregion

        #region TE

        public String TE
            => GetHeaderField(HTTPHeaderField.TE);

        #endregion

        #region Timeout

        public UInt64? Timeout
            => GetHeaderField_UInt64(HTTPHeaderField.Timeout);

        #endregion

        #region User-Agent

        public String UserAgent
            => GetHeaderField(HTTPHeaderField.UserAgent);

        #endregion

        #region Last-Event-Id

        public UInt64? LastEventId
            => GetHeaderField_UInt64(HTTPHeaderField.LastEventId);

        #endregion

        #region Cookie

        public HTTPCookie Cookie
            => HTTPCookie.Parse(GetHeaderField(HTTPHeaderField.Cookie));

        #endregion

        #endregion

        #region Non-standard request header fields

        #region X-Real-IP

        /// <summary>
        /// Intermediary HTTP proxies might include this field to
        /// indicate the real IP address of the HTTP client.
        /// </summary>
        /// <example>X-Real-IP: 95.91.73.30</example>
        public IIPAddress X_Real_IP
        {

            get
            {

                Object Value;

                if (!TryGetHeaderField(HTTPHeaderField.X_Real_IP, out Value))
                    return null;

                IPv4Address IPv4;
                IPv6Address IPv6;

                if      (IPv4Address.TryParse((String) Value, out IPv4))
                    return IPv4;

                else if (IPv6Address.TryParse((String) Value, out IPv6))
                    return IPv6;

                else return null;

            }

        }

        #endregion

        #region X-Forwarded-For

        /// <summary>
        /// Intermediary HTTP proxies might include this field to
        /// indicate the real IP address of the HTTP client.
        /// </summary>
        /// <example>X-Forwarded-For: 95.91.73.30</example>
        public IIPAddress X_Forwarded_For
        {

            get
            {

                Object Value;

                if (!TryGetHeaderField(HTTPHeaderField.X_Forwarded_For, out Value))
                    return null;

                IPv4Address IPv4;
                IPv6Address IPv6;

                if      (IPv4Address.TryParse((String) Value, out IPv4))
                    return IPv4;

                else if (IPv6Address.TryParse((String) Value, out IPv6))
                    return IPv6;

                else return null;

            }

        }

        #endregion

        #endregion


        #region Constructor(s)

        #region (private) HTTPRequest(RemoteSocket, LocalSocket, HTTPServer, HTTPHeader, HTTPBody = null, HTTPBodyStream = null, CancellationToken = null, EventTrackingId = null)

        /// <summary>
        /// Create a new http request header based on the given string representation.
        /// </summary>
        /// <param name="RemoteSocket">The remote TCP/IP socket.</param>
        /// <param name="LocalSocket">The local TCP/IP socket.</param>
        /// <param name="HTTPServer">The HTTP server of the request.</param>
        /// <param name="HTTPHeader">A valid string representation of a http request header.</param>
        /// <param name="HTTPBody">The HTTP body as an array of bytes.</param>
        /// <param name="HTTPBodyStream">The HTTP body as an stream of bytes.</param>
        /// <param name="CancellationToken">A token to cancel the HTTP request processing.</param>
        /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
        private HTTPRequest(IPSocket            RemoteSocket,
                            IPSocket            LocalSocket,
                            HTTPServer          HTTPServer,
                            String              HTTPHeader,
                            Byte[]              HTTPBody           = null,
                            Stream              HTTPBodyStream     = null,
                            CancellationToken?  CancellationToken  = null,
                            EventTracking_Id    EventTrackingId    = null)

            : base(RemoteSocket,
                   LocalSocket,
                   HTTPHeader,
                   HTTPBody,
                   HTTPBodyStream,
                   CancellationToken,
                   EventTrackingId)

        {

            this._HTTPServer = HTTPServer;

            #region Parse HTTPMethod (first line of the http request)

            var _HTTPMethodHeader = FirstPDULine.Split(_SpaceSeparator, StringSplitOptions.RemoveEmptyEntries);

            // e.g: PROPFIND /file/file Name HTTP/1.1
            if (_HTTPMethodHeader.Length != 3)
                throw new Exception("Bad request");

            // Parse HTTP method
            // Propably not usefull to define here, as we can not send a response having an "Allow-header" here!
            this._HTTPMethod = (HTTPMethod.TryParseString(_HTTPMethodHeader[0], out _HTTPMethod))
                                   ? _HTTPMethod
                                   : HTTPMethod.Create(_HTTPMethodHeader[0]);

            #endregion

            #region Parse URL and QueryString (first line of the http request)

            var RawUrl      = _HTTPMethodHeader[1];
            var _ParsedURL  = RawUrl.Split(_URLSeparator, 2, StringSplitOptions.None);
            this.URI       = HttpUtility.UrlDecode(_ParsedURL[0]);

            if (URI.StartsWith("http", StringComparison.Ordinal) || URI.StartsWith("https", StringComparison.Ordinal))
            {
                URI = URI.Substring(URI.IndexOf("://", StringComparison.Ordinal) + 3);
                URI = URI.Substring(URI.IndexOf("/",   StringComparison.Ordinal) + 1);
            }

            if (URI == "" || URI == null)
                URI = "/";

            // Parse QueryString after '?'
            if (RawUrl.IndexOf('?') > -1 && _ParsedURL[1].IsNeitherNullNorEmpty())
                this._QueryString = QueryString.Parse(_ParsedURL[1]);
            else
                this._QueryString = QueryString.Empty;

            #endregion

            #region Parse protocol name and -version (first line of the http request)

            var _ProtocolArray  = _HTTPMethodHeader[2].Split(_SlashSeparator, 2, StringSplitOptions.RemoveEmptyEntries);
            this._ProtocolName  = _ProtocolArray[0].ToUpper();

            if (ProtocolName.ToUpper() != "HTTP")
                throw new Exception("Bad request");

            HTTPVersion _HTTPVersion = null;

            if (HTTPVersion.TryParseVersionString(_ProtocolArray[1], out _HTTPVersion))
                this._ProtocolVersion  = _HTTPVersion;

            if (ProtocolVersion != HTTPVersion.HTTP_1_0 && ProtocolVersion != HTTPVersion.HTTP_1_1)
                throw new Exception("HTTP version not supported");

            #endregion


            #region Check Host header

            // rfc 2616 - Section 19.6.1.1
            // A client that sends an HTTP/1.1 request MUST send a Host header.

            // rfc 2616 - Section 14.23
            // All Internet-based HTTP/1.1 servers MUST respond with a 400 (Bad Request)
            // status code to any HTTP/1.1 request message which lacks a Host header field.

            // rfc 2616 - Section 5.2 The Resource Identified by a Request
            // 1. If Request-URI is an absoluteURI, the host is part of the Request-URI.
            //    Any Host header field value in the request MUST be ignored.
            // 2. If the Request-URI is not an absoluteURI, and the request includes a
            //    Host header field, the host is determined by the Host header field value.
            // 3. If the host as determined by rule 1 or 2 is not a valid host on the server,
            //    the response MUST be a 400 (Bad Request) error message. (Not valid for proxies?!)
            if (!_HeaderFields.ContainsKey(HTTPHeaderField.Host.Name))
                throw new Exception("The HTTP PDU does not have a HOST header!");

            // rfc 2616 - 3.2.2
            // If the port is empty or not given, port 80 is assumed.
            var    HostHeader  = _HeaderFields[HTTPHeaderField.Host.Name].ToString().
                                     Split(_ColonSeparator, StringSplitOptions.RemoveEmptyEntries).
                                     Select(v => v.Trim()).
                                     ToArray();

            UInt16 HostPort    = 80;

            if (HostHeader.Length == 1)
                _HeaderFields[HTTPHeaderField.Host.Name] = _HeaderFields[HTTPHeaderField.Host.Name].ToString();// + ":80"; ":80" will cause side effects!

            else if ((HostHeader.Length == 2 && !UInt16.TryParse(HostHeader[1], out HostPort)) || HostHeader.Length > 2)
                throw new Exception("Bad request");

            #endregion

        }

        #endregion

        #region HTTPRequest(HTTPServer, CancellationToken, EventTrackingId, RemoteSocket, LocalSocket, HTTPHeader, HTTPBodyStream)

        /// <summary>
        /// Create a new http request based on the given string representation of a HTTP header and a HTTP body stream.
        /// </summary>
        /// <param name="HTTPServer">The HTTP server of the request.</param>
        /// <param name="CancellationToken">A token to cancel the HTTP request processing.</param>
        /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
        /// <param name="RemoteSocket">The remote TCP/IP socket.</param>
        /// <param name="LocalSocket">The local TCP/IP socket.</param>
        /// <param name="HTTPHeader">A valid string representation of a http request header.</param>
        /// <param name="HTTPBodyStream">The HTTP body as an stream of bytes.</param>
        public HTTPRequest(HTTPServer         HTTPServer,
                           CancellationToken  CancellationToken,
                           EventTracking_Id   EventTrackingId,
                           IPSocket           RemoteSocket,
                           IPSocket           LocalSocket,
                           String             HTTPHeader,
                           Stream             HTTPBodyStream)

            : this(RemoteSocket,
                   LocalSocket,
                   HTTPServer,
                   HTTPHeader,
                   null,
                   HTTPBodyStream,
                   CancellationToken,
                   EventTrackingId)

        {

            // HTTP/1.0 might come without any host header
            if (!_HeaderFields.ContainsKey("Host"))
                _HeaderFields.Add("Host", "*");

        }

        #endregion

        #region HTTPRequest(HTTPHeader)

        /// <summary>
        /// Create a new http request header based on the given string representation.
        /// </summary>
        /// <param name="HTTPHeader">The string representation of a HTTP header.</param>
        public HTTPRequest(String HTTPHeader)

            : this(null, null, null, HTTPHeader)

        { }

        #endregion

        #region HTTPRequest(HTTPHeader, HTTPBody)

        /// <summary>
        /// Create a new http request header based on the given string representation.
        /// </summary>
        /// <param name="HTTPHeader">The string representation of a HTTP header.</param>
        /// <param name="HTTPBody">The HTTP body as an array of bytes.</param>
        public HTTPRequest(String  HTTPHeader,
                           Byte[]  HTTPBody)

            : this(null, null, null, HTTPHeader, HTTPBody)

        { }

        #endregion

        #region HTTPRequest(HTTPHeader, HTTPBodyStream)

        /// <summary>
        /// Create a new http request header based on the given string representation.
        /// </summary>
        /// <param name="HTTPHeader">The string representation of a HTTP header.</param>
        /// <param name="HTTPBodyStream">The HTTP body as an stream of bytes.</param>
        public HTTPRequest(String  HTTPHeader,
                           Stream  HTTPBodyStream)

            : this(null, null, null, HTTPHeader, null, HTTPBodyStream)
        { }

        #endregion

        #endregion


        #region (override) ToString()

        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override String ToString()
            => EntirePDU;

        #endregion

    }

}
