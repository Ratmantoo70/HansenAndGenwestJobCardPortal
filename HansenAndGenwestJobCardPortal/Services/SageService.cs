using Dapper;
using HansenAndGenwestJobCardPortal.Components.Pages;
using HansenAndGenwestJobCardPortal.Interfaces;
using HansenAndGenwestJobCardPortal.Models;
using Humanizer;
using Microsoft.Data.SqlClient;
using Syncfusion.Blazor.Grids.Internal;
using Syncfusion.Blazor.Kanban.Internal;
using Syncfusion.Drawing;
using Syncfusion.ExcelExport;
using Syncfusion.XlsIO;
using System.Reflection.Emit;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace HansenAndGenwestJobCardPortal.Services
{
    public class SageService : ISageInterface
    {
        private string? SageConnectionString { get; set; }
        public SageService(IConfiguration configuration)
        {
            SageConnectionString = configuration.GetConnectionString("SageConnection");
        }
        public async Task<List<Product>> GetSageProducts()
        {
            using (var connection = new SqlConnection(SageConnectionString))
            {
                var sql = @$"SELECT StockLink, Code, Description_1 AS Description, ItemActive,0 as Type
                             FROM StkItem
                             ORDER BY Code";
                var results = await connection.QueryAsync<Product>(sql);
                return results.ToList();
            }
        }

        public async Task<List<Product>> GetMakeUpProducts()
        {
            using (var connection = new SqlConnection(SageConnectionString))
            {
                var sql = @$"SELECT StockLink, Code,  Description_1 AS Description, ItemActive,Type
                             FROM __MakeUpProducts
                             ORDER BY Code";
                var results = await connection.QueryAsync<Product>(sql);
                return results.ToList();
            }
        }

        public async Task<List<MakeUpProductComponents>> GetMakeUpProductComponents(string StockLink)
        {
            using (var connection = new SqlConnection(SageConnectionString))
            {
                var sql = @$"SELECT Id,ParentStockLink, StockLink,Code,Description, Reference, Quantity, [Procedure], Price, UseSagePrice
                            FROM __MakeUpProductComponents
                            WHERE (ParentStockLink = '{StockLink}')";
                var results = await connection.QueryAsync<MakeUpProductComponents>(sql);
                return results.ToList();
            }
        }

        public async Task<List<Product>> GetAllProducts()
        {
            using (var connection = new SqlConnection(SageConnectionString))
            {
                var sql = @$"SELECT StockLink, Code, Description_1 as Description, ItemActive,Type
                            FROM __MakeUpProducts
                            UNION
                            SELECT  CAST(StockLink as NVARCHAR(50)) as Stocklink, Code, Description_1 AS Description, ItemActive,0 as Type
                            FROM StkItem
                            ORDER BY Code";
                var results = await connection.QueryAsync<Product>(sql);
                return results.ToList();
            }
        }

        public async Task<List<SageClient>> GetSageClients()
        {
            using (var connection = new SqlConnection(SageConnectionString))
            {
                var sql = @$"SELECT DCLink, Account, Name, On_Hold, Physical1, Physical2, Physical3, Physical4, Physical5, PhysicalPC, Post1, Post2, Post3, Post4, Post5, PostPC
                            FROM Client";
                var results = await connection.QueryAsync<SageClient>(sql);
                return results.ToList();
            }
        }
        public async Task<SageClient> GetSageClientByID(int DCLink)
        {
            using (var connection = new SqlConnection(SageConnectionString))
            {
                var sql = @$"SELECT DCLink, Account, Name, On_Hold, Physical1, Physical2, Physical3, Physical4, Physical5, PhysicalPC, Post1, Post2, Post3, Post4, Post5, PostPC
                            FROM Client WHERE DCLink = {DCLink}";
                var results = await connection.QueryFirstAsync<SageClient>(sql);
                return results;
            }
        }
        public async Task<List<ImportHeader>> GetImportHeaders()
        {
            using (var connection = new SqlConnection(SageConnectionString))
            {
                var sql = @"SELECT ID, Customer, CustomerID, [Job Description] as JobDescription, [External Order Number] as ExternalOrderNumber, StartDate, DeliveryDate, ClosingDate, JCNumber, JCID
                                FROM __Import_Header
                                ORDER BY ID DESC";
                var results = await connection.QueryAsync<ImportHeader>(sql);
                return results.ToList();
            }
        }
        public async Task<ImportHeader> GetImportHeaderByID(int Id)
        {
            using (var connection = new SqlConnection(SageConnectionString))
            {
                var sql = @$"SELECT ID, Customer, CustomerID, [Job Description] as JobDescription, [External Order Number] as ExternalOrderNumber, StartDate, DeliveryDate, ClosingDate, JCNumber, JCID
                                FROM __Import_Header Where ID = {Id}
                               ";
                var results = await connection.QueryFirstOrDefaultAsync<ImportHeader>(sql);
                return results;
            }
        }
        public async Task UserEvoPrice(int LineID, double EVOPrice)
        {
            using (var connection = new SqlConnection(SageConnectionString))
            {
                var sql = @$"UPDATE __Import_Lines SET Price = '{EVOPrice}'  WHERE ID = {LineID}";
                var results = await connection.ExecuteAsync(sql);
            }
        }
        public async Task<List<ImportLine>> GetImportLinesByHeaderID(int HeaderId)
        {
            using (var connection = new SqlConnection(SageConnectionString))
            {
                var sql = @$"SELECT       __Import_Lines.ID, StkItem.StockLink AS EvoID, StkItem.Code AS EvoCode, StkItem.Description_1 AS EvoDesc, __Import_Lines.[Item No] AS ExcelCode, __Import_Lines.Description AS ExcelDesc, __Import_Lines.Price AS ExcelPrice, 
                            _etblStockCosts.LatestCost AS EvoPrice, __Import_Lines.Qty, __Import_Lines.[Procedure], __Import_Lines.Reference, __Import_Lines.ID, __Import_Lines.JCLine, _etblStockCosts.LatestCost, __Import_Lines.ParentCode, __Import_Lines.ParentsParentCode
                        FROM StkItem LEFT OUTER JOIN
                            _etblStockCosts ON StkItem.StockLink = _etblStockCosts.StockID RIGHT OUTER JOIN
                            __Import_Lines ON StkItem.Code = __Import_Lines.[Item No]
                        WHERE (__Import_Lines.HeaderID = {HeaderId})";
                var results = await connection.QueryAsync<ImportLine>(sql);
                return results.ToList();
            }
        }
        public async Task<List<ImportLine>> GetErrorImportLinesByHeaderID(int HeaderId)
        {
            using (var connection = new SqlConnection(SageConnectionString))
            {
                var sql = @$"SELECT       __Import_Lines.ID, StkItem.StockLink AS EvoID, StkItem.Code AS EvoCode, StkItem.Description_1 AS EvoDesc, __Import_Lines.[Item No] AS ExcelCode, __Import_Lines.Description AS ExcelDesc, __Import_Lines.Price AS ExcelPrice, 
                            _etblStockCosts.LatestCost AS EvoPrice, __Import_Lines.Qty, __Import_Lines.[Procedure], __Import_Lines.Reference, __Import_Lines.ID, __Import_Lines.JCLine, _etblStockCosts.LatestCost, __Import_Lines.ParentCode, __Import_Lines.ParentsParentCode
                        FROM StkItem LEFT OUTER JOIN
                            _etblStockCosts ON StkItem.StockLink = _etblStockCosts.StockID RIGHT OUTER JOIN
                            __Import_Lines ON StkItem.Code = __Import_Lines.[Item No]
                        WHERE (__Import_Lines.HeaderID = {HeaderId} AND ROUND( _etblStockCosts.LatestCost,2) != ROUND(CAST(__Import_Lines.Price as float),2))";
                var results = await connection.QueryAsync<ImportLine>(sql);
                return results.ToList();
            }
        }
        public async Task<List<ImportLine>> GetUnexportedImportLinesByHeaderID(int HeaderId)
        {
            using (var connection = new SqlConnection(SageConnectionString))
            {
                var sql = @$"SELECT       __Import_Lines.ID, StkItem.StockLink AS EvoID, StkItem.Code AS EvoCode, StkItem.Description_1 AS EvoDesc, __Import_Lines.[Item No] AS ExcelCode, __Import_Lines.Description AS ExcelDesc, __Import_Lines.Price AS ExcelPrice, 
                            _etblStockCosts.LatestCost AS EvoPrice, __Import_Lines.Qty, __Import_Lines.[Procedure], __Import_Lines.Reference, __Import_Lines.ID, __Import_Lines.JCLine, _etblStockCosts.LatestCost, __Import_Lines.ParentCode, __Import_Lines.ParentsParentCode
                        FROM StkItem LEFT OUTER JOIN
                            _etblStockCosts ON StkItem.StockLink = _etblStockCosts.StockID RIGHT OUTER JOIN
                            __Import_Lines ON StkItem.Code = __Import_Lines.[Item No]
                        WHERE (__Import_Lines.HeaderID = {HeaderId} AND ROUND( _etblStockCosts.LatestCost,2) = ROUND(CAST(__Import_Lines.Price as float),2) AND (__Import_Lines.JCLine is null))";
                var results = await connection.QueryAsync<ImportLine>(sql);
                return results.ToList();
            }
        }
        public async Task<List<ImportLine>> GetExportedImportLinesByHeaderID(int HeaderId)
        {
            using (var connection = new SqlConnection(SageConnectionString))
            {
                var sql = @$"SELECT       __Import_Lines.ID, StkItem.StockLink AS EvoID, StkItem.Code AS EvoCode, StkItem.Description_1 AS EvoDesc, __Import_Lines.[Item No] AS ExcelCode, __Import_Lines.Description AS ExcelDesc, __Import_Lines.Price AS ExcelPrice, 
                            _etblStockCosts.LatestCost AS EvoPrice, __Import_Lines.Qty, __Import_Lines.[Procedure], __Import_Lines.Reference, __Import_Lines.ID, __Import_Lines.JCLine, _etblStockCosts.LatestCost, __Import_Lines.ParentCode, __Import_Lines.ParentsParentCode
                        FROM StkItem LEFT OUTER JOIN
                            _etblStockCosts ON StkItem.StockLink = _etblStockCosts.StockID RIGHT OUTER JOIN
                            __Import_Lines ON StkItem.Code = __Import_Lines.[Item No]
                        WHERE (__Import_Lines.HeaderID = {HeaderId} AND ROUND( _etblStockCosts.LatestCost,2) = ROUND(CAST(__Import_Lines.Price as float),2) AND (__Import_Lines.JCLine is not null))";
                var results = await connection.QueryAsync<ImportLine>(sql);
                return results.ToList();
            }
        }
        public async Task DeleteImportHeaderByHeaderID(int Id)
        {
            using (var connection = new SqlConnection(SageConnectionString))
            {
                var sql = @$"DELETE FROM [__Import_Header]  WHERE ID ={Id}";
                var results = await connection.ExecuteAsync(sql);
            }
        }
        public async Task DeleteImportLinesByHeaderID(int HeaderId)
        {
            using (var connection = new SqlConnection(SageConnectionString))
            {
                var sql = @$"DELETE FROM [__Import_Lines]   WHERE HeaderID ={HeaderId} and JCLine is null";
                var results = await connection.ExecuteAsync(sql);
            }
        }
        public async Task DeleteImportLinesByID(int Id)
        {
            using (var connection = new SqlConnection(SageConnectionString))
            {
                var sql = @$"DELETE FROM [__Import_Lines]   WHERE [ID] ={Id}";
                var results = await connection.ExecuteAsync(sql);
            }
        }
        public async Task<bool> BulkInsertLine(int HeaderID, List<ExcelFile> excelFileList)
        {
            using (var connection = new SqlConnection(SageConnectionString))
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    foreach (var excelFile in excelFileList)
                    {
                        var sql = @$" INSERT INTO __Import_Lines ([HeaderID]
                           ,[Item No]
                           ,[Description]
                           ,[Description 2]
                           ,[Reference]
                           ,[Qty]
                           ,[Procedure]
                           ,[Price])
                            VALUES 
                            (
                            {HeaderID},
                            '{excelFile.ItemNumber}',
                            '{excelFile.Description}',
                            '{excelFile.Description2}',
                            '{excelFile.Reference}',
                            {excelFile.Quantity},
                            '{excelFile.Procedure}',
                            {excelFile.Price}
                            )";
                        try
                        {
                            var results = await connection.ExecuteAsync(sql, null, transaction: tran);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                            tran.Rollback();
                            return false;
                        }
                    }
                    tran.Commit();
                    return true;
                }
            }
        }
        public async Task<bool> BulkInsertLinesToMakeUpProduct(string ParentStockLink, List<ExcelFile> excelFileList)
        {
            using (var connection = new SqlConnection(SageConnectionString))
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                   List<HansenAndGenwestJobCardPortal.Models.Product> theProducts = await GetAllProducts();
                    foreach (var excelFile in excelFileList)
                    {
                        Product product = theProducts.FirstOrDefault(x => x.Code == excelFile.ItemNumber!);

                        var sql = $@"INSERT INTO __MakeUpProductComponents
                         (ParentStockLink, StockLink, Code, Description, Reference, Quantity, [Procedure], Price, UseSagePrice)
                         VALUES ('{ParentStockLink}', '{product!.StockLink}', '{excelFile.ItemNumber}', 
                        '{excelFile.Description}', '{excelFile.Reference}', {excelFile.Quantity}, '{excelFile.Procedure}', {excelFile.Price}, 1)";

                        try
                        {
                            var results = await connection.ExecuteAsync(sql, null, transaction: tran);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                            tran.Rollback();
                            return false;
                        }
                    }
                    tran.Commit();
                    return true;
                }
            }
        }
        public async Task<bool> InsertLine(int HeaderID, ExcelFile excelFile)
        {
            using (var connection = new SqlConnection(SageConnectionString))
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    
                    var sql = @$" INSERT INTO __Import_Lines ([HeaderID]
                        ,[Item No]
                        ,[Description]
                        ,[Description 2]
                        ,[Reference]
                        ,[Qty]
                        ,[Procedure]
                        ,[Price]
                        ,[ParentCode]
                        ,[ParentsParentCode])
                        VALUES 
                        (
                        {HeaderID},
                        '{excelFile.ItemNumber}',
                        '{excelFile.Description}',
                        '{excelFile.Description2}',
                        '{excelFile.Reference}',
                        {excelFile.Quantity},
                        '{excelFile.Procedure}',
                        {excelFile.Price},
                        '{excelFile.ParentCode}',
                        '{excelFile.ParentsParentCode}'
                        )";
                    try
                    {
                        var results = await connection.ExecuteAsync(sql, null, transaction: tran);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        tran.Rollback();
                        return false;
                    }
                    
                    tran.Commit();
                    return true;
                }
            }
        }
        public async Task AddImportHeader(ImportHeader ImportHeader)
        {
            using (var connection = new SqlConnection(SageConnectionString))
            {
                var sql = @$"INSERT INTO [__Import_Header]
                       ([Customer]
                       ,[CustomerID]
                       ,[Job Description]
                       ,[External Order Number]
                       ,[StartDate]
                       ,[DeliveryDate]
                       ,[ClosingDate])
                         VALUES
                        ('{ImportHeader.Customer}',
                        '{ImportHeader.CustomerID}',
                        '{ImportHeader.JobDescription}',
                        '{ImportHeader.ExternalOrderNumber}',
                        '{ImportHeader.StartDate.ToString("yyyy-MM-dd")}',
                        '{ImportHeader.DeliveryDate.ToString("yyyy-MM-dd")}',
                        '{ImportHeader.ClosingDate.ToString("yyyy-MM-dd")}');";
                var results = await connection.ExecuteAsync(sql);
            }
        }
        public async Task<List<string>> GetProcedures()
        {
            using (var connection = new SqlConnection(SageConnectionString))
            {
                var sql = @$"SELECT        cLookupOptions
                             FROM            _rtblUserDict
                             WHERE        (cFieldName = 'ulJCTxCMProcedure')";
                
                 string results = await connection.QueryFirstAsync<string>(sql);
                 return results.Split(";").ToList ();  
                    
            }
        }
        public async Task<double> GetEvoPrice(string StockLink)
        {
            using (var connection = new SqlConnection(SageConnectionString))
            {
                var sql = $@"SELECT        ROUND(LatestCost, 2) AS EvoPrice
                            FROM            _etblStockCosts
                            where StockID = {StockLink}";

                try
                {
                    double result = await connection.QueryFirstAsync<double>(sql);
                    return result;
                }
                catch
                {
                    return 0;
                }
            }

        }
        public async Task<JobDef> GetJOBDef()
        {
            using (var connection = new SqlConnection(SageConnectionString))
            {
                var sql = @$"Select CurrentNumber,cNextJCQuote,cJobPrefix,idJobDef from JOBDEF";

                var results = await connection.QueryFirstAsync<JobDef>(sql);

                sql = "update JOBDEF set CurrentNumber=convert(varchar, convert(int, CurrentNumber)+1) where (isnull(JOBDEF_iBranchID,0)=0)";
                await connection.ExecuteAsync(sql);
                sql = "update JOBDEF set cNextJCQuote=convert(varchar, convert(int, cNextJCQuote)+1) where (isnull(JOBDEF_iBranchID,0)=0)";
                await connection.ExecuteAsync(sql);

                return results;
            }
        }
        public async Task<double> GetTaxRate()
        {
            using (var connection = new SqlConnection(SageConnectionString))
            {
                var sql = @$"select TAXRATE from TAXRATE where (IdTaxRate in (1))";

                var results = await connection.QueryFirstAsync<double>(sql);              

                return results;
            }
        }
        public async Task<int> PostJCMasterStore(int JobMasterID,string JobCode,JobDef job, ImportHeader importHeader,SageClient client)
        {
            using (var connection = new SqlConnection(SageConnectionString))
            {
                var sql = @$"DECLARE	@return_value int

                    EXEC	@return_value = [dbo].[_bspJCMasterStore]
                    		@JobMastID = {JobMasterID},
                    		@JobCode = N'{JobCode}',
                    		@JobDescription = N'{importHeader.JobDescription}',
                    		@JobStatus = 0,
                    		@PostingMethod = 1,
                    		@WIPAccLink = 0,
                    		@SalesAccLink = 0,
                    		@COSAccLink = 0,
                    		@RecoveryAccLink = 0,
                    		@CustomerID ={importHeader.CustomerID},
                    		@OrderNo = N'0',
                    		@FinalInvoiceNo = NULL,
                    		@DeliveryNoteNo = NULL,
                    		@FinalCheckBy = N'',
                    		@AuthorisedBy = N'',
                    		@StartDate = N'{importHeader.StartDate.ToString("yyyy-MM-dd  00:00:00.000")}',
                    		@CompletionDate = NULL,
                    		@DeliveryDate = N'{importHeader.DeliveryDate.ToString("yyyy-MM-dd  00:00:00.000")}',
                    		@ClosingDate = N'{importHeader.ClosingDate.ToString("yyyy-MM-dd  00:00:00.000")}',
                    		@JobQuote = 0,
                    		@IsTemplate = 0,
                    		@FinalInvoice = 0,
                    		@DeliveryMethodID = 0,
                    		@ProjectID = 0,
                    		@InclusiveEntry =  0,
                    		@DiscountPercent = 0,
                    		@RepID = 0,
                    		@InvDate = N'1899-12-30 00:00:00',
                    		@Address1 = N'{client.Physical1}',
                    		@Address2 = N'{client.Physical2 }',
                    		@Address3 = N'{client.Physical3 }',
                    		@Address4 = N'{client.Physical4 }',
                    		@Address5 = N'{client.Physical5 }',
                    		@Address6 = N'{client.PhysicalPC }',
                    		@PAddress1 = N'{client.Post1 }',
                    		@PAddress2 = N'{client.Post2 }',
                    		@PAddress3 = N'{client.Post3 }',
                    		@PAddress4 = N'{client.Post4 }',
                    		@PAddress5 = N'{client.Post5 }',
                    		@PAddress6 = N'{client.PostPC }',
                    		@Message1 = N'0',
                    		@Message2 = N'0',
                    		@Message3 = N'0',
                    		@Narration = N'0',
                    		@ExtOrderNo = N'',
                    		@CurrencyID = 0,
                    		@SettlementTermsID = 0,
                    		@EUNoTCIDAdd = 0,
                    		@EUNoTCIDRem = 0,
                    		@iBranchID = 0,
                    		@TaxPerLineEntry = 1,
                    		@iSortDocID = 0,
                            @WIPVarianceAccountLink = 0,
                            @WIPVarianceValue = 0

                    SELECT	'Return Value' = @return_value";

                int results = await connection.QueryFirstAsync<int>(sql);

                return results;
            }
        }
        public async Task SetucJCADDITIONALINFORMATION(int idJCMaster,string extOrderNo)
        {
            using (var connection = new SqlConnection(SageConnectionString))
            {               
                var sql = $"UPDATE _btblJCMaster SET ucJCADDITIONALINFORMATION='{extOrderNo}' WHERE IdJCMaster={idJCMaster}";
                await connection.ExecuteAsync(sql);
                sql = $@"INSERT INTO _etblUserHistLink ([UserDictID],[TableID],[UserValue],[LastModifiedDate])
                                            VALUES (70,{idJCMaster},'{extOrderNo}',GETDATE())";
                await connection.ExecuteAsync(sql);
            }
        }
        public async Task SetJCNumber(string JobCode,int ID,int JCID)
        {
            using (var connection = new SqlConnection(SageConnectionString))
            {
                var sql = $@"UPDATE [__Import_Header]
                            SET [JCNumber] = '{JobCode}',
                            [JCID] = {JCID}
                            WHERE ID = {ID} ";
                await connection.ExecuteAsync(sql);
            }
        }
        public async Task PostJCLines(int JCMaster,List<ImportLine> importLines,double TaxRate,ImportHeader importHeader)
        {            
            using (var connection = new SqlConnection(SageConnectionString))
            {
                foreach (var line in importLines)
                {
                    var sql = @$"DECLARE	@return_value int,
                    		@JCTxLinesID bigint

                    EXEC	@return_value = [dbo].[_bspJCTxLinesStore]
                    		@JCTxLinesID = @JCTxLinesID OUTPUT,
                    		@iJCMasterID = {JCMaster},
                    		@iJobTxTpID = 9,
                    		@iSource = 0,
                    		@iStockID = {line.EvoID},
                    		@iSupplierID = 0,
                    		@iLedgerID = 0,
                    		@cDescription = N'{line.EvoDesc.Replace("'", "")}',
                    		@iStatus =0,
                    		@iDuration = 0,
                    		@dStartDate = N'{importHeader.StartDate.ToString("yyyy-MM-dd  00:00:00.000")}',
                    		@dEndDate = N'{importHeader.ClosingDate.ToString("yyyy-MM-dd  00:00:00.000")}',
                    		@fMainDiscount = 0,
                    		@fUnitPriceExcl = {line.EvoPrice},
                    		@fUnitPriceIncl = {line.EvoPrice * ((100 + TaxRate) / 100)},
                    		@fExchangeRate = 0,
                    		@fUnitPriceExclForeign = 0,
                    		@fUnitPriceInclForeign = 0,
                    		@fUnitCost = 0,
                    		@iGrvCurrencyID = 0,
                    		@fExchangeRateGrv = 0,
                    		@fLineDiscount = 0,
                    		@iTaxTypeIDInv = 1,
                    		@fTaxRateInv = {TaxRate},
                    		@fTransQty = {line.Qty},
                    		@fTransQtyToInvoice = 0,
                    		@iUnitsOfMeasureStockingID = 0,
                    		@iUnitsOfMeasureCategoryID = 0,
                    		@iUnitsOfMeasureID = 0,
                    		@iWarehouseID = 0,
                    		@iPriceListNameID = 0,
                    		@iTaxTypeIDGrv = 1,
                    		@fTaxRateGrv = 0,
                    		@fTaxAmountGrv = 0,
                    		@fTaxAmountGrvForeign = 0,
                    		@iEmployeeID = 0,
                    		@fBudgetUnitPriceExcl = {line.EvoPrice},
                    		@fBudgetUnitPriceIncl = {line.EvoPrice * ((100 + TaxRate) / 100)},
                    		@fBudgetUnitPriceExclForeign = 0,
                    		@fBudgetUnitPriceInclForeign = 0,
                    		@fBudgetUnitCost = 0,
                    		@fBudgetLineTotalExcl = {line.EvoPrice * line.Qty},
                    		@fBudgetLineTotalIncl = {line.EvoPrice * line.Qty * ((100 + TaxRate) / 100)},
                    		@fBudgetLineTotalExclForeign =  {line.EvoPrice * line.Qty},
                    		@fBudgetLineTotalInclForeign = {line.EvoPrice * line.Qty * ((100 + TaxRate) / 100)},
                    		@fBudgetLineTotalTaxAmountInv =  {(line.EvoPrice * line.Qty * ((100 + TaxRate) / 100)) - (line.EvoPrice * line.Qty)},
                    		@fBudgetLineTotalTaxAmountInvForeign = {(line.EvoPrice * line.Qty * ((100 + TaxRate) / 100)) - (line.EvoPrice * line.Qty)},
                    		@fBudgetLineTotalTaxAmountGrv = 0,
                    		@fBudgetLineTotalTaxAmountGrvForeign = 0,
                    		@fBudgetLineTotalCost = 0,
                    		@fLineTotalExcl =   {line.EvoPrice * line.Qty},
                    		@fLineTotalIncl =  {line.EvoPrice * line.Qty * ((100 + TaxRate) / 100)},
                    		@fLineTotalExclForeign = 0,
                    		@fLineTotalInclForeign = 0,
                    		@fLineTotalTaxAmountInv = {(line.EvoPrice * line.Qty * ((100 + TaxRate) / 100)) - (line.EvoPrice * line.Qty)},
                    		@fLineTotalTaxAmountInvForeign = 0,
                    		@fLineTotalExclToInvoice = 0,
                    		@fLineTotalInclToInvoice = 0,
                    		@fLineTotalExclForeignToInvoice = 0,
                    		@fLineTotalInclForeignToInvoice = 0,
                    		@fLineTotalTaxAmountInvToInvoice = 0,
                    		@fLineTotalTaxAmountInvForeignToInvoice = 0,
                    		@fLineTotalCost = 0,
                    		@bPosted = 1,
                    		@bInvoiced = 0,
                    		@iJobNumID = 0,
                    		@cinvNumber = N'',
                    		@cUserName = N'',
                    		@iJobStockGroupID = 0,
                    		@iSNGroupID = 0,
                    		@cLineNotes = N'',
                    		@iInvNumID = 0,
                    		@bPicked = 0,
                    		@iLineRepID = 0,
                    		@iLineProjectID = 0,
                    		@ChargeCom = 0,
                    		@iMFPID = null,
                    		@iLotID = 0,
                    		@Reference = N'{line.Reference}',
                    		@iInvSettlementTermsID = 0,
                    		@iAPSettlementTermsID = 0,
                    		@iEUNoTCID = 0,
                    		@UpdateWIPQty = null,
                    		@iSNInvoicedGroupID = 0,
                    		@iSNToInvoiceGroupID = 0,
                    		@fTransQtyAvailable =  {line.Qty},
                    		@cLineUserFields = N'',
                    		@iLineID = 1

                    SELECT	@JCTxLinesID as N'@JCTxLinesID'";

                    int JCLine = await connection.QueryFirstAsync<int>(sql);

                    await PostCustomLine(line, JCLine);

                    sql = @$"UPDATE [__Import_Lines]
                             SET [JCLine] ={JCLine}
                             WHERE ID = {line.ID}";
                    await connection.ExecuteAsync(sql);
                }               
            }
        }
        public async Task PostCustomLine(ImportLine line,int JCLine)
        {
            using (var connection = new SqlConnection(SageConnectionString))
            {                
                var sql = @$"UPDATE _btblJCTxLines
                        SET ulJCTxCMProcedure = '{line.Procedure}', ufJCTxCMOriginalQty = {line.Qty}
                        WHERE (idJCTxLines = {JCLine})";
                await connection.ExecuteAsync(sql);

                sql = @$"INSERT INTO _etblUserHistLink ([UserDictID],[TableID],[UserValue],[LastModifiedDate])
                                        VALUES (70,{JCLine},'{line.Procedure}',GETDATE())";
                await connection.ExecuteAsync(sql);

                sql = @$"INSERT INTO _etblUserHistLink ([UserDictID],[TableID],[UserValue],[LastModifiedDate])
                                        VALUES (71,{JCLine},'{line.Qty}',GETDATE())";
                await connection.ExecuteAsync(sql);

                sql = @$"UPDATE [__Import_Lines]
                            SET [JCLine] ={JCLine}
                            WHERE ID = {line.ID}";                
            }
        }
        public async Task<PostToSageResult> PostToSage(int HeaderID)
        {
            ImportHeader importHeader = await GetImportHeaderByID(HeaderID);
            List<ImportLine> importLines = await GetUnexportedImportLinesByHeaderID(HeaderID);
            SageClient? customer = new SageClient();

            if (importHeader.CustomerID != 0)
            {
                customer = await GetSageClientByID(importHeader.CustomerID);
                if (customer != null )
                {               
                    if(customer.Physical1.Length == 0)
                    {
                        customer.Physical1 = customer.Post1;
                        customer.Physical2 = customer.Post2;
                        customer.Physical3 = customer.Post3;
                        customer.Physical4 = customer.Post4;
                        customer.Physical5 = customer.Post5;
                        customer.PhysicalPC = customer.PostPC;
                    }
                }
                else
                {
                    return new PostToSageResult
                    {
                        Message = $"Customer Not Found!",
                        Success = false
                    };
                }
            }

            JobDef job = await GetJOBDef();
            double TaxRate = await GetTaxRate();
            string JobCode = job.cJobPrefix + job.CurrentNumber.ToString("D5");
            int JCMaster = await PostJCMasterStore(0, JobCode, job, importHeader, customer);
           
            await SetucJCADDITIONALINFORMATION(JCMaster, importHeader.JobDescription);
            await PostJCLines(JCMaster, importLines, TaxRate, importHeader);
            await SetJCNumber(JobCode, HeaderID,JCMaster);

            return new PostToSageResult
            {
                JobCode = JobCode,
                Success = true
            };
        }
        public async Task PostCustomToSage(int HeaderID)
        {
            List<ImportLine> importLines = await GetExportedImportLinesByHeaderID(HeaderID);
            foreach(ImportLine importLine in importLines)
            {
                await PostCustomLine(importLine, importLine.JCLine);
            }

        }
        public async Task<PostToSageResult> PostAdditionsToSage(int HeaderID)
        {
            ImportHeader importHeader = await GetImportHeaderByID(HeaderID);
            List<ImportLine> importLines = await GetUnexportedImportLinesByHeaderID(HeaderID);
            SageClient? customer = new SageClient();

            if (importHeader.CustomerID != 0)
            {
                customer = await GetSageClientByID(importHeader.CustomerID);
                if (customer != null)
                {
                    if (customer.Physical1.Length == 0)
                    {
                        customer.Physical1 = customer.Post1;
                        customer.Physical2 = customer.Post2;
                        customer.Physical3 = customer.Post3;
                        customer.Physical4 = customer.Post4;
                        customer.Physical5 = customer.Post5;
                        customer.PhysicalPC = customer.PostPC;
                    }
                }
                else
                {
                    return new PostToSageResult
                    {
                        Message = $"Customer Not Found!",
                        Success = false
                    };
                }
            }
           
            double TaxRate = await GetTaxRate();
            int JCMaster = importHeader.JCID!.Value;

          
            await PostJCLines(JCMaster, importLines, TaxRate, importHeader);

            return new PostToSageResult
            {
                Message = "Additions Posted Successfully!",
                Success = true
            };
        }
        public async Task AddMakeUpProduct(Product product)
        {
            using (var connection = new SqlConnection(SageConnectionString))
            {                
                var sql = @$"INSERT INTO __MakeUpProducts
                         (StockLink, Code, Description_1, ItemActive, Type)
                         VALUES  
                         ('{product.StockLink}', '{product.Code}', '{product.Description}', 1, 1)";
                    
                    var results = await connection.ExecuteAsync(sql);                   
            }
        }
        public async Task DeleteMakeUpProduct(string stocklink)
        {
            using (var connection = new SqlConnection(SageConnectionString))
            {
                var sql = @$"UPDATE __MakeUpProducts SET ItemActive = 0 WHERE StockLink = '{stocklink}'";

                var results = await connection.ExecuteAsync(sql);
            }
        }
        public async Task AddUpdateMakeUpProductComponent(MakeUpProductComponents makeUpProductComponent)
        {
            int useSagePrice = 0;
            if (makeUpProductComponent.UseSagePrice)
            {
                useSagePrice = 1;
            }
            if (makeUpProductComponent.Id==0)
            {
                using (var connection = new SqlConnection(SageConnectionString))
                {
                    var sql = $@"INSERT INTO __MakeUpProductComponents
                         (ParentStockLink, StockLink, Code, Description, Reference, Quantity, [Procedure], Price, UseSagePrice)
                         VALUES ('{makeUpProductComponent.ParentStockLink}', '{makeUpProductComponent.StockLink}', '{makeUpProductComponent.Code}', '{makeUpProductComponent.Description}', '{makeUpProductComponent.Reference}', {makeUpProductComponent.Quantity}, '{makeUpProductComponent.Procedure}', {makeUpProductComponent.Price}, {useSagePrice})";

                    var results = await connection.ExecuteAsync(sql);
                }
            }
            else
            {
                using (var connection = new SqlConnection(SageConnectionString))
                {
                    var sql = $@"UPDATE __MakeUpProductComponents
                            SET ParentStockLink = '{makeUpProductComponent.ParentStockLink}', StockLink = '{makeUpProductComponent.ParentStockLink}', Code = '{makeUpProductComponent.Code}', Description = '{makeUpProductComponent.Description}', Reference = '{makeUpProductComponent.Reference}', Quantity = {makeUpProductComponent.Quantity}, [Procedure] = '{makeUpProductComponent.Procedure}', Price = {makeUpProductComponent.Price}, UseSagePrice =  {useSagePrice}
                            WHERE (Id = {makeUpProductComponent.Id})";
                    var results = await connection.ExecuteAsync(sql);
                }
            }
        }
        public async Task<Product> GetAllProductsByID(string stockLink)
        {
            using (var connection = new SqlConnection(SageConnectionString))
            {
                var sql = @$"SELECT A.* FROM
                            (SELECT StockLink, Code, Description_1 as Description, ItemActive,Type
                            FROM __MakeUpProducts
                            UNION
                            SELECT  CAST(StockLink as NVARCHAR(50)) as Stocklink, Code, Description_1 AS Description, ItemActive,0 as Type
                            FROM StkItem) A
                            WHERE Stocklink = '{stockLink}'";
                var results = await connection.QueryFirstAsync<Product>(sql);
                return results;
            }
        }
        public async Task<Product> GetAllProductsByCode(string Code)
        {
            using (var connection = new SqlConnection(SageConnectionString))
            {
                var sql = @$"SELECT A.* FROM
                            (SELECT StockLink, Code, Description_1 as Description, ItemActive,Type
                            FROM __MakeUpProducts
                            UNION
                            SELECT  CAST(StockLink as NVARCHAR(50)) as Stocklink, Code, Description_1 AS Description, ItemActive,0 as Type
                            FROM StkItem) A
                            WHERE Code = '{Code}'";
                var results = await connection.QueryFirstAsync<Product>(sql);
                return results;
            }
        }
        public async Task<Product> GetMakeUpProductByCode(string Code)
        {
            using (var connection = new SqlConnection(SageConnectionString))
            {
                Console.WriteLine(Code);
                var sql = @$"SELECT StockLink, Code,  Description_1 AS Description, ItemActive,Type
                             FROM __MakeUpProducts
                             WHERE Code = '{Code}'";
                var results = await connection.QueryFirstAsync<Product>(sql);
                return results;
            }
        }
        public async Task DeleteMakeUpProductComponent(int ID)
        {
            using (var connection = new SqlConnection(SageConnectionString))
            {
                var sql = @$"DELETE FROM [__MakeUpProductComponents]   WHERE [Id] ={ID}";
                var results = await connection.ExecuteAsync(sql);
            }
        }
        public async Task<List<LabourExportResult>> PostLabourToSage(List<LabourImport> labourLines)
        {
            string Message = "";
            List<LabourExportResult> LERs = new List<LabourExportResult>();
            double Price;
            using (var connection = new SqlConnection(SageConnectionString))
            {
                var sql = @$"SELECT [LatestCost] FROM [_etblStockCosts]  where StockID = 13098";
                Price = await connection.QueryFirstAsync<int>(sql);
            }

            foreach (LabourImport labourImport in labourLines)
            {
                int JCMaster = 0;
                Message = "";
                LabourExportResult ler = new LabourExportResult();
                ler.JobNo= labourImport.JobNo;

                try
                {
                    Message = "Job Card Not Found in Sage!";
                    using (var connection = new SqlConnection(SageConnectionString))
                    {
                        var sql = @$"SELECT IdJCMaster FROM _btblJCMaster WHERE (cJobCode = 'JOB{labourImport.JobNo}')";
                        JCMaster = await connection.QueryFirstAsync<int>(sql);
                    }
                    Message = "Post to Sage failed!";
                    await PostLabourLine(JCMaster,labourImport.Procedure, labourImport.Reference, Price, labourImport.Hours);
                    ler.Success = true;
                    ler.Message = "Success";
                }
                catch
                {
                    ler.Success = false;
                    ler.Message = Message;
                }
                LERs.Add(ler);
            }


            return LERs;
        }

        

        public async Task PostLabourLine(int JCMaster, string Procedure, string Reference, double Price, double Quantity)
        {
            double TaxRate = await GetTaxRate();
            DateTime dtNow = DateTime.Now;

            using (var connection = new SqlConnection(SageConnectionString))
            {
                var sql = @$"DECLARE	@return_value int,
                    		@JCTxLinesID bigint

                    EXEC	@return_value = [dbo].[_bspJCTxLinesStore]
                    		@JCTxLinesID = @JCTxLinesID OUTPUT,
                    		@iJCMasterID = {JCMaster},
                    		@iJobTxTpID = 9,
                    		@iSource = 0,
                    		@iStockID = 13098,
                    		@iSupplierID = 0,
                    		@iLedgerID = 0,
                    		@cDescription = N'Labour - Standard',
                    		@iStatus =0,
                    		@iDuration = 0,
                    		@dStartDate = '{dtNow.ToString("dd MMM yyyy")}',
                    		@dEndDate = '{dtNow.ToString("dd MMM yyyy")}',
                    		@fMainDiscount = 0,
                    		@fUnitPriceExcl = {Price},
                    		@fUnitPriceIncl = {Price * ((100 + TaxRate) / 100)},
                    		@fExchangeRate = 0,
                    		@fUnitPriceExclForeign = 0,
                    		@fUnitPriceInclForeign = 0,
                    		@fUnitCost = 0,
                    		@iGrvCurrencyID = 0,
                    		@fExchangeRateGrv = 0,
                    		@fLineDiscount = 0,
                    		@iTaxTypeIDInv = 1,
                    		@fTaxRateInv = {TaxRate},
                    		@fTransQty = {Quantity},
                    		@fTransQtyToInvoice = 0,
                    		@iUnitsOfMeasureStockingID = 0,
                    		@iUnitsOfMeasureCategoryID = 0,
                    		@iUnitsOfMeasureID = 0,
                    		@iWarehouseID = 0,
                    		@iPriceListNameID = 0,
                    		@iTaxTypeIDGrv = 1,
                    		@fTaxRateGrv = 0,
                    		@fTaxAmountGrv = 0,
                    		@fTaxAmountGrvForeign = 0,
                    		@iEmployeeID = 0,
                    		@fBudgetUnitPriceExcl = {Price},
                    		@fBudgetUnitPriceIncl = {Price * ((100 + TaxRate) / 100)},
                    		@fBudgetUnitPriceExclForeign = 0,
                    		@fBudgetUnitPriceInclForeign = 0,
                    		@fBudgetUnitCost = 0,
                    		@fBudgetLineTotalExcl = {Price * Quantity},
                    		@fBudgetLineTotalIncl = {Price * Quantity * ((100 + TaxRate) / 100)},
                    		@fBudgetLineTotalExclForeign =  {Price * Quantity},
                    		@fBudgetLineTotalInclForeign = {Price * Quantity * ((100 + TaxRate) / 100)},
                    		@fBudgetLineTotalTaxAmountInv =  {(Price * Quantity * ((100 + TaxRate) / 100)) - (Price * Quantity)},
                    		@fBudgetLineTotalTaxAmountInvForeign = {(Price * Quantity * ((100 + TaxRate) / 100)) - (Price * Quantity)},
                    		@fBudgetLineTotalTaxAmountGrv = 0,
                    		@fBudgetLineTotalTaxAmountGrvForeign = 0,
                    		@fBudgetLineTotalCost = 0,
                    		@fLineTotalExcl =   {Price * Quantity},
                    		@fLineTotalIncl =  {Price * Quantity * ((100 + TaxRate) / 100)},
                    		@fLineTotalExclForeign = 0,
                    		@fLineTotalInclForeign = 0,
                    		@fLineTotalTaxAmountInv = {(Price * Quantity * ((100 + TaxRate) / 100)) - (Price * Quantity)},
                    		@fLineTotalTaxAmountInvForeign = 0,
                    		@fLineTotalExclToInvoice = 0,
                    		@fLineTotalInclToInvoice = 0,
                    		@fLineTotalExclForeignToInvoice = 0,
                    		@fLineTotalInclForeignToInvoice = 0,
                    		@fLineTotalTaxAmountInvToInvoice = 0,
                    		@fLineTotalTaxAmountInvForeignToInvoice = 0,
                    		@fLineTotalCost = 0,
                    		@bPosted = 1,
                    		@bInvoiced = 0,
                    		@iJobNumID = 0,
                    		@cinvNumber = N'',
                    		@cUserName = N'',
                    		@iJobStockGroupID = 0,
                    		@iSNGroupID = 0,
                    		@cLineNotes = N'',
                    		@iInvNumID = 0,
                    		@bPicked = 0,
                    		@iLineRepID = 0,
                    		@iLineProjectID = 0,
                    		@ChargeCom = 0,
                    		@iMFPID = null,
                    		@iLotID = 0,
                    		@Reference = N'{Reference}',
                    		@iInvSettlementTermsID = 0,
                    		@iAPSettlementTermsID = 0,
                    		@iEUNoTCID = 0,
                    		@UpdateWIPQty = null,
                    		@iSNInvoicedGroupID = 0,
                    		@iSNToInvoiceGroupID = 0,
                    		@fTransQtyAvailable =  {Quantity},
                    		@cLineUserFields = N'',
                    		@iLineID = 1

                    SELECT	@JCTxLinesID as N'@JCTxLinesID'";

                int JCLine = await connection.QueryFirstAsync<int>(sql);                
            }
        }
    }
}

