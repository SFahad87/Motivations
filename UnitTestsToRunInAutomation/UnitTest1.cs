// =============================================================================
//  UnitTestsToRunInAutomation  –  NUnit 3  •  .NET 8
//
//  Test classes:
//    1. CalculatorTests          – pure-logic unit tests (original, fixed)
//    2. ItemsControllerTests     – ItemsController CRUD action tests
//    3. CRUDItemRepositoryTests  – ICRUDItemRepository mock tests (Moq)
// =============================================================================

using ALLRESTAPI;
using ALLRESTAPI.CRUDItem;
using ALLRESTAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Timers;

namespace UnitTestsToRunInAutomation
{
	// =========================================================================
	// 1. CALCULATOR  –  pure logic (kept from original UnitTest1.cs, corrected)
	//
	//    Original TestCase data was wrong:
	//      [TestCase(2, 3, 1)]  → 2 * 3 = 6, not 1
	//      [TestCase(-5, 4, -1)] → -5 * 4 = -20, not -1
	//      [TestCase(0, 10, 0)]  → 0 * 10 = 0  ✓  (this one was correct)
	//    Fixed expected values below.
	// =========================================================================
	public class Calculator
	{
		public int Multiply(int a, int b) => a * b;
		public int Add(int a, int b) => a + b;
		public int Subtract(int a, int b) => a - b;
		public double Divide(int a, int b)
		{
			if (b == 0) throw new DivideByZeroException("Cannot divide by zero.");
			return (double)a / b;
		}
	}

	[TestFixture]
	public class CalculatorTests
	{
		private Calculator _calculator = null!;

		[SetUp]
		public void Setup() => _calculator = new Calculator();

		// --- Multiply ----------------------------------------------------------

		[Test]
		public void Multiply_FiveByFour_ReturnsTwenty()
		{
			Assert.That(_calculator.Multiply(5, 4), Is.EqualTo(20));
		}

		// ⚠ Original expected values were wrong – fixed below
		[TestCase(2, 3, 6)]    // was 1  → fixed to 6
		[TestCase(-5, 4, -20)]    // was -1 → fixed to -20
		[TestCase(0, 10, 0)]    // correct
		[TestCase(1, 1, 1)]
		[TestCase(-3, -3, 9)]
		public void Multiply_VariousInputs_ReturnsCorrectProduct(int a, int b, int expected)
		{
			Assert.That(_calculator.Multiply(a, b), Is.EqualTo(expected));
		}

		// --- Add ---------------------------------------------------------------

		[TestCase(2, 3, 5)]
		[TestCase(-1, 1, 0)]
		[TestCase(0, 0, 0)]
		public void Add_VariousInputs_ReturnsCorrectSum(int a, int b, int expected)
		{
			Assert.That(_calculator.Add(a, b), Is.EqualTo(expected));
		}

		// --- Subtract ----------------------------------------------------------

		[TestCase(10, 3, 7)]
		[TestCase(0, 5, -5)]
		public void Subtract_VariousInputs_ReturnsCorrectDifference(int a, int b, int expected)
		{
			Assert.That(_calculator.Subtract(a, b), Is.EqualTo(expected));
		}

		// --- Divide ------------------------------------------------------------

		[Test]
		public void Divide_TenByTwo_ReturnsFive()
		{
			Assert.That(_calculator.Divide(10, 2), Is.EqualTo(5.0));
		}

		[Test]
		public void Divide_ByZero_ThrowsDivideByZeroException()
		{
			Assert.Throws<DivideByZeroException>(() => _calculator.Divide(10, 0));
		}
	}
/*
	// =========================================================================
	// 2. ITEMS CONTROLLER  –  in-memory list CRUD
	//    ItemsController operates on a static list; we reset it via reflection
	//    before each test so tests are isolated.
	// =========================================================================
	[TestFixture]
	public class ItemsControllerTests
	{
		private ItemsController _controller = null!;
		private Mock<ICRUDItemRepository> _repoMock = null!;

		// Helper: reset the private static _items list in ItemsController
		// so every test starts with the same three seed items.
		private static void ResetStaticItems()
		{
			var field = typeof(ItemsController)
				.GetField("_items",
					System.Reflection.BindingFlags.Static |
					System.Reflection.BindingFlags.NonPublic)!;

			field.SetValue(null, new System.Collections.Generic.List<Item>
			{
				new Item { Id = 1, Name = "Walk the dog",   IsComplete = false },
				new Item { Id = 2, Name = "Buy groceries",  IsComplete = true  },
				new Item { Id = 3, Name = "Laundry",        IsComplete = false },
			});
		}

		[SetUp]
		public void Setup()
		{
			ResetStaticItems();
			_repoMock = new Mock<ICRUDItemRepository>();
			_controller = new ItemsController(_repoMock.Object);
		}

		// --- GET all items -----------------------------------------------------

		[Test]
		public void GetItems_ReturnsOkWithThreeItems()
		{
			var result = _controller.GetItems();
			var okResult = result.Result as OkObjectResult;

			Assert.That(okResult, Is.Not.Null);
			Assert.That(okResult!.StatusCode, Is.EqualTo(200));

			var items = okResult.Value as System.Collections.Generic.List<Item>;
			Assert.That(items, Is.Not.Null);
			Assert.That(items!.Count, Is.EqualTo(3));
		}

		// --- GET by ID ----------------------------------------------------------

		[Test]
		public void GetItem_ExistingId_ReturnsOkWithItem()
		{
			var result = _controller.GetItem(1);
			var okResult = result.Result as OkObjectResult;

			Assert.That(okResult, Is.Not.Null);
			Assert.That(okResult!.StatusCode, Is.EqualTo(200));

			var item = okResult.Value as Item;
			Assert.That(item, Is.Not.Null);
			Assert.That(item!.Name, Is.EqualTo("Walk the dog"));
		}

		[Test]
		public void GetItem_NonExistingId_ReturnsNotFound()
		{
			var result = _controller.GetItem(999);
			var notFound = result.Result as NotFoundResult;

			Assert.That(notFound, Is.Not.Null);
			Assert.That(notFound!.StatusCode, Is.EqualTo(404));
		}

		// --- POST ---------------------------------------------------------------

		[Test]
		public void PostItem_ValidItem_ReturnsCreatedAtAction()
		{
			var newItem = new Item { Name = "Read a book", IsComplete = false };

			var result = _controller.PostItem(newItem);
			var created = result.Result as CreatedAtActionResult;

			Assert.That(created, Is.Not.Null);
			Assert.That(created!.StatusCode, Is.EqualTo(201));
			Assert.That(created.ActionName, Is.EqualTo(nameof(_controller.GetItem)));

			var returnedItem = created.Value as Item;
			Assert.That(returnedItem, Is.Not.Null);
			Assert.That(returnedItem!.Id, Is.EqualTo(4));   // max(1,2,3) + 1
			Assert.That(returnedItem.Name, Is.EqualTo("Read a book"));
		}

		// --- PUT ----------------------------------------------------------------

		[Test]
		public void PutItem_MatchingIds_ReturnsNoContent()
		{
			var updated = new Item { Id = 1, Name = "Walk the cat", IsComplete = true };
			var result = _controller.PutItem(1, updated) as NoContentResult;

			Assert.That(result, Is.Not.Null);
			Assert.That(result!.StatusCode, Is.EqualTo(204));
		}

		[Test]
		public void PutItem_MismatchedIds_ReturnsBadRequest()
		{
			var updated = new Item { Id = 2, Name = "Wrong", IsComplete = false };
			var result = _controller.PutItem(1, updated) as BadRequestResult;

			Assert.That(result, Is.Not.Null);
			Assert.That(result!.StatusCode, Is.EqualTo(400));
		}

		[Test]
		public void PutItem_NonExistingId_ReturnsNotFound()
		{
			var updated = new Item { Id = 99, Name = "Ghost", IsComplete = false };
			var result = _controller.PutItem(99, updated) as NotFoundResult;

			Assert.That(result, Is.Not.Null);
			Assert.That(result!.StatusCode, Is.EqualTo(404));
		}

		// --- PATCH --------------------------------------------------------------

		[Test]
		public void PatchItem_ExistingId_ReturnsNoContent()
		{
			var patch = new Item { Name = "Patched name", IsComplete = true };
			var result = _controller.PatchItem(1, patch) as NoContentResult;

			Assert.That(result, Is.Not.Null);
			Assert.That(result!.StatusCode, Is.EqualTo(204));
		}

		[Test]
		public void PatchItem_NullBody_ReturnsBadRequest()
		{
			var result = _controller.PatchItem(1, null!) as BadRequestObjectResult;

			Assert.That(result, Is.Not.Null);
			Assert.That(result!.StatusCode, Is.EqualTo(400));
		}

		[Test]
		public void PatchItem_NonExistingId_ReturnsNotFound()
		{
			var patch = new Item { Name = "Ghost patch", IsComplete = false };
			var result = _controller.PatchItem(999, patch) as NotFoundResult;

			Assert.That(result, Is.Not.Null);
			Assert.That(result!.StatusCode, Is.EqualTo(404));
		}

		// --- DELETE -------------------------------------------------------------

		[Test]
		public void DeleteItem_ExistingId_ReturnsNoContent()
		{
			var result = _controller.DeleteItem(1) as NoContentResult;

			Assert.That(result, Is.Not.Null);
			Assert.That(result!.StatusCode, Is.EqualTo(204));
		}

		[Test]
		public void DeleteItem_NonExistingId_ReturnsNotFound()
		{
			var result = _controller.DeleteItem(999) as NotFoundResult;

			Assert.That(result, Is.Not.Null);
			Assert.That(result!.StatusCode, Is.EqualTo(404));
		}

		// --- GetCRUDItems (mocked repo) ----------------------------------------

		[Test]
		public async Task GetCRUDItems_RepoReturnsList_ReturnsOkWithItems()
		{
			var fakeItems = new List<CRUDItem>
			{
				new CRUDItem { Id = 1, ItemName = "Alpha", CreateDate = DateTime.UtcNow, LastUpdateDate = DateTime.UtcNow },
				new CRUDItem { Id = 2, ItemName = "Beta",  CreateDate = DateTime.UtcNow, LastUpdateDate = DateTime.UtcNow },
			};

			_repoMock
				.Setup(r => r.GetCRUDItems())
				.ReturnsAsync(fakeItems);

			var result = await _controller.GetCRUDItems();
			var okResult = result.Result as OkObjectResult;

			Assert.That(okResult, Is.Not.Null);
			Assert.That(okResult!.StatusCode, Is.EqualTo(200));

			var returned = okResult.Value as IEnumerable<CRUDItem>;
			Assert.That(returned, Is.Not.Null);
			Assert.That(returned!.Count(), Is.EqualTo(2));
		}

		[Test]
		public async Task GetCRUDItems_RepoReturnsEmpty_ReturnsOkWithEmptyList()
		{
			_repoMock
				.Setup(r => r.GetCRUDItems())
				.ReturnsAsync(new List<CRUDItem>());

			var result = await _controller.GetCRUDItems();
			var okResult = result.Result as OkObjectResult;

			Assert.That(okResult, Is.Not.Null);
			var returned = okResult!.Value as IEnumerable<CRUDItem>;
			Assert.That(returned, Is.Not.Null);
			Assert.That(returned!.Any(), Is.False);
		}
	}

	// =========================================================================
	// 3. ICRUD ITEM REPOSITORY  –  Moq-based contract tests
	//    Verifies that callers of ICRUDItemRepository receive and handle the
	//    values the repository is supposed to return.
	// =========================================================================
	[TestFixture]
	public class CRUDItemRepositoryContractTests
	{
		private Mock<ICRUDItemRepository> _repoMock = null!;

		[SetUp]
		public void Setup() => _repoMock = new Mock<ICRUDItemRepository>();

		[Test]
		public async Task GetCRUDItems_MockSetupWithTwoItems_ReturnsBothItems()
		{
			var expected = new List<CRUDItem>
			{
				new CRUDItem { Id = 10, ItemName = "Widget A", CreateDate = DateTime.UtcNow, LastUpdateDate = DateTime.UtcNow },
				new CRUDItem { Id = 11, ItemName = "Widget B", CreateDate = DateTime.UtcNow, LastUpdateDate = DateTime.UtcNow },
			};

			_repoMock.Setup(r => r.GetCRUDItems()).ReturnsAsync(expected);

			var result = await _repoMock.Object.GetCRUDItems();
			var list = result.ToList();

			Assert.That(list.Count, Is.EqualTo(2));
			Assert.That(list[0].ItemName, Is.EqualTo("Widget A"));
			Assert.That(list[1].Id, Is.EqualTo(11));
		}

		[Test]
		public async Task GetCRUDItems_CalledOnce_VerifyInteraction()
		{
			_repoMock.Setup(r => r.GetCRUDItems()).ReturnsAsync(new List<CRUDItem>());

			await _repoMock.Object.GetCRUDItems();

			// Verify the method was called exactly once
			_repoMock.Verify(r => r.GetCRUDItems(), Times.Once);
		}

		[Test]
		public async Task GetCRUDItems_NeverCalled_VerifyZeroInteractions()
		{
			// Don't call the repo at all
			_repoMock.Verify(r => r.GetCRUDItems(), Times.Never);
			await Task.CompletedTask;
		}

		[Test]
		public async Task GetCRUDItems_AllItemsHavePositiveIds()
		{
			var items = new List<CRUDItem>
			{
				new CRUDItem { Id = 1, ItemName = "A", CreateDate = DateTime.UtcNow, LastUpdateDate = DateTime.UtcNow },
				new CRUDItem { Id = 2, ItemName = "B", CreateDate = DateTime.UtcNow, LastUpdateDate = DateTime.UtcNow },
			};

			_repoMock.Setup(r => r.GetCRUDItems()).ReturnsAsync(items);

			var result = (await _repoMock.Object.GetCRUDItems()).ToList();

			Assert.That(result.All(i => i.Id > 0), Is.True, "All IDs must be positive");
		}
	}*/
}
