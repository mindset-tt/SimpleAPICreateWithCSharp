using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestAPICreateWithC_.Models;

namespace TestAPICreateWithC_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeItemsController : ControllerBase
    {
        private readonly EmployeeContext _context;

        public EmployeeItemsController(EmployeeContext context)
        {
            _context = context;
        }

        // GET: api/EmployeeItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeItemDTO>>> GetEmployeeItem()
        {
            
            var employeeItems = await _context.EmployeeItems.ToListAsync();
            if (employeeItems.Count == 0)
            {
                return NotFound("No records found in the database");
            }

            return employeeItems.Select(x => EmployeeItemDTO(x)).ToList();

        }

        // GET: api/EmployeeItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeItemDTO>> GetEmployeeItem(long id)
        {
            var employeeItem = await _context.EmployeeItems.FindAsync(id);
            if (employeeItem == null)
            {
                return NotFound("No records found in the database");
            }
            return EmployeeItemDTO(employeeItem);
        }

        // PUT: api/EmployeeItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployeeItem(long id, EmployeeItemDTO employeeItemDTO)
        {
            if (id != employeeItemDTO.Id)
            {
                return BadRequest("Id in the URL does not match the Id in the body please fix it");
            }
            var employeeItem = await _context.EmployeeItems.FindAsync(id);
            if (employeeItem == null)
            {
                return NotFound("No records found in the database");
            }

            bool isUpdated = false;
            var properties = typeof(EmployeeItemDTO).GetProperties();

            foreach (var property in properties)
            {
                if (property.Name == "Id") continue;

                var newValue = property.GetValue(employeeItemDTO);
                var existingProperty = typeof(EmployeeItem).GetProperty(property.Name);
                if (existingProperty == null) continue;

                var oldValue = existingProperty.GetValue(employeeItem);
                if (object.Equals(oldValue, newValue)) continue;

                var propertyName = property.Name;
                if (property.CustomAttributes.Any(x => x.AttributeType.Name == "required") && newValue == null)
                    return BadRequest($"{propertyName} is required");

                if (propertyName == "Email" && (newValue?.ToString() is not string email || !email.Contains('@') || !email.Contains('.')))
                    return BadRequest("Email is not valid");

                if (propertyName == "Phone" && (newValue?.ToString() is not string phone || !phone.All(char.IsDigit)))
                    return BadRequest("Phone is not valid");

                if (propertyName == "DOB" && (newValue?.ToString() is not string dob || !DateTime.TryParse(dob, out _)))
                    return BadRequest("DOB is not valid");

                if (propertyName is "CreatedAt" or "CreatedBy")
                    return BadRequest($"{propertyName} cannot be updated");

                if (propertyName == "UpdatedAt" && newValue == null)
                    return BadRequest("UpdatedAt is required");

                if (propertyName == "UpdatedBy" && newValue == null)
                    return BadRequest("UpdatedBy is required");

                existingProperty.SetValue(employeeItem, newValue);
                isUpdated = true;
            }

            if (!isUpdated)
            {
                return NoContent();
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!EmployeeItemExists(id))
            {
                return NotFound("No records found in the database");
            }

            return Ok("Record updated successfully in the database");
        }

        // POST: api/EmployeeItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<EmployeeItemDTO>> PostEmployeeItem(EmployeeItemDTO employeeItemDTO)
        {

            //check the required fields are not null
            if (employeeItemDTO.Name is null || employeeItemDTO.Surname is null || employeeItemDTO.Email is null || employeeItemDTO.Phone is null)
                return BadRequest("Name, Surname, DOB, Email and Phone are required fields");

            //check if the email is valid or not
            if (employeeItemDTO.Email is not string email || !email.Contains('@') || !email.Contains('.'))
                return BadRequest("Email is not valid");

            //check if the phone is valid or not
            if (employeeItemDTO.Phone is not string phone || !phone.All(char.IsDigit))
                return BadRequest("Phone is not valid");

            //check if the DOB is valid or not
            if (!DateTime.TryParse(employeeItemDTO.DOB.ToString(), out _))
                return BadRequest("DOB is not valid");

            //check if the created by is null or not
            if (employeeItemDTO.CreatedBy is null)
                return BadRequest("CreatedBy is required");

            //check if the created at is null or not
            if (employeeItemDTO?.CreatedAt == null)
                return BadRequest("CreatedAt is required");

            var employeeItem = new EmployeeItem
            {
                Name = employeeItemDTO.Name,
                Surname = employeeItemDTO.Surname,
                Position = employeeItemDTO.Position,
                DOB = employeeItemDTO.DOB,
                Role = employeeItemDTO.Role,
                Salary = employeeItemDTO.Salary,
                Email = employeeItemDTO.Email,
                Phone = employeeItemDTO.Phone,
                Address = employeeItemDTO.Address,
                Zip = employeeItemDTO.Zip,
                CreatedAt = employeeItemDTO.CreatedAt,
                CreatedBy = employeeItemDTO.CreatedBy!,
                UpdatedAt = employeeItemDTO.UpdatedAt,
                UpdatedBy = employeeItemDTO.UpdatedBy!,
            };
            _context.EmployeeItems.Add(employeeItem);
            await _context.SaveChangesAsync();
            return CreatedAtAction(
                nameof(GetEmployeeItem),
                new { id = employeeItem.Id },
                EmployeeItemDTO(employeeItem));

        }

        // DELETE: api/EmployeeItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployeeItem(long id)
        {
            var employeeItem = await _context.EmployeeItems.FindAsync(id);
            if (employeeItem == null)
            {
                return NotFound("No records found in the database");
            }

            _context.EmployeeItems.Remove(employeeItem);
            await _context.SaveChangesAsync();

            return Ok("Record deleted successfully from the database");
        }

        private bool EmployeeItemExists(long id)
        {
            return _context.EmployeeItems.Any(e => e.Id == id);
        }


        private static EmployeeItemDTO EmployeeItemDTO(EmployeeItem employeeItem) =>
            new()
            {
                Id = employeeItem.Id,
                Name = employeeItem.Name,
                Surname = employeeItem.Surname,
                Position = employeeItem.Position,
                DOB = employeeItem.DOB,
                Role = employeeItem.Role,
                Salary = employeeItem.Salary,
                Email = employeeItem.Email,
                Phone = employeeItem.Phone,
                Address = employeeItem.Address,
                Zip = employeeItem.Zip,
                CreatedAt = employeeItem.CreatedAt,
                CreatedBy = employeeItem.CreatedBy,
                UpdatedAt = employeeItem.UpdatedAt,
                UpdatedBy = employeeItem.UpdatedBy,
            };
    }
}
