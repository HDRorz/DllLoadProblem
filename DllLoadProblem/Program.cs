// See https://aka.ms/new-console-template for more information
using log4net.Config;
using log4net;
using System.Reflection;

Console.WriteLine("Hello, World!");



var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

//var asm = Assembly.Load(@"Easy.Logger");
var asm = Assembly.Load(@"Easy.Logger, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
//var asm = Assembly.LoadFrom(@"Easy.Logger.dll");

log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType).Debug("");

Console.ReadLine();