using ALLRESTAPI;
using ALLRESTAPI.CRUDItem;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

[Route("api/[controller]")]
[ApiController]
public class ItemsController : ControllerBase
{
	private readonly ALLRESTAPI.CRUDItem.ICRUDItemRepository cRUDItemRepository;
	public ItemsController(ALLRESTAPI.CRUDItem.ICRUDItemRepository cRUDItemRepository)
	{
		this.cRUDItemRepository = cRUDItemRepository;
	}


	// A simple static list to simulate a database for this example.
	private static List<Item> _items = new List<Item>
	{
		new Item { Id = 1, Name = "Walk the dog", IsComplete = false },
		new Item { Id = 2, Name = "Buy groceries", IsComplete = true },
		new Item { Id = 3, Name = "Laundry", IsComplete = false }
	};

	// 1. GET: Retrieve all items
	// Matches the Read operation in CRUD.
	[HttpGet]
	public ActionResult<IEnumerable<Item>> GetItems()
	{
		return Ok(_items);
	}

	// 1. GET: Retrieve all items
	// Matches the Read operation in CRUD.
	[HttpGet("CRUDItems")]
	public async Task<ActionResult<IEnumerable<ALLRESTAPI.Models.CRUDItem>>> GetCRUDItems()
	{
		
		var cRUDItems = await cRUDItemRepository.GetCRUDItems();
		var cRUDItemsList = cRUDItems.ToList();
		return Ok(cRUDItems);
	}

	// 2. GET by ID: Retrieve a specific item
	[HttpGet("{id}")]
	public ActionResult<Item> GetItem(int id)
	{
		var item = _items.FirstOrDefault(i => i.Id == id);
		if (item == null)
		{
			return NotFound();
		}
		return Ok(item);
	}

	// 3. POST: Create a new item
	// Matches the Create operation in CRUD.
	[HttpPost]
	public ActionResult<Item> PostItem(Item newItem)
	{
		newItem.Id = _items.Count > 0 ? _items.Max(i => i.Id) + 1 : 1;
		_items.Add(newItem);
		// Returns a 201 Created response with a location header.
		return CreatedAtAction(nameof(GetItem), new { id = newItem.Id }, newItem);
	}

	// 4. PUT: Update an existing item (replaces the entire resource)
	// Matches the Update operation in CRUD.
	[HttpPut("{id}")]
	public IActionResult PutItem(int id, Item updatedItem)
	{
		if (id != updatedItem.Id)
		{
			return BadRequest();
		}

		var existingItem = _items.FirstOrDefault(i => i.Id == id);
		if (existingItem == null)
		{
			return NotFound();
		}

		existingItem.Name = updatedItem.Name;
		existingItem.IsComplete = updatedItem.IsComplete;

		// Returns a 204 No Content response for a successful update.
		return NoContent();
	}

	// 5. PATCH: Partially update an existing item (modifies specific fields)
	[HttpPatch("{id}")]
	public IActionResult PatchItem(int id, [FromBody] Item patchData) // Simplified for example, actual JSON Patch uses a specific format
	{
		var existingItem = _items.FirstOrDefault(i => i.Id == id);
		if (existingItem == null)
		{
			return NotFound();
		}

		if (patchData == null)
		{
			return BadRequest("Patch data cannot be null.");
		}

		// Apply partial updates based on non-null fields in patchData (simplified logic)
		if (!string.IsNullOrEmpty(patchData.Name))
		{
			existingItem.Name = patchData.Name;
		}

		// For boolean, a more robust check might be needed in a real app to differentiate between false and not provided.
		// This simple example assumes a provided 'IsComplete' value should be applied.
		existingItem.IsComplete = patchData.IsComplete;

		// Returns a 204 No Content response for a successful partial update.
		return NoContent();
	}

	// 6. DELETE: Remove a resource
	// Matches the Delete operation in CRUD.
	[HttpDelete("{id}")]
	public IActionResult DeleteItem(int id)
	{
		var item = _items.FirstOrDefault(i => i.Id == id);
		if (item == null)
		{
			return NotFound();
		}

		_items.Remove(item);

		// Returns a 204 No Content response for a successful deletion.
		return NoContent();
	}
}