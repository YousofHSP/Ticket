using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data.Contracts;
using Entities.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebFramework.Filters;

namespace WebFramework.Api;

[ApiVersion("1")]
public class CrudController<TDto, TResDto, TEntity, TKey> : BaseController
    where TEntity : class, IEntity<TKey>, new()
    where TDto : BaseDto<TDto, TEntity, TKey>, new()
    where TResDto : BaseDto<TResDto, TEntity, TKey>, new()
{
    private readonly IRepository<TEntity> _repository;
    private readonly IMapper _mapper;

    public CrudController(IRepository<TEntity> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet]
    public virtual async Task<ActionResult<List<TResDto>>> Get(CancellationToken cancellationToken)
    {
        var list = await _repository.TableNoTracking
            .ProjectTo<TDto>(_mapper.ConfigurationProvider).ToListAsync(cancellationToken);
        return Ok(list);
    }

    [HttpGet("{id}")]
    public virtual async Task<ApiResult<TResDto>> Get(TKey id, CancellationToken cancellationToken)
    {
        var dto = await _repository.TableNoTracking.ProjectTo<TResDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(p => p.Id!.Equals(id), cancellationToken);
        if (dto == null)
            return NotFound();
        return dto;
    }

    [HttpPost]
    public virtual async Task<ApiResult<TResDto>> Create([FromForm]TDto dto, CancellationToken cancellationToken)
    {
        var model = dto.ToEntity(_mapper);

        await _repository.AddAsync(model, cancellationToken);
        var resultDto = await _repository.TableNoTracking.ProjectTo<TResDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(p => p.Id!.Equals(model.Id), cancellationToken);
        return resultDto!;
    }

    [HttpPatch]
    public virtual async Task<ApiResult<TResDto>> Update(TKey id, TDto dto, CancellationToken cancellationToken)
    {
        var model = await _repository.GetByIdAsync(cancellationToken, id);

        model = dto.ToEntity(model, _mapper);

        await _repository.UpdateAsync(model, cancellationToken);
        var resultDto = await _repository.TableNoTracking
            .ProjectTo<TResDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(p => p.Id!.Equals(model.Id), cancellationToken);
        return resultDto!;
    }

    [HttpDelete("{id}")]
    public virtual async Task<ApiResult> Delete(TKey id, CancellationToken cancellationToken)
    {
        var model = await _repository.GetByIdAsync(cancellationToken, id!);
        await _repository.DeleteAsync(model, cancellationToken);

        return Ok();
    }

}
public class CrudController<TDto, TResDto, TEntity> : CrudController<TDto, TResDto, TEntity, int>
    where TEntity : class, IEntity<int>, new()
    where TDto : BaseDto<TDto, TEntity, int>, new()
    where TResDto : BaseDto<TResDto, TEntity, int>, new()
{
    public CrudController(IRepository<TEntity> repository, IMapper mapper) : base(repository, mapper)
    {
    }
}

public class CrudController<TDto, TEntity> : CrudController<TDto, TDto, TEntity, int>
    where TEntity : class, IEntity<int>, new()
    where TDto : BaseDto<TDto, TEntity, int>, new()
{
    public CrudController(IRepository<TEntity> repository, IMapper mapper) : base(repository, mapper)
    {
    }
}
