using HansenAndGenwestJobCardPortal.Models;
using static HansenAndGenwestJobCardPortal.Components.Pages.JobCards;

namespace HansenAndGenwestJobCardPortal.Interfaces
{
    public interface ISageInterface
    {
        Task<List<Product>> GetSageProducts();
        Task<List<Product>> GetMakeUpProducts();
        Task<Product> GetMakeUpProductByCode(string Code);
        Task<List<MakeUpProductComponents>> GetMakeUpProductComponents(string StockLink);
        Task<List<Product>> GetAllProducts();
        Task<Product> GetAllProductsByID(string stockLink);
        Task<Product> GetAllProductsByCode(string Code);
        Task<List<SageClient>> GetSageClients();
        Task<List<ImportHeader>> GetImportHeaders();
        Task<ImportHeader> GetImportHeaderByID(int Id);
        Task UserEvoPrice(int LineID, double EVOPrice);
        Task<List<ImportLine>> GetImportLinesByHeaderID(int HeaderId);
        Task<List<ImportLine>> GetErrorImportLinesByHeaderID(int HeaderId);
        Task DeleteImportHeaderByHeaderID(int HeaderId);
        Task DeleteImportLinesByHeaderID(int HeaderId);
        Task DeleteImportLinesByID (int Id);
        Task<Boolean> BulkInsertLine(int HeaderID, List<ExcelFile> excelFileList);
        Task<Boolean> BulkInsertLinesToMakeUpProduct(string ComponentHeaderID, List<ExcelFile> excelFileList);
        Task<Boolean> InsertLine(int HeaderID, ExcelFile excelFile);
        Task AddImportHeader(ImportHeader ImportHeader);
        Task AddMakeUpProduct(Product product );
        Task DeleteMakeUpProduct(string stocklink);
        Task DeleteMakeUpProductComponent(int ID);
        Task<List<string>> GetProcedures();
        Task<double> GetEvoPrice(string StockLink);
        Task<PostToSageResult> PostToSage(int HeaderID);
        Task<PostToSageResult> PostAdditionsToSage(int HeaderID);
        Task PostCustomToSage(int HeaderID);
        Task AddUpdateMakeUpProductComponent(MakeUpProductComponents makeUpProductComponent);
        Task<List<LabourExportResult>> PostLabourToSage(List<LabourImport> labourLines);
       
    }
}
