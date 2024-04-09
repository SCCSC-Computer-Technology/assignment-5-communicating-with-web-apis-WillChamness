using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentProfileWebApi.Models;

namespace StudentProfileWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentProfilesController : ControllerBase
    {
        private readonly StudentProfileContext _context;

        public StudentProfilesController(StudentProfileContext context)
        {
            _context = context;
        }

        // GET: api/StudentProfiles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentProfile>>> GetStudentProfiles()
        {
            return await _context.StudentProfiles.ToListAsync();
        }

        // GET: api/StudentProfiles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StudentProfile>> GetStudentProfile(long id)
        {
            var studentProfile = await _context.StudentProfiles.FindAsync(id);

            if (studentProfile == null)
            {
                return NotFound();
            }

            return studentProfile;
        }

        // PUT: api/StudentProfiles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudentProfile(long id, StudentProfile studentProfile)
        {
            if (id != studentProfile.Id)
            {
                return BadRequest();
            }

            _context.Entry(studentProfile).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentProfileExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/StudentProfiles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<StudentProfile>> PostStudentProfile(StudentProfile studentProfile)
        {
            byte[] salt;
            var shahash = new System.Security.Cryptography.SHA1CryptoServiceProvider();

            _context.StudentProfiles.Add(studentProfile);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStudentProfile", new { id = studentProfile.Id }, studentProfile);
        }

        // DELETE: api/StudentProfiles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudentProfile(long id)
        {
            var studentProfile = await _context.StudentProfiles.FindAsync(id);
            if (studentProfile == null)
            {
                return NotFound();
            }

            _context.StudentProfiles.Remove(studentProfile);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StudentProfileExists(long id)
        {
            return _context.StudentProfiles.Any(e => e.Id == id);
        }
    }
}
