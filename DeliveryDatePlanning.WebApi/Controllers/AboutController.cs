using Microsoft.AspNetCore.Mvc;
using DeliveryDatePlanning.Protocol;
using DeliveryDatePlanning.Protocol.Models;

namespace DeliveryDatePlanning.WebApi.Controllers;

[Route(Uris.About)]
public class AboutController
{
    private IWebHostEnvironment Environment { get; }

    public AboutController(IWebHostEnvironment environment)
    {
        this.Environment = environment;
    }

    [HttpGet]
    public About Get()
    {
        return new About
        {
            Description = "DeliveryDatePlanning - API для получения данных по ПДД и РСД.",
            Environment = this.Environment.EnvironmentName,
            Version = this.GetType().Assembly.GetName().Version?.ToString()
        };
    }
}