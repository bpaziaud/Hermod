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

using org.GraphDefined.Vanaheimr.Hermod.Mail;
using System;

#endregion

namespace org.GraphDefined.Vanaheimr.Hermod.SMTP
{

    /// <summary>
    /// An SMTP exception.
    /// </summary>
    public class SMTPClientException : SMTPException
    {

        /// <summary>
        /// Create a new SMTP exception.
        /// </summary>
        /// <param name="Message">The exception message.</param>
        public SMTPClientException(String Message)
            : base(Message, null)
        { }

        /// <summary>
        /// Create a new SMTP exception.
        /// </summary>
        /// <param name="Message">The exception message.</param>
        /// <param name="InnerException">An optional inner exception.</param>
        public SMTPClientException(String Message, Exception InnerException)
            : base(Message, InnerException)
        { }

    }

}
