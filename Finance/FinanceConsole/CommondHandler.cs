using Finance.Account.Source;
using Finance.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceConsole
{
    public class CommondHandler
    {
        static ILogger logger = Logger.GetLogger(typeof(CommondHandler));
        public static int Process(string commondString)
        {
            try
            {
                string[] tmp = commondString.Split(new char[] { ' ', '\t' }, options: StringSplitOptions.RemoveEmptyEntries);
                if (tmp.Length == 0)
                    return 0;
                string commondName = tmp[0];
                switch (commondName)
                {
                    case "exit":
                        return -1;
                    case "clear":
                        return 1;
                    default:
                        return Exec(commondName, tmp);
                }
            }
            catch (FinanceException)
            {
                PrintHelp();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return 0;
        }
      
        static int Exec(string cmd, string[] args)
        {
            switch (cmd)
            {
                case "init":
                    logger.Warn("This is a dangerous command. Are you sure you want to execute it?(yes/no)");
                    var sure = Console.ReadLine();
                    if (sure == "yes")
                        AccountCtlMain.Init();
                    break;
                case "act.print":
                    AccountCtlMain.AccoutPrint();
                    break;
                case "act.create":
                    AccountCtlMain.CreateAccount(GetParm(args, 1), GetParm(args, 2));
                    break;
                case "act.load":
                    AccountCtlMain.LoadAccount(GetParm(args, 1), GetParm(args, 2));
                    break;
                case "act.unload":
                    var parm1 = GetParm(args, 1);
                    logger.Warn("This is a dangerous command. Are you sure you want to execute it?(yes/no)");
                    var yes = Console.ReadLine();
                    if (yes == "yes")
                        AccountCtlMain.UnloadAccount(parm1);
                    break;
                case "act.init":
                    var parm2 = GetParm(args, 1);
                    logger.Warn("This is a dangerous command. Are you sure you want to execute it?(yes/no)");
                    var yes1 = Console.ReadLine();
                    if (yes1 == "yes")
                    {
                        if (args.Count() > 2)
                        {                            
                            AccountCtlMain.InitAccount(parm2, GetParm(args, 2));
                        }
                        else
                            AccountCtlMain.InitAccount(parm2);
                    }                        
                    break;
                case "usr.pwd":
                    var userName = GetParm(args, 1);
                    Console.Write("Please input user's old password : ");
                    var pwd = ReadPwd();
                    var bSuc = AccountCtlMain.Verification(userName, pwd);
                    if (bSuc)
                    {
                        Console.Write("Please input user's new password : ");
                        var pwd1 = ReadPwd();
                        Console.Write("Confirm new password : ");
                        var pwd2 = ReadPwd();
                        if (pwd1 != pwd2)
                        {
                            logger.Error("Two times input password mismatch.");
                            return 0;
                        }
                        AccountCtlMain.ChagePwd(userName, pwd1);
                    }
                    else
                    {
                        Console.WriteLine("Incorrect user name or password.");
                    }
                    break;
                case "usr.add":
                    var no = GetParm(args, 1);
                    var name = GetParm(args, 2);
                    Console.Write("Please input user's password : ");
                    var apwd1 = ReadPwd();
                    Console.Write("Confirm password : ");
                    var apwd2 = ReadPwd();
                    if (apwd1 != apwd2)
                    {
                        logger.Error("Two times input password mismatch.");
                        return 0;
                    }
                    AccountCtlMain.AddUser(no, name, apwd1);
                    break;
                case "usr.delete":
                    var duserName = GetParm(args, 1);
                    Console.Write("Please input user's password : ");
                    var dpwd = ReadPwd();
                    var dbSuc = AccountCtlMain.Verification(duserName, dpwd);
                    if (dbSuc)
                    {
                        AccountCtlMain.DeleteUser(duserName);
                    }
                    else
                    {
                        Console.WriteLine("Incorrect user name or password.");
                    }
                    break;
                case "usr.print":
                    AccountCtlMain.UserPrint();
                    break;
                default:
                    logger.Debug(string.Format("Don't has this command [{0}].", cmd));
                    throw new FinanceException(FinanceResult.IMPERFECT_DATA);                  
            }
            logger.Info("execute success.");
            return 0;
        }

        static string GetParm(string[] args, int index)
        {
            if (args.Count() < index + 1)
                throw new FinanceException(FinanceResult.IMPERFECT_DATA);
            return args[index];
        }

        static void PrintHelp()
        {
            logger.Debug(@"please check your command and parms like this:
    init            - Initialize the entire system;
    act.print       - Print all of accounts information;
    act.create      - Create account like as ""act.create demo 演示账套"";
    act.load        - Load exists dbname into system like as ""act.load demo 演示账套"";
    act.unload      - Unload account like as ""act.unload demo"";
    act.init        - Initialize the account like as ""act.init demo"" or ""act.init demo -k"" to keep accout subject object;
    usr.pwd         - Change account manager's passwork like as ""usr.pwd admin"";
    usr.add         - Add account manager like as ""usr.add admin 管理员"";
    usr.delete      - Delete account manager like as ""usr.delete admin"";
    usr.print       - Print all of account manager information.
            ");
        }

        public static string ReadPwd()
        {
            char[] revisekeys = new char[3];
            revisekeys[0] = (char)0x08;
            revisekeys[1] = (char)0x20;
            revisekeys[2] = (char)0x08;

            StringBuilder sb = new StringBuilder();
            while (true)
            {
                ConsoleKeyInfo kinfo = Console.ReadKey(true);

                if (kinfo.Key == ConsoleKey.Enter)
                {
                    break;
                }

                if (kinfo.Key == ConsoleKey.Backspace)
                {
                    if (sb.Length != 0)
                    {
                        int rIndex = sb.Length - 1;
                        sb.Remove(rIndex, 1);
                        Console.Write(revisekeys);
                    }
                    continue;
                }
                sb.Append(Convert.ToString(kinfo.KeyChar));
                Console.Write("*");
            }
            Console.Write("\r\n");
            return sb.ToString();
        }
    }
}
