using System;
using CommandLine;
using QQ.Framework;
using QQ.Framework.Domains;
using QQ.Framework.Sockets;
using QQ.Framework.Utils;
using QQLoginTest.Robots;

namespace QQLoginTest
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed(o =>
                {
                    if (o.ServerIndex == 0)
                    {
                        o.ServerIndex = new Random().Next(1, 10);
                    }

                    QQGlobal.DebugLog = o.DebugView;
                    Console.WriteLine($"调试信息: {(QQGlobal.DebugLog ? "" : "不")}显示");
                    Run(o.QQNumber, o.QQPassword, o.ServerAddress ??
                                                  (o.ServerIndex == 1
                                                      ? "sz.tencent.com"
                                                      : $"sz{o.ServerIndex}.tencent.com"));
                })
                .WithNotParsed(e =>
                {
                    foreach (var error in e)
                    {
                        Console.WriteLine(error);
                    }
                });
        }

        private static void Run(long account, string password, string host)
        {
            var hostip = Util.GetHostAddresses(host);
            var accountStr = account.ToString();
            accountStr = accountStr.Substring(0, 3) + new string('*', accountStr.Length - 6) +
                         accountStr.Substring(accountStr.Length - 3);
            Console.WriteLine(
                $"QQ号码: {accountStr}\r\n密码: {new string('*', password.Length)}\r\n服务器: {host}\r\n服务器IP: {hostip}");
            var user = new QQUser(account, password);
            var socketServer = new SocketServiceImpl(user, hostip);
            var transponder = new Transponder();
            var sendService = new SendMessageServiceImpl(socketServer, user);
            var manage = new MessageManage(socketServer, user, transponder);
            var robot = new TestRobot(sendService, transponder, user);
            manage.Init();
            Console.ReadKey();
        }

        public class Options
        {
            [Option("qq", Required = true)]
            public long QQNumber { get; set; }

            [Option("pass", Required = true)]
            public string QQPassword { get; set; }

            [Option("debug", Required = false, Default = false)]
            public bool DebugView { get; set; }

            [Option("si", Required = false, Default = 0, SetName = "server")]
            public int ServerIndex { get; set; }

            [Option("server", Required = false, Default = null, SetName = "server")]
            public string ServerAddress { get; set; }
        }
    }
}