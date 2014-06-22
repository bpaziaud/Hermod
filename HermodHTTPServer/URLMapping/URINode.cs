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
using System.Text;
using System.Reflection;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

using eu.Vanaheimr.Illias.Commons;
using System.Collections.Generic;

#endregion

namespace eu.Vanaheimr.Hermod.HTTP
{

    /// <summary>
    /// A URL node which stores some childnodes and a callback
    /// </summary>
    public class URINode
    {

        #region Properties

        /// <summary>
        /// The URL template for this service.
        /// </summary>
        public String       URITemplate         { get; private set; }

        /// <summary>
        /// The URI regex for this service.
        /// </summary>
        public Regex        URIRegex            { get; private set; }

        /// <summary>
        /// The number of parameters within this URLNode for shorting best-matching URLs.
        /// </summary>
        public UInt16       ParameterCount      { get; private set; }

        /// <summary>
        /// The lenght of the minimalized URL template for shorting best-matching URLs.
        /// </summary>
        public UInt16       SortLength          { get; private set; }

        /// <summary>
        /// The method handler.
        /// </summary>
        public MethodInfo   MethodHandler       { get; private set; }

        /// <summary>
        /// A delegate called for each incoming HTTP request.
        /// </summary>
        public HTTPDelegate HTTPDelegate        { get; private set; }

        /// <summary>
        /// This and all subordinated nodes demand an explicit url authentication.
        /// </summary>
        public Boolean      URLAuthentication   { get; private set; }

        /// <summary>
        /// A general error handling method.
        /// </summary>
        public MethodInfo   URLErrorHandler     { get; private set; }

        /// <summary>
        /// Error handling methods for specific http status codes.
        /// </summary>
        public Dictionary<HTTPStatusCode, MethodInfo> URIErrorHandlers { get; private set; }

        /// <summary>
        /// A mapping from HTTPMethods to HTTPMethodNodes.
        /// </summary>
        public Dictionary<HTTPMethod, HTTPMethodNode> HTTPMethods      { get; private set; }

        #endregion

        #region Constructor(s)

        #region URLNode(URITemplate, MethodHandler, URIAuthentication = false, URLErrorHandler = null)

        /// <summary>
        /// Creates a new URLNode.
        /// </summary>
        /// <param name="URITemplate">The URI template for this service.</param>
        /// <param name="MethodHandler">The method handler.</param>
        /// <param name="URIAuthentication">This and all subordinated nodes demand an explicit url authentication.</param>
        /// <param name="URIErrorHandler">A general error handling method.</param>
        public URINode(String      URITemplate,
                       MethodInfo  MethodHandler,
                       Boolean     URIAuthentication  = false,
                       MethodInfo  URIErrorHandler    = null)

        {

            URITemplate.FailIfNullOrEmpty();

            if (MethodHandler != null)
                throw new ArgumentNullException();

            this.URITemplate           = URITemplate;
            this.MethodHandler         = MethodHandler;
            this.URLAuthentication     = URIAuthentication;
            this.URLErrorHandler       = URIErrorHandler;
            this.URIErrorHandlers      = new Dictionary<HTTPStatusCode, MethodInfo>();
            this.HTTPMethods           = new Dictionary<HTTPMethod, HTTPMethodNode>();

            var _ReplaceLastParameter  = new Regex(@"\{[^/]+\}$");
            this.ParameterCount        = (UInt16) _ReplaceLastParameter.Matches(URITemplate).Count;
            var URLTemplate2           = _ReplaceLastParameter.Replace(URITemplate, "([^\n]+)");
            var URLTemplateWithoutVars = _ReplaceLastParameter.Replace(URITemplate, "");

            var _ReplaceAllParameters  = new Regex(@"\{[^/]+\}");
            this.ParameterCount       += (UInt16) _ReplaceAllParameters.Matches(URLTemplate2).Count;
            this.URIRegex              = new Regex("^" + _ReplaceAllParameters.Replace(URLTemplate2, "([^/]+)"));
            this.SortLength            = (UInt16) _ReplaceAllParameters.Replace(URLTemplateWithoutVars, "").Length;

        }

        #endregion

        #region URLNode(URITemplate, HTTPDelegate, URIAuthentication = false, URLErrorHandler = null)

        ///// <summary>
        ///// Creates a new URLNode.
        ///// </summary>
        ///// <param name="URITemplate">The URI template for this service.</param>
        ///// <param name="MethodHandler">The method handler.</param>
        ///// <param name="URIAuthentication">This and all subordinated nodes demand an explicit url authentication.</param>
        ///// <param name="URIErrorHandler">A general error handling method.</param>
        //public URINode(String        URITemplate,
        //               HTTPDelegate  HTTPDelegate,
        //               Boolean       URIAuthentication  = false,
        //               HTTPDelegate  URIErrorHandler    = null)

        //{

        //    URITemplate.FailIfNullOrEmpty();

        //    if (HTTPDelegate != null)
        //        throw new ArgumentNullException();

        //    this.URITemplate           = URITemplate;
        //    this.HTTPDelegate          = HTTPDelegate;

        //    this.URLAuthentication     = URIAuthentication;
        //    this.URLErrorHandler       = URIErrorHandler;
        //    this.URIErrorHandlers      = new Dictionary<HTTPStatusCode, MethodInfo>    ();
        //    this.HTTPMethods           = new Dictionary<HTTPMethod,     HTTPMethodNode>();

        //    var _ReplaceLastParameter  = new Regex(@"\{[^/]+\}$");
        //    this.ParameterCount        = (UInt16) _ReplaceLastParameter.Matches(URITemplate).Count;
        //    var URLTemplate2           = _ReplaceLastParameter.Replace(URITemplate, "([^\n]+)");
        //    var URLTemplateWithoutVars = _ReplaceLastParameter.Replace(URITemplate, "");

        //    var _ReplaceAllParameters  = new Regex(@"\{[^/]+\}");
        //    this.ParameterCount       += (UInt16) _ReplaceAllParameters.Matches(URLTemplate2).Count;
        //    this.URIRegex              = new Regex("^" + _ReplaceAllParameters.Replace(URLTemplate2, "([^/]+)"));
        //    this.SortLength            = (UInt16) _ReplaceAllParameters.Replace(URLTemplateWithoutVars, "").Length;

        //}

        #endregion

        #endregion

        #region ToString()

        /// <summary>
        /// Return a string represtentation of this object.
        /// </summary>
        public override String ToString()
        {

            var _URLAuthentication = "";
            if (URLAuthentication)
                _URLAuthentication = " (auth)";

            var _URLErrorHandler = "";
            if (URLErrorHandler != null)
                _URLErrorHandler = " (errhdl)";

            var _HTTPMethods = "";
            if (HTTPMethods.Count > 0)
            {
                var _StringBuilder = HTTPMethods.Keys.ForEach(new StringBuilder(" ["), (__StringBuilder, __HTTPMethod) => __StringBuilder.Append(__HTTPMethod.MethodName).Append(", "));
                _StringBuilder.Length = _StringBuilder.Length - 2;
                _HTTPMethods = _StringBuilder.Append("]").ToString();
            }

            return String.Concat(URITemplate, _URLAuthentication, _URLErrorHandler, _HTTPMethods);

        }

        #endregion

    }

}
