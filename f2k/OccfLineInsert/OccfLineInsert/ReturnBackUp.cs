using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace OccfLineInsert
{
    class ReturnBackUp
    {
        private const string Appender = @".back";
        private readonly int _apdLength = Appender.Length;

        //(fileName).backからの復元
        public void RevertBackUp(string dirPath)
        {
            var dirInfo = new DirectoryInfo(dirPath);

            foreach (var fileInfo in dirInfo.GetFiles("*"+Appender, SearchOption.AllDirectories)) {
                RevertFile(fileInfo.FullName);
            }
        }

        //指定したバックアップファイルからオリジナルファイルを復元してバックアップファイルを削除
        public void RevertFile(string fileFulllName)
        {
            var fullNameLength = fileFulllName.Length;
            var originFileFullName = fileFulllName.Substring(0, fullNameLength - _apdLength);

            File.Copy(fileFulllName, originFileFullName, true);
            File.Delete(fileFulllName);
        }



    }
}
