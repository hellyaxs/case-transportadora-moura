using Api.Modules.Collections.Application.Contracts;
using Api.Modules.Collections.Application.Dtos;
using Api.Modules.Collections.Domain.Enums;
using Api.Shared.Domain.Exceptions;

namespace Api.Modules.Collections.Application.UseCases;

public sealed class ListCollectionsUseCase(
    ICollectionRepository repository,
    IClock clock)
{
    public async Task<PaginatedCollectionResponseDto> ExecuteAsync(
        CollectionStatus? status,
        Guid? customerId,
        DateOnly? startDate,
        DateOnly? endDate,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        if (startDate.HasValue && endDate.HasValue && startDate > endDate)
        {
            throw new BusinessRuleException("Start date cannot be greater than end date.", "invalid_date_range");
        }

        var normalizedPage = page < 1 ? 1 : page;
        var normalizedPageSize = pageSize switch
        {
            < 1 => 10,
            > 50 => 50,
            _ => pageSize,
        };

        var totalCount = await repository.CountAsync(status, customerId, startDate, endDate, cancellationToken);
        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)normalizedPageSize);
        var effectivePage = totalPages == 0 ? 1 : Math.Min(normalizedPage, totalPages);

        var collections = await repository.ListPagedAsync(
            status,
            customerId,
            startDate,
            endDate,
            effectivePage,
            normalizedPageSize,
            cancellationToken);
        var metrics = await repository.GetListMetricsAsync(
            status,
            customerId,
            startDate,
            endDate,
            clock.Today,
            cancellationToken);

        return new PaginatedCollectionResponseDto(
            collections.Select(collection => CollectionDtoMapper.ToSummary(collection, clock.Today)).ToList(),
            effectivePage,
            normalizedPageSize,
            totalCount,
            totalPages,
            metrics);
    }
}
