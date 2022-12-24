using Catalog.Core.Dto;
using Catalog.Core.Model;

using Mediator;

namespace Catalog.Core.Requests;

public record GetProductById(ProductId ProductId) : IRequest<ProductDto?>;