﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace Microsoft.Http2.Owin.Server
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    public class SocketServerFactory
    {
        public static void Initialize(IDictionary<string, object> properties)
        {
            if (properties == null)
            {
                throw new ArgumentNullException("properties");
            }
        }

        public static IDisposable Create(AppFunc app, IDictionary<string, object> properties)
        {
            bool useHandshake = ConfigurationManager.AppSettings["handshakeOptions"] != "no-handshake";
            bool usePriorities = ConfigurationManager.AppSettings["prioritiesOptions"] != "no-priorities";
            bool useFlowControl = ConfigurationManager.AppSettings["flowcontrolOptions"] != "no-flowcontrol";

            properties.Add("use-handshake", useHandshake);
            properties.Add("use-priorities", usePriorities);
            properties.Add("use-flowControl", useFlowControl);

            return new HttpSocketServer(app, properties);
        }
    }
}
