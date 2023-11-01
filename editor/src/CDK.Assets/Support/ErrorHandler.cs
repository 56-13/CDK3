using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Net;

namespace CDK.Assets.Support
{
    public static class ErrorHandler
    {
        internal static void Start()
        {
            if (Debugger.IsAttached) return;

            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        internal static void Stop()
        {
            if (Debugger.IsAttached) return;

            Application.ThreadException -= Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Record(e.Exception, true);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Record((Exception)e.ExceptionObject, true);
        }

        public static void Record(Exception e, bool showMessage = false)
        {
            if (Debugger.IsAttached) return;

            if (showMessage) AssetManager.Instance.Message("처리되지 않은 에러가 발생했습니다. 관련 메일이 개발자에게 전송되고 로그에 기록됩니다.");

            RecordToLocal(e, showMessage);
            ReportToTelegram(e, showMessage);
        }

        private static void RecordToLocal(Exception e, bool showMessage)
        {
            var subject = $"{AssetManager.EditorVersion} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}";

            try
            {
                using (var writer = new StreamWriter("error.log", true))
                {
                    writer.WriteLine(subject);
                    writer.WriteLine(e.ToString());
                    writer.WriteLine();
                }
            }
            catch
            {
                if (showMessage) AssetManager.Instance.Message("로그 기록에 실패했습니다. 관리자에게 알려주세요.");
            }
        }

        private const string TelegramToken = "5742423951:AAFK6HYliBJwcsN84Ttx9jovvds-rqhChbM";
        private const string TelegramChatId = "5655746332";

        private static void ReportToTelegram(Exception e, bool showMessage)
        {
            var message = $"CDK.AssetEditor {AssetManager.EditorVersion} Error Occured\r\n{e}";

            var url = $"https://api.telegram.org/bot{TelegramToken}/sendMessage?chat_id={TelegramChatId}&text={message}";

            try
            {
                var webRequest = WebRequest.Create(url);
                var objStream = webRequest.GetResponse().GetResponseStream();

                var objReader = new StreamReader(objStream ?? throw new IOException());

                var json = objReader.ReadToEnd();

                Console.WriteLine(json);
            }
            catch
            {
                if (showMessage) AssetManager.Instance.Message("메시지 전송에 실패했습니다. 관리자에게 알려주세요.");
            }
        }
    }
}
