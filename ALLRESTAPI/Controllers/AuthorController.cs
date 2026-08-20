using ALLRESTAPI;
using ALLRESTAPI.CRUDItem;
using ALLRESTAPI.Database;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleLibraryEF;
using System.Collections.Generic;
using System.Linq;

[Route("api/[controller]")]
[ApiController]

public class AuthorController : ControllerBase
{
	private readonly LibraryContext _dbContext;
	public AuthorController(LibraryContext _appDBContext)
	{
		_dbContext = _appDBContext;
	}

	[HttpGet]
	public async Task<IActionResult> GetAuthors()
	{
		var Authors = await _dbContext.Authors.Include(b=>b.Books).ToListAsync();
		return Ok(
			new
			{
				message = "All authors are now returned",
				data =Authors
			});
	
	}
}

