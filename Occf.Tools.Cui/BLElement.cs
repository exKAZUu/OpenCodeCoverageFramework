using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Occf.Tools.Cui
{
    public class BLElement {
        public string FileName;
        public int StartLine;
        public int EndLine;
        public int PassedCount;
        public int PassedAndExecutedCount;
        public int FailedCount;
        public int FailedAndExecutedCount;

        public BLElement(
                string fileName, int startLine, int endLine,
                int pass, int passExe, int fail, int faileExe) {

            FileName = fileName;
            StartLine = startLine;
            EndLine = endLine;
            PassedCount = pass;
            PassedAndExecutedCount = passExe;
            FailedCount = fail;
            FailedAndExecutedCount = faileExe;
        }

        public string CsvString(){
            var csvString = StartLine + "," + EndLine + ","
                    + PassedCount + "," + PassedAndExecutedCount + ","
                    + FailedCount + "," + FailedAndExecutedCount;

            return csvString;
        }

        public string AllCsvString() {
            var csvString = FileName + "," + StartLine + "," + EndLine + ","
                    + PassedCount + "," + PassedAndExecutedCount + ","
                    + FailedCount + "," + FailedAndExecutedCount;  
            
            return csvString;
        }
    }
}
