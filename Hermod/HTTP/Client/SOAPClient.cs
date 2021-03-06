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
using System.Linq;
using System.Xml.Linq;
using System.Threading;
using System.Net.Security;
using System.Threading.Tasks;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using System.Security.Cryptography.X509Certificates;

#endregion

namespace org.GraphDefined.Vanaheimr.Hermod.SOAP
{

    public delegate XElement XMLNamespacesDelegate(XElement XML);

    #region SOAP XML Namespace

    /// <summary>
    /// SOAP XML Namespace
    /// </summary>
    public static class NS
    {

        /// <summary>
        /// The namespace for the XML SOAP v1.1 envelope.
        /// </summary>
        public static readonly XNamespace SOAPEnvelope_v1_1  = "http://schemas.xmlsoap.org/soap/envelope/";

        /// <summary>
        /// The namespace for the XML SOAP v1.2 envelope.
        /// </summary>
        public static readonly XNamespace SOAPEnvelope_v1_2  = "http://www.w3.org/2003/05/soap-envelope";

        /// <summary>
        /// SOAP Adressing extentions.
        /// </summary>
        public static readonly XNamespace SOAPAdressing      = "http://www.w3.org/2005/08/addressing";

    }

    #endregion

    #region SOAPClient

    /// <summary>
    /// A specialized HTTP client for the Simple Object Access Protocol (SOAP).
    /// </summary>
    public class SOAPClient : HTTPClient
    {

        #region Properties

        #region HTTPVirtualHost

        private readonly String _HTTPVirtualHost;

        /// <summary>
        /// The HTTP virtual host to use.
        /// </summary>
        public String HTTPVirtualHost
        {
            get
            {
                return _HTTPVirtualHost;
            }
        }

        #endregion

        #region URIPrefix

        private readonly String _URIPrefix;

        /// <summary>
        /// The URI-prefix of the OICP service.
        /// </summary>
        public String URIPrefix
        {
            get
            {
                return _URIPrefix;
            }
        }

        #endregion

        #region UserAgent

        private readonly String _UserAgent;

        /// <summary>
        /// The HTTP user agent.
        /// </summary>
        public String UserAgent
        {
            get
            {
                return _UserAgent;
            }
        }

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Create a new specialized HTTP client for the Simple Object Access Protocol (SOAP).
        /// </summary>
        /// <param name="SOAPHost">The hostname of the remote SOAP service.</param>
        /// <param name="SOAPPort">The TCP port of the remote SOAP service.</param>
        /// <param name="HTTPVirtualHost">The HTTP virtual host to use.</param>
        /// <param name="URIPrefix">The URI-prefix of the SOAP service.</param>
        /// <param name="RemoteCertificateValidator">A delegate to verify the remote TLS certificate.</param>
        /// <param name="ClientCert">The TLS client certificate to use.</param>
        /// <param name="UserAgent">The HTTP user agent to use.</param>
        /// <param name="DNSClient">An optional DNS client.</param>
        public SOAPClient(String                               SOAPHost,
                          IPPort                               SOAPPort,
                          String                               HTTPVirtualHost,
                          String                               URIPrefix,
                          RemoteCertificateValidationCallback  RemoteCertificateValidator  = null,
                          X509Certificate                      ClientCert                  = null,
                          String                               UserAgent                   = "GraphDefined SOAP Client",
                          DNSClient                            DNSClient                   = null)

            : base(SOAPHost,
                   SOAPPort,
                   RemoteCertificateValidator,
                   ClientCert,
                   DNSClient)

        {

            this._HTTPVirtualHost  = HTTPVirtualHost;
            this._URIPrefix        = URIPrefix.IsNotNullOrEmpty() ? URIPrefix : "/";
            this._UserAgent        = UserAgent;

        }

        #endregion


        #region Query(QueryXML, SOAPAction, OnSuccess, OnSOAPFault, OnHTTPError, OnException, TimeoutMSec = 60000)

        /// <summary>
        /// Create a new SOAP query task.
        /// </summary>
        /// <typeparam name="T">The type of the return data structure.</typeparam>
        /// <param name="QueryXML">The SOAP query XML.</param>
        /// <param name="SOAPAction">The SOAP action.</param>
        /// <param name="OnSuccess">The delegate to call for every successful result.</param>
        /// <param name="OnSOAPFault">The delegate to call whenever a SOAP fault occured.</param>
        /// <param name="OnHTTPError">The delegate to call whenever a HTTP error occured.</param>
        /// <param name="OnException">The delegate to call whenever an exception occured.</param>
        /// <param name="QueryTimeout">An optional timeout of the HTTP client [default 60 sec.]</param>
        /// <returns>The data structured after it had been processed by the OnSuccess delegate, or a fault.</returns>
        public Task<HTTPResponse<T>>

            Query<T>(XElement                                                         QueryXML,
                     String                                                           SOAPAction,
                     Func<HTTPResponse<XElement>,                   HTTPResponse<T>>  OnSuccess,
                     Func<DateTime, Object, HTTPResponse<XElement>, HTTPResponse<T>>  OnSOAPFault,
                     Func<DateTime, Object, HTTPResponse,           HTTPResponse<T>>  OnHTTPError,
                     Func<DateTime, Object, Exception,              HTTPResponse<T>>  OnException,
                     Action<HTTPRequestBuilder>                                       HTTPRequestBuilder   = null,
                     ClientRequestLogHandler                                          RequestLogDelegate   = null,
                     ClientResponseLogHandler                                         ResponseLogDelegate  = null,
                     CancellationToken?                                               CancellationToken    = null,
                     EventTracking_Id                                                 EventTrackingId      = null,
                     TimeSpan?                                                        QueryTimeout         = null)

        {

            #region Initial checks

            if (QueryXML    == null)
                throw new ArgumentNullException(nameof(QueryXML),     "The 'Query'-string must not be null!");

            if (SOAPAction.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(SOAPAction),   "The 'SOAPAction'-string must not be null or empty!");

            if (OnSuccess   == null)
                throw new ArgumentNullException(nameof(OnSuccess),    "The 'OnSuccess'-delegate must not be null!");

            if (OnSOAPFault == null)
                throw new ArgumentNullException(nameof(OnSOAPFault),  "The 'OnSOAPFault'-delegate must not be null!");

            if (OnHTTPError == null)
                throw new ArgumentNullException(nameof(OnHTTPError),  "The 'OnHTTPError'-delegate must not be null!");

            if (OnException == null)
                throw new ArgumentNullException(nameof(OnException),  "The 'OnException'-delegate must not be null!");

            #endregion

            var _RequestBuilder = this.POST(_URIPrefix);
            _RequestBuilder.Host               = HTTPVirtualHost;
            _RequestBuilder.Content            = QueryXML.ToUTF8Bytes();
            _RequestBuilder.ContentType        = HTTPContentType.XMLTEXT_UTF8;
            _RequestBuilder.Set("SOAPAction",  @"""" + SOAPAction + @"""");
            _RequestBuilder.UserAgent          = UserAgent;
            _RequestBuilder.FakeURIPrefix      = "https://" + HTTPVirtualHost;

            HTTPRequestBuilder?.Invoke(_RequestBuilder);

            return this.Execute(_RequestBuilder,
                                RequestLogDelegate,
                                ResponseLogDelegate,
                                QueryTimeout               ?? TimeSpan.FromSeconds(60),
                                CancellationToken.HasValue  ? CancellationToken.Value : new CancellationTokenSource().Token).

                        ContinueWith(HttpResponseTask => {

                            if (HttpResponseTask.Result                 == null              ||
                                HttpResponseTask.Result.HTTPStatusCode  != HTTPStatusCode.OK ||
                                HttpResponseTask.Result.HTTPBody        == null              ||
                                HttpResponseTask.Result.HTTPBody.Length == 0)
                            {

                                var OnHTTPErrorLocal = OnHTTPError;
                                if (OnHTTPErrorLocal != null)
                                    return OnHTTPErrorLocal(DateTime.Now, this, HttpResponseTask?.Result);

                                return new HTTPResponse<XElement>(HttpResponseTask?.Result,
                                                                  new XElement("HTTPError"),
                                                                  IsFault: true) as HTTPResponse<T>;

                            }

                            try
                            {

                                var SOAPXML = XDocument.Parse(HttpResponseTask.Result.HTTPBody.ToUTF8String()).
                                                        Root.
                                                        Element(NS.SOAPEnvelope_v1_1 + "Body").
                                                        Descendants().
                                                        FirstOrDefault();

                                // <S:Fault xmlns:ns4="http://www.w3.org/2003/05/soap-envelope" xmlns:S="http://schemas.xmlsoap.org/soap/envelope/">
                                //   <faultcode>S:Client</faultcode>
                                //   <faultstring>Validation error: The request message is invalid</faultstring>
                                //   <detail>
                                //     <Validation>
                                //       <Errors>
                                //         <Error column="65" errorXpath="/OICP:Envelope/OICP:Body/EVSEStatus:eRoamingPullEvseStatusById/EVSEStatus:EvseId" line="3">Value '+45*045*010*0A96296' is not facet-valid with respect to pattern '([A-Za-z]{2}\*?[A-Za-z0-9]{3}\*?E[A-Za-z0-9\*]{1,30})|(\+?[0-9]{1,3}\*[0-9]{3,6}\*[0-9\*]{1,32})' for type 'EvseIDType'.</Error>
                                //         <Error column="65" errorXpath="/OICP:Envelope/OICP:Body/EVSEStatus:eRoamingPullEvseStatusById/EVSEStatus:EvseId" line="3">The value '+45*045*010*0A96296' of element 'EVSEStatus:EvseId' is not valid.</Error>
                                //       </Errors>
                                //       <OriginalDocument>
                                //         ...
                                //       </OriginalDocument>
                                //     </Validation>
                                //   </detail>
                                // </S:Fault>

                                if (SOAPXML.Name.LocalName != "Fault")
                                {

                                    var OnSuccessLocal = OnSuccess;
                                    if (OnSuccessLocal != null)
                                        return OnSuccessLocal(new HTTPResponse<XElement>(HttpResponseTask.Result, SOAPXML));

                                }

                                var OnSOAPFaultLocal = OnSOAPFault;
                                if (OnSOAPFaultLocal != null)
                                    return OnSOAPFaultLocal(DateTime.Now, this, new HTTPResponse<XElement>(HttpResponseTask.Result, SOAPXML));

                                return new HTTPResponse<XElement>(HttpResponseTask.Result,
                                                                  new XElement("SOAPFault"),
                                                                  IsFault: true) as HTTPResponse<T>;


                            } catch (Exception e)
                            {

                                OnException?.Invoke(DateTime.Now, this, e);

                                //var OnFaultLocal = OnSOAPFault;
                                //if (OnFaultLocal != null)
                                //    return OnFaultLocal(new HTTPResponse<XElement>(HttpResponseTask.Result, e));

                                return new HTTPResponse<XElement>(HttpResponseTask.Result,
                                                                  new XElement("exception", e.Message),
                                                                  IsFault: true) as HTTPResponse<T>;

                            }

                        });

        }

        #endregion

    }

    #endregion

}
