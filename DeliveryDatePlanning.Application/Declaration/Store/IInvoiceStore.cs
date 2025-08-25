using DeliveryDatePlanning.Domain.Core.Entity;

namespace DeliveryDatePlanning.Application.Declaration.Store;

public interface IInvoiceStore
{
    Task<Invoice> Find(InvoiceFilter filter, CancellationToken token = default);
    Task<Invoice> FindLinked(InvoiceFilter filter, CancellationToken token = default);
}