using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OccfInsertToTestCode
{

    class ReturnBackUp
    {
        private const string Appender = @".backt";
        private readonly int _apdLength = Appender.Length;

        //(fileName).backtからの復元
        public void RevertBackUp(string dirPath)
        {
            var dirInfo = new DirectoryInfo(dirPath);
            foreach (var fileInfo in from fileInfo in dirInfo.GetFiles()
                                     let fileName = fileInfo.Name
                                     let nameLength = fileName.Length
                                     where nameLength >= _apdLength && fileName.Substring(nameLength - _apdLength, _apdLength).Equals(Appender)
                                     select fileInfo)
            {
                RevertFile(fileInfo.FullName);
            }
        }

        //指定したバックアップファイルからオリジナルファイルを復元してバックアップファイルを削除
        public void RevertFile(string fileFullName)
        {
            var fullNameLength = fileFullName.Length;
            var originFileFullName = fileFullName.Substring(0, fullNameLength - _apdLength);

            File.Copy(fileFullName, originFileFullName, true);
            File.Delete(fileFullName);
        }

    }
}
