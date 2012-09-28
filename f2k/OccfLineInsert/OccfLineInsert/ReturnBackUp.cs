using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace OccfLineInsert
{
    class ReturnBackUp
    {
        //(fileName).backからの復元
        public void RevertBackUp(String dirPath)
        {
            var dirInfo = new DirectoryInfo(dirPath);
            foreach (var fileInfo in from fileInfo in dirInfo.GetFiles()
                                     let fileName = fileInfo.Name
                                     let nameLength = fileName.Length
                                     where nameLength >= 5 && fileName.Substring(nameLength - 5, 5).Equals(@".back")
                                     select fileInfo)
            {
                RevertFile(fileInfo.FullName);
            }
        }

        //指定したバックアップファイルからオリジナルファイルを復元してバックアップファイルを削除
        public void RevertFile(string fileFulllName)
        {
            var fullNameLength = fileFulllName.Length;
            var originFileFullName = fileFulllName.Substring(0, fullNameLength - 5);

            File.Copy(fileFulllName, originFileFullName, true);
            File.Delete(fileFulllName);
        }



    }
}
