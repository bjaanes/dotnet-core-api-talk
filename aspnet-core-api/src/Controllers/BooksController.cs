using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using WebAPIApplication.DataAccess;

namespace WebAPIApplication.Controllers
{
    [Route("api/[controller]")]
    public class BooksController : Controller
    {
        private readonly IBookRepository _bookRepository;
        public BooksController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        [HttpGet]
        public List<BookView> Get()
        {
            IEnumerable<Book> booksFromDb = _bookRepository.GetBooks();

            List<BookView> bookViews = new List<BookView>();
            foreach (Book book in booksFromDb)
            {
                bookViews.Add(bookToBookView(book));
            }

            return bookViews;
        }

        [HttpPost]
        public IActionResult Post([FromBody]BookView bookView)
        {
            if (ModelState.IsValid)
            {
                return createBook(bookView);
            }
            else
            {
                return handleModelStateError();
            }
        }

        private IActionResult createBook(BookView bookView)
        {
            Book book = bookViewToBook(bookView);
            Book savedBook = _bookRepository.Create(book);
            return new ObjectResult(bookToBookView(savedBook));
        }

        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody]BookView bookView)
        {
            if (ModelState.IsValid)
            {
                return updateBook(id, bookView);
            }
            else
            {
                return handleModelStateError();
            }
        }

        private IActionResult updateBook(string id, BookView bookView)
        {
            if (bookExists(id))
            {
                Book book = bookViewToBook(bookView);
                Book savedBook = _bookRepository.Update(id, book);
                return new ObjectResult(bookToBookView(savedBook));
            }
            else
            {
                return NotFound();
            }
        }

        private IActionResult handleModelStateError()
        {
            List<ErrorView> listOfErrors = new List<ErrorView>();
            foreach (var modelError in ModelState)
            {
                string propertyName = modelError.Key;
                if (modelError.Value.Errors.Count > 0)
                {
                    ErrorView errorView = new ErrorView();
                    errorView.ErrorMessage = "Invalid " + propertyName;
                    listOfErrors.Add(errorView);
                }
            }
            return new BadRequestObjectResult(listOfErrors);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            if (bookExists(id))
            {
                _bookRepository.Remove(id);
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }

        private bool bookExists(string id)
        {
            Book book = _bookRepository.GetBook(id);
            return book != null;
        }
        private BookView bookToBookView(Book dbBook)
        {
            BookView bookView = new BookView();
            bookView.Id = dbBook.Id.ToString();
            bookView.Author = dbBook.Author;
            bookView.Title = dbBook.Title;
            bookView.Isbn = dbBook.Isbn;

            return bookView;
        }

        private Book bookViewToBook(BookView bookView)
        {
            Book dbBook = new Book();

            dbBook.Author = bookView.Author;
            dbBook.Title = bookView.Title;
            dbBook.Isbn = bookView.Isbn;

            return dbBook;
        }
    }
}
