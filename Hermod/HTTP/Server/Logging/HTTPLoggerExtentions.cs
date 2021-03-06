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

namespace org.GraphDefined.Vanaheimr.Hermod.HTTP
{

    /// <summary>
    /// Extentions methods for HTTP loggers.
    /// </summary>
    public static class HTTPLoggerExtentions
    {

        #region RegisterDefaultConsoleLogTarget(this HTTPRequestLogger, HTTPLogger)

        /// <summary>
        /// Register the default console logger.
        /// </summary>
        /// <param name="HTTPRequestLogger">A HTTP request logger.</param>
        /// <param name="HTTPLogger">A HTTP logger.</param>
        public static HTTPLogger.HTTPServerRequestLogger RegisterDefaultConsoleLogTarget(this HTTPLogger.HTTPServerRequestLogger  HTTPRequestLogger,
                                                                                   HTTPLogger                         HTTPLogger)
        {

            return HTTPRequestLogger.RegisterLogTarget(LogTargets.Console,
                                                      (Context, LogEventName, Request) => HTTPLogger.Default_LogHTTPRequest_toConsole(Context, LogEventName, Request));

        }

        #endregion

        #region RegisterDefaultConsoleLogTarget(this HTTPClientRequestLogger, HTTPLogger)

        /// <summary>
        /// Register the default console logger.
        /// </summary>
        /// <param name="HTTPClientRequestLogger">A HTTP request logger.</param>
        /// <param name="HTTPLogger">A HTTP logger.</param>
        public static HTTPLogger.HTTPClientRequestLogger RegisterDefaultConsoleLogTarget(this HTTPLogger.HTTPClientRequestLogger  HTTPClientRequestLogger,
                                                                                         HTTPLogger                               HTTPLogger)
        {

            return HTTPClientRequestLogger.RegisterLogTarget(LogTargets.Console,
                                                             (Context, LogEventName, Request) => HTTPLogger.Default_LogHTTPRequest_toConsole(Context, LogEventName, Request));

        }

        #endregion

        #region RegisterDefaultConsoleLogTarget(this HTTPResponseLogger, HTTPLogger)

        /// <summary>
        /// Register the default console logger.
        /// </summary>
        /// <param name="HTTPResponseLogger">A HTTP response logger.</param>
        /// <param name="HTTPLogger">A HTTP logger.</param>
        public static HTTPLogger.HTTPServerResponseLogger RegisterDefaultConsoleLogTarget(this HTTPLogger.HTTPServerResponseLogger  HTTPResponseLogger,
                                                                                    HTTPLogger                          HTTPLogger)
        {

            return HTTPResponseLogger.RegisterLogTarget(LogTargets.Console,
                                                       (Context, LogEventName, Request, Response) => HTTPLogger.Default_LogHTTPResponse_toConsole(Context, LogEventName, Request, Response));

        }

        #endregion

        #region RegisterDefaultConsoleLogTarget(this HTTPClientResponseLogger, HTTPLogger)

        /// <summary>
        /// Register the default console logger.
        /// </summary>
        /// <param name="HTTPClientResponseLogger">A HTTP response logger.</param>
        /// <param name="HTTPLogger">A HTTP logger.</param>
        public static HTTPLogger.HTTPClientResponseLogger RegisterDefaultConsoleLogTarget(this HTTPLogger.HTTPClientResponseLogger  HTTPClientResponseLogger,
                                                                                          HTTPLogger                                HTTPLogger)
        {

            return HTTPClientResponseLogger.RegisterLogTarget(LogTargets.Console,
                                                              (Context, LogEventName, Request, Response) => HTTPLogger.Default_LogHTTPResponse_toConsole(Context, LogEventName, Request, Response));

        }

        #endregion


        #region RegisterDefaultDiscLogTarget(this HTTPRequestLogger, HTTPLogger)

        /// <summary>
        /// Register the default disc logger.
        /// </summary>
        /// <param name="HTTPRequestLogger">A HTTP request logger.</param>
        /// <param name="HTTPLogger">A HTTP logger.</param>
        public static HTTPLogger.HTTPServerRequestLogger RegisterDefaultDiscLogTarget(this HTTPLogger.HTTPServerRequestLogger  HTTPRequestLogger,
                                                                                HTTPLogger                         HTTPLogger)
        {

            return HTTPRequestLogger.RegisterLogTarget(LogTargets.Disc,
                                                      (Context, LogEventName, Request) => HTTPLogger.Default_LogHTTPRequest_toDisc(Context, LogEventName, Request));

        }

        #endregion

        #region RegisterDefaultDiscLogTarget(this HTTPClientRequestLogger, HTTPLogger)

        /// <summary>
        /// Register the default disc logger.
        /// </summary>
        /// <param name="HTTPClientRequestLogger">A HTTP request logger.</param>
        /// <param name="HTTPLogger">A HTTP logger.</param>
        public static HTTPLogger.HTTPClientRequestLogger RegisterDefaultDiscLogTarget(this HTTPLogger.HTTPClientRequestLogger  HTTPClientRequestLogger,
                                                                                      HTTPLogger                               HTTPLogger)
        {

            return HTTPClientRequestLogger.RegisterLogTarget(LogTargets.Disc,
                                                             (Context, LogEventName, Request) => HTTPLogger.Default_LogHTTPRequest_toDisc(Context, LogEventName, Request));

        }

        #endregion

        #region RegisterDefaultDiscLogTarget(this HTTPResponseLogger, HTTPLogger)

        /// <summary>
        /// Register the default disc logger.
        /// </summary>
        /// <param name="HTTPResponseLogger">A HTTP response logger.</param>
        /// <param name="HTTPLogger">A HTTP logger.</param>
        public static HTTPLogger.HTTPServerResponseLogger RegisterDefaultDiscLogTarget(this HTTPLogger.HTTPServerResponseLogger  HTTPResponseLogger,
                                                                                 HTTPLogger                           HTTPLogger)
        {

            return HTTPResponseLogger.RegisterLogTarget(LogTargets.Disc,
                                                       (Context, LogEventName, Request, Response) => HTTPLogger.Default_LogHTTPResponse_toDisc(Context, LogEventName, Request, Response));

        }

        #endregion

        #region RegisterDefaultDiscLogTarget(this HTTPClientResponseLogger, HTTPLogger)

        /// <summary>
        /// Register the default disc logger.
        /// </summary>
        /// <param name="HTTPClientResponseLogger">A HTTP response logger.</param>
        /// <param name="HTTPLogger">A HTTP logger.</param>
        public static HTTPLogger.HTTPClientResponseLogger RegisterDefaultDiscLogTarget(this HTTPLogger.HTTPClientResponseLogger  HTTPClientResponseLogger,
                                                                                       HTTPLogger                                HTTPLogger)
        {

            return HTTPClientResponseLogger.RegisterLogTarget(LogTargets.Disc,
                                                              (Context, LogEventName, Request, Response) => HTTPLogger.Default_LogHTTPResponse_toDisc(Context, LogEventName, Request, Response));

        }

        #endregion

    }

}
