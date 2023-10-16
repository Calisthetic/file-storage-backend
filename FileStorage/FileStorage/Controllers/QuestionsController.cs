using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FileStorage.Data;
using FileStorage.Models.Db;
using MapsterMapper;
using FileStorage.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using FileStorage.Models.Outcoming.Folder;
using Mapster;
using FileStorage.Models.Outcoming;
using FileStorage.Models.Incoming.Question;
using FileStorage.Models.Incoming;
using Asp.Versioning;

namespace FileStorage.Controllers
{
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/questions")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class QuestionsController : ControllerBase
    {
        private readonly ApiDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public QuestionsController(ApiDbContext context, IUserService userService, IMapper mapper)
        {
            _context = context;
            _userService = userService;
            _mapper = mapper;
        }

        // GET: api/questions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuestionInfoDto>>> GetQuestions()
        {
            if (_context.Questions == null)
            {
                return NotFound();
            }

            // If user authorized
            if (!int.TryParse(_userService.GetUserId(), out int userId))
            {
                return Unauthorized();
            }

            return await _mapper.From(_context.Questions.Where(x => x.UserId == userId)).ProjectToType<QuestionInfoDto>().ToListAsync();
        }


        // POST: api/questions
        [HttpPost]
        [ActionName(nameof(PostQuestion))]
        public async Task<ActionResult<Question>> PostQuestion(QuestionCreateDto question)
        {
            if (_context.Questions == null)
            {
                return Problem("Entity set 'ApiDbContext.Questions'  is null.");
            }

            // If user authorized
            if (!int.TryParse(_userService.GetUserId(), out int userId))
            {
                return Unauthorized();
            }

            var newQuestion = new Question()
            {
                Text = question.Text,
                CreatedAt = DateTime.Now,
                UserId = userId
            };
            _context.Questions.Add(newQuestion);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(PostQuestion), new { id = newQuestion.Id });
        }

        // PATCH: api/questions
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchQuestion(int id, QuestionAnswerDto data)
        {
            if (_context.Questions == null)
            {
                return Problem("Entity set 'ApiDbContext.Questions'  is null.");
            }

            // If user authorized
            if (!int.TryParse(_userService.GetUserId(), out int userId))
            {
                return Unauthorized();
            }

            var currentQuestion = await _context.Questions.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (currentQuestion is null)
            {
                return NotFound();
            }

            currentQuestion.Answer = data.Answer;
            currentQuestion.RespondedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/questions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            if (_context.Questions == null)
            {
                return NotFound();
            }

            // If user authorized
            if (!int.TryParse(_userService.GetUserId(), out int userId))
            {
                return Unauthorized();
            }

            var question = await _context.Questions.FirstOrDefaultAsync(x => x.UserId == userId && x.Id == id);
            if (question == null)
            {
                return NotFound();
            }

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool QuestionExists(int id)
        {
            return (_context.Questions?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
