using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgLab.Util.Container;
using System.Threading;
using Microsoft.Win32;
using System.Security.AccessControl;
using System.Security.Principal;

namespace UT
{
    class Program
    {
        static void Main(string[] args)
        {
            //SwitchUT.UT();
            //SwitchUT.UTDataWorker();

            //LatestUT.UT(); 
            //LogSystemUT.UT();

            //StorageUT.UT4();
            //RWLockDictionaryUT.UT_RWLockDictionary();
            //Modify();

            int[] intArray = InitArray<int>(10);
            InitObject[] obj = InitArray<InitObject>(10); 
        }

        public class InitObject
        {
            private static int id = 0;

            public int ID
            {
                get;
                set;
            }

            public InitObject()
            {
                ID = id++;
            }
        }

        static void Modify()
        {
            try
            {              
                //Registry.LocalMachine.CreateSubKey( @"SYSTEM\CurrentControlSet\Control\StorageDevicePolicies", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryOptions.None,   

                RegistryKey regKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\StorageDevicePolicies", false);
                RegistrySecurity resSec = regKey.GetAccessControl();
                AuthorizationRuleCollection authRules = resSec.GetAccessRules(true, true, typeof(NTAccount));

                foreach (RegistryAccessRule rule in authRules)
                {
                    if (rule.IdentityReference.Value == "TW\\0007989")
                    {
                        if (rule.RegistryRights != RegistryRights.FullControl)
                        {
                            // Set full 
                            RegistryAccessRule newRule = new RegistryAccessRule(rule.IdentityReference, RegistryRights.FullControl, AccessControlType.Allow);
                            bool isModified = false;
                            if (resSec.ModifyAccessRule(AccessControlModification.Add, newRule, out isModified) == false)
                            {
                                Console.WriteLine("Modify access rule failed");
                            }
                        }
                    }
                }

                regKey.Close();
            }
            catch (Exception exp)
            {
                string s = exp.ToString();
            }
        }

        static T[] InitArray<T>(int count) where T:new()
        {
            T[] result = new T[count];
            for (int i = 0; i < count; ++i)
            {
                result[i] = new T();
            }

            return result;
        }
    }

    

    
}
