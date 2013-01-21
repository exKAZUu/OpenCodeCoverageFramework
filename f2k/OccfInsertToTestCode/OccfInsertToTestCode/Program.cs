using System;
using System.Collections.Generic;
using System.IO;
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

            string[] sti = {";", "{", "}", ")"};

            /*
            foreach (var s in sti) {
                Console.WriteLine("ens1: " + "sssaa;      ".TrimEnd(' ').EndsWith(s));
                Console.WriteLine("ens2: " + "sssaa)      ".TrimEnd(' ').EndsWith(s));
                Console.WriteLine("ens3: " + "sssaa{      ".TrimEnd(' ').EndsWith(s));
                Console.WriteLine("ens4: " + "sssaa}      ".TrimEnd(' ').EndsWith(s));
            }
            */

            var endFlag = sti.Any(s => "sssaa{      ".TrimEnd(' ').EndsWith(s));
            /*
             var endFlag = false;
            foreach (var s in sti){
                if ("sssaa{      ".TrimEnd(' ').EndsWith(s)) {
                    endFlag = true;
                    break;
                }
                //Console.WriteLine("ens3: " + "sssaa{      ".TrimEnd(' ').EndsWith(s));
            }
             */

            if (sti.Any(s => "  sss sa)    ".TrimEnd(' ').EndsWith(s))) {
                Console.WriteLine("enddTTTT ");
            } else {
                Console.WriteLine("enddFFFF ");
            }
            //Console.WriteLine("ens1: " + "sssaa;      ".TrimEnd(' ').EndsWith(";"))};

            //埋め込み
            //codeInsert.CodeInserts(relativePath1);
            //その復元
            //returnBackUp.RevertBackUp(relativePath1);

            //復元
            //returnBackUp.RevertBackUp(relativePath2);

            //復元＆埋め込み
            //returnBackUp.RevertBackUp(relativePath1);
            //codeInsert.CodeInserts(relativePath1);

            //const string main = "int main  (int a) { aaa }";

            //Console.WriteLine(main.IndexOf("main", System.StringComparison.Ordinal));
            //Console.WriteLine(main.IndexOf("aaaa", System.StringComparison.Ordinal));

            /*
            var rootDir = new DirectoryInfo(".");
            Console.WriteLine("root : "+ rootDir.FullName);
            var fileInfos = new List<FileInfo>();
            List<FileInfo> nullF = new List<FileInfo>();
            fileInfos.AddRange(rootDir.GetFiles("*"));
            var fn = fileInfos.Concat(nullF);
            */

            var dic = new Dictionary<int, int>();
            //var add = new Dictionary<int, int>();
            //add.Add(134, 0);

            
            dic.Add(290, 1);
            dic.Add(389, 2);
            
            dic.Add(463, 3);
            dic.Add(589, 4);

            // 
            int sp = 395;
            //spよりkeyが大きくてkeyが最小のもののvalue
            var a = dic[dic.Keys.Where(s => s >= sp).Min()]; //3 after
            var b = dic[dic.Keys.Where(s => s <= sp).Max()]; //2 before

            Console.WriteLine("min : " + a);
            Console.WriteLine("max : "  + b);


            var linst = "pass,1988,12";
            var lists = linst.Split(',');
            int ss = int.Parse(lists[1]) + int.Parse(lists[2]);
            foreach (var list in lists) {
                Console.WriteLine("line : " + list);
            }
            Console.WriteLine("plus : " + ss);


            var stlist = new List<string>();
            stlist.Add("aaa");
            stlist.Add("bbb");
            stlist.Add("ccc");

            foreach (var st in stlist) {
                Console.WriteLine(st);
            }
            Console.WriteLine("dddd");
            stlist.AddRange(stlist);

            foreach (var st in stlist) {
                Console.WriteLine(st);
            }

            int sb;
            var sb1 = int.TryParse("c\\/get_sign.c", out sb);

            Console.WriteLine(sb1);
        }
    }
}
