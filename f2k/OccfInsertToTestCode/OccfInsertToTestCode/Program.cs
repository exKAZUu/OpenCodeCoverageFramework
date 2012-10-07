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

            //復元＆埋め込み
            //returnBackUp.RevertBackUp(relativePath1);
            //codeInsert.CodeInserts(relativePath1);

            const string main = "int main  (int a) { aaa }";

            Console.WriteLine(main.IndexOf("main", System.StringComparison.Ordinal));
            Console.WriteLine(main.IndexOf("aaaa", System.StringComparison.Ordinal));

        }
    }
}
