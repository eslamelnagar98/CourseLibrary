using AutoMapper;
using CourseLibrary.API.Dtos;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Helpers;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseLibrary.API.Controllers
{
    [ApiController]
    [Route("api/authorcollrctions")]
    public class AuthorCollectionsController : ControllerBase
    {
        private readonly ICourseLibraryRepository _courseLibraryRepository;
        private readonly IMapper _mapper;

        public AuthorCollectionsController(
            ICourseLibraryRepository courseLibraryRepository,
            IMapper mapper)
        {
            _courseLibraryRepository = courseLibraryRepository ??
                throw new ArgumentNullException(nameof(courseLibraryRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }


        [HttpGet("({ids})", Name = "GetAuthorCollection")]
        public IActionResult GetAuthorCollection([FromRoute]
        [ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            if (ids is null) return BadRequest();
            var authorEntites = _courseLibraryRepository?.GetAuthors(author => ids.Contains(author.Id)) ?? new List<Author>();
            if (ids.Count() != authorEntites.Count()) return NotFound();
            var authorToReturn = _mapper.Map<IEnumerable<AuthorDto>>(authorEntites);
            return Ok(authorToReturn);
            //return ids is null ? BadRequest() : NoContent();
        }


        [HttpPost]
        public ActionResult<IEnumerable<AuthorDto>> CreateAuthorCollection(
               IEnumerable<AuthorForCreationDto> authorCollection)
        {
            var authorEntites = _mapper.Map<IEnumerable<Author>>(authorCollection);
            foreach (var author in authorEntites)
                _courseLibraryRepository.AddAuthor(author);

            _courseLibraryRepository.Save();
            var authorCollectionToReturn = _mapper.Map<IEnumerable<AuthorDto>>(authorEntites);
            var idsAsString = string.Join(",", authorCollectionToReturn.Select(a => a.Id));
            return CreatedAtRoute("GetAuthorCollection", new { ids = idsAsString },
                                                        authorCollectionToReturn);
        }


    }
}
