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
using System.Linq;
using System.Collections.Generic;

using org.GraphDefined.Vanaheimr.Illias;
using System.Threading;

#endregion

namespace org.GraphDefined.Vanaheimr.Hermod.HTTP
{

    public static class HTTPContentHelper
    {

        public static HTTPResponse<TResult> ParseContent<TResult>(this HTTPResponse      Response,
                                                                  Func<Byte[], TResult>  ContentParser)

            => new HTTPResponse<TResult>(Response, ContentParser(Response.HTTPBody));


        public static HTTPResponse<TResult> ParseContentStream<TResult>(this HTTPResponse      Response,
                                                                        Func<Stream, TResult>  ContentParser)

            => new HTTPResponse<TResult>(Response, ContentParser(Response.HTTPBodyStream));

    }


    #region HTTPResponse<TContent>

    /// <summary>
    /// A helper class to transport HTTP data and its metadata.
    /// </summary>
    /// <typeparam name="TContent">The type of the parsed data.</typeparam>
    public class HTTPResponse<TContent> : HTTPResponse
    {

        #region Data

        private readonly Boolean _IsFault;

        #endregion

        #region Properties

        /// <summary>
        /// The parsed content.
        /// </summary>
        public TContent   Content    { get; }

        /// <summary>
        /// An exception during parsing.
        /// </summary>
        public Exception  Exception  { get; }

        /// <summary>
        /// An error during parsing.
        /// </summary>
        public Boolean HasErrors
            => Exception != null && !_IsFault;

        #endregion

        #region Constructor(s)

        #region HTTPResponse(Response, Content, IsFault = false, Exception = null)

        public HTTPResponse(HTTPResponse  Response,
                            TContent      Content,
                            Boolean       IsFault    = false,
                            Exception     Exception  = null)

            : base(Response)

        {

            this.Content    = Content;
            this._IsFault   = IsFault;
            this.Exception  = Exception;

        }

        #endregion

        #region HTTPResponse(Response, IsFault)

        public HTTPResponse(HTTPResponse  Response,
                            Boolean       IsFault)

            : this(Response,
                   default(TContent),
                   IsFault)

        { }

        #endregion

        #region HTTPResponse(Response, Exception)

        public HTTPResponse(HTTPResponse  Response,
                            Exception     Exception)

            : this(Response,
                   default(TContent),
                   true,
                   Exception)

        { }

        #endregion


        #region (private) HTTPResponse(Content, Exception)

        private HTTPResponse(TContent   Content,
                             Exception  Exception)

            : base(null)

        {

            this.Content    = Content;
            this._IsFault   = true;
            this.Exception  = Exception;

        }

        #endregion


        #region HTTPResponse(Request, Content)

        private HTTPResponse(HTTPRequest  Request,
                             TContent     Content)

            : this(HTTPResponseBuilder.OK(Request), Content, false)

        { }

        #endregion

        #region HTTPResponse(Request, Exception)

        public HTTPResponse(HTTPRequest  Request,
                            Exception    Exception)

            : this(new HTTPResponseBuilder(Request) { HTTPStatusCode = HTTPStatusCode.BadRequest },
                   default(TContent),
                   true,
                   Exception)

        { }

        #endregion

        #endregion


        #region ConvertContent<TResult>(ContentConverter)

        /// <summary>
        /// Convert the content of the HTTP response body via the given
        /// content converter delegate.
        /// </summary>
        /// <typeparam name="TResult">The type of the converted HTTP response body content.</typeparam>
        /// <param name="ContentConverter">A delegate to convert the given HTTP response content.</param>
        public HTTPResponse<TResult> ConvertContent<TResult>(Func<TContent, TResult> ContentConverter)
        {

            if (ContentConverter == null)
                throw new ArgumentNullException(nameof(ContentConverter),  "The given content converter delegate must not be null!");

            return new HTTPResponse<TResult>(this, ContentConverter(this.Content));

        }

        #endregion

        #region ConvertContent<TResult>(ContentConverter, OnException = null)

        /// <summary>
        /// Convert the content of the HTTP response body via the given
        /// content converter delegate.
        /// </summary>
        /// <typeparam name="TResult">The type of the converted HTTP response body content.</typeparam>
        /// <param name="ContentConverter">A delegate to convert the given HTTP response content.</param>
        /// <param name="OnException">A delegate to call whenever an exception during the conversion occures.</param>
        public HTTPResponse<TResult> ConvertContent<TResult>(Func<TContent, OnExceptionDelegate, TResult>  ContentConverter,
                                                             OnExceptionDelegate                           OnException = null)
        {

            if (ContentConverter == null)
                throw new ArgumentNullException(nameof(ContentConverter), "The given content converter delegate must not be null!");

            return new HTTPResponse<TResult>(this, ContentConverter(this.Content, OnException));

        }

        #endregion


        #region ConvertContent<TRequest, TResult>(Request, ContentConverter, OnException = null)

        /// <summary>
        /// Convert the content of the HTTP response body via the given
        /// content converter delegate.
        /// </summary>
        /// <typeparam name="TRequest">The type of the converted HTTP request body content.</typeparam>
        /// <typeparam name="TResult">The type of the converted HTTP response body content.</typeparam>
        /// <param name="Request">The request leading to this response.</param>
        /// <param name="ContentConverter">A delegate to convert the given HTTP response content.</param>
        /// <param name="OnException">A delegate to call whenever an exception during the conversion occures.</param>
        public HTTPResponse<TResult> ConvertContent<TRequest, TResult>(TRequest                                                Request,
                                                                       Func<TRequest, TContent, OnExceptionDelegate, TResult>  ContentConverter,
                                                                       OnExceptionDelegate                                     OnException = null)
        {

            if (ContentConverter == null)
                throw new ArgumentNullException(nameof(ContentConverter), "The given content converter delegate must not be null!");

            return new HTTPResponse<TResult>(this, ContentConverter(Request, this.Content, OnException));

        }

        #endregion



        public static HTTPResponse<TContent> OK(HTTPRequest  HTTPRequest,
                                                TContent     Content)

            => new HTTPResponse<TContent>(HTTPRequest, Content);

        public static HTTPResponse<TContent> OK(TContent Content)

            => new HTTPResponse<TContent>(null, Content);

        public static HTTPResponse<TContent> ClientError(TContent Content)

            => new HTTPResponse<TContent>(null, Content, IsFault: true);

        public static HTTPResponse<TContent> ExceptionThrown(TContent   Content,
                                                             Exception  Exception)

            => new HTTPResponse<TContent>(Content, Exception);

    }

    #endregion

    #region HTTPResponse

    /// <summary>
    /// A read-only HTTP response header.
    /// </summary>
    public class HTTPResponse : AHTTPPDU
    {

        #region HTTPRequest

        private readonly HTTPRequest _HTTPRequest;

        /// <summary>
        /// The HTTP request for this HTTP response.
        /// </summary>
        public HTTPRequest  HTTPRequest
        {
            get
            {
                return _HTTPRequest;
            }
        }

        #endregion

        #region First PDU line

        #region HTTPStatusCode

        private readonly HTTPStatusCode _HTTPStatusCode;

        /// <summary>
        /// The HTTP status code.
        /// </summary>
        public HTTPStatusCode HTTPStatusCode
        {
            get
            {
                return _HTTPStatusCode;
            }
        }

        #endregion

        #endregion

        #region Standard response header fields

        #region Age

        public UInt64? Age
        {
            get
            {
                return GetHeaderField_UInt64(HTTPHeaderField.Age);
            }
        }

        #endregion

        #region Allow

        public List<HTTPMethod> Allow
        {
            get
            {
                return GetHeaderField<List<HTTPMethod>>(HTTPHeaderField.Allow);
            }
        }

        #endregion

        #region DAV

        public String DAV
        {
            get
            {
                return GetHeaderField(HTTPHeaderField.DAV);
            }
        }

        #endregion

        #region ETag

        public String ETag
        {
            get
            {
                return GetHeaderField(HTTPHeaderField.ETag);
            }
        }

        #endregion

        #region Expires

        public String Expires
        {
            get
            {
                return GetHeaderField(HTTPHeaderField.Expires);
            }
        }

        #endregion

        #region LastModified

        public String LastModified
        {
            get
            {
                return GetHeaderField(HTTPHeaderField.LastModified);
            }
        }

        #endregion

        #region Location

        public String Location
        {
            get
            {
                return GetHeaderField(HTTPHeaderField.Location);
            }
        }

        #endregion

        #region ProxyAuthenticate

        public String ProxyAuthenticate
        {
            get
            {
                return GetHeaderField(HTTPHeaderField.ProxyAuthenticate);
            }
        }

        #endregion

        #region RetryAfter

        public String RetryAfter
        {
            get
            {
                return GetHeaderField(HTTPHeaderField.RetryAfter);
            }
        }

        #endregion

        #region Server

        public String Server
        {
            get
            {
                return GetHeaderField(HTTPHeaderField.Server);
            }
        }

        #endregion

        #region Vary

        public String Vary
        {
            get
            {
                return GetHeaderField(HTTPHeaderField.Vary);
            }
        }

        #endregion

        #region WWWAuthenticate

        public String WWWAuthenticate
        {
            get
            {
                return GetHeaderField(HTTPHeaderField.WWWAuthenticate);
            }
        }

        #endregion

        #region TransferEncoding

        public String TransferEncoding
        {
            get
            {
                return GetHeaderField(HTTPHeaderField.TransferEncoding);
            }
        }

        #endregion

        #endregion

        #region Non-standard response header fields

        #endregion


        #region Constructor(s)

        #region (internal) HTTPResponse(Response)

        /// <summary>
        /// Create a new HTTP response based on the given HTTP response.
        /// (e.g. upgrade a HTTPResponse to a HTTPResponse&lt;TContent&gt;)
        /// </summary>
        /// <param name="Response">A HTTP response.</param>
        internal HTTPResponse(HTTPResponse Response)

            : base(Response)

        {

            this._HTTPRequest     = Response?.HTTPRequest;
            this._HTTPStatusCode  = Response?.HTTPStatusCode;

        }

        #endregion

        #region (private) HTTPResponse(...)

        /// <summary>
        /// Parse the given HTTP response header.
        /// </summary>
        /// <param name="HTTPRequest">The HTTP request for this HTTP response.</param>
        /// <param name="RemoteSocket">The remote TCP/IP socket.</param>
        /// <param name="LocalSocket">The local TCP/IP socket.</param>
        /// <param name="HTTPHeader">A valid string representation of a http response header.</param>
        /// <param name="HTTPBody">The HTTP body as an array of bytes.</param>
        /// <param name="HTTPBodyStream">The HTTP body as an stream of bytes.</param>
        /// <param name="CancellationToken">A token to cancel the HTTP response processing.</param>
        /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
        private HTTPResponse(HTTPRequest         HTTPRequest,
                             IPSocket            RemoteSocket,
                             IPSocket            LocalSocket,
                             String              HTTPHeader,
                             Byte[]              HTTPBody           = null,
                             Stream              HTTPBodyStream     = null,
                             CancellationToken?  CancellationToken  = null,
                             EventTracking_Id    EventTrackingId    = null)

            : base(RemoteSocket, LocalSocket, HTTPHeader, HTTPBody, HTTPBodyStream, CancellationToken, EventTrackingId)

        {

            this._HTTPRequest  = HTTPRequest;

            #region Parse HTTP status code

            var _StatusCodeLine = FirstPDULine.Split(' ');

            if (_StatusCodeLine.Length < 3)
                throw new Exception("Bad request");

            this._HTTPStatusCode = HTTPStatusCode.ParseString(_StatusCodeLine[1]);

            #endregion

        }

        #endregion


        // Parse the HTTP response from its text-representation...

        #region (private) HTTPResponse(ResponseHeader, Request)

        /// <summary>
        /// Create a new HTTP response.
        /// </summary>
        /// <param name="ResponseHeader">The HTTP header of the response.</param>
        /// <param name="Request">The HTTP request leading to this response.</param>
        private HTTPResponse(String       ResponseHeader,
                             HTTPRequest  Request)

            : this(Request,
                   null,
                   null,
                   ResponseHeader,
                   null,
                   new MemoryStream(),
                   Request?.CancellationToken,
                   Request?.EventTrackingId)

        {

            this._HTTPRequest  = Request;

            #region Parse HTTP status code

            var _StatusCodeLine = FirstPDULine.Split(' ');

            if (_StatusCodeLine.Length < 3)
                throw new Exception("Bad request");

            this._HTTPStatusCode = HTTPStatusCode.ParseString(_StatusCodeLine[1]);

            #endregion

        }

        #endregion

        #region (private) HTTPResponse(ResponseHeader, ResponseBody, Request)

        /// <summary>
        /// Create a new HTTP response.
        /// </summary>
        /// <param name="ResponseHeader">The HTTP header of the response.</param>
        /// <param name="ResponseBody">The HTTP body of the response.</param>
        /// <param name="Request">The HTTP request leading to this response.</param>
        private HTTPResponse(String       ResponseHeader,
                             Byte[]       ResponseBody,
                             HTTPRequest  Request)

            : this(Request,
                   null,
                   null,
                   ResponseHeader,
                   ResponseBody,
                   null,
                   Request?.CancellationToken,
                   Request?.EventTrackingId)

        { }


        /// <summary>
        /// Create a new HTTP response.
        /// </summary>
        /// <param name="ResponseHeader">The HTTP header of the response.</param>
        /// <param name="ResponseBody">The HTTP body of the response.</param>
        /// <param name="Request">The HTTP request leading to this response.</param>
        private HTTPResponse(String       ResponseHeader,
                             Stream       ResponseBody,
                             HTTPRequest  Request)

            : this(Request,
                   null,
                   null,
                   ResponseHeader,
                   null,
                   ResponseBody,
                   Request?.CancellationToken,
                   Request?.EventTrackingId)

        { }

        #endregion

        #endregion


        #region (static) Parse(HTTPResponseHeader, HTTPRequest)

        /// <summary>
        /// Parse the HTTP response from its text-representation.
        /// </summary>
        /// <param name="HTTPResponseHeader">The HTTP header of the response.</param>
        /// <param name="HTTPRequest">The HTTP request leading to this response.</param>
        public static HTTPResponse Parse(String       HTTPResponseHeader,
                                         HTTPRequest  HTTPRequest)
        {

            return new HTTPResponse(HTTPResponseHeader,
                                    HTTPRequest);

        }

        #endregion

        #region (static) Parse(HTTPResponseHeader, HTTPResponseBody, HTTPRequest)

        /// <summary>
        /// Parse the HTTP response from its text-representation and
        /// attach the given HTTP body.
        /// </summary>
        /// <param name="HTTPResponseHeader">The HTTP header of the response.</param>
        /// <param name="HTTPResponseBody">The HTTP body of the response.</param>
        /// <param name="HTTPRequest">The HTTP request leading to this response.</param>
        public static HTTPResponse Parse(String       HTTPResponseHeader,
                                         Byte[]       HTTPResponseBody,
                                         HTTPRequest  HTTPRequest)
        {

            return new HTTPResponse(HTTPResponseHeader,
                                    HTTPResponseBody,
                                    HTTPRequest);

        }

        /// <summary>
        /// Parse the HTTP response from its text-representation and
        /// attach the given HTTP body.
        /// </summary>
        /// <param name="HTTPResponseHeader">The HTTP header of the response.</param>
        /// <param name="HTTPResponseBody">The HTTP body of the response.</param>
        /// <param name="HTTPRequest">The HTTP request leading to this response.</param>
        public static HTTPResponse Parse(String       HTTPResponseHeader,
                                         Stream       HTTPResponseBody,
                                         HTTPRequest  HTTPRequest)
        {

            return new HTTPResponse(HTTPResponseHeader,
                                    HTTPResponseBody,
                                    HTTPRequest);

        }

        #endregion


        #region (static) BadRequest

        public static HTTPResponse BadRequest
            => new HTTPResponse(new HTTPResponseBuilder(null, HTTPStatusCode.BadRequest));

        #endregion

        #region (static) ServiceUnavailable

        public static HTTPResponse ServiceUnavailable
            => new HTTPResponse(new HTTPResponseBuilder(null, HTTPStatusCode.ServiceUnavailable));

        #endregion


        #region (override) ToString()

        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override String ToString()
        {
            return EntirePDU;
        }

        #endregion

    }

    #endregion

}
