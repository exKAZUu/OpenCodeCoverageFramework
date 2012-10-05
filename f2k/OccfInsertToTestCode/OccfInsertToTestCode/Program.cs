using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OccfInsertToTestCode
{
    class Program
    {
        static void Main(string[] args) {

            var codeInsert = new CodeInsert();
            var returnBackUp = new ReturnBackUp();

            Console.WriteLine("current dir");
            Console.WriteLine(System.IO.Directory.GetCurrentDirectory());

            const string relativePath1 = @"../../sample/insert";
            const string relativePath2 = @"../../sample/revert";

            //埋め込み
            codeInsert.CodeInserts(relativePath1);
            //その復元
            //returnBackUp.RevertBackUp(relativePath1);

            //復元
            //returnBackUp.RevertBackUp(relativePath2);

        }
    }
}
