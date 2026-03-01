using ResearchApps.Domain;
using ResearchApps.Domain.Common;

namespace ResearchApps.Repo.Interface;

public interface IPhpRepo
{
    // Php_Select - Get list of all Php records with pagination
    Task<PagedList<PhpHeader>> PhpSelect(PagedListRequest request, CancellationToken cancellationToken);
    
    // Php_SelectById - Get Php by RecId
    Task<PhpHeader> PhpSelectById(int recId, CancellationToken cancellationToken);
    
    // Php_Insert - Create new Php (returns RecId and PhpId)
    Task<(int RecId, string PhpId)> PhpInsert(PhpHeader php, CancellationToken cancellationToken);
    
    // Php_Update - Update existing Php
    Task PhpUpdate(PhpHeader php, CancellationToken cancellationToken);
    
    // Php_Delete - Delete Php (and its lines)
    Task PhpDelete(int recId, string modifiedBy, CancellationToken cancellationToken);
    
    // PhpLine_SelectByPhp - Get lines for a Php
    Task<IEnumerable<PhpLine>> PhpLineSelectByPhp(int phpRecId, CancellationToken cancellationToken);
    
    // PhpLine_SelectById - Get specific line
    Task<PhpLine?> PhpLineSelectById(int phpLineId, CancellationToken cancellationToken);
    
    // PhpLine_Insert - Insert Php line (returns PhpId)
    Task<string> PhpLineInsert(PhpLine phpLine, CancellationToken cancellationToken);
    
    // PhpLine_Update - Update Php line
    Task<string> PhpLineUpdate(PhpLine phpLine, CancellationToken cancellationToken);
    
    // PhpLine_Delete - Delete Php line
    Task<string> PhpLineDelete(int phpLineId, string modifiedBy, CancellationToken cancellationToken);
}
