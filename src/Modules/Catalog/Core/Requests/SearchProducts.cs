using Catalog.Core.Dto;

using Mediator;

namespace Catalog.Core.Requests;

public sealed record SearchProducts(int Page, int PageSize) : IStreamRequest<ProductDto>;