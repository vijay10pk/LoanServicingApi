using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using LoanServicingApi.Exceptions;
using LoanServicingApi.Helpers;
using LoanServicingApi.Interfaces;
using LoanServicingApi.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LoanServicingApi.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportRepository _reportRepository;
        private readonly LoanServicingApiHelper _helper;

        public ReportController(IReportRepository reportRepository, LoanServicingApiHelper helper)
        {
            _reportRepository = reportRepository;
            _helper = helper;
        }

        
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateReport([FromBody] JsonElement requestBody)
        {
            if (!requestBody.TryGetProperty("reportType", out JsonElement reportTypeElement))
            {
                return BadRequest("ReportType is required.");
            }

            string reportTypeString = reportTypeElement.GetString();

            if (!Enum.TryParse<ReportType>(reportTypeString, true, out ReportType reportType))
            {
                return BadRequest($"Invalid report type: {reportTypeString}. Valid types are: {string.Join(", ", Enum.GetNames(typeof(ReportType)))}");
            }

            try
            {
                var userId = _helper.GetCurrentUserId();
                var report = await _reportRepository.GenerateReport(reportType, userId);
                return CreatedAtAction(nameof(GetReport), new { id = report.Id }, report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while generating the report: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllReports()
        {
            var reports = await _reportRepository.GetAllReports();
            return Ok(reports);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReport(int id)
        {
            try
            {
                var report = await _reportRepository.GetReportById(id);
                return Ok(report);
            }
            catch (LoanServicingExceptions.NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetUserReports()
        {
            var userId = _helper.GetCurrentUserId();
            var reports = await _reportRepository.GetReportsByUser(userId);
            return Ok(reports);
        }
    }
}
