using Finance.Account.Source;
using Finance.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceConsole
{
    class Program
    {
        static void ConsoleMain(string[] args)
        {
            try
            {
                var defaultConnectionString = ConfigHelper.XmlReadConnectionString("Finance.exe.config", "default");
                DBHelper.DefaultInstance = new DBHelper(defaultConnectionString);           
                Console.Write("login as : ");
                string userName = Console.ReadLine();
            LOGIN_FLAG:
                Console.Write("password : ");
                string pwd = CommondHandler.ReadPwd();                
                var bSuc = AccountCtlMain.Verification(userName, pwd);
                if (!bSuc)
                {
                    Console.WriteLine("Incorrect user name or password.");
                    goto LOGIN_FLAG;
                }
                Console.WriteLine("Welcome to use finance.");

                while (true)
                {
                    Console.Write("$ ");
                    string commond = Console.ReadLine();
                    if (!string.IsNullOrEmpty(commond))
                    {
                        int returnCode = CommondHandler.Process(commond);
                        switch (returnCode)
                        {
                            case -1:
                                Console.WriteLine("Commond handler return code:" + returnCode + ", exit this process.");
                                return;
                            case 1:
                                Console.Clear();
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

    }
}
