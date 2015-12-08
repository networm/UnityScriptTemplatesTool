using Microsoft.Win32;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Unity3dScriptTemplatesTool
{
    class Program
    {
        static void Main(string[] args)
        {
            // find reg key
            const string userRoot = "HKEY_CURRENT_USER";
            const string subkey = @"SOFTWARE\Unity Technologies\Installer\Unity";
            const string keyName = userRoot + "\\" + subkey;

            // get install path
            string installPath = (string)Registry.GetValue(keyName, "Location x64", string.Empty);
            if (string.IsNullOrEmpty(installPath))
            {
                Console.WriteLine("Couldn't find Unity3d install path!");
                return;
            }

            // get script templates path
            string scriptTemplatesDir = @"Editor\Data\Resources\ScriptTemplates";
            string scriptTemplatesPath = Path.Combine(installPath, scriptTemplatesDir);

            // modify script template file
            try
            {
                Console.WriteLine("Modify script templates:");

                string[] scriptTemplates = Directory.GetFiles(scriptTemplatesPath, "*.txt", SearchOption.TopDirectoryOnly);

                foreach (var file in scriptTemplates)
                {
                    string fileName = Path.GetFileName(file);
                    Console.WriteLine("\t" + fileName);

                    string text = File.ReadAllText(file);

                    // remove empty line
                    // .NET regex engine matches the $ between \r and \n
                    // so we need to do it before change line breaks
                    // http://stackoverflow.com/questions/7647716/how-to-remove-empty-lines-from-a-formatted-string
                    text = Regex.Replace(text, @"^\s+$", string.Empty, RegexOptions.Multiline);

                    // change line breaks
                    text = Regex.Replace(text, @"\r\n?|\n", "\r\n");

                    // change using declaration
                    if (fileName.Contains("C#"))
                    {
                        text = text.Replace("using System.Collections;", "using System.Collections.Generic;");
                    }

                    File.WriteAllText(file, text, Encoding.UTF8);
                }

                Console.WriteLine("Job done!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Couldn't modify script templates!");
                Console.WriteLine("Exception: " + e.Message);
            }

            Console.Write("Press any key to continue . . . ");
            Console.ReadKey(true);
        }
    }
}
