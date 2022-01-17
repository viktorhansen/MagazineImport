using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using MagazineImport.Code.Helpers;
using MagazineImport.Code.Mapping;
using Serilog;
using SmartXLS;

namespace MagazineImport.Code.Importers
{
    public class PrenaxImporter : BaseMultiImporter
    {
        private const string strPathUpload = "C:\\ftpImport\\MagazineImport\\upload";
        private const string strPathArchive = "C:\\ftpImport\\MagazineImport\\archive";
        public const string strFilePrefix = "prenax_";

        protected override bool DoImport()
        {
            //Make sure paths exists
            if (!Directory.Exists(strPathUpload))
                Directory.CreateDirectory(strPathUpload);
            if (!Directory.Exists(strPathArchive))
                Directory.CreateDirectory(strPathArchive);

            //Get all excel files in path
            var filePaths = Directory.GetFiles(strPathUpload)
                .Where(s => (s.EndsWith(".xls") || s.EndsWith(".xlsx")) && Path.GetFileName(s).StartsWith(strFilePrefix))
                .ToList();

            if (filePaths.Count == 0)
            {
                Log.Logger?.Information("No Magazine files with prefix '{MagazineImportFilePrefix}' to import!", strFilePrefix);
                return true;
            }

            List<IMagazineMapper> offers;
            var bitReturn = true;

            //Import all files
            foreach (var strFullFileName in filePaths)
            {
                Log.Logger?.Information("File: {PrenaxImportFileName}", strFullFileName);

                //Init work book
                using(var wb = new WorkBook())
                {
                    if (strFullFileName.EndsWith(".xlsx"))
                        wb.readXLSX(strFullFileName);
                    else
                        wb.read(strFullFileName);

                    var dt = wb.ExportDataTableFullFixed(true);
                    offers = dt.AsEnumerable().Select(dr => (IMagazineMapper)new PrenaxMapper(dr, Path.GetFileName(strFullFileName))).ToList();
                }

                //Import
                bitReturn &= base.ImportToDatabase(offers);

                ArchiveAndLog(strFullFileName, strPathArchive);
            }
            
            return bitReturn;
        }
    }
}
