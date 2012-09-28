using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OccfInsertToTestCode
{
    class CodeInsert
    {
        //引数に入力ファイルのディレクトリのパス
        public void CodeInserts(string dirPath) {
            var dirInfo = new DirectoryInfo(dirPath);
            foreach (var fileInfo in dirInfo.GetFiles()) {
                var defaultFilePath = fileInfo.FullName;
                const string appendExtension = @".back";
                var backUpFilePath = defaultFilePath + appendExtension;
                var backUpFileName = fileInfo.Name + appendExtension;

                File.Copy(defaultFilePath, backUpFilePath);

                using (var reader = new StreamReader(backUpFilePath)) {
                using (var writer = new StreamWriter(fileInfo.FullName, false)) {
                    
    
                }}
            }

        }

    }
}
