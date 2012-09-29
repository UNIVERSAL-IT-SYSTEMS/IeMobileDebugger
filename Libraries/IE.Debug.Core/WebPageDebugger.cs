﻿using Microsoft.Phone.Controls;
using Support;
using System;

namespace IE.Debug.Core
{
    public static class WebPageDebugger
    {
        private static WebBrowser Browser {get; set;}
        
        public static string PageHtml
        {
            get { return Browser.SaveToString(); }
        }

        public static string PageUri
        {
            get { return String.Empty; }
        }

        public static void Initialize(WebBrowser browser) 
        {
            WebPageDebugger.Browser = browser;
        }

        public static bool InstallDebugConsole()
        {
            try
            {
                Browser.InvokeScript("eval", new string[] { FileUtils.ReadFileContent("app/www/js/console.js") });

                if (String.Compare("true", (string)Browser.InvokeScript("eval", new string[] { FileUtils.ReadFileContent("app/www/js/compatModeDetection.js") })) == 0)
                {
                    Messages.ShowError("Oops. It seems the page runs in compatibility mode. Please fix and try again.");
                    return false;
                }


                Browser.InvokeScript("eval", new string[] { Scripting.PhoneGapInjectScript });
                Browser.InvokeScript("eval", new string[] { FileUtils.ReadFileContent("app/www/js/wpHtmlDebugger.js") });

                string nativeReady = "(function(){ cordova.require('cordova/channel').onNativeReady.fire()})();";
                Browser.InvokeScript("execScript", new string[] { nativeReady });

                return true;
            }
            catch (Exception ex)
            {
                Support.Messages.ShowError("Sorry, an error occured. " + ex.Message);
            }

            return false;
        }

        public static void InstallFirebug()
        {
            Browser.InvokeScript("eval", new string[] { Scripting.FirebugInjectScript });
            //(@"function(F,i,r,e,b,u,g,L,I,T,E){if(F.getElementById(b))return;E=F[i+'NS']&&F.documentElement.namespaceURI;E=E?F[i+'NS'](E,'script'):F[i]('script');E[r]('id',b);E[r]('src',I+g+T);E[r](b,u);(F[e]('head')[0]||F[e]('body')[0]).appendChild(E);E=new%20Image;E[r]('src',I+L);})(document,'createElement','setAttribute','getElementsByTagName','FirebugLite','4','firebug-lite.js','releases/lite/latest/skin/xp/sprite.png','https://getfirebug.com/','#startOpened')();");
        }

        public static void ExecuteCustomScript(string script)
        {
            Browser.InvokeScript("eval", new string[] { script});
        }
    }
}
