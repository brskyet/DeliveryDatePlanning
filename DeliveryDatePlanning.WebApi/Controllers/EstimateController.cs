using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using DeliveryDatePlanning.Application.Declaration.Handler;
using DeliveryDatePlanning.Application.Declaration.Handler.Query;
using DeliveryDatePlanning.WebApi.Model;

namespace DeliveryDatePlanning.WebApi.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class EstimateController : ControllerBase
{
    private readonly IQueryHandler queryHandler;
    private readonly IMapper mapper;

    public EstimateController(IQueryHandler queryHandler, IMapper mapper)
    {
        this.queryHandler = queryHandler ?? throw new ArgumentNullException(nameof(queryHandler));
        this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// Метод возвращает данные по РСД. Поиск производится по "invoiceId-InvoiceOwnerId"
    /// </summary>
    /// <param name="invoiceKey">Составной ключ инвойса (invoiceId-InvoiceOwnerId), например, 123456-789</param>
    /// <returns></returns>
    /// <response code="200">Успешно. Документ найден </response>
    /// <response code="400">Ошибка. Невалидный запрос</response>
    /// <response code="404">Ошибка. Документ не найден</response>
    /// <response code="500">Ошибка. Возникло исключение в процессе обработки запроса</response>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(EstimateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get(string invoiceKey)
    {
        var query = new EstimateReadQuery(invoiceKey);

        var estimate = await this.queryHandler.Handle(query);

        var response = this.mapper.Map<EstimateResponse>(estimate);

        return Ok(response);
    }
}