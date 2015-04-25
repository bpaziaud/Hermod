﻿/*
 * Copyright (c) 2010-2015, Achim 'ahzf' Friedland <achim@graphdefined.org>
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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Hermod.Services.Mail
{

    /// <summary>
    /// A mailinglist e-mail builder.
    /// </summary>
    public class MailinglistEMailBuilder : AbstractEMailBuilder
    {

        #region Properties

        #region ListId

        /// <summary>
        /// The unique identification of the mailinglist.
        /// </summary>
        public ListId ListId
        {

            get
            {
                return ListId.Parse(base._AdditionalHeaders.
                                        Where(kvp => kvp.Key.ToLower() == "list-id").
                                        FirstOrDefault().
                                        Value);
            }

            set
            {
                if (value != null)
                    base._AdditionalHeaders.Add("List-Id", value.ToString());
            }

        }

        #endregion

        #region ListPost

        /// <summary>
        /// The e-mail address of the mailinglist for posting new e-mails.
        /// </summary>
        public SimpleEMailAddress ListPost
        {

            get
            {

                var v = base._AdditionalHeaders.
                            Where(kvp => kvp.Key.ToLower() == "list-post").
                            FirstOrDefault();

                if (v.Key.IsNullOrEmpty())
                    return null;

                return SimpleEMailAddress.Parse(v.Value.Replace("mailto:", ""));

            }

            set
            {
                if (value != null)
                    base._AdditionalHeaders.Add("List-Post", "<mailto:" + value.ToString() + ">");
            }

        }

        #endregion


        #region Text

        /// <summary>
        /// The body of the text e-mail.
        /// </summary>
        public String Text
        {
            get
            {
                return base.Body.Content.AggregateWith(Environment.NewLine);
            }
        }

        #endregion

        #region ContentLanguage

        private String _ContentLanguage;

        /// <summary>
        /// The language of the e-mail body.
        /// </summary>
        public String ContentLanguage
        {
            get
            {
                return _ContentLanguage;
            }

            set
            {
                if (value != null && value != String.Empty && value.Trim() != "")
                    _ContentLanguage = value;
            }

        }

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new mailinglist e-mail builder.
        /// </summary>
        public MailinglistEMailBuilder()
        { }

        #endregion


        #region (protected, override) _EncodeBodyparts()

        protected override EMailBodypart _EncodeBodyparts()
        {

            return new EMailBodypart(ContentType:              MailContentTypes.text_plain,
                                     ContentTransferEncoding:  "quoted-printable",//"8bit",
                                     Charset:                  "utf-8",//"ISO-8859-15",
                                     ContentLanguage:          ContentLanguage,
                                     Content:                  new MailBodyString(Text));

        }

        #endregion

    }

}