﻿
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StocksAPI.DTO;
using StocksAPI.Exceptions;
using StocksAPI.Models;
using StocksAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using StocksAPI.Features.StockFeatures.Commands;
using StocksAPI.Features.StockFeatures.Queries;

namespace StocksAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockController : ControllerBase
    {
        private IMediator _mediator;
       // protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();
        private readonly ILogger<StockController> _logger;
        private readonly IStockService _stockService;
        readonly IMessageProducerService messageProducer;
        public StockController(ILogger<StockController> logger,
         IStockService stockService, IMessageProducerService producerService,
         IMediator mediator
         )
        {
            _logger = logger;
            _stockService = stockService;
            messageProducer = producerService;
            _mediator=mediator;
        }
        [Authorize(AuthenticationSchemes =
JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [Route("{companycode}")]
        public async Task<IActionResult> Post([FromBody] StockDto stock,string companycode)
        {
            try
            {
                Stock _stock = MapToStock(stock);
                _stock.CompanyCode = companycode;
                AddStockCommand command = new AddStockCommand
                {
                    Stock = _stock
                };
                await _mediator.Send(command);
                _logger.LogInformation($"Sending stock info to Kafka for {companycode}");
                messageProducer.WriteMessage("StockInfo", _stock);
                return CreatedAtAction(nameof(Post), companycode);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Adding the stock: {ex.Message}");
                return StatusCode(500);
            }

        }

        private Stock MapToStock(StockDto stock)
        {
            Stock stock1 = new Stock
            {
                StockPrice = stock.StockPrice,
            };
            return stock1;
        }

        [HttpGet]
        [Route("{companycode}/{startdate}/{enddate}")]
        public async Task<IActionResult> Get(string companycode, DateTime startdate, DateTime enddate)
        {
            try
            {
                _logger.LogInformation($"Getting stock of {companycode} for the time-period {startdate} - {enddate}");
                var stocks = await _mediator.Send(new GetStockByCompanyCodeAndStartDateEndDateQuery()
                {
                    CompanyCode = companycode,
                    StartDate = startdate,
                    EndDate = enddate
                });
                //var stocks = await _stockService.GetAsync(companycode, startdate, enddate);
                var result=stocks.Select(x=>new Stock{
                    StockId=x.StockId,
                    StockPrice=x.StockPrice,
                    CompanyCode=x.CompanyCode,
                    StockDateTime=x.StockDateTime,
                    time=x.StockDateTime.ToString("HH:mm")

                }).ToList();
                return Ok(result);
            }
            catch (StockNotFoundException pnf)
            {
                _logger.LogInformation($"Required stock does not exist");
                return NotFound(pnf.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in fetching data: {ex.Message}");
                return StatusCode(500);
            }
        }
    }
}
